using KbAcpi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LedControl
{
    public partial class Main : Form
    {
        private Main.CONFIG_DATA ConfigData;

        public AcpiDriver acpi = new AcpiDriver();
        private MouseHookLib _mouseHook;
        private bool MouseHookInstalled;

        public int CurrentSetValue;
        public int CurrentGetValue;

        private void button1_Click_1(object sender, EventArgs e)
        {
            acpi.AcpiLoadDll();
            acpi.GetAcpiValueProc(64);
            acpi.AcpiUnLoadDll();
        }

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e) => DEBUG("Mouse Down");

        private void getRGB()
        {
            try
            {
                CurrentGetValue = int.Parse(new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_0'", null).InvokeMethod("Get", null, null)["Data"].ToString());
                DEBUG($"Get RGB: Init{CurrentGetValue:X8}");
            }
            catch (ManagementException ex)
            {
            }
        }
        private void setRGB()
        {
            try
            {
                ManagementObject managementObject = new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_0'", (ObjectGetOptions)null);
                ManagementBaseObject methodParameters = managementObject.GetMethodParameters("Set");
                methodParameters["Data"] = CalValue();
                managementObject.InvokeMethod("Set", methodParameters, (InvokeMethodOptions)null);
                CurrentSetValue = int.Parse(methodParameters["Data"].ToString());
                DEBUG("Set RGB: " + string.Format("Init{0:X8}", CurrentSetValue));
            }
            catch (ManagementException ex)
            {
            }
        }


        public Main()
        {
            InitializeComponent();
        }

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
                return (object)null;
            IntPtr num2 = Marshal.AllocHGlobal(num1);
            Marshal.Copy(bytes, 0, num2, num1);
            object structure = Marshal.PtrToStructure(num2, type);
            Marshal.FreeHGlobal(num2);
            return structure;
        }

        public void DEBUG(string message)
        {
            Console.WriteLine(message);
            string currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Application.StartupPath);
            File.AppendAllText(Path.Combine(Application.StartupPath, "msg.log"), message + "\r\n");
            Directory.SetCurrentDirectory(currentDirectory);
        }

        public void HookMouseProcess(MouseHookLib.HookStruct hookStruct, out bool handle)
        {
            handle = false;
            MouseHookLib.PointStruct point = hookStruct.point;
            if (point.x >= Left && point.x <= Left + Width && (point.y >= Top && point.y <= Top + Height) || point.y >= Top + Height)
                return;
            Visible = false;
        }

        private void InstallHookMouse()
        {
            if (_mouseHook != null || MouseHookInstalled)
                return;
            DEBUG("Install Hook Mouse");
            MouseHookInstalled = true;
            _mouseHook = new MouseHookLib();
            _mouseHook.InstallHook(new MouseHookLib.ProcessKeyHandle(HookMouseProcess));
        }

        public void ConfigInitialization()
        {
            byte[] numArray = new byte[40];
            IntPtr zero = IntPtr.Zero;
            string currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Application.StartupPath);
            DEBUG("Config Initial");
            try
            {
                FileStream fileStream = new FileStream("Config.cfg", FileMode.Open);
                fileStream.Seek(0L, SeekOrigin.Begin);
                fileStream.Read(numArray, 0, 40);
                fileStream.Close();
                for (int index = 0; index < 40; ++index)
                    DEBUG(string.Format("{0:X2}", (object)numArray[index]));
                ConfigData = (Main.CONFIG_DATA)ByteToStruct(numArray, typeof(Main.CONFIG_DATA));
                Color color = Color.FromArgb(ConfigData.SingleColorRGB);
                DEBUG(string.Format("Init{0:X8}", (object)ConfigData.SingleColorRGB));
                pictureBox2.BackColor = color;
                if (ConfigData.Mode == 2)
                {
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                }
                else if (ConfigData.Mode == 1)
                {
                    radioButton1.Checked = false;
                    radioButton2.Checked = true;
                }
                int autoStartUp = ConfigData.AutoStartUp;
                int startUpMinimize = ConfigData.StartUpMinimize;
                if (ConfigData.BacklightOn == 0)
                {
                    radioButton1.Enabled = false;
                    radioButton2.Enabled = false;
                    checkBox3.Checked = false;
                }
                else
                {
                    radioButton1.Enabled = true;
                    radioButton2.Enabled = true;
                    checkBox3.Checked = true;
                }
                checkBox2.Checked = ConfigData.Dustenable == 1;
            }
            catch (IOException ex)
            {
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                checkBox3.Checked = true;
                ConfigData.SingleColorRGB = 16646144;
            }
            Directory.SetCurrentDirectory(currentDirectory);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ConfigInitialization();
            ShowInTaskbar = false;
            TopMost = true;
            Visible = true;
            Image image = imageList1.Images[0];
            Size size = pictureBox1.Size;
            int width = size.Width;
            size = pictureBox1.Size;
            int height = size.Height;
            pictureBox1.Image = new Bitmap(image, width, height);
            DEBUG("Load form");
            InstallHookMouse();
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

        public void SaveConfig()
        {
            byte[] numArray = new byte[40];
            IntPtr zero = IntPtr.Zero;
            string currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Application.StartupPath);
            FileStream fileStream = new FileStream("Config.cfg", FileMode.Create);
            DEBUG("Save Config");
            ConfigData.StartUpMinimize = 1;
            byte[] bytes = StructToBytes((object)ConfigData, 40);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Flush();
            fileStream.Close();
            Directory.SetCurrentDirectory(currentDirectory);
        }

        public int CalValue() =>
            !checkBox3.Checked ?
            0x2000000 :
            ConfigData.Mode << 24 |
            ((ConfigData.SingleColorRGB & 0x00FF0000) >> 16) * 39 / byte.MaxValue << 16 |
            ((ConfigData.SingleColorRGB & 0x0000FF00) >> 8) * 39 / byte.MaxValue << 8 |
            (ConfigData.SingleColorRGB & 0x000000FF) * 39 / byte.MaxValue;

        private void SwitchTp(int parameter)
        {
            try
            {
                ManagementObject managementObject = new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_1'", null);
                ManagementBaseObject methodParameters = managementObject.GetMethodParameters("Set");
                methodParameters["Data"] = parameter;
                managementObject.InvokeMethod("Set", methodParameters, (InvokeMethodOptions)null);
                CurrentSetValue = int.Parse(methodParameters["Data"].ToString());
                DEBUG($"Set RGB: Init{CurrentSetValue:X8}");
            }
            catch (ManagementException ex)
            {
            }
        }

        private void Dust(int parameter)
        {
            try
            {
                ManagementObject managementObject = new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_2'", null);
                ManagementBaseObject methodParameters = managementObject.GetMethodParameters("Set");
                methodParameters["Data"] = parameter;
                managementObject.InvokeMethod("Set", methodParameters, (InvokeMethodOptions)null);
                CurrentSetValue = int.Parse(methodParameters["Data"].ToString());
                DEBUG($"Set Dust: Init{CurrentSetValue:X8}");
            }
            catch (ManagementException ex)
            {
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox3.Checked)
            {
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                ConfigData.BacklightOn = 0;
            }
            else
            {
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                ConfigData.BacklightOn = 1;
            }
            SaveConfig();
            setRGB();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            DEBUG(nameof(radioButton1_CheckedChanged));
            ConfigData.Mode = !radioButton1.Checked ? 1 : 2;
            SaveConfig();
            setRGB();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            DEBUG(nameof(radioButton2_CheckedChanged));
            SaveConfig();
            setRGB();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SwitchTp(0);
                Visible = false;
            }
            else
                SwitchTp(0x1000000);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                ConfigData.Dustenable = 1;
                SaveConfig();
                Dust(0x1000000);
            }
            else
            {
                ConfigData.Dustenable = 0;
                SaveConfig();
                Dust(0);
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Bitmap image = (Bitmap)pictureBox1.Image;
            DEBUG(nameof(pictureBox1_MouseClick));
            try
            {
                pictureBox2.BackColor = image.GetPixel(e.X, e.Y);
                ref Main.CONFIG_DATA local = ref ConfigData;
                int num1 = pictureBox2.BackColor.A << 24;
                Color backColor = pictureBox2.BackColor;
                int num2 = backColor.R << 16;
                int num3 = num1 | num2;
                backColor = pictureBox2.BackColor;
                int num4 = backColor.G << 8;
                int num5 = num3 | num4;
                backColor = pictureBox2.BackColor;
                int b = backColor.B;
                int num6 = num5 | b;
                local.SingleColorRGB = num6;
                SaveConfig();
                if (!radioButton1.Checked)
                    return;
                setRGB();
            }
            catch
            {
            }
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            if (!(CultureInfo.InstalledUICulture.NativeName.Substring(0, 2) == "中文"))
                return;
            checkBox3.Text = "跑马灯";
            radioButton1.Text = "单色模式";
            radioButton2.Text = "多彩模式";
            checkBox1.Text = "关闭触摸板";
            checkBox2.Text = "除尘/高性能";
        }

        private void Main_VisibleChanged(object sender, EventArgs e)
        {
            int height1 = Screen.PrimaryScreen.Bounds.Height;
            int height2 = Screen.PrimaryScreen.WorkingArea.Height;
            int width = Screen.PrimaryScreen.Bounds.Width;
            int num = height1 - height2;
            Top = height1 - Size.Height - num;
            Left = width - Size.Width - 26;
            ShowInTaskbar = false;
            if (CultureInfo.InstalledUICulture.NativeName.Substring(0, 2) == "中文")
            {
                checkBox3.Text = "跑马灯";
                radioButton1.Text = "单色模式";
                radioButton2.Text = "多彩模式";
                checkBox1.Text = "关闭触摸板";
            }
            DEBUG("Visible: " + Visible.ToString());
        }

        public void UninstallHookMouse()
        {
            DEBUG("Uninstall Hook Mouse");
            MouseHookInstalled = false;
            if (_mouseHook == null)
                return;
            _mouseHook.UninstallHook();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UninstallHookMouse();
            acpi.AcpiUnLoadDll();
            Dispose();
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            getRGB();
            if (CurrentGetValue != CurrentSetValue)
                setRGB();
            else
                timer1.Enabled = false;
        }
    }
}
