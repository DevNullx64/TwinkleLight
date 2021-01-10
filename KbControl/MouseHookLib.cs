// Decompiled with JetBrains decompiler
// Type: LedControl.MouseHookLib
// Assembly: LedControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2625D33F-FFFD-422A-94C4-E655C9C02803
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbControl.exe

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LedControl
{
  public class MouseHookLib
  {
    private const int WH_MOUSE_LL = 14;
    private const int WH_KEYDOWN_MS = 256;
    private const int WH_KEYUP_MS = 257;
    private const int WM_LBUTTONDOWN = 513;
    private static int _hHookValue;
    private MouseHookLib.HookHandle _MouseHookProcedure;
    private IntPtr _hookWindowPtr = IntPtr.Zero;
    private static MouseHookLib.ProcessKeyHandle _clientMethod;

    [DllImport("user32.dll")]
    private static extern int SetWindowsHookEx(
      int idHook,
      MouseHookLib.HookHandle lpfn,
      IntPtr hInstance,
      int threadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern bool UnhookWindowsHookEx(int idHook);

    [DllImport("user32.dll")]
    private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern int GetCurrentThreadId();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string name);

    private static void DEBUG(string message) => Console.WriteLine(message);

    public void InstallHook(MouseHookLib.ProcessKeyHandle clientMethod)
    {
      MouseHookLib._clientMethod = clientMethod;
      if (MouseHookLib._hHookValue != 0)
        return;
      this._MouseHookProcedure = new MouseHookLib.HookHandle(MouseHookLib.OnHookProc);
      this._hookWindowPtr = MouseHookLib.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
      MouseHookLib._hHookValue = MouseHookLib.SetWindowsHookEx(14, this._MouseHookProcedure, this._hookWindowPtr, 0);
      if (MouseHookLib._hHookValue != 0)
        return;
      this.UninstallHook();
    }

    public void UninstallHook()
    {
      if (MouseHookLib._hHookValue == 0 || !MouseHookLib.UnhookWindowsHookEx(MouseHookLib._hHookValue))
        return;
      MouseHookLib._hHookValue = 0;
    }

    private static int OnHookProc(int nCode, int wParam, IntPtr lParam)
    {
      if (nCode >= 0)
      {
        MouseHookLib.HookStruct structure = (MouseHookLib.HookStruct) Marshal.PtrToStructure(lParam, typeof (MouseHookLib.HookStruct));
        if (MouseHookLib._clientMethod != null && wParam == 513)
        {
          bool handle = false;
          MouseHookLib.DEBUG(wParam.ToString());
          MouseHookLib._clientMethod(structure, out handle);
          if (handle)
            return 1;
        }
      }
      return MouseHookLib.CallNextHookEx(MouseHookLib._hHookValue, nCode, wParam, lParam);
    }

    private delegate int HookHandle(int nCode, int wParam, IntPtr lParam);

    public delegate void ProcessKeyHandle(MouseHookLib.HookStruct param, out bool handle);

    [StructLayout(LayoutKind.Sequential)]
    public class PointStruct
    {
      public int x;
      public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class HookStruct
    {
      public MouseHookLib.PointStruct point;
      public int hwnd;
      public int wHitTestCode;
      public int dwExtraInfo;
    }
  }
}
