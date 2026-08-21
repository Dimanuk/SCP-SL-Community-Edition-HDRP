using System;
using System.Runtime.CompilerServices;

using Mirror;
using PlayerRoles.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Subroutines
{
	public abstract class SubroutineBase : MonoBehaviour
	{
        private byte _syncIndex;

        public global::PlayerRoles.PlayerRoleBase Role { get; private set; }

        public byte SyncIndex
        {
            get
            {
                if (_syncIndex != 0)
                {
                    return _syncIndex;
                }
                SubroutineBase[] allSubroutines = ((Role as global::PlayerRoles.PlayableScps.Subroutines.ISubroutinedScpRole) ?? throw new global::System.InvalidOperationException("Could not generate a SyncIndex of '" + base.name + "' subroutine. The role does not derive from ISubroutinedScpRole!")).SubroutineModule.AllSubroutines;
                for (int i = 0; i < allSubroutines.Length; i++)
                {
                    if (!(allSubroutines[i] != this))
                    {
                        _syncIndex = (byte)(i + 1);
                        return _syncIndex;
                    }
                }
                throw new global::System.InvalidOperationException("Could not generate a SyncIndex of '" + base.name + "' subroutine. It's not on the list of registered subroutines!");
            }
        }

        protected virtual void Awake()
        {
            Role = GetComponentInParent<global::PlayerRoles.PlayerRoleBase>();
        }

        protected virtual void OnValidate()
        {
            global::PlayerRoles.PlayableScps.Subroutines.SubroutineManagerModule componentInParent = GetComponentInParent<global::PlayerRoles.PlayableScps.Subroutines.SubroutineManagerModule>();
            if (!(componentInParent == null))
            {
                componentInParent.AllSubroutines = componentInParent.GetComponentsInChildren<SubroutineBase>();
            }
        }

        protected void ClientSendCmd()
        {
            if (!Role.Pooled)
            {
                if (!Role.IsLocalPlayer)
                {
                    throw new global::System.InvalidOperationException("ClientSendCmd can only be called on local player!");
                }
                global::Mirror.NetworkClient.Send(new SubroutineMessage(this, isConfirmation: false));
            }
        }

        protected void ServerSendRpc(bool toAll)
        {
            if (global::Mirror.NetworkServer.active && !Role.Pooled)
            {
                ReferenceHub hub;
                if (toAll)
                {
                    global::Mirror.NetworkServer.SendToReady(new SubroutineMessage(this, isConfirmation: true));
                }
                else if (Role.TryGetOwner(out hub))
                {
                    ServerSendRpc(hub);
                }
            }
        }

        protected void ServerSendRpc(ReferenceHub target)
        {
            if (global::Mirror.NetworkServer.active && !Role.Pooled)
            {
                target.connectionToClient.Send(new SubroutineMessage(this, isConfirmation: true));
            }
        }

        protected void ServerSendRpc(global::System.Func<ReferenceHub, bool> condition)
        {
            if (global::Mirror.NetworkServer.active && !Role.Pooled)
            {
                global::Utils.Networking.NetworkUtils.SendToHubsConditionally(new SubroutineMessage(this, isConfirmation: true), condition);
            }
        }

        public virtual void ClientWriteCmd(global::Mirror.NetworkWriter writer)
        {
        }

        public virtual void ServerProcessCmd(global::Mirror.NetworkReader reader)
        {
        }

        public virtual void ServerWriteRpc(global::Mirror.NetworkWriter writer)
        {
        }

        public virtual void ClientProcessRpc(global::Mirror.NetworkReader reader)
        {
        }
    }
}
