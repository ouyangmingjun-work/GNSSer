﻿namespace Gnsser.Winform
{
    partial class OFileToTableForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        { 
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage0
            // 
            this.tabPage0.Size = new System.Drawing.Size(567, 240);
            // 
            // tabPage2 
            // 
            // panel3
            // 
            this.panel3.Size = new System.Drawing.Size(593, 120);
            // 
            // panel2
            // 
            this.panel2.Size = new System.Drawing.Size(593, 120);
            // 
            // panel4
            // 
            this.panel4.Size = new System.Drawing.Size(561, 120);
            // 
            // fileOpenControl1
            // 
            this.fileOpenControl_inputPathes.FilePathes = new string[0];
            this.fileOpenControl_inputPathes.Size = new System.Drawing.Size(561, 98);
            // 
            // tabPage1
            // 
            this.tabPage1.Size = new System.Drawing.Size(567, 126);
            // 
            // OFileToTableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 508);
            this.IsShowProgressBar = true;
            this.Name = "OFileToTableForm";
            this.Text = "格式化转换Rinex观测文件";
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion



    }
}

