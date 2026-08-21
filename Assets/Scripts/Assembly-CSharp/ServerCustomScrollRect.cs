using UnityEngine;
using UnityEngine.UI;

public class ServerCustomScrollRect : MonoBehaviour
{

    public ServerFilter BrowserReference;

    public MouseAreaDetector ScrollArea;

    public Scrollbar TargetScrollbar;

    private void Update()
    {
        if (BrowserReference == null || BrowserReference.FilteredListItems == null)
            return;

        int itemCount = BrowserReference.FilteredListItems.Count;
        if (itemCount <= 0) return;

        float size = Mathf.Clamp(1f / itemCount, 0.03f, 1f);
        TargetScrollbar.size = size;
        TargetScrollbar.gameObject.SetActive(itemCount > 1);

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        float scrollDelta = Mathf.Clamp(scrollInput * 100f, -1f, 1f);

        int newScrollStart = Mathf.Clamp(
            BrowserReference.ScrollStartPoint - Mathf.RoundToInt(scrollDelta),
            0,
            itemCount - 1
        );

        if (newScrollStart != BrowserReference.ScrollStartPoint &&
            ScrollArea != null &&
            ScrollArea.IsInBorders())
        {
            BrowserReference.ScrollStartPoint = newScrollStart;
            BrowserReference.DisplayServers();
        }

        TargetScrollbar.value = itemCount > 1
            ? (float)BrowserReference.ScrollStartPoint / (itemCount - 1)
            : 0f;
    }

    public void ScrollbarValueChanged()
    {
        if (BrowserReference == null || BrowserReference.FilteredListItems == null)
            return;

        int itemCount = BrowserReference.FilteredListItems.Count;
        if (itemCount <= 0) return;

        float scrollValue = TargetScrollbar.value;
        int index = Mathf.RoundToInt((itemCount - 1) * scrollValue);
        BrowserReference.ScrollStartPoint = Mathf.Clamp(index, 0, itemCount - 1);
        BrowserReference.DisplayServers();
    }
}