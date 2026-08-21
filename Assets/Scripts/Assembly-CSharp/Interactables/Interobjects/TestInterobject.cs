using Interactables.Verification;
using Mirror;
using TMPro;
using UnityEngine;

namespace Interactables.Interobjects
{
    public class TestInterobject : NetworkBehaviour, IClientInteractable, IInteractable, IServerInteractable
    {
        [SerializeField] private TextMeshProUGUI ClientText;
        [SerializeField] private TextMeshProUGUI GlobalText;

        public IVerificationRule VerificationRule => StandardDistanceVerification.Default;

        [Client]
        public void ClientInteract(InteractableCollider collider)
        {
            if (!NetworkClient.active)
                return;

            if (ClientText != null)
            {
                ClientText.text = "You interacted with the test object!";
            }
        }

        [Server]
        public void ServerInteract(ReferenceHub ply, byte colliderId)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void Interactables.Interobjects.TestInterobject::ServerInteract(ReferenceHub,System.Byte)' called when server was not active");
                return;
            }

            RpcSendText($"Player {ply.nicknameSync.MyNick} interacted with the test object at {Time.time:F2}");
        }

        [ClientRpc]
        private void RpcSendText(string s)
        {
            if (GlobalText != null)
            {
                GlobalText.text = s;
            }
        }
    }
}