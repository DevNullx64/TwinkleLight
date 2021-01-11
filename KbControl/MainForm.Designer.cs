
namespace LedControl
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pbSelectColor = new System.Windows.Forms.PictureBox();
            this.pbLightColor = new System.Windows.Forms.PictureBox();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.chkDustMode = new System.Windows.Forms.CheckBox();
            this.chkDisableTP = new System.Windows.Forms.CheckBox();
            this.chkEnableLight = new System.Windows.Forms.CheckBox();
            this.rbLightDynamic = new System.Windows.Forms.RadioButton();
            this.rbLightStatic = new System.Windows.Forms.RadioButton();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSelectColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLightColor)).BeginInit();
            this.pnlOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenu.Name = "contextMenuStrip1";
            this.contextMenu.Size = new System.Drawing.Size(104, 48);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // pbSelectColor
            // 
            this.pbSelectColor.BackColor = System.Drawing.Color.Transparent;
            this.pbSelectColor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbSelectColor.Image = ((System.Drawing.Image)(resources.GetObject("pbSelectColor.Image")));
            this.pbSelectColor.Location = new System.Drawing.Point(30, 14);
            this.pbSelectColor.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pbSelectColor.Name = "pbSelectColor";
            this.pbSelectColor.Size = new System.Drawing.Size(131, 139);
            this.pbSelectColor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbSelectColor.TabIndex = 5;
            this.pbSelectColor.TabStop = false;
            this.pbSelectColor.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseClick);
            // 
            // pbLightColor
            // 
            this.pbLightColor.BackColor = System.Drawing.Color.Red;
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
            this.pnlOptions.Controls.Add(this.rbLightStatic);
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
            this.chkEnableLight.Checked = true;
            this.chkEnableLight.CheckState = System.Windows.Forms.CheckState.Checked;
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
            this.rbLightDynamic.CheckedChanged += new System.EventHandler(this.rbLightDynamic_CheckedChanged);
            // 
            // rbLightStatic
            // 
            this.rbLightStatic.AutoSize = true;
            this.rbLightStatic.Checked = true;
            this.rbLightStatic.Font = new System.Drawing.Font("Calibri", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbLightStatic.ForeColor = System.Drawing.Color.White;
            this.rbLightStatic.Location = new System.Drawing.Point(7, 33);
            this.rbLightStatic.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.rbLightStatic.Name = "rbLightStatic";
            this.rbLightStatic.Size = new System.Drawing.Size(92, 22);
            this.rbLightStatic.TabIndex = 0;
            this.rbLightStatic.TabStop = true;
            this.rbLightStatic.Text = "Singleness";
            this.rbLightStatic.UseVisualStyleBackColor = true;
            this.rbLightStatic.CheckedChanged += new System.EventHandler(this.rbLightStatic_CheckedChanged);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Twinkle Light";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseClick);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(375, 172);
            this.Controls.Add(this.pbSelectColor);
            this.Controls.Add(this.pnlOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.Opacity = 0.7D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Main";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.Load += new System.EventHandler(this.Main_Load);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.VisibleChanged += new System.EventHandler(this.Main_VisibleChanged);
            this.contextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSelectColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLightColor)).EndInit();
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.PictureBox pbSelectColor;
        private System.Windows.Forms.PictureBox pbLightColor;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.CheckBox chkDustMode;
        private System.Windows.Forms.CheckBox chkDisableTP;
        private System.Windows.Forms.CheckBox chkEnableLight;
        private System.Windows.Forms.RadioButton rbLightDynamic;
        private System.Windows.Forms.RadioButton rbLightStatic;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
    }
}