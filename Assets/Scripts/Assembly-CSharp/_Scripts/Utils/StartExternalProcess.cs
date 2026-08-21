namespace _Scripts.Utils
{
    public static class StartExternalProcess
    {
        private struct PROCESS_INFORMATION
        {
            internal global::System.IntPtr hProcess;

            internal global::System.IntPtr hThread;

            internal uint dwProcessId;

            internal uint dwThreadId;
        }

        private struct STARTUPINFO
        {
            internal uint cb;

            internal global::System.IntPtr lpReserved;

            internal global::System.IntPtr lpDesktop;

            internal global::System.IntPtr lpTitle;

            internal uint dwX;

            internal uint dwY;

            internal uint dwXSize;

            internal uint dwYSize;

            internal uint dwXCountChars;

            internal uint dwYCountChars;

            internal uint dwFillAttribute;

            internal uint dwFlags;

            internal ushort wShowWindow;

            internal ushort cbReserved2;

            internal global::System.IntPtr lpReserved2;

            internal global::System.IntPtr hStdInput;

            internal global::System.IntPtr hStdOutput;

            internal global::System.IntPtr hStdError;
        }

        [global::System.Flags]
        private enum ProcessCreationFlags : uint
        {
            NONE = 0u,
            CREATE_BREAKAWAY_FROM_JOB = 0x1000000u,
            CREATE_DEFAULT_ERROR_MODE = 0x4000000u,
            CREATE_NEW_CONSOLE = 0x10u,
            CREATE_NEW_PROCESS_GROUP = 0x200u,
            CREATE_NO_WINDOW = 0x8000000u,
            CREATE_PROTECTED_PROCESS = 0x40000u,
            CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x2000000u,
            CREATE_SECURE_PROCESS = 0x400000u,
            CREATE_SEPARATE_WOW_VDM = 0x800u,
            CREATE_SHARED_WOW_VDM = 0x1000u,
            CREATE_SUSPENDED = 4u,
            CREATE_UNICODE_ENVIRONMENT = 0x400u,
            DEBUG_ONLY_THIS_PROCESS = 2u,
            DEBUG_PROCESS = 1u,
            DETACHED_PROCESS = 8u,
            EXTENDED_STARTUPINFO_PRESENT = 0x80000u,
            INHERIT_PARENT_AFFINITY = 0x10000u
        }

        [global::System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = global::System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        private static extern bool CreateProcessW(string lpApplicationName, [global::System.Runtime.InteropServices.In] string lpCommandLine, global::System.IntPtr procSecAttrs, global::System.IntPtr threadSecAttrs, bool bInheritHandles, global::_Scripts.Utils.StartExternalProcess.ProcessCreationFlags dwCreationFlags, global::System.IntPtr lpEnvironment, string lpCurrentDirectory, ref global::_Scripts.Utils.StartExternalProcess.STARTUPINFO lpStartupInfo, ref global::_Scripts.Utils.StartExternalProcess.PROCESS_INFORMATION lpProcessInformation);

        public static uint Start(string path, string dir, bool hidden = false)
        {
            global::_Scripts.Utils.StartExternalProcess.ProcessCreationFlags dwCreationFlags = (hidden ? global::_Scripts.Utils.StartExternalProcess.ProcessCreationFlags.CREATE_NO_WINDOW : global::_Scripts.Utils.StartExternalProcess.ProcessCreationFlags.NONE);
            global::_Scripts.Utils.StartExternalProcess.STARTUPINFO lpStartupInfo = new global::_Scripts.Utils.StartExternalProcess.STARTUPINFO
            {
                cb = (uint)global::System.Runtime.InteropServices.Marshal.SizeOf<global::_Scripts.Utils.StartExternalProcess.STARTUPINFO>()
            };
            global::_Scripts.Utils.StartExternalProcess.PROCESS_INFORMATION lpProcessInformation = default(global::_Scripts.Utils.StartExternalProcess.PROCESS_INFORMATION);
            if (!CreateProcessW(null, path, global::System.IntPtr.Zero, global::System.IntPtr.Zero, bInheritHandles: false, dwCreationFlags, global::System.IntPtr.Zero, dir, ref lpStartupInfo, ref lpProcessInformation))
            {
                throw new global::System.ComponentModel.Win32Exception();
            }
            return lpProcessInformation.dwProcessId;
        }
    }
}
