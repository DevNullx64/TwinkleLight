
namespace LedControl
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.pbSelectColor = new System.Windows.Forms.PictureBox();
            this.pbLightColor = new System.Windows.Forms.PictureBox();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.chkDustMode = new System.Windows.Forms.CheckBox();
            this.chkDisableTP = new System.Windows.Forms.CheckBox();
            this.chkEnableLight = new System.Windows.Forms.CheckBox();
            this.rbLightDynamic = new System.Windows.Forms.RadioButton();
            this.rbLightSingle = new System.Windows.Forms.RadioButton();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSelectColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLightColor)).BeginInit();
            this.pnlOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(94, 26);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pbSelectColor
            // 
            this.pbSelectColor.BackColor = System.Drawing.Color.Transparent;
            this.pbSelectColor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbSelectColor.Location = new System.Drawing.Point(30, 14);
            this.pbSelectColor.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pbSelectColor.Name = "pbSelectColor";
            this.pbSelectColor.Size = new System.Drawing.Size(131, 139);
            this.pbSelectColor.TabIndex = 5;
            this.pbSelectColor.TabStop = false;
            this.pbSelectColor.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseClick);
            // 
            // pbLightColor
            // 
            this.pbLightColor.BackColor = System.Drawing.Color.DimGray;
            this.pbLightColor.Location = new System.Drawing.Point(96, 37);
            this.pbLightColor.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pbLightColor.Name = "pbLightColor";
            this.pbLightColor.Size = new System.Drawing.Size(57, 16);
            this.pbLightColor.TabIndex = 3;
            this.pbLightColor.TabStop = false;
            // 
            // pnlOptions
            // 
            this.pnlOptions.BackColor = System.Drawing.Color.Transparent;
            this.pnlOptions.Controls.Add(this.chkDustMode);
            this.pnlOptions.Controls.Add(this.chkDisableTP);
            this.pnlOptions.Controls.Add(this.chkEnableLight);
            this.pnlOptions.Controls.Add(this.rbLightDynamic);
            this.pnlOptions.Controls.Add(this.rbLightSingle);
            this.pnlOptions.Controls.Add(this.pbLightColor);
            this.pnlOptions.Location = new System.Drawing.Point(186, 16);
            this.pnlOptions.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(158, 142);
            this.pnlOptions.TabIndex = 6;
            // 
            // chkDustMode
            // 
            this.chkDustMode.AutoSize = true;
            this.chkDustMode.Font = new System.Drawing.Font("Calibri", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDustMode.ForeColor = System.Drawing.Color.White;
            this.chkDustMode.Location = new System.Drawing.Point(3, 110);
            this.chkDustMode.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.chkDustMode.Name = "chkDustMode";
            this.chkDustMode.Size = new System.Drawing.Size(133, 22);
            this.chkDustMode.TabIndex = 14;
            this.chkDustMode.Text = "Dust/Hight mode";
            this.chkDustMode.UseVisualStyleBackColor = true;
            this.chkDustMode.CheckedChanged += new System.EventHandler(this.CheckBox2_CheckedChanged);
            // 
            // chkDisableTP
            // 
            this.chkDisableTP.AutoSize = true;
            this.chkDisableTP.Font = new System.Drawing.Font("Calibri", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDisableTP.ForeColor = System.Drawing.Color.White;
            this.chkDisableTP.Location = new System.Drawing.Point(3, 82);
            this.chkDisableTP.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.chkDisableTP.Name = "chkDisableTP";
            this.chkDisableTP.Size = new System.Drawing.Size(90, 22);
            this.chkDisableTP.TabIndex = 13;
            this.chkDisableTP.Text = "Disable Tp";
            this.chkDisableTP.UseVisualStyleBackColor = true;
            this.chkDisableTP.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // chkEnableLight
            // 
            this.chkEnableLight.AutoSize = true;
            this.chkEnableLight.Font = new System.Drawing.Font("Calibri", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnableLight.ForeColor = System.Drawing.Color.White;
            this.chkEnableLight.Location = new System.Drawing.Point(3, 13);
            this.chkEnableLight.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.chkEnableLight.Name = "chkEnableLight";
            this.chkEnableLight.Size = new System.Drawing.Size(109, 22);
            this.chkEnableLight.TabIndex = 12;
            this.chkEnableLight.Text = "Twinkle Light";
            this.chkEnableLight.UseVisualStyleBackColor = true;
            this.chkEnableLight.CheckedChanged += new System.EventHandler(this.CheckBox3_CheckedChanged);
            // 
            // rbLightDynamic
            // 
            this.rbLightDynamic.AutoSize = true;
            this.rbLightDynamic.Font = new System.Drawing.Font("Calibri", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbLightDynamic.ForeColor = System.Drawing.Color.White;
            this.rbLightDynamic.Location = new System.Drawing.Point(7, 54);
            this.rbLightDynamic.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.rbLightDynamic.Name = "rbLightDynamic";
            this.rbLightDynamic.Size = new System.Drawing.Size(79, 22);
            this.rbLightDynamic.TabIndex = 1;
            this.rbLightDynamic.TabStop = true;
            this.rbLightDynamic.Text = "Dynamic";
            this.rbLightDynamic.UseVisualStyleBackColor = true;
            this.rbLightDynamic.CheckedChanged += new System.EventHandler(this.RadioButton2_CheckedChanged);
            // 
            // rbLightSingle
            // 
            this.rbLightSingle.AutoSize = true;
            this.rbLightSingle.Font = new System.Drawing.Font("Calibri", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbLightSingle.ForeColor = System.Drawing.Color.White;
            this.rbLightSingle.Location = new System.Drawing.Point(7, 33);
            this.rbLightSingle.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.rbLightSingle.Name = "rbLightSingle";
            this.rbLightSingle.Size = new System.Drawing.Size(92, 22);
            this.rbLightSingle.TabIndex = 0;
            this.rbLightSingle.TabStop = true;
            this.rbLightSingle.Text = "Singleness";
            this.rbLightSingle.UseVisualStyleBackColor = true;
            this.rbLightSingle.CheckedChanged += new System.EventHandler(this.RadioButton1_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(375, 172);
            this.Controls.Add(this.pbSelectColor);
            this.Controls.Add(this.pnlOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Opacity = 0.7D;
            this.Text = "Main";
            this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.Load += new System.EventHandler(this.Main_Load);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.VisibleChanged += new System.EventHandler(this.Main_VisibleChanged);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSelectColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLightColor)).EndInit();
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pbSelectColor;
        private System.Windows.Forms.PictureBox pbLightColor;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.CheckBox chkDustMode;
        private System.Windows.Forms.CheckBox chkDisableTP;
        private System.Windows.Forms.CheckBox chkEnableLight;
        private System.Windows.Forms.RadioButton rbLightDynamic;
        private System.Windows.Forms.RadioButton rbLightSingle;
    }
}