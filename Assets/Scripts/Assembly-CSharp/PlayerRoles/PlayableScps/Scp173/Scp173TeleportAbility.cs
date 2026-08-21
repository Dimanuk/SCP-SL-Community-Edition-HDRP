using System;
using System.Collections.Generic;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp173;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using RelativePositioning;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Utils.Networking;

namespace PlayerRoles.PlayableScps.Scp173
{
    public class Scp173TeleportAbility : ScpKeySubroutine<Scp173Role>
    {
        [Flags]
        private enum CmdTeleportData : byte
        {
            None = 0,
            Aiming = 1,
            WantsToTeleport = 2
        }

        private const float BlinkDistance = 8f;
        private const float BreakneckDistanceMultiplier = 1.8f;
        private const float KillRadiusSqr = 1.66f;
        private const float KillHeight = 2.2f;
        private const float KillBacktracking = 0.4f;
        private const float ClientDistanceAddition = 0.1f;
        private const int GlassLayerMask = 16384; 
        private const float GlassDestroyRadius = 0.8f;

        private static readonly Collider[] DetectedColliders = new Collider[8];

        private Scp173MovementModule _fpcModule;
        private Scp173ObserversTracker _observersTracker;
        private Scp173BreakneckSpeedsAbility _breakneckSpeedsAbility;
        private Scp173BlinkTimer _blinkTimer;
        private Scp173AudioPlayer _audioSubroutine;

        private bool _isAiming;
        private float _targetDis;
        private Vector3 _tpPosition;
        private float _lastBlink;
        private CmdTeleportData _cmdData;
        private bool _subroutinesInitialized; 

        [SerializeField]
        private Scp173TeleportIndicator _tpIndicator;

        [SerializeField]
        private AnimationCurve _blinkIntensity;

        [SerializeField]
        private PostProcessVolume _blinkEffect;

        private float EffectiveBlinkDistance => BlinkDistance * ((_breakneckSpeedsAbility != null && _breakneckSpeedsAbility.IsActive) ? BreakneckDistanceMultiplier : 1f);

        protected override ActionName TargetKey => ActionName.Zoom;

