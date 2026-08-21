using UnityEngine;

namespace InventorySystem.Items.Firearms.FunctionalParts
{
    public class AnimatedLaser : FunctionalFirearmPart
    {
        [SerializeField]
        private Transform _forwardTransform;

        [SerializeField]
        private Light _targetLight;

        [SerializeField]
        private AnimationCurve _truthnessOverAngle;

        private Transform _camForward;

        private Vector3 _localPos;

        private void Start()
        {
            Firearm fa = Firearm;
            _camForward = fa.Owner.PlayerCameraReference;
            _localPos.z = _targetLight.transform.localPosition.z;
        }

        private void LateUpdate()
        {
            Firearm fa = Firearm;
            _ = fa.HitregModule;

            Vector3 camDir = _camForward.forward;
            Vector3 barrelDir = _forwardTransform.forward;
            float angle = Vector3.Angle(camDir, barrelDir);
            float truthness = _truthnessOverAngle.Evaluate(angle);

            Transform lightT = _targetLight.transform;
            Quaternion targetRot = Quaternion.LookRotation(camDir);
            lightT.rotation = Quaternion.Lerp(_targetLight.transform.rotation, targetRot, truthness);

            lightT.localPosition = Vector3.forward * (_localPos.z * truthness);
            _targetLight.color = Color.Lerp(Color.black, Color.red, truthness);
        }
    }
}