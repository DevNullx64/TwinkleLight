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
        private const int WH_MOUSE_LL = 0x0000000E;
        private const int WH_KEYDOWN_MS = 0x00000100;
        private const int WH_KEYUP_MS = 0x00000101;
        private const int WM_LBUTTONDOWN = 0x00000201;

        private static int _hHookValue;
        private HookHandle _MouseHookProcedure;
        private readonly IntPtr _hookWindowPtr = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
        private static ProcessMouseHandle _clientMethod;

        [DllImport("user32.dll")]
        private static extern int SetWindowsHookEx(
          int idHook,
          HookHandle lpfn,
          IntPtr hInstance,
          int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, HookStruct lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string name);

        public void InstallHook(ProcessMouseHandle clientMethod)
        {
            _clientMethod = clientMethod;
            if (_hHookValue == 0)
            {
                _MouseHookProcedure = new HookHandle(OnHookProc);
                _hHookValue = SetWindowsHookEx(WH_MOUSE_LL, _MouseHookProcedure, _hookWindowPtr, 0);
                if (_hHookValue == 0)
                    UninstallHook();
            }
        }

        public void UninstallHook()
        {
            if (_hHookValue != 0 && UnhookWindowsHookEx(_hHookValue))
                _hHookValue = 0;
        }

        private static int OnHookProc(int nCode, int wParam, HookStruct lParam) =>
            nCode >= 0 && _clientMethod != null && wParam == WM_LBUTTONDOWN && _clientMethod(lParam)
                ? 1
                : CallNextHookEx(_hHookValue, nCode, wParam, lParam);

        private delegate int HookHandle(int nCode, int wParam, HookStruct lParam);

        public delegate bool ProcessMouseHandle(HookStruct param);

        [StructLayout(LayoutKind.Sequential)]
        public class PointStruct
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class HookStruct
        {
            public PointStruct point;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }
    }
}