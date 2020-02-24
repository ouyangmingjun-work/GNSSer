﻿//2018.11.27, czs, create in HMX, 模糊度固定的纯载波双差

using System;
using System.Collections.Generic;
using Gnsser.Domain;
using System.Text;
using Geo.Coordinates;
using Gnsser.Data.Rinex;
using Geo.Algorithm;
using Geo.Algorithm.Adjust;
using Gnsser.Service;
using Geo.Utils;
using Gnsser.Filter;
using Gnsser.Checkers;
using Geo.Common; 


namespace Gnsser.Service
{
    /// <summary>
    /// 模糊度固定的纯载波双差
    /// </summary>
    public class AmbiFixedEpochDoublePhaseOnlytDifferResult : DualSiteEpochDoubleDifferResult
    {

        /// <summary>
        /// 简易近距离单历元载波相位双差
        /// </summary>
        /// <param name="material"></param>
        /// <param name="Adjustment"></param>
        /// <param name="positioner"></param>
        /// <param name="baseSatPrn"></param>
        public AmbiFixedEpochDoublePhaseOnlytDifferResult(
            MultiSiteEpochInfo material,
            AdjustResultMatrix Adjustment,
             GnssParamNameBuilder positioner,
            SatelliteNumber baseSatPrn
            )
            : base(material, Adjustment, positioner)
        {

            this.BasePrn = baseSatPrn;
            this.Option = positioner.Option;
        }

        /// <summary>
        /// 获取模糊度参数估计结果。单位为周，权逆阵单位依然为米。
        /// </summary>
        /// <returns></returns>
        public WeightedVector GetFixableVectorInUnit()
        {
       
            this.ResultMatrix.Estimated.ParamNames = this.ResultMatrix.ParamNames;
            //Lamdba方法计算模糊度
            var ambiParamNames = new List<string>();
            foreach (var name in ResultMatrix.ParamNames)
            {
                if (HasUnstablePrn(name)) { continue; }

                if (name.Contains(Gnsser.ParamNames.DoubleDifferAmbiguity))
                { ambiParamNames.Add(name); }
            }
            var estVector = this.ResultMatrix.Estimated.GetWeightedVector(ambiParamNames);
            if (Option.IsPhaseInMetterOrCycle)
            {
                //更新模糊度。
                Frequence Frequence = Gnsser.Frequence.GetFrequence(this.BasePrn, Option.ObsDataType, this.Material.ReceiverTime);
                var vectorInCycle = estVector * (1.0 / Frequence.WaveLength);
                return vectorInCycle;
            }
            return estVector;
        }

        /// <summary>
        /// 固定后的模糊度,单位周，值应该为整数。
        /// </summary>
        public WeightedVector FixedParams { get; set; }

    }
}
