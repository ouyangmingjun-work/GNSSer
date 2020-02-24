﻿// 2014.09.03, czs, 独立于Ppp.cs
// 2014.09.05, czs, edit, 实现 IAdjustMatrixBuilder 接口
//2016.10.10, czs, edit, 保持模糊度以米单位，多系统下具有可比性，
//2018.06.03, lly & czs, edit in zz & hmx, 修改伽利略频率，按照RINEX的顺序 1 5 7 8 6

using System;
using System.Collections.Generic;
using System.Text;
using Geo.Algorithm;
using Geo.Utils;
using Geo.Common;
using Geo.Coordinates;
using Geo.Algorithm.Adjust;
using Gnsser.Times;
using Gnsser.Domain;
using Gnsser.Service;
using Gnsser.Data.Rinex;
using Gnsser.Checkers;
using Gnsser.Models;
using Geo.Times; 
using Geo;

namespace Gnsser.Service
{
    /// <summary>
    /// 精密单点定位矩阵生成类。
    /// </summary>
    public class IonoFreePppMatrixBuilderOld : BasePppPositionMatrixBuilder
    {
        /// <summary>
        /// 先构造，再设置历元值。
        /// </summary>
        /// <param name="option">解算选项</param> 
        public IonoFreePppMatrixBuilderOld(
            GnssProcessOption option)
            : base(option)
        { 
            ParamNameBuilder = new IonoFreePppParamNameBuilder(option);      
        }

        #region 全局基础属性 

        /// <summary>
        /// 参数数量
        /// </summary>
        public override int ParamCount { get { return EnabledSatCount + 4 + Option.SatelliteTypes.Count; } } 
        #endregion
        
        #region 创建观测信息

        /// <summary>
        /// 具有权值的观测值。
        /// </summary> 
        public override WeightedVector Observation
        {
            get
            {
                IMatrix inverseWeightOfObs = BulidInverseWeightOfObs();
                WeightedVector deltaObs = new WeightedVector(ObservationVector, inverseWeightOfObs);
                return deltaObs;
            }
        }

        /// <summary>
        /// 观测值。
        /// 自由项 l，观测值减去先验值或估计值。
        /// 常数项，观测误差方程的常数项,或称自由项
        /// </summary>
        public virtual IVector ObservationVector
        {
            get
            {
                Vector L = new Vector(ObsCount);
                int satIndex = 0;
                XYZ staXyz = CurrentMaterial.SiteInfo.EstimatedXyz;

                Vector rangeVector = CurrentMaterial.GetAdjustVector( SatObsDataType.IonoFreeRange);

                Vector phaseVector = null;
                if (this.Option.IsAliningPhaseWithRange)
                {
                   phaseVector = CurrentMaterial.GetAdjustVector(SatObsDataType.AlignedIonoFreePhaseRange, true);
                }
                else
                {
                    phaseVector = CurrentMaterial.GetAdjustVector(SatObsDataType.IonoFreePhaseRange, true);
                } 
                foreach (var item in CurrentMaterial.EnabledPrns)
                {
                    L[satIndex] = rangeVector[satIndex];
                    L.ParamNames[satIndex] = item + "_IF_C";

                    L[satIndex + EnabledSatCount] = phaseVector[satIndex];
                    L.ParamNames[satIndex + EnabledSatCount] = item + "_IF_L";

                    satIndex++;
                }

                return L;
            }
        }

        /// <summary>
        /// 观测量的权逆阵，一个对角阵。按照先伪距，后载波的顺序排列。
        /// 标准差由常数确定，载波比伪距标准差通常是 1:100，其方差比是 1:10000.
        /// 1.0/140*140=5.10e-5
        /// </summary> 
        /// <returns></returns>
        protected IMatrix BulidInverseWeightOfObs()
        {
            int satCount = EnabledSatCount;// EpochInfo.EnabledSatCount;
            int row = satCount * 2;
           // double[][] inverseWeight = Geo.Utils.MatrixUtil.Create(row);
            DiagonalMatrix inverseWeight = new DiagonalMatrix(row, 1);
            double []inverseWeightVector=inverseWeight.Vector;
            double invFactorOfRange = 1;
            double invFactorOfPhase = Option.PhaseCovaProportionToRange;
            //   double invFactorOfPhase =  0.003 * 0.003;

            int i = 0;
            foreach (var prn in CurrentMaterial.EnabledPrns)// 一颗卫星1行 
            {
                EpochSatellite e = this.CurrentMaterial[prn];
                double inverseWeightOfSat = SatWeightProvider.GetInverseWeightOfRange(e);

                //如果具有周跳，则权值同伪距 ？？？
                //if (e.HasCycleSlip) 
                //    invFactorOfPhase = invFactorOfRange;

                //inverseWeight[time][time] =8.87 * inverseWeightOfSat * invFactorOfRange; //8.87是按误差传播定律算的组合值的方差
                //inverseWeight[time + satCount][time + satCount] = 8.87 *inverseWeightOfSat * invFactorOfPhase;
                inverseWeightVector[i] = inverseWeightOfSat * invFactorOfRange;

                //if(e.IsUnstable  && Option.AdjustmentType == AdjustmentType.序贯平差)
                //{
                //    inverseWeightVector[i + satCount] = inverseWeightOfSat * invFactorOfPhase;// + 10;
                //}
                //else
                {
                   inverseWeightVector[ i + satCount] = inverseWeightOfSat * invFactorOfPhase;
                }

                i++;
            }
            return (inverseWeight);
        }

