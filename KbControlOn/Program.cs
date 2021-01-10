// Decompiled with JetBrains decompiler
// Type: KbControlOn.Program
// Assembly: KbControlOn, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 429CF7B2-9973-4180-BEB2-334841D88BD5
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbControlOn.exe

using System;
using System.Windows.Forms;

namespace KbControlOn
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form) new ControlOn());
    }
  }
}
