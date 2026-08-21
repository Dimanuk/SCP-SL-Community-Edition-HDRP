using PlayerStatsSystem;
using PostProcessing;
using SCPE;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Interactables.Interobjects
{
	public class SqueakInteraction : PopupInterobject, IDestructible
	{
		private SqueakSpawner _spawner;

		private TextMeshProUGUI _mouseText;

		private GameObject _mouseGameObject;

		private PostProcessVolume _tempVolume;

        public uint NetworkId => _spawner.netId;

        public Vector3 CenterOfMass => Vector3.zero;

        public bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHitPos)
        {
            if (handler is AttackerDamageHandler attackerDamageHandler && attackerDamageHandler.Attacker.Hub != null)
            {
                _spawner.TargetHitMouse(attackerDamageHandler.Attacker.Hub.networkIdentity.connectionToClient);
                return true;
            }
            return false;
        }

        protected override void OnClientStateChange()
        {
            
            if (this._tempVolume != null)
            {
                if (this._mouseGameObject != null)
                    this._mouseGameObject.SetActive(true);

                RuntimeUtilities.DestroyVolume(this._tempVolume, true, true);
                this._tempVolume = null;
                return;
            }

            if (this._mouseGameObject != null)
                this._mouseGameObject.SetActive(true);

            var ripples = ScriptableObject.CreateInstance<Ripples>();   
            ripples.enabled.Override(true);
            ripples.strength.Override(0f);      
            ripples.distance.Override(0f);
            ripples.width.Override(0f);
            ripples.height.Override(0f);

            var darken = ScriptableObject.CreateInstance<Darken>();          
            darken.enabled.Override(true);
            darken.intensity.Override(0f);

            this._tempVolume = PostProcessManager.instance.QuickVolume(
                layer: 23,
                priority: 0f,
                ripples,
                darken
            );

            if (this._tempVolume != null)
            {
                this._tempVolume.weight = 0f;
                this._tempVolume.isGlobal = true;  
            }
            else
            {
                Debug.LogError("[SqueakInteraction] Не удалось создать PostProcessVolume через QuickVolume!");
            }
        }

        protected override void OnClientUpdate(float enableRatio)
        {
            if (_tempVolume != null)
            {
                _tempVolume.weight = enableRatio;
            }

            if (_mouseText != null)
            {
                Color textColor = _mouseText.color;
                textColor.a = enableRatio * 0.5f;
                _mouseText.color = textColor;
            }
        }

        private void Awake()
        {
            _spawner = GetComponentInParent<SqueakSpawner>();

            if (UserMainInterface.Singleton != null)
            {
                _mouseGameObject = UserMainInterface.Singleton.mouseGameObject;
                _mouseText = _mouseGameObject?.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }
}
