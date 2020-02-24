﻿
//2016.12.4, double, create in hongqing, 将SBS SSR信息整合到一起

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geo.Utils;

namespace Gnsser.Ntrip.Rtcm
{
    /// <summary>
    /// 160bit. 
    /// </summary>
    public class Message1252 : Message1060
    {
        public Message1252()
        {
           this.Length = 135;
        }
        public new static Message1252 Parse(List<byte> data)
        {
            //首先化为二进制字符串
            string binStr = BitConvertUtil.GetBinString(data);
            return Parse(binStr);
        }
        public new static Message1252 Parse(string binStr)
        {
            StringSequence sequence = new StringSequence();
            sequence.EnQuence(binStr);

            Message1252 msg = new Message1252();
            msg.SatelliteID = BitConvertUtil.GetUInt(sequence.DeQueue(6));
            msg.IODE = BitConvertUtil.GetUInt(sequence.DeQueue(9));
            msg.IodCrc = BitConvertUtil.GetUInt(sequence.DeQueue(24));
            msg.DeltaRadial = BitConvertUtil.GetInt(sequence.DeQueue(22)); 
            msg.DeltaAlongTrack = BitConvertUtil.GetInt(sequence.DeQueue(20));
            msg.DeltaCrossTrack = BitConvertUtil.GetInt(sequence.DeQueue(20));  
            msg.DotDeltaRadial = BitConvertUtil.GetInt(sequence.DeQueue(21));
            msg.DotDeltaAlongTrack = BitConvertUtil.GetInt(sequence.DeQueue(19));
            msg.DotDeltaCrossTrack = BitConvertUtil.GetInt(sequence.DeQueue(19)); 

            return msg;
        }
    }

    /// <summary>
    /// 76bit.
    /// </summary>
    public class Message1253 : Message1060
    {
        public Message1253()
        {
            this.Length = 76;
        }
        public new static Message1253 Parse(List<byte> data)
        {
            //首先化为二进制字符串
            string binStr = BitConvertUtil.GetBinString(data);
            return Parse(binStr);
        }
        public new static Message1253 Parse(string binStr)
        {
            StringSequence sequence = new StringSequence();
            sequence.EnQuence(binStr);

            Message1253 msg = new Message1253();
            msg.SatelliteID = BitConvertUtil.GetUInt(sequence.DeQueue(6));
            msg.DeltaClockC0 = BitConvertUtil.GetInt(sequence.DeQueue(22));
            msg.DeltaClockC1 = BitConvertUtil.GetInt(sequence.DeQueue(21));
            msg.DeltaClockC2 = BitConvertUtil.GetInt(sequence.DeQueue(27));

            return msg;
        }
    }
    /// <summary>
    /// 30bit.
    /// Satellite Specific Part of the SSR SBS Satellite Code Bias  11bit
    /// Code Specific Part of the SSR SBS Satellite Code Bias   19bit
    /// </summary>
    public class Message1254 : BaseRtcmMessage
    {
        public Message1254()
        {
            this.Length = 30;
        }
        /// <summary>
        /// uint6 , 6
        /// </summary>
        public uint SatelliteID { get; set; }
        /// <summary>
        /// uint5
        /// </summary>
        public uint NoofCodeBiasProcess { get; set; }
        /// <summary>
        /// uint5
        /// </summary>
        public uint SignalandTrackingModeIndicator { get; set; }
        /// <summary>
        /// int14
        /// </summary>
        public int CodeBias { get; set; }

        public static Message1254 Parse(List<byte> data)
        {
            //首先化为二进制字符串
            string binStr = BitConvertUtil.GetBinString(data);
            return Parse(binStr);
        }

        public static Message1254 Parse(string binStr)
        {
            StringSequence sequence = new StringSequence();
            sequence.EnQuence(binStr);

            Message1254 msg = new Message1254();
            //Satellite Specific Part of the SSR SBS Satellite Code Bias 
            msg.SatelliteID = BitConvertUtil.GetUInt(sequence.DeQueue(6));
            msg.NoofCodeBiasProcess = BitConvertUtil.GetUInt(sequence.DeQueue(5));
            //Code Spoecific Part of the SSR SBS Satellite Code Bias 
            msg.SignalandTrackingModeIndicator = BitConvertUtil.GetUInt(sequence.DeQueue(5));
            msg.CodeBias = BitConvertUtil.GetInt(sequence.DeQueue(14));

            return msg;
        }
    }
    /// <summary>
    /// 正常情况下的观测值，没有压缩
    /// </summary>
    public class NormalMessage1255 : NormalSSRMessage
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Message1261"></param>
        public NormalMessage1255(Message1255 Message1255)
        {
            this.Message1255 = Message1255;
        }

