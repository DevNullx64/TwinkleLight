// Decompiled with JetBrains decompiler
// Type: LedControl.Form1
// Assembly: LedControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2625D33F-FFFD-422A-94C4-E655C9C02803
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbControl.exe

using KbAcpi;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LedControl
{
  public class Form1 : Form
  {
    public AcpiDriver acpi = new AcpiDriver();
    private MouseHookLib _mouseHook;
    private bool MouseHookInstalled;
    private bool FirstStartUp;
    private const int AW_HOR_POSITIVE = 1;
    private const int AW_HOR_NEGATIVE = 2;
    private const int AW_VER_POSITIVE = 4;
    private const int AW_VER_NEGATIVE = 8;
    private const int AW_CENTER = 16;
    private const int AW_HIDE = 65536;
    private const int AW_ACTIVE = 131072;
    private const int AW_SLIDE = 262144;
    private const int AW_BLEND = 524288;
    public int CurrentSetValue;
    public int CurrentGetValue;
    private Form1.CONFIG_DATA ConfigData;
    private Point mouseOffset;
    private bool isMouseDown;
    private IContainer components;
    private PictureBox pictureBox1;
    private PictureBox pictureBox2;
    private Panel panel1;
    private RadioButton radioButton2;
    private RadioButton radioButton1;
    private ContextMenuStrip contextMenuStrip1;
    private ToolStripMenuItem openToolStripMenuItem;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ImageList imageList1;
    private Timer timer1;
    private CheckBox checkBox3;
    private CheckBox checkBox1;
    private CheckBox checkBox2;

    [DllImport("user32")]
    private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);

    public void DEBUG(string message)
    {
      Console.WriteLine(message);
      string currentDirectory = Directory.GetCurrentDirectory();
      Directory.SetCurrentDirectory(Application.StartupPath);
      File.AppendAllText("msg.log", message + "\r\n");
      Directory.SetCurrentDirectory(currentDirectory);
    }

    public Form1() => this.InitializeComponent();

    public byte[] StructToBytes(object structObj, int size)
    {
      byte[] destination = new byte[size];
      IntPtr num = Marshal.AllocHGlobal(size);
      Marshal.StructureToPtr(structObj, num, false);
      Marshal.Copy(num, destination, 0, size);
      Marshal.FreeHGlobal(num);
      return destination;
    }

    public object ByteToStruct(byte[] bytes, System.Type type)
    {
      int num1 = Marshal.SizeOf(type);
      if (num1 > bytes.Length)
        return (object) null;
      IntPtr num2 = Marshal.AllocHGlobal(num1);
      Marshal.Copy(bytes, 0, num2, num1);
      object structure = Marshal.PtrToStructure(num2, type);
      Marshal.FreeHGlobal(num2);
      return structure;
    }

    public void ConfigInitialization()
    {
      byte[] numArray = new byte[40];
      IntPtr zero = IntPtr.Zero;
      string currentDirectory = Directory.GetCurrentDirectory();
      Directory.SetCurrentDirectory(Application.StartupPath);
      this.DEBUG("Config Initial");
      try
      {
        FileStream fileStream = new FileStream("Config.cfg", FileMode.Open);
        fileStream.Seek(0L, SeekOrigin.Begin);
        fileStream.Read(numArray, 0, 40);
        fileStream.Close();
        for (int index = 0; index < 40; ++index)
          this.DEBUG(string.Format("{0:X2}", (object) numArray[index]));
        this.ConfigData = (Form1.CONFIG_DATA) this.ByteToStruct(numArray, typeof (Form1.CONFIG_DATA));
        Color color = Color.FromArgb(this.ConfigData.SingleColorRGB);
        this.DEBUG(string.Format("Init{0:X8}", (object) this.ConfigData.SingleColorRGB));
        this.pictureBox2.BackColor = color;
        if (this.ConfigData.Mode == 2)
        {
          this.radioButton1.Checked = true;
          this.radioButton2.Checked = false;
        }
        else if (this.ConfigData.Mode == 1)
        {
          this.radioButton1.Checked = false;
          this.radioButton2.Checked = true;
        }
        int autoStartUp = this.ConfigData.AutoStartUp;
        int startUpMinimize = this.ConfigData.StartUpMinimize;
        if (this.ConfigData.BacklightOn == 0)
        {
          this.radioButton1.Enabled = false;
          this.radioButton2.Enabled = false;
          this.checkBox3.Checked = false;
        }
        else
        {
          this.radioButton1.Enabled = true;
          this.radioButton2.Enabled = true;
          this.checkBox3.Checked = true;
        }
        this.checkBox2.Checked = this.ConfigData.Dustenable == 1;
      }
      catch (IOException ex)
      {
        this.radioButton1.Enabled = true;
        this.radioButton2.Enabled = true;
        this.checkBox3.Checked = true;
        this.ConfigData.SingleColorRGB = 16646144;
      }
      Directory.SetCurrentDirectory(currentDirectory);
    }

    public void SaveConfig()
    {
      byte[] numArray = new byte[40];
      IntPtr zero = IntPtr.Zero;
      string currentDirectory = Directory.GetCurrentDirectory();
      Directory.SetCurrentDirectory(Application.StartupPath);
      FileStream fileStream = new FileStream("Config.cfg", FileMode.Create);
      this.DEBUG("Save Config");
      this.ConfigData.StartUpMinimize = 1;
      byte[] bytes = this.StructToBytes((object) this.ConfigData, 40);
      fileStream.Write(bytes, 0, bytes.Length);
      fileStream.Flush();
      fileStream.Close();
      Directory.SetCurrentDirectory(currentDirectory);
    }

    public static Bitmap KiResizeImage(Bitmap bmp, int newW, int newH)
    {
      try
      {
        Bitmap bitmap = new Bitmap(newW, newH);
        Graphics graphics = Graphics.FromImage((Image) bitmap);
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.DrawImage((Image) bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
        graphics.Dispose();
        return bitmap;
      }
      catch
      {
        return (Bitmap) null;
      }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      this.ConfigInitialization();
      this.ShowInTaskbar = false;
      this.TopMost = true;
      this.Visible = true;
      Image image = this.imageList1.Images[0];
      Size size = this.pictureBox1.Size;
      int width = size.Width;
      size = this.pictureBox1.Size;
      int height = size.Height;
      this.pictureBox1.Image = (Image) new Bitmap(image, width, height);
      this.DEBUG("Load form");
      this.InstallHookMouse();
    }

    private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
    {
    }

    private void setRGB()
    {
      try
      {
        ManagementObject managementObject = new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_0'", (ObjectGetOptions) null);
        ManagementBaseObject methodParameters = managementObject.GetMethodParameters("Set");
        methodParameters["Data"] = (object) this.CalValue();
        managementObject.InvokeMethod("Set", methodParameters, (InvokeMethodOptions) null);
        this.CurrentSetValue = int.Parse(methodParameters["Data"].ToString());
        this.DEBUG("Set RGB: " + string.Format("Init{0:X8}", (object) this.CurrentSetValue));
      }
      catch (ManagementException ex)
      {
      }
    }

    private void SwitchTp(int parameter)
    {
      try
      {
        ManagementObject managementObject = new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_1'", (ObjectGetOptions) null);
        ManagementBaseObject methodParameters = managementObject.GetMethodParameters("Set");
        methodParameters["Data"] = (object) parameter;
        managementObject.InvokeMethod("Set", methodParameters, (InvokeMethodOptions) null);
        this.CurrentSetValue = int.Parse(methodParameters["Data"].ToString());
        this.DEBUG("Set RGB: " + string.Format("Init{0:X8}", (object) this.CurrentSetValue));
      }
      catch (ManagementException ex)
      {
      }
    }

    private void Dust(int parameter)
    {
      try
      {
        ManagementObject managementObject = new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_2'", (ObjectGetOptions) null);
        ManagementBaseObject methodParameters = managementObject.GetMethodParameters("Set");
        methodParameters["Data"] = (object) parameter;
        managementObject.InvokeMethod("Set", methodParameters, (InvokeMethodOptions) null);
        this.CurrentSetValue = int.Parse(methodParameters["Data"].ToString());
        this.DEBUG("Set Dust: " + string.Format("Init{0:X8}", (object) this.CurrentSetValue));
      }
      catch (ManagementException ex)
      {
      }
    }

    private void getRGB()
    {
      try
      {
        this.CurrentGetValue = int.Parse(new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_0'", (ObjectGetOptions) null).InvokeMethod("Get", (ManagementBaseObject) null, (InvokeMethodOptions) null)["Data"].ToString());
        this.DEBUG("Get RGB: " + string.Format("Init{0:X8}", (object) this.CurrentGetValue));
      }
      catch (ManagementException ex)
      {
      }
    }

    private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
    {
      Bitmap image = (Bitmap) this.pictureBox1.Image;
      this.DEBUG(nameof (pictureBox1_MouseClick));
      try
      {
        this.pictureBox2.BackColor = image.GetPixel(e.X, e.Y);
        ref Form1.CONFIG_DATA local = ref this.ConfigData;
        int num1 = (int) this.pictureBox2.BackColor.A << 24;
        Color backColor = this.pictureBox2.BackColor;
        int num2 = (int) backColor.R << 16;
        int num3 = num1 | num2;
        backColor = this.pictureBox2.BackColor;
        int num4 = (int) backColor.G << 8;
        int num5 = num3 | num4;
        backColor = this.pictureBox2.BackColor;
        int b = (int) backColor.B;
        int num6 = num5 | b;
        local.SingleColorRGB = num6;
        this.SaveConfig();
        if (!this.radioButton1.Checked)
          return;
        this.setRGB();
      }
      catch
      {
      }
    }

    public void Mousedown(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;
      this.mouseOffset = new Point(-e.X - SystemInformation.FrameBorderSize.Width + 10, -e.Y - SystemInformation.CaptionHeight - SystemInformation.FrameBorderSize.Height + 30);
      this.isMouseDown = true;
    }

    public void Mousemove(object sender, MouseEventArgs e)
    {
      if (!this.isMouseDown)
        return;
      Point mousePosition = Control.MousePosition;
      mousePosition.Offset(this.mouseOffset.X, this.mouseOffset.Y);
      this.Location = mousePosition;
    }

    public void Mouseup(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;
      this.isMouseDown = false;
    }

    private void pictureBox3_Click(object sender, EventArgs e)
    {
    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.UninstallHookMouse();
      this.acpi.AcpiUnLoadDll();
      this.Dispose();
      this.Close();
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
      this.DEBUG(nameof (checkBox1_CheckedChanged));
      this.SaveConfig();
    }

    public int CalValue() => !this.checkBox3.Checked ? 33554432 : this.ConfigData.Mode << 24 | ((this.ConfigData.SingleColorRGB & 16711680) >> 16) * 39 / (int) byte.MaxValue << 16 | ((this.ConfigData.SingleColorRGB & 65280) >> 8) * 39 / (int) byte.MaxValue << 8 | (this.ConfigData.SingleColorRGB & (int) byte.MaxValue) * 39 / (int) byte.MaxValue;

    private void radioButton1_CheckedChanged(object sender, EventArgs e)
    {
      this.DEBUG(nameof (radioButton1_CheckedChanged));
      this.ConfigData.Mode = !this.radioButton1.Checked ? 1 : 2;
      this.SaveConfig();
      this.setRGB();
    }

    private void InstallHookMouse()
    {
      if (this._mouseHook != null || this.MouseHookInstalled)
        return;
      this.DEBUG("Install Hook Mouse");
      this.MouseHookInstalled = true;
      this._mouseHook = new MouseHookLib();
      this._mouseHook.InstallHook(new MouseHookLib.ProcessKeyHandle(this.HookMouseProcess));
    }

    public void UninstallHookMouse()
    {
      this.DEBUG("Uninstall Hook Mouse");
      this.MouseHookInstalled = false;
      if (this._mouseHook == null)
        return;
      this._mouseHook.UninstallHook();
    }

    public void HookKeyProcess(KeyboardHookLib.HookStruct hookStruct, out bool handle)
    {
      handle = false;
      ((Keys) hookStruct.vkCode).ToString();
      if (hookStruct.scanCode != 112 || this.Visible)
        return;
      this.Visible = true;
    }

    public void HookMouseProcess(MouseHookLib.HookStruct hookStruct, out bool handle)
    {
      handle = false;
      MouseHookLib.PointStruct point = hookStruct.point;
      if (point.x >= this.Left && point.x <= this.Left + this.Width && (point.y >= this.Top && point.y <= this.Top + this.Height) || point.y >= this.Top + this.Height)
        return;
      this.Visible = false;
    }

    private void Form1_KeyPress(object sender, KeyPressEventArgs e)
    {
    }

    private void button1_Click_1(object sender, EventArgs e)
    {
      this.acpi.AcpiLoadDll();
      this.acpi.GetAcpiValueProc(64);
      this.acpi.AcpiUnLoadDll();
    }

    private void radioButton2_CheckedChanged(object sender, EventArgs e)
    {
      this.DEBUG(nameof (radioButton2_CheckedChanged));
      this.SaveConfig();
      this.setRGB();
    }

    private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
    {
    }

    private void notifyIcon1_MouseDown(object sender, MouseEventArgs e) => this.DEBUG("Mouse Down");

    private void pictureBox2_Click(object sender, EventArgs e)
    {
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      this.getRGB();
      if (this.CurrentGetValue != this.CurrentSetValue)
        this.setRGB();
      else
        this.timer1.Enabled = false;
    }

    private void checkBox3_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.checkBox3.Checked)
      {
        this.radioButton1.Enabled = false;
        this.radioButton2.Enabled = false;
        this.ConfigData.BacklightOn = 0;
      }
      else
      {
        this.radioButton1.Enabled = true;
        this.radioButton2.Enabled = true;
        this.ConfigData.BacklightOn = 1;
      }
      this.SaveConfig();
      this.setRGB();
    }

    private void Form1_Shown(object sender, EventArgs e)
    {
      if (!(CultureInfo.InstalledUICulture.NativeName.Substring(0, 2) == "中文"))
        return;
      this.checkBox3.Text = "跑马灯";
      this.radioButton1.Text = "单色模式";
      this.radioButton2.Text = "多彩模式";
      this.checkBox1.Text = "关闭触摸板";
      this.checkBox2.Text = "除尘/高性能";
    }

    private void Form1_VisibleChanged(object sender, EventArgs e)
    {
      int height1 = Screen.PrimaryScreen.Bounds.Height;
      int height2 = Screen.PrimaryScreen.WorkingArea.Height;
      int width = Screen.PrimaryScreen.Bounds.Width;
      int num = height1 - height2;
      this.Top = height1 - this.Size.Height - num;
      this.Left = width - this.Size.Width - 26;
      this.ShowInTaskbar = false;
      if (CultureInfo.InstalledUICulture.NativeName.Substring(0, 2) == "中文")
      {
        this.checkBox3.Text = "跑马灯";
        this.radioButton1.Text = "单色模式";
        this.radioButton2.Text = "多彩模式";
        this.checkBox1.Text = "关闭触摸板";
      }
      this.DEBUG("Visible: " + this.Visible.ToString());
    }

    private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
    {
      if (this.checkBox1.Checked)
      {
        this.SwitchTp(0);
        this.Visible = false;
      }
      else
        this.SwitchTp(16777216);
    }

    private void checkBox2_CheckedChanged(object sender, EventArgs e)
    {
      if (this.checkBox2.Checked)
      {
        this.ConfigData.Dustenable = 1;
        this.SaveConfig();
        this.Dust(16777216);
      }
      else
      {
        this.ConfigData.Dustenable = 0;
        this.SaveConfig();
        this.Dust(0);
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Form1));
      this.pictureBox1 = new PictureBox();
      this.pictureBox2 = new PictureBox();
      this.panel1 = new Panel();
      this.checkBox1 = new CheckBox();
      this.checkBox3 = new CheckBox();
      this.radioButton2 = new RadioButton();
      this.radioButton1 = new RadioButton();
      this.contextMenuStrip1 = new ContextMenuStrip(this.components);
      this.openToolStripMenuItem = new ToolStripMenuItem();
      this.exitToolStripMenuItem = new ToolStripMenuItem();
      this.imageList1 = new ImageList(this.components);
      this.timer1 = new Timer(this.components);
      this.checkBox2 = new CheckBox();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      ((ISupportInitialize) this.pictureBox2).BeginInit();
      this.panel1.SuspendLayout();
      this.contextMenuStrip1.SuspendLayout();
      this.SuspendLayout();
      this.pictureBox1.BackColor = Color.Transparent;
      this.pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
      this.pictureBox1.Location = new Point(23, 15);
      this.pictureBox1.Margin = new Padding(2, 3, 2, 3);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(131, 139);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.Click += new EventHandler(this.pictureBox1_Click);
      this.pictureBox1.MouseClick += new MouseEventHandler(this.pictureBox1_MouseClick);
      this.pictureBox1.MouseDown += new MouseEventHandler(this.pictureBox1_MouseDown);
      this.pictureBox2.BackColor = Color.DimGray;
      this.pictureBox2.Location = new Point(96, 37);
      this.pictureBox2.Margin = new Padding(2, 3, 2, 3);
      this.pictureBox2.Name = "pictureBox2";
      this.pictureBox2.Size = new Size(57, 16);
      this.pictureBox2.TabIndex = 3;
      this.pictureBox2.TabStop = false;
      this.pictureBox2.Click += new EventHandler(this.pictureBox2_Click);
      this.panel1.BackColor = Color.Transparent;
      this.panel1.Controls.Add((Control) this.checkBox2);
      this.panel1.Controls.Add((Control) this.checkBox1);
      this.panel1.Controls.Add((Control) this.checkBox3);
      this.panel1.Controls.Add((Control) this.radioButton2);
      this.panel1.Controls.Add((Control) this.radioButton1);
      this.panel1.Controls.Add((Control) this.pictureBox2);
      this.panel1.Location = new Point(179, 17);
      this.panel1.Margin = new Padding(2, 3, 2, 3);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(158, 142);
      this.panel1.TabIndex = 4;
      this.checkBox1.AutoSize = true;
      this.checkBox1.Font = new Font("Calibri", 10.8f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.checkBox1.ForeColor = Color.White;
      this.checkBox1.Location = new Point(3, 82);
      this.checkBox1.Margin = new Padding(2, 3, 2, 3);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new Size(90, 22);
      this.checkBox1.TabIndex = 13;
      this.checkBox1.Text = "Disable Tp";
      this.checkBox1.UseVisualStyleBackColor = true;
      this.checkBox1.CheckedChanged += new EventHandler(this.checkBox1_CheckedChanged_1);
      this.checkBox3.AutoSize = true;
      this.checkBox3.Font = new Font("Calibri", 10.8f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.checkBox3.ForeColor = Color.White;
      this.checkBox3.Location = new Point(3, 13);
      this.checkBox3.Margin = new Padding(2, 3, 2, 3);
      this.checkBox3.Name = "checkBox3";
      this.checkBox3.Size = new Size(109, 22);
      this.checkBox3.TabIndex = 12;
      this.checkBox3.Text = "Twinkle Light";
      this.checkBox3.UseVisualStyleBackColor = true;
      this.checkBox3.CheckedChanged += new EventHandler(this.checkBox3_CheckedChanged);
      this.radioButton2.AutoSize = true;
      this.radioButton2.Font = new Font("Calibri", 10.8f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.radioButton2.ForeColor = Color.White;
      this.radioButton2.Location = new Point(7, 54);
      this.radioButton2.Margin = new Padding(2, 3, 2, 3);
      this.radioButton2.Name = "radioButton2";
      this.radioButton2.Size = new Size(79, 22);
      this.radioButton2.TabIndex = 1;
      this.radioButton2.TabStop = true;
      this.radioButton2.Text = "Dynamic";
      this.radioButton2.UseVisualStyleBackColor = true;
      this.radioButton2.CheckedChanged += new EventHandler(this.radioButton2_CheckedChanged);
      this.radioButton1.AutoSize = true;
      this.radioButton1.Font = new Font("Calibri", 10.8f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.radioButton1.ForeColor = Color.White;
      this.radioButton1.Location = new Point(7, 33);
      this.radioButton1.Margin = new Padding(2, 3, 2, 3);
      this.radioButton1.Name = "radioButton1";
      this.radioButton1.Size = new Size(92, 22);
      this.radioButton1.TabIndex = 0;
      this.radioButton1.TabStop = true;
      this.radioButton1.Text = "Singleness";
      this.radioButton1.UseVisualStyleBackColor = true;
      this.radioButton1.CheckedChanged += new EventHandler(this.radioButton1_CheckedChanged);
      this.contextMenuStrip1.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.openToolStripMenuItem,
        (ToolStripItem) this.exitToolStripMenuItem
      });
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new Size(104, 48);
      this.openToolStripMenuItem.Name = "openToolStripMenuItem";
      this.openToolStripMenuItem.Size = new Size(103, 22);
      this.openToolStripMenuItem.Text = "Open";
      this.openToolStripMenuItem.Click += new EventHandler(this.openToolStripMenuItem_Click);
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new Size(103, 22);
      this.exitToolStripMenuItem.Text = "Exit";
      this.exitToolStripMenuItem.Click += new EventHandler(this.exitToolStripMenuItem_Click);
      this.imageList1.ImageStream = (ImageListStreamer) componentResourceManager.GetObject("imageList1.ImageStream");
      this.imageList1.TransparentColor = Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "未标题-1.png");
      this.timer1.Interval = 10000;
      this.timer1.Tick += new EventHandler(this.timer1_Tick);
      this.checkBox2.AutoSize = true;
      this.checkBox2.Font = new Font("Calibri", 10.8f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.checkBox2.ForeColor = Color.White;
      this.checkBox2.Location = new Point(3, 110);
      this.checkBox2.Margin = new Padding(2, 3, 2, 3);
      this.checkBox2.Name = "checkBox2";
      this.checkBox2.Size = new Size(133, 22);
      this.checkBox2.TabIndex = 14;
      this.checkBox2.Text = "Dust/Hight mode";
      this.checkBox2.UseVisualStyleBackColor = true;
      this.checkBox2.CheckedChanged += new EventHandler(this.checkBox2_CheckedChanged);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.FromArgb(64, 64, 64);
      this.ClientSize = new Size(375, 172);
      this.Controls.Add((Control) this.panel1);
      this.Controls.Add((Control) this.pictureBox1);
      this.FormBorderStyle = FormBorderStyle.None;
      this.Margin = new Padding(2, 3, 2, 3);
      this.Name = nameof (Form1);
      this.Opacity = 0.7;
      this.Text = nameof (Form1);
      this.TransparencyKey = SystemColors.Control;
      this.Load += new EventHandler(this.Form1_Load);
      this.Shown += new EventHandler(this.Form1_Shown);
      this.VisibleChanged += new EventHandler(this.Form1_VisibleChanged);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      ((ISupportInitialize) this.pictureBox2).EndInit();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.contextMenuStrip1.ResumeLayout(false);
      this.ResumeLayout(false);
    }

    public struct CONFIG_DATA
    {
      public int Mode;
      public int SingleColorRGB;
      public int R;
      public int G;
      public int B;
      public int AutoStartUp;
      public int StartUpMinimize;
      public int BacklightOn;
      public int Dustenable;
      public int Reserve7;
    }
  }
}
