// Decompiled with JetBrains decompiler
// Type: KbControlOn.ControlOn
// Assembly: KbControlOn, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 429CF7B2-9973-4180-BEB2-334841D88BD5
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbControlOn.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KbControlOn
{
  public class ControlOn : Form
  {
    private IContainer components;

    public ControlOn() => this.InitializeComponent();

    private void Form1_Load(object sender, EventArgs e)
    {
      this.ShowInTaskbar = false;
      this.Visible = false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(8f, 15f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(0, 0);
      this.FormBorderStyle = FormBorderStyle.None;
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new EventHandler(this.Form1_Load);
      this.ResumeLayout(false);
    }
  }
}
