﻿//2015.10.24, czs, create in 彭州, 单点定位

using System;
using System.IO;
using Geo.Common;
using Geo.Coordinates;
using System.Collections;
using System.Collections.Generic;
using Geo.IO;

namespace Gnsser.Api
{
    /// <summary>
    ///复制的参数文件读取
    /// </summary>
    public class DoubleDifferParamReader : LineFileReader<DoubleDifferParam>
    {  
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DoubleDifferParamReader() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="metaFilePath"></param>
        public DoubleDifferParamReader(string filePath, string metaFilePath = null)
            : base(filePath, metaFilePath)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="Gmetadata"></param>
        public DoubleDifferParamReader(string filePath, Gmetadata Gmetadata)
            : base(filePath, Gmetadata)
        {
        }

        /// <summary>
        /// 字符串列表解析为属性
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public override DoubleDifferParam Parse(string[] items)
        {
            var row = base.Parse(items);
            row.InputPath = Geo.Utils.PathUtil.GetAbsPath(row.InputPath, this.BaseDirectory);
            row.OutputPath = Geo.Utils.PathUtil.GetAbsPath(row.OutputPath, this.BaseDirectory);
            row.EphemerisPath = Geo.Utils.PathUtil.GetAbsPath(row.EphemerisPath, this.BaseDirectory);
            row.ClockPath = Geo.Utils.PathUtil.GetAbsPath(row.ClockPath, this.BaseDirectory);
            row.SiteInfoPath = Geo.Utils.PathUtil.GetAbsPath(row.SiteInfoPath, this.BaseDirectory);
            row.BaselinePath = Geo.Utils.PathUtil.GetAbsPath(row.BaselinePath, this.BaseDirectory); 

            return row;
        }

    }
}
