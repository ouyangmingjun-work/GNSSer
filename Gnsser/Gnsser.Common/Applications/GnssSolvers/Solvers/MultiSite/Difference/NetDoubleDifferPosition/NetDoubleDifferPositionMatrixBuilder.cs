﻿//2018.11.02, czs, create in HMX, 双差网解定位
//2018.12.31, czs, edit in hmx, 实现模糊度固定

using System;
using System.Collections.Generic;
using System.Text;
using Geo.Algorithm; 
using Geo.Utils;
using Geo.Common;
using Geo.Coordinates; 
using Geo.Algorithm.Adjust;
using Geo.Times;
using Gnsser;
using Gnsser.Times;
using Gnsser.Service;
using Gnsser.Domain;
using Gnsser.Checkers;
using Gnsser.Models;
using Gnsser.Data.Rinex;

namespace Gnsser.Service
{
    /// <summary>
    /// 双差网解定位
    /// </summary>
    public class NetDoubleDifferPositionMatrixBuilder : MultiSiteMatrixBuilder// BasePositionMatrixBuilder
    {
        /// <summary>
        /// 无电离层双差矩阵构造器
        /// </summary>
        /// <param name="option"></param>
        /// <param name="baseParamCount"></param>
        public NetDoubleDifferPositionMatrixBuilder(
            GnssProcessOption option, int baseParamCount)
            : base(option)
        {
            IsEstimateTropWetZpd = option.IsEstimateTropWetZpd;
            this.ParamNameBuilder = new NetDoubleDifferPositionParamNameBuilder(option);
            this.BaseParamCount = baseParamCount;

            this.IsDualIonoFreeComObservation = Option.IsDualIonoFreeComObservation; 
        }
        /// <summary>
        /// 是否估计对流层湿延迟参数。
        /// </summary>
        public bool IsEstimateTropWetZpd { get; set; }

        /// <summary>
        /// 是否采用双频无电离层组合观测值
        /// </summary>
        public bool IsDualIonoFreeComObservation { get; set; }

        /// <summary>
        /// 参数是否改变。
        /// </summary>
        public override bool IsParamsChanged
        {
            get
            {
                return base.IsParamsChanged || this.IsBaseSatUnstable;
            }
        }  

        /// <summary>
        /// 基础参数的总数，即除了模糊度的剩余参数的个数
        /// 长基线时是5，即三个坐标参数+两个对流层参数
        /// 短基线时是3，即三个坐标参数
        /// </summary>
        public int BaseParamCount { get; set; } 
        /// <summary>
        /// 构建
        /// </summary>
        public override void Build()
        {
            if (String.IsNullOrWhiteSpace(BaseSiteName))
            {
                BaseSiteName = CurrentMaterial.BaseEpochInfo.SiteName;
                log.Info("没有设置基站, 基准测站设置为 " + this.BaseSiteName);
            }



            //本类所独有的
            GnssParamNameBuilder.BaseParamCount = BaseParamCount;

            foreach (var item in CurrentMaterial)
            {
                if (item[CurrentBasePrn].IsUnstable)
                {
                    IsBaseSatUnstable = true;
                }
            }

            base.Build();
        }

        /// <summary>
        /// 观测值数量,伪距加载波，除去一个测站，一个卫星。
        /// </summary>
        public override int ObsCount { get { return 2 * (EnabledSatCount - 1) * (SiteCount - 1 ); } }
        /// <summary>
        /// 测站数量
        /// </summary>
        public override int SiteCount { get => CurrentMaterial.Count; }


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
                deltaObs.ParamNames = BuildObsNames();
                 
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
                int enableSatCount = this.EnabledSatCount;
                //默认为无电离层组合
                SatObsDataType rangeSatObsDataType = SatObsDataType.IonoFreeRange;
                SatObsDataType phaseSatObsDataType = SatObsDataType.IonoFreePhaseRange;
                SatApproxDataType rangeSatApproxDataType = SatApproxDataType.IonoFreeApproxPseudoRange;
                SatApproxDataType phaseSatApproxDataType = SatApproxDataType.IonoFreeApproxPhaseRange;

