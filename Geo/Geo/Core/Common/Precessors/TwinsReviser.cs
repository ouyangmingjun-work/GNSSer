﻿//2014.10.14，czs, create in hailutu, 处理器，输入对象本身改变对象内部结构，属于构造函数的延续。
//2014.12.17, czs, edit in namu, 让Process返回布尔值，判断其是否成功或中断执行。
using System;
using Geo.IO;

namespace Geo
{ 
    /// <summary>
    ///  双对象处理器，输入对象本身改变对象内部结构，属于构造函数的延续。
    /// 或者用于遍历数据。访问者设计模式的一种实现。
    /// </summary>
    /// <typeparam name="TMaster">待处理类型</typeparam>
    public interface ITwinsReviser<TMaster>
    {
        /// <summary>
        /// 处理数据,这里添加 ref 标记，是提醒：对象内容将会改变。成功则返回 true，发生无法继续进行错误，则返回 false。
        /// </summary>
        /// <param name="objA">待处理对象 A</param>
        /// <param name="objB">待处理对象 B</param>
        bool Revise(ref TMaster objA, ref TMaster objB);

        /// <summary>
        /// 执行过程信息回馈
        /// </summary>
        string Message { get; set; }
    }

    /// <summary>
    /// 双对象处理器，输入对象本身改变对象内部结构，属于构造函数的延续。
    /// 或者用于遍历数据。访问者设计模式的一种实现。
    /// </summary>
    /// <typeparam name="TMaster">待处理(主人，访问者)类型</typeparam>
    public abstract class TwinsReviser<TMaster> : ITwinsReviser<TMaster>
    {
        /// <summary>
        /// 访问处理器， 默认构造函数
        /// </summary> 
        /// <param name="name">处理器名称，推荐写上</param>
        public TwinsReviser(string name = "") { this.Name = name; }

        /// <summary>
        /// 日志记录。错误信息记录在日志里面。
        /// </summary>
        protected ILog log = Log.GetLog(typeof(Reviser<TMaster>));

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 执行信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="objA">待处理对象 A</param>
        /// <param name="objB">待处理对象 B</param>
        public abstract bool Revise(ref TMaster objA, ref TMaster objB);
    }
     
}
