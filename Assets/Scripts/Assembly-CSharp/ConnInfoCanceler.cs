using Mirror;
using UnityEngine;

public class ConnInfoCanceler : ConnInfoButton
{
    public override void UseButton()
    {
        base.UseButton();
        CustomNetworkManager manager = Object.FindFirstObjectByType<CustomNetworkManager>();
        if (manager != null)
            manager.StopClient();
    }
}