                //若不采用无电离层组合，则采用单频计算。适用于小区域布网,如阵地
                if (!IsDualIonoFreeComObservation && Option.ObsPhaseType != ObsPhaseType.L1AndL2)
                {
                    if (Option.ObsDataType.ToString().Contains(FrequenceType.A.ToString()))
                    {
                        rangeSatObsDataType = SatObsDataType.PseudoRangeA;
                        phaseSatObsDataType = SatObsDataType.PhaseRangeA;
                        rangeSatApproxDataType = SatApproxDataType.ApproxPseudoRangeA;
                        phaseSatApproxDataType = SatApproxDataType.ApproxPhaseRangeA;

                    }
                    else if (Option.ObsDataType.ToString().Contains(FrequenceType.B.ToString()))
                    {
                        rangeSatObsDataType = SatObsDataType.PseudoRangeB;
                        phaseSatObsDataType = SatObsDataType.PhaseRangeB;
                        rangeSatApproxDataType = SatApproxDataType.ApproxPseudoRangeB;
                        phaseSatApproxDataType = SatApproxDataType.ApproxPhaseRangeB;
                    }
                    //直接指定AB
                    if (Option.ObsPhaseType == ObsPhaseType.L1)
                    {
                        rangeSatObsDataType = SatObsDataType.PseudoRangeA;
                        phaseSatObsDataType = SatObsDataType.PhaseRangeA;
                        rangeSatApproxDataType = SatApproxDataType.ApproxPseudoRangeA;
                        phaseSatApproxDataType = SatApproxDataType.ApproxPhaseRangeA;

                    }
                    else if (Option.ObsPhaseType == ObsPhaseType.L2)
                    {
                        rangeSatObsDataType = SatObsDataType.PseudoRangeB;
                        phaseSatObsDataType = SatObsDataType.PhaseRangeB;
                        rangeSatApproxDataType = SatApproxDataType.ApproxPseudoRangeB;
                        phaseSatApproxDataType = SatApproxDataType.ApproxPhaseRangeB;
                    }
                }
              

                Vector rangeObs = CurrentMaterial.GetDoubleDifferResidualVectorBeweenSats(rangeSatObsDataType, rangeSatApproxDataType, this.BaseSiteName, CurrentBasePrn);
                Vector phaseObs = CurrentMaterial.GetDoubleDifferResidualVectorBeweenSats(phaseSatObsDataType, phaseSatApproxDataType, this.BaseSiteName, CurrentBasePrn);

                //--------------------check----------------
                if (false)
                {
                    Vector rangeObs1 = CurrentMaterial.GetNetDoubleDifferResidualVectorBeweenSites(rangeSatObsDataType, rangeSatApproxDataType, this.BaseSiteName, CurrentBasePrn);
                    if (rangeObs1.Equals(rangeObs))//【校验通过】
                    {
                        int iii = 0;
                    }
                }
                //-----------------end check -------------------


                Vector obsMinusAppMain = new Vector(this.ObsCount);
                int siteCount = SiteCount;

