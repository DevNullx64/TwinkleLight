// Decompiled with JetBrains decompiler
// Type: LedControl.Program
// Assembly: LedControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2625D33F-FFFD-422A-94C4-E655C9C02803
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbControl.exe

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace LedControl
{
  internal static class Program
  {
    private const int WS_SHOWNORMAL = 1;

    [DllImport("User32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

    [DllImport("User32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [STAThread]
    private static void Main()
    {
      Process instance = Program.RunningInstance();
      if (instance == null)
      {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Thread.Sleep(500);
        Application.Run((Form) new Icon());
      }
      else
        Program.HandleRunningInstance(instance);
    }

    private static Process RunningInstance()
    {
      Process currentProcess = Process.GetCurrentProcess();
      foreach (Process process in Process.GetProcessesByName(currentProcess.ProcessName.Replace(".vshost", "")))
      {
        if (process.Id != currentProcess.Id && Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == process.MainModule.FileName)
          return process;
      }
      return (Process) null;
    }

    private static void HandleRunningInstance(Process instance)
    {
      Program.ShowWindow(instance.MainWindowHandle, 1);
      Program.SetForegroundWindow(instance.MainWindowHandle);
    }
  }
}
