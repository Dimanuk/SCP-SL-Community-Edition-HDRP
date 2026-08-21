using System;
using System.Collections.Generic;
using InventorySystem.Items.Thirdperson;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;
using AudioPooling;

namespace InventorySystem.Items.Flashlight
{
    public class FlashlightThirdpersonItem : IdleThirdpersonItem
    {
        [SerializeField]
        private Light _lightSource;

        [SerializeField]
        private Renderer[] _targetRenderers;

        [SerializeField]
        private Material _onMat;

        [SerializeField]
        private Material _offMat;

        private static FlashlightItem Template => FlashlightItem.Template;

        internal override void Initialize(HumanCharacterModel model, ItemIdentifier id)
        {
            base.Initialize(model, id);

            FlashlightNetworkHandler.OnStatusReceived += ProcesReceivedStatus;

            if (FlashlightNetworkHandler.ReceivedStatuses.TryGetValue(id.SerialNumber, out bool newState))
            {
                SetState(newState);
            }
        }

        private void OnDestroy()
        {
            FlashlightNetworkHandler.OnStatusReceived -= ProcesReceivedStatus;
        }

        private void ProcesReceivedStatus(FlashlightNetworkHandler.FlashlightMessage msg)
        {
            if (msg.Serial == Identifier.SerialNumber)
            {
                SetState(msg.NewState);
            }
        }

        private void SetState(bool newState)
        {
            if (_lightSource != null)
            {
                _lightSource.enabled = newState;
            }

            Material material = newState ? _onMat : _offMat;
            if (_targetRenderers != null)
            {
                foreach (Renderer renderer in _targetRenderers)
                {
                    if (renderer != null)
                    {
                        renderer.sharedMaterial = material;
                    }
                }
            }

            AudioClip clip = newState ? Template.OnClip : Template.OffClip;
            if (clip != null)
            {
                AudioSourcePoolManager.PlaySound(clip, transform.position, 1);
            }
        }
    }
}