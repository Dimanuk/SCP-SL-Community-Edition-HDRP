using InventorySystem.GUI;
using System;

public static class InventoryGuiDisplayExtensions
{
    public static ushort[] GetItemsInOrder(this IInventoryGuiDisplayType display)
    {
        if (display is RadialInventory radial)
        {
            ushort[] copy = new ushort[radial.OrganizedContent.Length];
            Array.Copy(radial.OrganizedContent, copy, copy.Length);
            return copy;
        }

        return Array.Empty<ushort>();
    }
}