using System;
using System.Runtime.InteropServices;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp049.Zombies
{

    public class ZombieConfirmationBox : MonoBehaviour
    {

        
        private const float ManualDeleteDelay = 2f;
        private const float AutomaticDeleteDelay = 15f;
        private const string TranslationKey = "SCP049_HUD";

        
        [SerializeField]
        private Image _progressBar;

        [SerializeField]
        private GameObject _root;

        [SerializeField]
        private TMP_Text _text;


        private readonly AbilityCooldown _cooldown;

        private KeyCode TargetKey => NewInput.GetKey((ActionName)16, KeyCode.None);


        private void Confirm()
        {
            NetworkClient.Send(new ScpReviveBlockMessage(), 0);

            string message = TranslationReader.Get(TranslationKey, 8, "You have denied further resurrection from SCP-049!");
            _text.text = message;

            _cooldown.Trigger(20f);
        }

        private void Start()
        {
            _root.SetActive(HideHUDController.IsHUDVisible);

            HideHUDController.ToggleHUD += _root.SetActive;

            string format = TranslationReader.Get(TranslationKey, 9, "Press {0} to deny further resurrection.");
            string keyName = $"<color=red>{TargetKey}</color>";
            _text.text = string.Format(format, keyName);

        }

        private void OnDestroy()
        {
            HideHUDController.ToggleHUD -= _root.SetActive;
        }

        private void Update()
        {
            _progressBar.fillAmount = 1f - _cooldown.Readiness;

            if (Input.GetKeyDown(TargetKey))
            {
                Confirm();
            }

            if (_cooldown.IsReady)
            {
                if (ReferenceHub.TryGetLocalHub(out var hub) && PlayerRolesUtils.IsAlive(hub))
                {
                    Destroy(gameObject);
                }
            }
        }

       private static void ServerReceiveMessage(NetworkConnection conn, ScpReviveBlockMessage msg)
        {
            if (!NetworkServer.active)
                return;

            if (conn?.identity == null)
                return;

            if (!ReferenceHub.TryGetHubNetID(conn.identity.netId, out var hub))
                return;

            if (Scp049ResurrectAbility.GetResurrectionsNumber(hub) == 0)
                return;

            Scp049ResurrectAbility.RegisterPlayerResurrection(hub, 4);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += () =>
            {
               NetworkServer.ReplaceHandler<ScpReviveBlockMessage>(ServerReceiveMessage, true);
            };
        }

        public ZombieConfirmationBox()
        {
            _cooldown = new AbilityCooldown();
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct ScpReviveBlockMessage : NetworkMessage
        {
        }
    }
}