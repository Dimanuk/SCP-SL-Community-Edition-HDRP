using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldableButton : Button
{
    private readonly Stopwatch _holdSw = new Stopwatch();
    private bool _eventCalled;

    [CompilerGenerated]
    [SerializeField]
    private ButtonClickedEvent _003COnHeld_003Ek__BackingField;

    public bool IsHeld => IsPressed();

    public bool IsHovering
    {
        get
        {
            if (IsHighlighted())
                return true;
            return IsPressed();
        }
    }

    public float HeldPercent
    {
        get
        {
            if (HoldTime <= 0f)
                return 0f;
            if (!IsPressed())
                return 0f;
            return Mathf.Clamp01((float)_holdSw.Elapsed.TotalSeconds / HoldTime);
        }
    }

    [field: SerializeField]
    public float HoldTime { get; private set; }

    public ButtonClickedEvent OnHeld
    {
        [CompilerGenerated]
        get => _003COnHeld_003Ek__BackingField;
        [CompilerGenerated]
        private set => _003COnHeld_003Ek__BackingField = value;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        _holdSw.Restart();
        _eventCalled = false;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        _holdSw.Reset();
    }

    private void Update()
    {
        if (_eventCalled)
            return;
        if (!IsPressed())
            return;
        if (HoldTime <= 0f)
            return;
        if (!IsPressed())
            return;

        float percent = Mathf.Clamp01((float)_holdSw.Elapsed.TotalSeconds / HoldTime);
        if (percent < 1f)
            return;

        _eventCalled = true;
        OnHeld?.Invoke();
    }
}