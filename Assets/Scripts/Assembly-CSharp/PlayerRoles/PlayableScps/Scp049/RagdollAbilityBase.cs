using System;
using System.Runtime.CompilerServices;

using CursorManagement;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Ragdolls;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp049
{
	public abstract class RagdollAbilityBase<T> : ScpKeySubroutine<T>, ICursorOverride where T : FpcStandardScp
	{
        private readonly global::PlayerRoles.PlayableScps.Subroutines.AbilityCooldown _process = new global::PlayerRoles.PlayableScps.Subroutines.AbilityCooldown();

        private Transform _ragdollTransform;

		private DynamicRagdoll _syncRagdoll;

		private byte _errorCode;

		private double _completionTime;

        public virtual global::CursorManagement.CursorOverrideMode CursorOverride => global::CursorManagement.CursorOverrideMode.NoOverride;

        public virtual bool LockMovement
        {
            get
            {
                if (IsInProgress)
                {
                    return base.Owner.isLocalPlayer;
                }
                return false;
            }
        }

        protected override ActionName TargetKey => ActionName.Interact;

        public bool IsInProgress
        {
            get
            {
                return _completionTime != 0.0;
            }
            private set
            {
                _completionTime = (value ? (global::Mirror.NetworkTime.time + (double)Duration) : 0.0);
                ServerSendRpc(toAll: true);
            }
        }

        public float ProgressStatus => _process.Readiness;

        protected abstract float RangeSqr { get; }

		protected abstract float Duration { get; }

        protected BasicRagdoll CurRagdoll { get; private set; }

        public event global::System.Action<byte> OnErrorReceived;

        public override void ClientWriteCmd(global::Mirror.NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            global::Mirror.NetworkWriterExtensions.WriteNetworkBehaviour(writer, _syncRagdoll);
        }

        public override void ServerProcessCmd(global::Mirror.NetworkReader reader)
        {
            base.ServerProcessCmd(reader);
            global::UnityEngine.Vector3 position = base.ScpRole.FpcModule.Position;
            _syncRagdoll = global::Mirror.NetworkReaderExtensions.ReadNetworkBehaviour<global::PlayerRoles.Ragdolls.DynamicRagdoll>(reader);
            if (_syncRagdoll == null)
            {
                if (IsInProgress)
                {
                    _errorCode = ServerValidateCancel();
                    if (_errorCode != 0)
                    {
                        ServerSendRpc(toAll: true);
                    }
                    else
                    {
                        IsInProgress = false;
                    }
                }
            }
            else
            {
                if (IsInProgress || !IsCorpseNearby(position, _syncRagdoll, out var ragdollPosition))
                {
                    return;
                }
                global::UnityEngine.Transform ragdollTransform = _ragdollTransform;
                BasicRagdoll curRagdoll = CurRagdoll;
                _ragdollTransform = ragdollPosition;
                CurRagdoll = _syncRagdoll;
                _errorCode = ServerValidateBegin(_syncRagdoll);
                bool flag = _errorCode != 0;
                if (flag || !ServerValidateAny())
                {
                    _ragdollTransform = ragdollTransform;
                    CurRagdoll = curRagdoll;
                    if (flag)
                    {
                        ServerSendRpc(toAll: true);
                    }
                }
                else
                {
                    IsInProgress = true;
                }
            }
        }

        public override void ServerWriteRpc(global::Mirror.NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            global::Mirror.NetworkWriterExtensions.WriteDouble(writer, _completionTime);
            writer.WriteByte(_errorCode);
            _errorCode = 0;
        }

        public override void ClientProcessRpc(global::Mirror.NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            bool flag = IsInProgress && !global::Mirror.NetworkServer.active;
            _completionTime = global::Mirror.NetworkReaderExtensions.ReadDouble(reader);
            byte b = reader.ReadByte();
            if (base.Owner.isLocalPlayer && b != 0)
            {
                this.OnErrorReceived?.Invoke(b);
                ClientProcessErrorCode(b);
            }
            if (!IsInProgress)
            {
                _process.Clear();
            }
            else if (!flag)
            {
                _process.Trigger((float)(_completionTime - global::Mirror.NetworkTime.time));
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            if (base.Owner.isLocalPlayer)
            {
                global::CursorManagement.CursorManager.Register(this);
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();
            global::CursorManagement.CursorManager.Unregister(this);
            _completionTime = 0.0;
            _process.Clear();
        }


        protected abstract void ServerComplete();

		protected abstract byte ServerValidateBegin(BasicRagdoll ragdoll);

		protected virtual byte ServerValidateCancel()
		{
			return (byte)0;
		}

        protected virtual bool ServerValidateAny()
        {
            return IsCloseEnough(base.ScpRole.FpcModule.Position, _ragdollTransform.position);
        }

        protected virtual void ClientProcessErrorCode(byte code)
		{
			
		}

        protected override void Update()
        {
            base.Update();
            if (global::Mirror.NetworkServer.active && IsInProgress)
            {
                if (!ServerValidateAny())
                {
                    IsInProgress = false;
                }
                else if (!(global::Mirror.NetworkTime.time < _completionTime))
                {
                    ServerComplete();
                    IsInProgress = false;
                }
            }
        }

        protected void ClientTryStart()
        {
            if (CanFindCorpse(base.Owner.PlayerCameraReference, out var ragdoll) && IsCorpseNearby(base.ScpRole.FpcModule.Position, ragdoll, out var _))
            {
                _syncRagdoll = ragdoll;
                ClientSendCmd();
            }
        }

        protected void ClientTryCancel()
        {
            _syncRagdoll = null;
            ClientSendCmd();
        }

        private bool IsCorpseNearby(global::UnityEngine.Vector3 position, global::PlayerRoles.Ragdolls.DynamicRagdoll ragdoll, out global::UnityEngine.Transform ragdollPosition)
        {
            global::UnityEngine.Rigidbody[] linkedRigidbodies = ragdoll.LinkedRigidbodies;
            foreach (global::UnityEngine.Rigidbody rigidbody in linkedRigidbodies)
            {
                if (IsCloseEnough(position, rigidbody.position))
                {
                    ragdollPosition = rigidbody.transform;
                    return true;
                }
            }
            ragdollPosition = ragdoll.transform;
            return false;
        }

        private bool CanFindCorpse(global::UnityEngine.Transform tr, out global::PlayerRoles.Ragdolls.DynamicRagdoll ragdoll)
        {
            ragdoll = null;
            if (!global::UnityEngine.Physics.Raycast(tr.position, tr.forward, out var hitInfo, RangeSqr))
            {
                return false;
            }
            return ragdoll = hitInfo.transform.GetComponentInParent<global::PlayerRoles.Ragdolls.DynamicRagdoll>();
        }

        private bool IsCloseEnough(global::UnityEngine.Vector3 position, global::UnityEngine.Vector3 ragdollPosition)
        {
            return (ragdollPosition - position).sqrMagnitude < RangeSqr;
        }
	}
}
