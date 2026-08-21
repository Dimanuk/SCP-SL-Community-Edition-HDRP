using InventorySystem.Items.Firearms.Attachments.Components;
using System;
using UnityEngine;

namespace InventorySystem.Items.Firearms.FunctionalParts
{
    public class ReflexSight : FunctionalFirearmPart
    {
        private static readonly int HashTexture;
        private static readonly int HashColor;
        private static readonly int HashSize;

        [SerializeField] private Renderer _targetRenderer;
        [SerializeField] private float _sizeMultiplier;
        [SerializeField] private Quaternion _calibratedRotation;

        private ReflexSightAttachment _sightAtt;
        private Transform _rendT;

        private const float StartAngle = 2.9f;
        private const float EndAngle = 5f;

        private Quaternion CamRotation
        {
            get
            {
                Firearm fa = Firearm;
                Transform t = fa.Owner.PlayerCameraReference;
                return t.rotation;
            }
        }

        private void Start()
        {
            Material src = _targetRenderer.sharedMaterial;
            Material inst = new Material(src);
            _targetRenderer.material = inst;
            _rendT = _targetRenderer.transform;

            Firearm fa = Firearm;
            if (!(fa.ViewModel is AnimatedFirearmViewmodel viewmodel))
                return;

            // Attachment components live on the firearm object, not the viewmodel.
            // This ReflexSight sits on one of the viewmodel's per-attachment
            // ToggleableObjects — that entry's index maps to fa.Attachments.
            var settings = viewmodel.Attachments;
            for (int i = 0; i < settings.Length; i++)
            {
                GameObject[] toggleables = settings[i].ToggleableObjects;
                if (toggleables == null)
                    continue;

                for (int j = 0; j < toggleables.Length; j++)
                {
                    if (toggleables[j] == null)
                        continue;

                    if (toggleables[j].GetInstanceID() != gameObject.GetInstanceID())
                        continue;

                    _sightAtt = fa.Attachments[i] as ReflexSightAttachment;
                    break;
                }
            }

            if (_sightAtt == null)
                return;

            UpdateValues();
            _sightAtt.OnValuesChanged = (Action)Delegate.Combine(
                _sightAtt.OnValuesChanged,
                new Action(UpdateValues));
        }

        private void LateUpdate()
        {
            _rendT.rotation = CamRotation;

            float angle = Quaternion.Angle(_rendT.localRotation, _calibratedRotation);
            float t = Mathf.InverseLerp(StartAngle, EndAngle, angle);
            _rendT.localRotation = Quaternion.Lerp(_calibratedRotation, _rendT.localRotation, t);
        }


        private void UpdateValues()
        {
            ReflexSightReticlePack texOptions = _sightAtt.TextureOptions;
            Texture reticle = texOptions[_sightAtt.CurTexture];

            float[] sizes = ReflexSightAttachment.Sizes;
            float size = sizes[_sightAtt.CurSize];

            Color[] colors = ReflexSightAttachment.Colors;
            int colorIndex = Mathf.Clamp(_sightAtt.CurColor, 0, colors.Length - 1);
            Color color = colors[colorIndex];

            SetMaterial(reticle, size, color);
        }

        private void SetMaterial(Texture texture, float size, Color color)
        {
            Material mat = _targetRenderer.sharedMaterial;

            mat.SetColor(HashColor, color);
            mat.SetTexture(HashTexture, texture);
            mat.SetFloat(HashSize, size * _sizeMultiplier);
        }

        static ReflexSight()
        {
            HashTexture = Shader.PropertyToID("_RedDotTex");
            HashColor = Shader.PropertyToID("_RedDotColor");
            HashSize = Shader.PropertyToID("_RedDotSize");
        }
    }
}