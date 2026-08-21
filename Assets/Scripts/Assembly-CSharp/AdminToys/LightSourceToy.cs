using System;
using Mirror;
using UnityEngine;

namespace AdminToys
{
    public class LightSourceToy : AdminToyBase
    {
        [SerializeField] private Light _light;

        [SyncVar] public float LightIntensity;
        [SyncVar] public float LightRange;
        [SyncVar] public Color LightColor;
        [SyncVar] public bool LightShadows;

        public override string CommandName => "LightSource";

        public override void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
        {
            base.OnSpawned(admin, arguments);
            transform.position = admin.PlayerCameraReference.position;
            transform.localScale = Vector3.one;
        }

        private void Update()
        {
            _light.intensity = LightIntensity;
            _light.range = LightRange;
            _light.color = LightColor;
            _light.shadows = LightShadows ? UnityEngine.LightShadows.Soft : UnityEngine.LightShadows.None;
        }
    }
}