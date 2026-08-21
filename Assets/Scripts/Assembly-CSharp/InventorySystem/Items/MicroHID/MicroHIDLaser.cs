using System.Diagnostics;
using GameObjectPools;
using InventorySystem.Items.Thirdperson;
using UnityEngine;

namespace InventorySystem.Items.MicroHID
{
    // Restored from CODES/ClientCode/CPP2IL_ISIL_Client/IsilDump/.../MicroHIDLaser.txt.
    // The lightning beam is NOT positioned from code: _targetTransform is the "ShotFX" node
    // (rest scale 0,0,0) that parents every firing visual (Lightning_mesh VAT arc,
    // Lightning_Strokes particles, sparks, HidLight). Update only scales it —
    // (thickness, lengthCurve, thickness) — so never call SetPositionAndRotation here.
    // For the local player _laserTriggered is set by an AnimationEvent ("TriggerLaser")
    // inside Micro_FireStart.anim and stays true for the whole firing state.
    public class MicroHIDLaser : MonoBehaviour, IPoolSpawnable
    {
        [SerializeField]
        private MicroHIDViewmodel _hidViewmodel;

        [SerializeField]
        private Transform _targetTransform;

        [SerializeField]
        private Light _targetLight;

        [SerializeField]
        private Transform _forwardTransform;

        [SerializeField]
        private Transform _gauge;

        [SerializeField]
        private LayerMask _laserMask;

        [SerializeField]
        private float _laserMaxDistance;

        [SerializeField]
        private AnimationCurve _laserScaleOverDistance;

        [SerializeField]
        private float _scalingSpeed;

        private float _currentScale;
        private float _lastFiringScale;
        private bool _laserTriggered;
        private bool _thirdpersonMode;
        private ushort _serial;

        private readonly Stopwatch _firingWarmup = new Stopwatch();

        private const double ThirdpersonWarmupSeconds = 1.7;
        private const float FiringShakeAmount = 0.06f;

        private bool Firing
        {
            get
            {
                if (!_thirdpersonMode)
                {
                    return _hidViewmodel.State == HidState.Firing && _laserTriggered;
                }

                if (MicroHIDHandler.SyncStates.TryGetValue(_serial, out HidStatusMessage message))
                {
                    if (message.MessageCode == (byte)HidState.Firing)
                    {
                        if (!_firingWarmup.IsRunning)
                        {
                            _firingWarmup.Restart();
                        }

                        return _firingWarmup.Elapsed.TotalSeconds >= ThirdpersonWarmupSeconds;
                    }

                    _firingWarmup.Stop();
                }

                return false;
            }
        }

        private void Start()
        {
            SpawnObject();
        }

        private void Update()
        {
            if (_thirdpersonMode)
            {
                MicroHIDHandler.SyncEnergy.TryGetValue(_serial, out float energy);
                MicroHIDViewmodel.LerpGauge(_gauge, energy, 1f);
            }

            if (Firing)
            {
                _currentScale = Mathf.Clamp01(_currentScale + _scalingSpeed * Time.deltaTime);

                Ray ray = new Ray(transform.position, _forwardTransform.forward);
                float distance = Physics.Raycast(ray, out RaycastHit hit, _laserMaxDistance, _laserMask)
                    ? hit.distance
                    : _laserMaxDistance;

                _lastFiringScale = _laserScaleOverDistance.Evaluate(distance * _currentScale);
                _targetTransform.localScale = new Vector3(_currentScale, _lastFiringScale, _currentScale);
                _targetLight.intensity = _currentScale;
                _targetLight.enabled = true;

                if (!_thirdpersonMode)
                {
                    ExplosionCameraShake.singleton.Shake(FiringShakeAmount);
                }
            }
            else
            {
                _laserTriggered = false;
                _currentScale = Mathf.Clamp01(_currentScale - _scalingSpeed * Time.deltaTime);

                _targetTransform.localScale = new Vector3(_currentScale, _currentScale * _lastFiringScale, _currentScale);
                _targetLight.intensity = _currentScale;
                _targetLight.enabled = _currentScale > 0f;
            }
        }

        public void SpawnObject()
        {
            _thirdpersonMode = _hidViewmodel == null;

            if (!_thirdpersonMode)
            {
                return;
            }

            _serial = TryGetComponent(out IdleThirdpersonItem idleItem)
                ? idleItem.Identifier.SerialNumber
                : (ushort)0;
        }

        public void TriggerLaser()
        {
            _laserTriggered = true;
        }
    }
}
