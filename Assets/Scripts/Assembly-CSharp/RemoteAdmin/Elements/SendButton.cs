using UnityEngine;

namespace RemoteAdmin.Elements
{
    public class SendButton : CustomButton
    {
        public string CustomCommand;
        public string CustomFormat;

        public override void Select()
        {
            if (string.IsNullOrEmpty(CustomCommand))
            {
                Debug.LogError(
                    $"[SendButton] {gameObject.name} has a null/empty custom command.",
                    gameObject);
                return;
            }

            SendCommand(CustomCommand, CustomFormat);
            base.Select();
        }

        protected virtual void SendCommand(string command, string format)
        {
            if (CommandMenu == null)
            {
                Debug.LogError($"[SendButton] {gameObject.name} has no CommandMenu.", gameObject);
                return;
            }

            CommandMenu.SendCommand(command, format);
        }
    }
}