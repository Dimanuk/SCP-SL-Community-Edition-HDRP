using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class BroadcastAssigner : MonoBehaviour
{
    public static bool Displaying;
    public static bool MessageDisplayed;
    private static float _timeLeft;

    public TMP_Text Display;

    private void Start()
    {
        Displaying = false;
        MessageDisplayed = false;
        _timeLeft = 0f;

        if (Display != null)
        {
            Display.enabled = false;
            Display.text = string.Empty;
        }
    }

    private void Update()
    {
        if (!Displaying)
            return;

        if (MessageDisplayed)
        {
            _timeLeft -= Time.deltaTime;

            if (_timeLeft > 0f)
                return;

            MessageDisplayed = false;
        }

        if (Broadcast.Messages == null || Broadcast.Messages.Count == 0)
        {
            Displaying = false;
            
            if (Display != null)
            {
                Display.enabled = false;
                Display.text = string.Empty;
            }
            
            _timeLeft = 0f;
            return;
        }

        BroadcastMessage msg = Broadcast.Messages.Dequeue();
        MessageDisplayed = true;

        if (msg != null && Display != null)
        {
            _timeLeft = msg.Time;

            Display.text = msg.Text;
            Display.overflowMode = msg.Truncated ? TextOverflowModes.Truncate : TextOverflowModes.Ellipsis;
            Display.enabled = true;
        }
    }
}