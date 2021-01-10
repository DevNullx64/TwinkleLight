// Decompiled with JetBrains decompiler
// Type: KbService.Service1
// Assembly: KbService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1EE4A412-47D9-4D2C-8294-3E09959B4F08
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbService.exe

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace KbService
{
  public class Service1 : ServiceBase
  {
    private Timer MT;
    private IContainer components;

    public Service1() => this.InitializeComponent();

    protected override void OnStart(string[] args)
    {
      this.MyTimer();
      this.KbControlOn();
    }

    protected override void OnStop()
    {
      this.DisableMyTimer();
      this.KillProcess("KbControlOn");
      this.KillProcess("KbControl");
      this.KbControlOff();
    }

    public string GetWindowsServiceInstallPath(string ServiceName)
    {
      string name = "SYSTEM\\CurrentControlSet\\Services\\" + ServiceName;
      return new FileInfo(Registry.LocalMachine.OpenSubKey(name).GetValue("ImagePath").ToString().Replace("\"", string.Empty)).Directory.ToString();
    }

    private void KbControlOn()
    {
      string str = this.GetWindowsServiceInstallPath("KbService.exe") + "\\KbControlOn.exe";
      RegistryKey localMachine = Registry.LocalMachine;
      RegistryKey subKey = localMachine.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
      subKey.SetValue(nameof (KbControlOn), (object) str);
      subKey.Close();
      localMachine.Close();
    }

    private void KbControlOff()
    {
      string str = "";
      RegistryKey localMachine = Registry.LocalMachine;
      RegistryKey subKey = localMachine.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
      subKey.SetValue("KbControlOn", (object) str);
      subKey.Close();
      localMachine.Close();
    }

    private void DisableMyTimer() => this.MT.Enabled = false;

    private void MyTimer()
    {
      this.MT = new Timer(20000.0);
      this.MT.Enabled = false;
      this.MT.Elapsed += new ElapsedEventHandler(this.MTimedEvent);
      this.MT.Enabled = true;
    }

    private void MTimedEvent(object source, ElapsedEventArgs e)
    {
      if (((IEnumerable<Process>) Process.GetProcessesByName("KbControlOn")).Count<Process>() == 0)
        return;
      WinAPI_Interop.CreateProcessAd((string) null, new StringBuilder("KbControl.exe"));
    }

    public void KillProcess(string strProcessesByName)
    {
      foreach (Process process in Process.GetProcessesByName(strProcessesByName))
      {
        try
        {
          process.Kill();
          process.WaitForExit();
        }
        catch (Win32Exception ex)
        {
        }
        catch (InvalidOperationException ex)
        {
        }
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
      this.ServiceName = nameof (Service1);
    }
  }
}
