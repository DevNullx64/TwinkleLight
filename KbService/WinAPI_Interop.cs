// Decompiled with JetBrains decompiler
// Type: KbService.WinAPI_Interop
// Assembly: KbService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1EE4A412-47D9-4D2C-8294-3E09959B4F08
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbService.exe

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace KbService
{
    public class WinAPI_Interop
    {
        public const int GENERIC_ALL_ACCESS = 0x10000000;
        private const int TOKEN_QUERY = 8;
        private const int TOKEN_ADJUST_PRIVILEGES = 32;
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        public static void ShowServiceMessage(string message, string title)
        {
            WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, WTSGetActiveConsoleSessionId(), title, title.Length, message, message.Length, 0, 0, out int pResponse, false);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSSendMessage(
          IntPtr hServer,
          int SessionId,
          string pTitle,
          int TitleLength,
          string pMessage,
          int MessageLength,
          int Style,
          int Timeout,
          out int pResponse,
          bool bWait);

        [DllImport("WTSAPI32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WTSEnumerateSessions(
          IntPtr hServer,
          [MarshalAs(UnmanagedType.U4)] uint Reserved,
          [MarshalAs(UnmanagedType.U4)] uint Version,
          ref IntPtr ppSessionInfo,
          [MarshalAs(UnmanagedType.U4)] ref uint pSessionInfoCount);

        [DllImport("WTSAPI32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("WTSAPI32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WTSQueryUserToken(uint sessionId, out IntPtr Token);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DuplicateTokenEx(
          IntPtr hExistingToken,
          int dwDesiredAccess,
          ref SECURITY_ATTRIBUTES lpThreadAttributes,
          int ImpersonationLevel,
          int dwTokenType,
          ref IntPtr phNewToken);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSQueryUserToken(int sessionId, out IntPtr Token);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool CreateEnvironmentBlock(
          out IntPtr lpEnvironment,
          IntPtr hToken,
          bool bInherit);

        [DllImport("ADVAPI32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CreateProcessAsUser(
          IntPtr hToken,
          string lpApplicationName,
          StringBuilder lpCommandLine,
          IntPtr lpProcessAttributes,
          IntPtr lpThreadAttributes,
          bool bInheritHandles,
          uint dwCreationFlags,
          IntPtr lpEnvironment,
          string lpCurrentDirectory,
          ref STARTUPINFO lpStartupInfo,
          out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("KERNEL32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(
          IntPtr htok,
          bool disall,
          ref TOKEN_PRIVILEGE newst,
          int len,
          IntPtr prev,
          IntPtr relen);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetTokenInformation(
          IntPtr hToken,
          TOKEN_INFORMATION_CLASS tokenInfoClass,
          IntPtr pTokenInfo,
          int tokenInfoLength,
          out int returnLength);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
          int dwDesiredAccess,
          bool bInheritHandle,
          int dwProcessId);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        public static void CreateProcess(string ChildProcName)
        {
            IntPtr _base = IntPtr.Zero;
            uint pSessionInfoCount = 0;
            if (!WTSEnumerateSessions(WTS_CURRENT_SERVER_HANDLE, 0U, 1U, ref _base, ref pSessionInfoCount))
                return;
            for (int index = 0; index < pSessionInfoCount; ++index)
            {
                WTS_SESSION_INFO structure = (WTS_SESSION_INFO)Marshal.PtrToStructure(_base + index * Marshal.SizeOf(typeof(WTS_SESSION_INFO)), typeof(WTS_SESSION_INFO));
                if (structure.State == WTS_CONNECTSTATE_CLASS.WTSActive)
                {
                    IntPtr Token = IntPtr.Zero;
                    if (WTSQueryUserToken(structure.SessionID, out Token))
                    {
                        PROCESS_INFORMATION lpProcessInformation;
                        STARTUPINFO a = STARTUPINFO.New();
                        if (CreateProcessAsUser(Token, ChildProcName, null, IntPtr.Zero, IntPtr.Zero, false, 0U, IntPtr.Zero, null, ref a, out lpProcessInformation))
                        {
                            CloseHandle(lpProcessInformation.hThread);
                            CloseHandle(lpProcessInformation.hProcess);
                        }
                        else
                            ShowServiceMessage("CreateProcessAsUser Failed", nameof(CreateProcess));
                        CloseHandle(Token);
                        break;
                    }
                }
            }
            WTSFreeMemory(_base);
        }

        public static void CreateProcessAd(string app, StringBuilder path)
        {
            IntPtr zero = IntPtr.Zero;
            SECURITY_ATTRIBUTES lpThreadAttributes = new SECURITY_ATTRIBUTES();
            lpThreadAttributes.Length = Marshal.SizeOf(lpThreadAttributes);
            STARTUPINFO lpStartupInfo = new STARTUPINFO();
            lpStartupInfo.cb = Marshal.SizeOf(lpStartupInfo);
            lpStartupInfo.lpDesktop = "winsta0\\default";
            WTSQueryUserToken(WTSGetActiveConsoleSessionId(), out IntPtr Token);
            IntPtr num = OpenProcess(0x1F0FFF, false, Process.GetProcessesByName("winlogon")[0].Id);
            if (!OpenProcessToken(num, 0x201EB, ref Token))
                throw new Exception("Open Process Token fail");
            if (!DuplicateTokenEx(Token, 0x10000000, ref lpThreadAttributes, 1, 1, ref zero))
                ShowServiceMessage("DuplicateTokenEx failed", "AlertService Message");

            if (!CreateEnvironmentBlock(out IntPtr lpEnvironment, zero, false))
                ShowServiceMessage("CreateEnvironmentBlock failed", "AlertService Message");
            if (!CreateProcessAsUser(zero, app, path, IntPtr.Zero, IntPtr.Zero, false, 0x400U, lpEnvironment, null, ref lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation))
                ShowServiceMessage(string.Format("CreateProcessAsUser Error: {0}", Marshal.GetLastWin32Error()), "AlertService Message");
            if (lpProcessInformation.hProcess != IntPtr.Zero)
                CloseHandle(lpProcessInformation.hProcess);
            if (lpProcessInformation.hThread != IntPtr.Zero)
                CloseHandle(lpProcessInformation.hThread);
            if (zero != IntPtr.Zero)
                CloseHandle(zero);
            if (Token != IntPtr.Zero)
                CloseHandle(Token);
            if (num != IntPtr.Zero)
                CloseHandle(num);
        }

        private enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WTS_SESSION_INFO
        {
            public uint SessionID;
            public string pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;

            public static STARTUPINFO New() => new STARTUPINFO() { cb = Marshal.SizeOf(typeof(STARTUPINFO)) };
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        public struct SECURITY_ATTRIBUTES
        {
            public int Length;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        public enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation,
        }

        public enum TOKEN_TYPE
        {
            TokenPrimary = 1,
            TokenImpersonation = 2,
        }

        internal enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups = 2,
            TokenPrivileges = 3,
            TokenOwner = 4,
            TokenPrimaryGroup = 5,
            TokenDefaultDacl = 6,
            TokenSource = 7,
            TokenType = 8,
            TokenImpersonationLevel = 9,
            TokenStatistics = 10, // 0x0000000A
            TokenRestrictedSids = 11, // 0x0000000B
            TokenSessionId = 12, // 0x0000000C
            TokenGroupsAndPrivileges = 13, // 0x0000000D
            TokenSessionReference = 14, // 0x0000000E
            TokenSandBoxInert = 15, // 0x0000000F
            TokenAuditPolicy = 16, // 0x00000010
            TokenOrigin = 17, // 0x00000011
            TokenElevationType = 18, // 0x00000012
            TokenLinkedToken = 19, // 0x00000013
            TokenElevation = 20, // 0x00000014
            TokenHasRestrictions = 21, // 0x00000015
            TokenAccessInformation = 22, // 0x00000016
            TokenVirtualizationAllowed = 23, // 0x00000017
            TokenVirtualizationEnabled = 24, // 0x00000018
            TokenIntegrityLevel = 25, // 0x00000019
            TokenUIAccess = 26, // 0x0000001A
            TokenMandatoryPolicy = 27, // 0x0000001B
            TokenLogonSid = 28, // 0x0000001C
            MaxTokenInfoClass = 29, // 0x0000001D
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct LUID
        {
            internal uint LowPart;
            internal uint HighPart;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct LUID_AND_ATTRIBUTES
        {
            internal long Luid;
            internal uint Attributes;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct TOKEN_PRIVILEGE
        {
            internal uint PrivilegeCount;
            internal LUID_AND_ATTRIBUTES Privilege;
        }
    }
}