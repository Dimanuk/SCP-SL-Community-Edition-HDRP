using UnityEngine;
using UnityEngine.UI;

public class DropdownArrowRotator : MonoBehaviour
{
    [SerializeField]
    private Image ArrowImage;

    private void OnEnable()
    {
        if (ArrowImage != null)
        {
            ArrowImage.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    private void OnDestroy()
    {
        if (ArrowImage != null)
        {
            ArrowImage.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}