        public ReferenceHub BestTarget
        {
            get
            {
                ReferenceHub result = null;
                float bestSqrDist = float.MaxValue;

                foreach (ReferenceHub hub in ReferenceHub.AllHubs)
                {
                    if (!(hub.roleManager.CurrentRole is HumanRole humanRole))
                        continue;

                    Vector3 position = humanRole.FpcModule.Position;
                    Vector3 tpPos = _tpPosition;

                    if ((position - tpPos).MagnitudeOnlyY() < KillHeight)
                    {
                        position.y = 0f;
                        tpPos.y = 0f;
                    }

                    float sqrDist = (position - tpPos).sqrMagnitude;
                    if (sqrDist > bestSqrDist)
                        continue;

                    result = hub;
                    bestSqrDist = sqrDist;
                }

                return bestSqrDist <= KillRadiusSqr ? result : null;
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }
        private void EnsureSubroutinesInitialized()
        {
            if (_subroutinesInitialized)
                return;

            if (ScpRole == null)
                return;

            _fpcModule = ScpRole.FpcModule as Scp173MovementModule;

            SubroutineManagerModule subroutineModule = ScpRole.SubroutineModule;
            if (subroutineModule == null)
                return;

            subroutineModule.TryGetSubroutine(out _observersTracker);
            subroutineModule.TryGetSubroutine(out _breakneckSpeedsAbility);
            subroutineModule.TryGetSubroutine(out _audioSubroutine);
            subroutineModule.TryGetSubroutine(out _blinkTimer);

            _subroutinesInitialized = true;
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            EnsureSubroutinesInitialized();
        }

        protected override void Update()
        {
            if (!_subroutinesInitialized)
            {
                EnsureSubroutinesInitialized();
                if (!_subroutinesInitialized)
                    return;
            }

            base.Update();

            float timeSinceBlink = Time.timeSinceLevelLoad - _lastBlink;
            if (_blinkEffect != null && _blinkIntensity != null)
                _blinkEffect.weight = _blinkIntensity.Evaluate(timeSinceBlink);

            if (!Owner.isLocalPlayer && !SpectatorNetworking.IsLocallySpectated(Owner))
            {
                if (_isAiming)
                {
                    _isAiming = false;
                    _tpIndicator.UpdateVisibility(false);
                }
                return;
            }

            bool wantsAim;
            if (Owner.isLocalPlayer)
                wantsAim = IsKeyHeld && !Cursor.visible;
            else
                wantsAim = HasDataFlag(CmdTeleportData.Aiming);

            if (_isAiming)
            {
                UpdateAiming(!wantsAim);
            }
            else if (wantsAim)
            {
                _isAiming = true;
            }
        }

        private void UpdateAiming(bool wantsToTeleport)
        {
            if (_fpcModule == null)
                return;

            bool canTeleport = _fpcModule.TryGetTeleportPos(EffectiveBlinkDistance, out _tpPosition, out _targetDis);

            if (!wantsToTeleport)
            {
                _tpIndicator.UpdateVisibility(canTeleport && _blinkTimer != null && _blinkTimer.AbilityReady);
                _tpIndicator.transform.position = _tpPosition;

                if (!HasDataFlag(CmdTeleportData.Aiming))
                {
                    _cmdData = CmdTeleportData.Aiming;
                    ClientSendCmd();
                }
            }
            else
            {
                if (Owner.isLocalPlayer)
                {
                    _cmdData = canTeleport ? CmdTeleportData.WantsToTeleport : CmdTeleportData.Aiming;
                    ClientSendCmd();
                }

                _isAiming = false;
                _tpIndicator.UpdateVisibility(false);
            }
        }

        private bool TryBlink(float maxDis)
        {
            if (_fpcModule == null || _blinkTimer == null)
                return false;

            maxDis = Mathf.Clamp(maxDis, 0f, EffectiveBlinkDistance);

            if (!_blinkTimer.AbilityReady)
                return false;

            if (!_fpcModule.TryGetTeleportPos(maxDis, out _tpPosition, out _))
                return false;

            float halfHeight = _fpcModule.CharController.height / 2f;
            Vector3 blinkPos = _tpPosition + Vector3.up * halfHeight;
            _blinkTimer.ServerBlink(blinkPos);
            return true;
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            writer.WriteByte((byte)_cmdData);

            if (HasDataFlag(CmdTeleportData.WantsToTeleport))
            {
                writer.WriteQuaternion(Owner.PlayerCameraReference.rotation);
                writer.WriteFloat(_targetDis + ClientDistanceAddition);
                ReferenceHubReaderWriter.WriteReferenceHub(writer, BestTarget);
            }
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            _cmdData = (CmdTeleportData)reader.ReadByte();

            if (!HasDataFlag(CmdTeleportData.WantsToTeleport))
            {
                ServerSendRpc(toAll: true);
                return;
            }

            if (_blinkTimer == null || !_blinkTimer.AbilityReady)
                return;

            if (_fpcModule == null || _observersTracker == null)
                return;

            var displayClass = new __c__DisplayClass33_0();
            displayClass.prevObservers = new HashSet<ReferenceHub>(_observersTracker.Observers);

            CmdTeleportData cmdData = _cmdData;
            _cmdData = CmdTeleportData.None;
            ServerSendRpc(toAll: true);
            _cmdData = cmdData;

            Quaternion originalRotation = Owner.PlayerCameraReference.rotation;
            Owner.PlayerCameraReference.rotation = NetworkReaderExtensions.ReadQuaternion(reader);

            float targetDistance = NetworkReaderExtensions.ReadFloat(reader);
            bool blinkSuccess = TryBlink(targetDistance);

            Owner.PlayerCameraReference.rotation = originalRotation;

            if (!blinkSuccess)
                return;

            displayClass.prevObservers.UnionWith(_observersTracker.Observers);
            ServerSendRpc(displayClass.__ServerProcessCmd_b__0);

            if (_audioSubroutine != null)
                _audioSubroutine.ServerSendSound(Scp173AudioPlayer.Scp173SoundId.Teleport);
                
            if (_breakneckSpeedsAbility != null && _breakneckSpeedsAbility.IsActive)
                return;

            int colliderCount = Physics.OverlapSphereNonAlloc(
                _fpcModule.Position,
                GlassDestroyRadius,
                DetectedColliders,
                GlassLayerMask);

            for (int i = 0; i < colliderCount; i++)
            {
                if (DetectedColliders[i].TryGetComponent(out BreakableWindow window))
                {
                    window.Damage(window.health, ScpRole.DamageHandler, Vector3.zero);
                }
            }

            ReferenceHub target = ReferenceHubReaderWriter.ReadReferenceHub(reader);
            if (target == null || !(target.roleManager.CurrentRole is HumanRole))
                return;

            IFpcRole fpcRole = target.roleManager.CurrentRole as IFpcRole;
            if (fpcRole == null)
                return;

            Bounds bounds = fpcRole.FpcModule.Tracer.GenerateBounds(KillBacktracking, ignoreTeleports: true);
            bounds.Encapsulate(new Bounds(fpcRole.FpcModule.Position, Vector3.up * KillHeight));

            if (bounds.SqrDistance(_fpcModule.Position) > KillRadiusSqr)
                return;

            if (!target.playerStats.DealDamage(ScpRole.DamageHandler))
                return;

            Hitmarker.SendHitmarker(Owner, 1f);

            if (_audioSubroutine != null)
                _audioSubroutine.ServerSendSound(Scp173AudioPlayer.Scp173SoundId.Snap);
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            writer.WriteByte((byte)_cmdData);

            if (HasDataFlag(CmdTeleportData.WantsToTeleport))
            {
                RelativePositionSerialization.WriteRelativePosition(
                    writer,
                    new RelativePosition(_fpcModule.Position));
            }
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            _cmdData = (CmdTeleportData)reader.ReadByte();

            if (HasDataFlag(CmdTeleportData.WantsToTeleport))
            {
                if (_fpcModule == null)
                    return;

                RelativePosition relPos = RelativePositionSerialization.ReadRelativePosition(reader);
                _fpcModule.Motor.ReceivedPosition = relPos;
                _fpcModule.Position = relPos.Position;

                _lastBlink = Time.timeSinceLevelLoad;
                _blinkEffect.weight = 1f;
                if (_fpcModule.CharacterModelInstance is Scp173CharacterModel model)
                    model.Frozen = false;
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _lastBlink = 0f;
            if (_blinkEffect != null)
                _blinkEffect.weight = 0f;
        }

        private bool HasDataFlag(CmdTeleportData ctd)
        {
            return (_cmdData & ctd) == ctd;
        }

        private class __c__DisplayClass33_0
        {
            public HashSet<ReferenceHub> prevObservers;

            internal bool __ServerProcessCmd_b__0(ReferenceHub x)
            {
                return prevObservers.Contains(x);
            }
        }
    }
}
