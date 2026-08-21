using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using NorthwoodLib;

public class WindowsUpdateWarning : MonoBehaviour
{
    public GameObject warning;
    public GameObject menu;

    private void Start()
    {
        bool required = UpdateRequired();
        warning.SetActive(required);

        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            menu.SetActive(!warning.activeSelf);
        }
    }

    public static bool UpdateRequired()
    {
        if (SystemInfo.operatingSystemFamily != OperatingSystemFamily.Windows)
            return false;
		
        if (NorthwoodLib.OperatingSystem.Version.Major >= 7)
            return false;

        string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string dllPath = Path.Combine(systemFolder, "API-MS-WIN-CRT-MATH-L1-1-0.dll");


        if (File.Exists(dllPath))
            return false;

        try
        {
            return !CheckDll("API-MS-WIN-CRT-MATH-L1-1-0.dll");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return true; 
        }
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr LoadLibrary(string name);

    [DllImport("kernel32.dll")]
    private static extern bool FreeLibrary(IntPtr handle);

    private static bool CheckDll(string name)
    {
        IntPtr handle = LoadLibrary(name);
        if (handle == IntPtr.Zero)
        {
            throw new System.ComponentModel.Win32Exception();
        }
        FreeLibrary(handle);
        return true;
    }
}