                for (int i = 0; i < this.ObsCount; i++)
                {
                    if (i < rangeObs.Count)
                    {
                        obsMinusAppMain[i] = rangeObs[i];
                    }
                    else
                    {
                        int index = i - phaseObs.Count;
                        obsMinusAppMain[i] = phaseObs[index];
                    }
                }
                //WeightedVector deltaObs = new WeightedVector(obsMinusAppMain, BulidInverseWeightOfObs()) { ParamNames = GetObsNames() } ;
                //return deltaObs;
                return obsMinusAppMain;
            }
        }

        /// <summary>
        /// 观测量的权逆阵。按照先伪距，后载波的顺序排列。
        /// 标准差由常数确定，载波比伪距标准差通常是 1:100，其方差比是 1:10000.
        /// 1.0/140*140=5.10e-5
        /// </summary> 
        /// <returns></returns>
        protected IMatrix BulidInverseWeightOfObs()
        {
            DiagonalMatrix primevalCova = BuildPrmevalObsCovaMatrix();
            Matrix undiffInverseWeigth = new Matrix(primevalCova);
            DifferCoeffBuilder differCoeffBuilder = new DifferCoeffBuilder(CurrentMaterial, BaseSiteName, CurrentBasePrn);
            Matrix sigleCoeff = differCoeffBuilder.BuildDifferCoeefOfBetweenSats(); //构建星间单差系数阵的转换矩阵
            //单差协因数阵
            var singleDifferCova = sigleCoeff * undiffInverseWeigth * sigleCoeff.Transposition;

            //组建双差系数阵
            Matrix doubleCoeff = differCoeffBuilder.BuildDifferCoeefBetweenSites();

            //误差传播定律
            var doubleDifferCova = doubleCoeff * singleDifferCova * doubleCoeff.Transposition;

            return doubleDifferCova;
        }


        /// <summary>
        /// 观测名称
        /// </summary>
        /// <returns></returns>
        public List<string> BuildObsNames()
        {
            var names = new string[ObsCount];
            int rangeRow = 0;
            int siteCount = CurrentMaterial.Count;
            int rangeRowCount = this.ObsCount / 2;
            foreach (var site in CurrentMaterial)
            {
                var siteName = site.SiteName;
                if (String.Equals(siteName, BaseSiteName, StringComparison.CurrentCultureIgnoreCase)) { continue; }

                foreach (var prn in this.EnabledPrns)
                {
                    if (prn == CurrentBasePrn) { continue; }

                    int phaseRow = rangeRow + rangeRowCount;
                    names[rangeRow] = GnssParamNameBuilder.GetDoubleDifferObsPCodeName(siteName, BaseSiteName, prn, CurrentBasePrn);
                    names[phaseRow] = GnssParamNameBuilder.GetDoubleDifferObsLCodeName(siteName, BaseSiteName, prn, CurrentBasePrn);
                    rangeRow++;
                }
            }
            return new List<string>(names);
        }

        #endregion

        #region 公共矩阵生成

        #region 系数矩阵生成 

        /// <summary>
        /// 参数平差系数阵 A。前一半行数为伪距观测量，后一半为载波观测量。构建为稀疏矩阵
        /// </summary>
        public override Matrix Coefficient
        {
            get
            {
                var paramNameBuilder = (NetDoubleDifferPositionParamNameBuilder)this.ParamNameBuilder;

                var A = new Matrix(ObsCount, ParamCount);
                if (ObsCount < ParamCount)
                {
                    log.Warn("小心：观测方程少于参数数量： " + ObsCount + " < " + ParamCount + ", " + this.CurrentMaterial);
                }
                //相位模糊度系数，可以为米和周，按照设置文件决定。
                double coeefOfPhase = CheckAndGetCoeefOfPhase();

                int rangeRowCount = this.ObsCount / 2;
                int rangeRow = 0;//行的索引号，对应观测方程行
                int colIndex = 0;//列索引，对应参数编号
                                 //第一次为伪距，第二次为载波

                var baseSite = this.CurrentMaterial[BaseSiteName];
                var baseZpdname = paramNameBuilder.GetSiteWetTropZpdName(BaseSiteName);
                int colIndexOfBaseZpdname = this.ParamNames.IndexOf(baseZpdname); //基准站对流层位置相同，但是各颗卫星系数（映射函数）不同

                foreach (var site in CurrentMaterial)
                {
                    var siteName = site.SiteName;
                    if (siteName == BaseSiteName) { continue; }

                    //1.测站相关 
                    //基准站对流层
                    var name = paramNameBuilder.GetSiteWetTropZpdName(siteName);
                    int colIndexOfZpd = this.ParamNames.IndexOf(name);
                    var refSiteRefSat = baseSite[CurrentBasePrn];//基准站的当前星，用于获取对流层映射函数值

                    //当前站的基准卫星
                    var rovSiteRefSat = site[CurrentBasePrn];
                    var baseVector = rovSiteRefSat.EstmatedVector;

                    //2.卫星相关，或站星相关
                    foreach (var sat in site.EnabledSats)
                    {
                        var prn = sat.Prn;
                        if (prn == CurrentBasePrn) { continue; }

                        int phaseRow = rangeRow + rangeRowCount;
                        var vector = sat.EstmatedVector;

                        //测站坐标
                        var satXyzNames = paramNameBuilder.GetSiteDxyz(site.SiteName);
                        colIndex = this.ParamNames.IndexOf(satXyzNames[0]);
                        A[rangeRow, colIndex + 0] = -(vector.CosX - baseVector.CosX);
                        A[rangeRow, colIndex + 1] = -(vector.CosY - baseVector.CosY);
                        A[rangeRow, colIndex + 2] = -(vector.CosZ - baseVector.CosZ);
                        A[phaseRow, colIndex + 0] = A[rangeRow, colIndex + 0];
                        A[phaseRow, colIndex + 1] = A[rangeRow, colIndex + 1];
                        A[phaseRow, colIndex + 2] = A[rangeRow, colIndex + 2];

                        if (IsEstimateTropWetZpd)
                        {
                            //基准测站对流层湿延迟
                            var baseMapDiffer = baseSite[prn].WetMap - refSiteRefSat.WetMap;
                            A[rangeRow, colIndexOfBaseZpdname] = -baseMapDiffer;
                            A[phaseRow, colIndexOfBaseZpdname] = -baseMapDiffer;

                            //流动站对流层湿延迟天顶距
                            var zpdName = paramNameBuilder.GetSiteWetTropZpdName(site);
                            colIndex = this.ParamNames.IndexOf(zpdName);
                            var rovMapDiffer = sat.WetMap - rovSiteRefSat.WetMap;
                            A[rangeRow, colIndex] = rovMapDiffer;
                            A[phaseRow, colIndex] = rovMapDiffer;
                        }

                        //载波相位模糊度，只有相位才有此参数
                        var ambi = paramNameBuilder.GetDoubleDifferAmbiParamName(siteName, sat.Prn);
                        colIndex = this.ParamNames.IndexOf(ambi);
                        A[phaseRow, colIndex] = coeefOfPhase;

                        rangeRow++;
                    }
                }
                A.ColNames = ParamNames;
                A.RowNames = BuildObsNames();
                return A;
            }
        }


        #endregion

        #endregion
    }//end class
}