using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public static class GpuDriver
{
    public const string Library = "GpuDriver.dll";

    public const CallingConvention CallingConv = CallingConvention.StdCall;

    public static string _driverVersion;

    public static readonly object _dataLock = new object();

    public static string DriverVersion
    {
        get
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return SystemInfo.graphicsDeviceVersion;
            }

            lock (_dataLock)
            {
                if (!string.IsNullOrWhiteSpace(_driverVersion))
                {
                    return _driverVersion;
                }

                try
                {
                    string gpuName = SystemInfo.graphicsDeviceName;
                    Thread thread = new Thread((ThreadStart)delegate
                    {
                        lock (_dataLock)
                        {
                            IntPtr driverVersion = GetDriverVersion(gpuName);
                            if (driverVersion == IntPtr.Zero)
                            {
                                Debug.LogWarning("GPU Driver version for " + gpuName + " not found!");
                                driverVersion = GetDriverVersion(null);
                            }

                            _driverVersion = Marshal.PtrToStringUni(driverVersion) ?? "Loading failed";
                            Free(driverVersion);
                            Debug.Log("GPU Driver version: " + _driverVersion);
                            MainThreadDispatcher.Dispatch(delegate
                            {
                                GpuDriver.DriverLoaded?.Invoke(_driverVersion);
                            });
                        }
                    });
                    thread.IsBackground = true;
                    thread.SetApartmentState(ApartmentState.MTA);
                    thread.Start();
                    return "Loading...";
                }
                catch (Exception message)
                {
                    Debug.Log(message);
                    return SystemInfo.graphicsDeviceVersion;
                }
            }
        }
    }

    public static event Action<string> DriverLoaded;

    [DllImport("GpuDriver.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "get_gpu_driver")]
    public static extern IntPtr GetDriverVersion([MarshalAs(UnmanagedType.LPWStr)] string name);

    [DllImport("GpuDriver.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "free_driver")]
    public static extern void Free(IntPtr version);
}
