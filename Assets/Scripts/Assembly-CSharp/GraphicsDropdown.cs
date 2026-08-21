using NorthwoodLib;
using UnityEngine;
using UnityEngine.Rendering;

public class GraphicsDropdown : MonoBehaviour
{
    public DropDownController controller;

    private void Start()
    {
        bool enable = false;
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11)
        {
            if (OperatingSystem.Version.Major >= 10)
            {
                enable = true;
            }
        }

        controller.EnableOption(2, enable);
    }
}