        #endregion

        #region 公共矩阵生成

        /// <summary>
        /// 参数平差系数阵 A。前一半行数为伪距观测量，后一半为载波观测量。
        /// </summary> 
        /// <returns></returns>
        public override Matrix Coefficient
        {
            get
            {
                Geo.Coordinates.GeoCoord geoCoord = Geo.Coordinates.CoordTransformer.XyzToGeoCoord(CurrentMaterial.SiteInfo.EstimatedXyz, Geo.Coordinates.AngleUnit.Radian);

             //   Time time = EpochInfo.CorrectedTime;
                Time time = CurrentMaterial.ReceiverTime;

                //NeillTropModel pTroModel = new NeillTropModel(geoCoord.Height, geoCoord.Lat * CoordConsts.RadToDegMultiplier, time.DayOfYear);

                int satCount = EnabledSatCount;
                int rowCount = satCount * 2;
                int colCount = satCount + 4 + Option.SatelliteTypes.Count;

                double[][] A = Geo.Utils.MatrixUtil.Create(rowCount, colCount);
                int row = 0;//卫星编号
                int satIndex = 0;
                #region
                //double f1 = CurrentMaterial.First.FrequenceA.Frequence.Value * 1e6;
                //double f2 = CurrentMaterial.First.FrequenceB.Frequence.Value * 1e6;
                //double lam_LC = GnssConst.LIGHT_SPEED / (f1 + f2);

                //double lam_L1 = GnssConst.LIGHT_SPEED / (f1);


                //int row = 0;//卫星编号
                //int satIndex = 0;
                //foreach (var prn in CurrentMaterial.EnabledPrns)// 一颗卫星2行
                //{
                //    IEphemeris sat = CurrentMaterial[prn].Ephemeris;

                //    XYZ vector = sat.XYZ - this.CurrentMaterial.SiteInfo.EstimatedXyz;// this.ReceiverXyz;

                //    double wetMap = CurrentMaterial[prn].WetMap;

                //    double wetMap0 = CurrentMaterial[prn].Vmf1WetMap;

                //    A[row][0] = -vector.CosX;
                //    A[row][1] = -vector.CosY;
                //    A[row][2] = -vector.CosZ;
                //    A[row][3] = -1.0;            //接收机钟差对应的距离 = clkError * 光速
                //    A[row][4] = wetMap;// DryWetM[1];   //A[row][3] = 299792458.0;//光速

                //    int next = row + satCount;

                //    A[next][0] = A[row][0];
                //    A[next][1] = A[row][1];
                //    A[next][2] = A[row][2];
                //    A[next][3] = A[row][3];
                //    A[next][4] = A[row][4];
                //    A[next][5 + satIndex] =  1;
                //    //2016.10.10, czs, 保持以米单位，多系统下具有可比性，取消下面注释：
                //    //模糊度,保持以周为单位,，但不具有整周特性
                //    row++;
                //    satIndex++;
                //}
                #endregion
                foreach (var prn in CurrentMaterial.EnabledPrns)// 一颗卫星2行
                {
                    IEphemeris sat = CurrentMaterial[prn].Ephemeris;
                    XYZ vector = sat.XYZ - this.CurrentMaterial.SiteInfo.EstimatedXyz;// this.ReceiverXyz;    

                    double wetMap = CurrentMaterial[prn].WetMap;
                    double wetMap0 = CurrentMaterial[prn].Vmf1WetMap;

                    #region 只有一个卫星导航系统
                    if (Option.SatelliteTypes.Count == 1)
                    {                        
                        A[row][0] = -vector.CosX;
                        A[row][1] = -vector.CosY;
                        A[row][2] = -vector.CosZ;
                        A[row][3] = 1.0;            //接收机钟差对应的距离 = clkError * 光速
                        A[row][4] = wetMap;// DryWetM[1];   //A[row][3] = 299792458.0;//光速

                        int next = row + satCount;
                        A[next][0] = A[row][0];
                        A[next][1] = A[row][1];
                        A[next][2] = A[row][2];
                        A[next][3] = A[row][3];
                        A[next][4] = A[row][4];
                        A[next][5 + satIndex] = 1;                         
                    }
                    #endregion

                    #region 两个卫星导航系统，默认有GPS
                    if (Option.SatelliteTypes.Count == 2)
                    {
                        if (prn.SatelliteType == SatelliteType.G)
                        {
                            A[row][0] = -vector.CosX;
                            A[row][1] = -vector.CosY;
                            A[row][2] = -vector.CosZ;
                            A[row][3] = 1.0;            //接收机钟差对应的距离 = clkError * 光速
                            A[row][4] = 0;            //增加一个参数
                            A[row][5] = wetMap;// DryWetM[1];   //A[row][3] = 299792458.0;//光速

                            int next = row + satCount;
                            A[next][0] = A[row][0];
                            A[next][1] = A[row][1];
                            A[next][2] = A[row][2];
                            A[next][3] = A[row][3];
                            A[next][4] = A[row][4];
                            A[next][5] = A[row][5];
                            A[next][6 + satIndex] = 1;  
                        }

                        if (prn.SatelliteType == SatelliteType.E)
                        {
                            A[row][0] = -vector.CosX;
                            A[row][1] = -vector.CosY;
                            A[row][2] = -vector.CosZ;
                            A[row][3] = 1.0;            //接收机钟差对应的距离 = clkError * 光速
                            A[row][4] = 1.0;            //增加一个参数
                            A[row][5] = wetMap;// DryWetM[1];   //A[row][3] = 299792458.0;//光速

                            int next = row + satCount;
                            A[next][0] = A[row][0];
                            A[next][1] = A[row][1];
                            A[next][2] = A[row][2];
                            A[next][3] = A[row][3];
                            A[next][4] = A[row][4];
                            A[next][5] = A[row][5];
                            A[next][6 + satIndex] = 1;  
                        }
                        if (prn.SatelliteType == SatelliteType.C)
                        {
                            A[row][0] = -vector.CosX;
                            A[row][1] = -vector.CosY;
                            A[row][2] = -vector.CosZ;
                            A[row][3] = 1.0;            //接收机钟差对应的距离 = clkError * 光速
                            A[row][4] = 1.0;            //增加一个参数
                            A[row][5] = wetMap;// DryWetM[1];   //A[row][3] = 299792458.0;//光速

                            int next = row + satCount;
                            A[next][0] = A[row][0];
                            A[next][1] = A[row][1];
                            A[next][2] = A[row][2];
                            A[next][3] = A[row][3];
                            A[next][4] = A[row][4];
                            A[next][5] = A[row][5];
                            A[next][6 + satIndex] = 1;  
                        }
                    }
                    #endregion

                    #region 有三个卫星导航系统 GPS BDS Galileo
                    if (Option.SatelliteTypes.Count == 3)
                    {
                        if (prn.SatelliteType == SatelliteType.G)
                        {
                            A[row][0] = -vector.CosX;
                            A[row][1] = -vector.CosY;
                            A[row][2] = -vector.CosZ;
                            A[row][3] = 1.0;            //接收机钟差对应的距离 = clkError * 光速
                            A[row][4] = 0;            //增加系统间时间偏差
                            A[row][5] = 0;           //增加系统间时间偏差
                            A[row][6] = wetMap;// DryWetM[1];   //A[row][3] = 299792458.0;//光速

                            int next = row + satCount;
                            A[next][0] = A[row][0];
                            A[next][1] = A[row][1];
                            A[next][2] = A[row][2];
                            A[next][3] = A[row][3];
                            A[next][4] = A[row][4];
                            A[next][5] = A[row][5];
                            A[next][6] = A[row][6];
                            A[next][7 + satIndex] = 1;  //P2的电离层参数的系数
                        }
                        if (prn.SatelliteType == SatelliteType.E)
                        {
                            A[row][0] = -vector.CosX;
                            A[row][1] = -vector.CosY;
                            A[row][2] = -vector.CosZ;
                            A[row][3] = 1.0;            //接收机钟差对应的距离 = clkError * 光速
                            A[row][4] = 1.0;            //增加系统间时间偏差
                            A[row][5] = 0;           //增加系统间时间偏差
                            A[row][6] = wetMap;// DryWetM[1];   //A[row][3] = 299792458.0;//光速

                            int next = row + satCount;
                            A[next][0] = A[row][0];
                            A[next][1] = A[row][1];
                            A[next][2] = A[row][2];
                            A[next][3] = A[row][3];
                            A[next][4] = A[row][4];
                            A[next][5] = A[row][5];
                            A[next][6] = A[row][6];
                            A[next][7 + satIndex] = 1;  //P2的电离层参数的系数
                        }
                        if (prn.SatelliteType == SatelliteType.C)
                        {
                            A[row][0] = -vector.CosX;
                            A[row][1] = -vector.CosY;
                            A[row][2] = -vector.CosZ;
                            A[row][3] = 1.0;            //接收机钟差对应的距离 = clkError * 光速
                            A[row][4] = 0;            //增加系统间时间偏差
                            A[row][5] = 1.0;           //增加系统间时间偏差
                            A[row][6] = wetMap;// DryWetM[1];   //A[row][3] = 299792458.0;//光速

                            int next = row + satCount;
                            A[next][0] = A[row][0];
                            A[next][1] = A[row][1];
                            A[next][2] = A[row][2];
                            A[next][3] = A[row][3];
                            A[next][4] = A[row][4];
                            A[next][5] = A[row][5];
                            A[next][6] = A[row][6];
                            A[next][7 + satIndex] = 1;  
                        }
                    }
                    #endregion

                    row++;
                    satIndex++;
                }
                return new Matrix(A);
            }
        }
      
        #endregion  
    }//end class
}