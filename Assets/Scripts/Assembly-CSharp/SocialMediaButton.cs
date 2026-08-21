using UnityEngine;
using UnityEngine.EventSystems;

public class SocialMediaButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static int _enable;

    [Header("References")]
    public RectTransform Parent;
    public Animator _animator;
    static SocialMediaButton()
    {
        _enable = Animator.StringToHash("Enable");
    }

    private void Start()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Parent != null) Parent.SetAsLastSibling();
        
        if (_animator != null) 
            _animator.SetBool(_enable, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_animator != null) 
            _animator.SetBool(_enable, false);
    }
}