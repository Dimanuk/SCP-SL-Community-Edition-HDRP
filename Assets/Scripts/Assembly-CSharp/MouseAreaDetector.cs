using UnityEngine;
using UnityEngine.UI;

public class MouseAreaDetector : MonoBehaviour
{
    public CanvasScaler SourceCanvas;
    public RectTransform TopLeftBorder;
    public RectTransform TopRightBorder;
    public RectTransform BottomLeftBorder;
    public RectTransform BottomRightBorder;

    private void Start()
    {
        if (SourceCanvas == null)
            return;

        if (SourceCanvas.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            Debug.LogError("Canvas must be set to ScaleWithScreenSize, so MouseAreaDetector can work");
            enabled = false;
        }
    }

    public bool IsInBorders()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        if (TopRightBorder != null)
        {
            Vector3 pos = TopRightBorder.transform.position;
            if (mousePos2D.x > pos.x)
                return false;
        }
        else
        {
            return false;
        }

        if (TopLeftBorder != null)
        {
            Vector3 pos = TopLeftBorder.transform.position;
            if (mousePos2D.y > pos.y)
                return false;
        }
        else
        {
            return false;
        }

        if (BottomLeftBorder != null)
        {
            Vector3 pos = BottomLeftBorder.transform.position;
            if (mousePos2D.x < pos.x)
                return false;
        }
        else
        {
            return false;
        }

        if (BottomRightBorder != null)
        {
            Vector3 pos = BottomRightBorder.transform.position;
            if (mousePos2D.y < pos.y)
                return false;
        }
        else
        {
            return false;
        }

        return true;
    }
}