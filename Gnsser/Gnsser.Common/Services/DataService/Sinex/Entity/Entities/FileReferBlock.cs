﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Geo.Coordinates;
using Geo.Utils;

namespace Gnsser.Data.Sinex
{

    public class FileReferBlockLabel
    {
        public const string DESCRIPTION = "DESCRIPTION";
        public const string OUTPUT = "OUTPUT";
        public const string CONTACT = "CONTACT";
        public const string SOFTWARE = "SOFTWARE";
        public const string HARDWARE = "HARDWARE";
        public const string INPUT = "INPUT";
    }
    /// <summary>
    /// 参考信息
    /// </summary>
    public class FileReferBlock : DataBlock
    {
        public string Description { get; set; }
        public string Output { get; set; }
        public string Contack { get; set; }
        public string Software { get; set; }
        public string Hardware { get; set; }
        public string Input { get; set; }

        public static FileReferBlock Read(StreamReader reader)
        {
            FileReferBlock fileRefferblock = new FileReferBlock();
            fileRefferblock.Comments = new List<string>();
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith(CharDefinition.COMMENT))
                {
                    fileRefferblock.Comments.Add(line.Trim());
                    continue;//注释
                }
                if (line.StartsWith(CharDefinition.TITLE_END)) break;
                line = line.Trim();
                if (line.Length <= 19) continue;
                if (line.StartsWith(FileReferBlockLabel.DESCRIPTION))
                    fileRefferblock.Description = line.Substring(19).Trim();
                if (line.StartsWith(FileReferBlockLabel.OUTPUT))
                    fileRefferblock.Output = line.Substring(19).Trim();
                if (line.StartsWith(FileReferBlockLabel.CONTACT))
                    fileRefferblock.Contack = line.Substring(19).Trim();
                if (line.StartsWith(FileReferBlockLabel.SOFTWARE))
                    fileRefferblock.Software = line.Substring(19).Trim();
                if (line.StartsWith(FileReferBlockLabel.HARDWARE))
                    fileRefferblock.Hardware = line.Substring(19).Trim();
                if (line.StartsWith(FileReferBlockLabel.INPUT))
                    fileRefferblock.Input = line.Substring(19).Trim();

            }
            return fileRefferblock;
        }

        /**
         * DESCRIPTION        My agency/institute                                        
         * OUTPUT             One-session solution generated by RNX2SNX BerBpe               
         * CONTACT            My east-mail address                                           
         * SOFTWARE           Bernese GPS Software Version 5.0                            
         * HARDWARE           My computer                                                 
         * INPUT              IGS/IGLOS GNSS tracking satData 
         */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("+FILE/REFERENCE ");
            if(Comments!= null) foreach (var item in Comments) sb.AppendLine(item);
            sb.AppendLine(" " + StringUtil.FillSpace(FileReferBlockLabel.DESCRIPTION, 18) + " " + StringUtil.FillSpace(this.Description, 60));
            sb.AppendLine(" " + StringUtil.FillSpace(FileReferBlockLabel.OUTPUT, 18) + " " + StringUtil.FillSpace(this.Output, 60));
            sb.AppendLine(" " + StringUtil.FillSpace(FileReferBlockLabel.CONTACT, 18) + " " + StringUtil.FillSpace(this.Contack, 60));
            sb.AppendLine(" " + StringUtil.FillSpace(FileReferBlockLabel.SOFTWARE, 18) + " " + StringUtil.FillSpace(this.Software, 60));
            sb.AppendLine(" " + StringUtil.FillSpace(FileReferBlockLabel.HARDWARE, 18) + " " + StringUtil.FillSpace(this.Hardware, 60));
            sb.AppendLine(" " + StringUtil.FillSpace(FileReferBlockLabel.INPUT, 18) + " " + StringUtil.FillSpace(this.Input, 60));
            sb.Append("-FILE/REFERENCE ");
            return sb.ToString();
        }
    }


}