        #region 核心属性

        /// <summary>
        /// 报文 1261.
        /// </summary>
        public Message1255 Message1255 { get; set; }
        #endregion

        /// <summary>
        /// 测站编号
        /// </summary>
        public uint ReferenceStationID { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        // public Time Time { get { return new Time(DateTime.UtcNow, Header.SBSEpochTimeInMs / 1000.0); } }

        /// <summary>
        /// 卫星编号
        /// </summary>
        public SatelliteNumber Prn { get { return new SatelliteNumber((int)Message1255.SatelliteID, SatelliteType.S); } }
    }
    /// <summary>
    /// 230bit. 
    /// </summary>
    public class Message1255 : Message1060
    {
        public Message1255()
        {
            this.Length = 205;
        }

        public new static Message1255 Parse(List<byte> data)
        {
            //首先化为二进制字符串
            string binStr = BitConvertUtil.GetBinString(data);
            return Parse(binStr);
        }

        public new static Message1255 Parse(string binStr)
        {
            StringSequence sequence = new StringSequence();
            sequence.EnQuence(binStr);

            Message1255 msg = new Message1255();
            msg.SatelliteID = BitConvertUtil.GetUInt(sequence.DeQueue(6));
            msg.IODE = BitConvertUtil.GetUInt(sequence.DeQueue(9));
            msg.IodCrc = BitConvertUtil.GetUInt(sequence.DeQueue(24));
            msg.DeltaRadial = BitConvertUtil.GetInt(sequence.DeQueue(22));
            msg.DeltaAlongTrack = BitConvertUtil.GetInt(sequence.DeQueue(20));
            msg.DeltaCrossTrack = BitConvertUtil.GetInt(sequence.DeQueue(20));
            msg.DotDeltaRadial = BitConvertUtil.GetInt(sequence.DeQueue(21));
            msg.DotDeltaAlongTrack = BitConvertUtil.GetInt(sequence.DeQueue(19));
            msg.DotDeltaCrossTrack = BitConvertUtil.GetInt(sequence.DeQueue(19));
            msg.DeltaClockC0 = BitConvertUtil.GetInt(sequence.DeQueue(22));
            msg.DeltaClockC1 = BitConvertUtil.GetInt(sequence.DeQueue(21));
            msg.DeltaClockC2 = BitConvertUtil.GetInt(sequence.DeQueue(27));

            return msg;
        }
    }
    /// <summary>
    /// 12bit. 
    /// Satellite Specific Part of the SSR SBS URA
    /// </summary>
    public class Message1256 : BaseRtcmMessage
    {
        public Message1256()
        {
            this.Length = 12;
        }
        /// <summary>
        /// uint6
        /// </summary>
        public uint SatelliteID { get; set; }
        /// <summary>
        /// bit(6)
        /// </summary>
        public uint SSRURA { get; set; }

        public static Message1256 Parse(List<byte> data)
        {
            //首先化为二进制字符串
            string binStr = BitConvertUtil.GetBinString(data);
            return Parse(binStr);
        }

        public static Message1256 Parse(string binStr)
        {
            StringSequence sequence = new StringSequence();
            sequence.EnQuence(binStr);

            Message1256 msg = new Message1256();
            msg.SatelliteID = BitConvertUtil.GetUInt(sequence.DeQueue(6));
            msg.SSRURA = BitConvertUtil.GetUInt(sequence.DeQueue(6));

            return msg;
        }
    }
    /// <summary>
    /// 28bit. 
    /// Satellite Specific Part of the SSR SBS High Rate Clock
    /// </summary>
    public class Message1257 : BaseRtcmMessage
    {
        public Message1257()
        {
            this.Length = 28;
        }
        /// <summary>
        /// uint6
        /// </summary>
        public uint SatelliteID { get; set; }
        /// <summary>
        /// int22
        /// </summary>
        public int HighRateClockCorrection { get; set; }

        public static Message1257 Parse(List<byte> data)
        {
            //首先化为二进制字符串
            string binStr = BitConvertUtil.GetBinString(data);
            return Parse(binStr);
        }

        public static Message1257 Parse(string binStr)
        {
            StringSequence sequence = new StringSequence();
            sequence.EnQuence(binStr);

            Message1257 msg = new Message1257();
            msg.SatelliteID = BitConvertUtil.GetUInt(sequence.DeQueue(6));
            msg.HighRateClockCorrection = BitConvertUtil.GetInt(sequence.DeQueue(22));

            return msg;
        }
    }
}