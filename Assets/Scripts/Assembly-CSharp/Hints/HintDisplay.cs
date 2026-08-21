using InventorySystem.Searching;
using Mirror;
using Security;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hints
{
    [RequireComponent(typeof(SearchCoordinator))]
    public class HintDisplay : NetworkBehaviour
    {
        private RateLimit _showRateLimit;
        private ReferenceHub _hub;
        private Stack<Hint> _hints;
        private TextMeshProUGUI _textbox;

        public static readonly HashSet<NetworkConnection> SuppressedReceivers = new HashSet<NetworkConnection>();

        public Hint ActiveHint { get; private set; }

        private void Start()
        {
            if (base.isLocalPlayer)
            {
                this._showRateLimit = new RateLimit(10, 5f, null);
                this._hints = new Stack<Hint>();

                if (UserMainInterface.Singleton != null)
                {
                    this._textbox = UserMainInterface.Singleton.hintText;
                }

                this._hub = ReferenceHub.GetHub(base.gameObject); 

                NetworkClient.ReplaceHandler<HintMessage>(this.ShowLocalRatelimited, true);
            }
        }

        private void Update()
        {
            if (!base.isLocalPlayer) return;

            if (ActiveHint == null)
            {
                if (_textbox != null && _textbox.text.Length != 0)
                    _textbox.text = string.Empty;
                return;
            }

            float currentTime = Time.time;
            float progress = ActiveHint.Update(currentTime);

            if (progress >= 1f)
            {
                ActiveHint.Dispose();

                if (_hints.Count > 0)
                {
                    _hints.Pop();
                }

                ActiveHint = (_hints.Count > 0) ? _hints.Peek() : null;

                if (ActiveHint == null && _textbox != null)
                    _textbox.text = string.Empty;
            }
        }

        private void PushHint(Hint hint)
        {
            if (hint == null) return;
            _hints.Push(hint);
            ActiveHint = hint;
        }

        private void PopHint()
        {
            if (_hints.Count > 0)
            {
                _hints.Pop();
            }
            ActiveHint = (_hints.Count > 0) ? _hints.Peek() : null;
        }

        private void ShowLocal(Hint hint)
        {
            if (hint != null)
            {
                SharedHintData data = new(base.gameObject, _textbox, Time.time);
                
                hint.Awake(data);
                _hints.Push(hint); 
                ActiveHint = hint;
            }
            else
            {
                 Debug.LogError("Attempted to display an empty hint!");
            }
        }

        private void ShowLocalRatelimited(HintMessage message)
        {
            if (_showRateLimit != null && _showRateLimit.CanExecute(true))
            {
                this.ShowLocal(message.Content);
            }
        }

        public void Show(Hint hint)
        {
            if (hint == null) throw new ArgumentNullException(nameof(hint));

            if (base.isLocalPlayer)
            {
                this.ShowLocal(hint);
            }
            else if (NetworkServer.active)
            {
                NetworkConnection conn = base.netIdentity.connectionToClient;
                if (conn != null && !SuppressedReceivers.Contains(conn))
                {
                    conn.Send(new HintMessage(hint));
                }
            }
        }
    }
}