// Decompiled with JetBrains decompiler
// Type: KbAcpi.AcpiDriver
// Assembly: LedControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2625D33F-FFFD-422A-94C4-E655C9C02803
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbControl.exe

using System;
using System.Runtime.InteropServices;

namespace KbAcpi
{
  public class AcpiDriver
  {
    public IntPtr hModule = IntPtr.Zero;
    public IntPtr getProc = IntPtr.Zero;
    public IntPtr setProc = IntPtr.Zero;
    public IntPtr ProcM = IntPtr.Zero;
    private string AcpiDriverDllName = "ACPIdriver.dll";

    [DllImport("Kernel32.dll")]
    private static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("Kernel32.dll")]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [DllImport("Kernel32.dll")]
    private static extern bool FreeLibrary(IntPtr hModule);

    public void DEBUG(string message) => Console.WriteLine(message);

    public void AcpiLoadDll()
    {
      this.hModule = AcpiDriver.LoadLibrary(this.AcpiDriverDllName);
      if (this.hModule == IntPtr.Zero)
      {
        this.DEBUG("LoadLibrary file " + this.AcpiDriverDllName + " failed");
        throw new Exception("Load " + this.AcpiDriverDllName + " failed");
      }
    }

    public void AcpiLoadFun(string lpProcName, IntPtr Proc)
    {
      if (this.hModule == IntPtr.Zero)
      {
        this.DEBUG("hModule is null");
        throw new Exception("hModule is null");
      }
      Proc = AcpiDriver.GetProcAddress(this.hModule, lpProcName);
      if (Proc == IntPtr.Zero)
      {
        this.DEBUG("GetProcAddress " + lpProcName + " failed");
        throw new Exception("GetProcAddress " + lpProcName + " failed");
      }
    }

    public int GetAcpiValueProc(int Offset)
    {
      if (this.hModule == IntPtr.Zero)
      {
        this.DEBUG("hModule is null");
        throw new Exception("hModule is null");
      }
      this.getProc = AcpiDriver.GetProcAddress(this.hModule, "GetAcpiValue");
      if (this.getProc == IntPtr.Zero)
      {
        this.DEBUG("GetProcAddress GetAcpiValue failed");
        throw new Exception("GetProcAddress GetAcpiValue failed");
      }
      return ((AcpiDriver.GetValue) Marshal.GetDelegateForFunctionPointer(this.getProc, typeof (AcpiDriver.GetValue)))(Offset);
    }

    public void SetAcpiValueProc(int Offset)
    {
      if (this.hModule == IntPtr.Zero)
      {
        this.DEBUG("hModule is null");
        throw new Exception("hModule is null");
      }
      this.setProc = AcpiDriver.GetProcAddress(this.hModule, "SetAcpiValue");
      if (this.setProc == IntPtr.Zero)
      {
        this.DEBUG("GetProcAddress SetAcpiValue failed");
        throw new Exception("GetProcAddress SetAcpiValue failed");
      }
      ((AcpiDriver.SetValue) Marshal.GetDelegateForFunctionPointer(this.setProc, typeof (AcpiDriver.SetValue)))(Offset);
    }

    public void AcpiUnLoadDll()
    {
      AcpiDriver.FreeLibrary(this.hModule);
      this.hModule = IntPtr.Zero;
      this.getProc = IntPtr.Zero;
      this.setProc = IntPtr.Zero;
    }

    internal delegate int GetValue(int Offset);

    private delegate void SetValue(int Offset);
  }
}
