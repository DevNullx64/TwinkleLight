// Decompiled with JetBrains decompiler
// Type: LedControl.KeyboardHookLib
// Assembly: LedControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2625D33F-FFFD-422A-94C4-E655C9C02803
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbControl.exe

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LedControl
{
  public class KeyboardHookLib
  {
    private const int WH_KEYBOARD_LL = 13;
    private const int WH_KEYDOWN_MS = 256;
    private const int WH_KEYUP_MS = 257;
    private static int _hHookValue;
    private KeyboardHookLib.HookHandle _KeyBoardHookProcedure;
    private IntPtr _hookWindowPtr = IntPtr.Zero;
    private static KeyboardHookLib.ProcessKeyHandle _clientMethod;

    [DllImport("user32.dll")]
    private static extern int SetWindowsHookEx(
      int idHook,
      KeyboardHookLib.HookHandle lpfn,
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

    public void InstallHook(KeyboardHookLib.ProcessKeyHandle clientMethod)
    {
      KeyboardHookLib._clientMethod = clientMethod;
      if (KeyboardHookLib._hHookValue != 0)
        return;
      this._KeyBoardHookProcedure = new KeyboardHookLib.HookHandle(KeyboardHookLib.OnHookProc);
      this._hookWindowPtr = KeyboardHookLib.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
      KeyboardHookLib._hHookValue = KeyboardHookLib.SetWindowsHookEx(13, this._KeyBoardHookProcedure, this._hookWindowPtr, 0);
      if (KeyboardHookLib._hHookValue != 0)
        return;
      this.UninstallHook();
    }

    public void UninstallHook()
    {
      if (KeyboardHookLib._hHookValue == 0 || !KeyboardHookLib.UnhookWindowsHookEx(KeyboardHookLib._hHookValue))
        return;
      KeyboardHookLib._hHookValue = 0;
    }

    private static int OnHookProc(int nCode, int wParam, IntPtr lParam)
    {
      if (nCode >= 0)
      {
        KeyboardHookLib.HookStruct structure = (KeyboardHookLib.HookStruct) Marshal.PtrToStructure(lParam, typeof (KeyboardHookLib.HookStruct));
        if (KeyboardHookLib._clientMethod != null && wParam == 256)
        {
          bool handle = false;
          KeyboardHookLib._clientMethod(structure, out handle);
          if (handle)
            return 1;
        }
      }
      return KeyboardHookLib.CallNextHookEx(KeyboardHookLib._hHookValue, nCode, wParam, lParam);
    }

    private delegate int HookHandle(int nCode, int wParam, IntPtr lParam);

    public delegate void ProcessKeyHandle(KeyboardHookLib.HookStruct param, out bool handle);

    [StructLayout(LayoutKind.Sequential)]
    public class HookStruct
    {
      public int vkCode;
      public int scanCode;
      public int flags;
      public int time;
      public int dwExtraInfo;
    }
  }
}
