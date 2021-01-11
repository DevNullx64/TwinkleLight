// Decompiled with JetBrains decompiler
// Type: LedControl.Icon
// Assembly: LedControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2625D33F-FFFD-422A-94C4-E655C9C02803
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbControl.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LedControl
{
  public partial class Icon : Form
  {
    public MainForm form1;
    private KeyboardHookLib _keyboardHook;
    private bool KeyboardHookKeyInstalled;
    private IContainer components;
    private NotifyIcon notifyIcon1;
    private ContextMenuStrip contextMenuStrip1;
    private ToolStripMenuItem openToolStripMenuItem;
    private ToolStripMenuItem exitToolStripMenuItem;

    public void DEBUG(string message)
    {
    }

    public Icon() => this.InitializeComponent();

    private void Icon_Load(object sender, EventArgs e)
    {
      this.ShowInTaskbar = false;
      this.notifyIcon1.Visible = true;
      this.TopMost = true;
      this.Visible = false;
      this.form1 = new MainForm();
      this.form1.Show();
      this.InstallHookKey();
    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e) => this.Visible = true;

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.form1.UninstallHookMouse();
      this.form1.acpi.AcpiUnLoadDll();
      this.form1.Dispose();
      this.form1.Close();
      this.UninstallHookKey();
      this.Dispose();
      this.Close();
    }

    private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
    {
    }

    private void notifyIcon1_MouseDown(object sender, MouseEventArgs e) => this.form1.Show();

    private void InstallHookKey()
    {
      if (this._keyboardHook != null || this.KeyboardHookKeyInstalled)
        return;
      this.KeyboardHookKeyInstalled = true;
      this._keyboardHook = new KeyboardHookLib();
      this._keyboardHook.InstallHook(new KeyboardHookLib.ProcessKeyHandle(this.HookKeyProcess));
    }

    public void UninstallHookKey()
    {
      this.KeyboardHookKeyInstalled = false;
      if (this._keyboardHook == null)
        return;
      this._keyboardHook.UninstallHook();
    }

    public void HookKeyProcess(KeyboardHookLib.HookStruct hookStruct, out bool handle)
    {
      handle = false;
      string str = ((Keys) hookStruct.vkCode).ToString();
      if (hookStruct.scanCode == 112)
      {
        if (!this.form1.Visible)
          this.form1.Show();
        else
          this.form1.Hide();
      }
      this.DEBUG(str + hookStruct.scanCode.ToString());
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
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Icon));
      this.notifyIcon1 = new NotifyIcon(this.components);
      this.contextMenuStrip1 = new ContextMenuStrip(this.components);
      this.openToolStripMenuItem = new ToolStripMenuItem();
      this.exitToolStripMenuItem = new ToolStripMenuItem();
      this.contextMenuStrip1.SuspendLayout();
      this.SuspendLayout();
      this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
      this.notifyIcon1.Icon = (System.Drawing.Icon) componentResourceManager.GetObject("notifyIcon1.Icon");
      this.notifyIcon1.Text = "Twinkle Light";
      this.notifyIcon1.Visible = true;
      this.notifyIcon1.MouseDown += new MouseEventHandler(this.notifyIcon1_MouseDown);
      this.contextMenuStrip1.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.openToolStripMenuItem,
        (ToolStripItem) this.exitToolStripMenuItem
      });
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new Size(119, 52);
      this.openToolStripMenuItem.Name = "openToolStripMenuItem";
      this.openToolStripMenuItem.Size = new Size(118, 24);
      this.openToolStripMenuItem.Text = "Open";
      this.openToolStripMenuItem.Click += new EventHandler(this.openToolStripMenuItem_Click);
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new Size(118, 24);
      this.exitToolStripMenuItem.Text = "Exit";
      this.exitToolStripMenuItem.Click += new EventHandler(this.exitToolStripMenuItem_Click);
      this.AutoScaleDimensions = new SizeF(8f, 15f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(0, 0);
      this.FormBorderStyle = FormBorderStyle.None;
      this.Name = nameof (Icon);
      this.Text = nameof (Icon);
      this.Load += new EventHandler(this.Icon_Load);
      this.contextMenuStrip1.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}
