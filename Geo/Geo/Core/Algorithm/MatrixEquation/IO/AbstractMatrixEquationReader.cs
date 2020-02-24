﻿//2019.02.20, czs, create in hongqing, 矩阵方程读取

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geo.IO;
using System.IO;
using System.Text;

namespace Geo.Algorithm
{
    /// <summary>
    /// 矩阵方程读取
    /// </summary>
    public abstract class AbstractMatrixEquationReader : AbstractReader<MatrixEquation>
    { 
        /// <summary>
      /// 本地文件读取
      /// </summary>
      /// <param name="path"></param>
      /// <param name="encoding"></param>
        public AbstractMatrixEquationReader(string path, Encoding encoding = null)
            : base(path, encoding)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="encoding">编码</param>
        public AbstractMatrixEquationReader(Stream stream, Encoding encoding) :
        base(stream, encoding)
        {

        }
    }
}
