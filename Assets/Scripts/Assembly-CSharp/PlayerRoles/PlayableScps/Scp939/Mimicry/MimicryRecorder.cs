namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{
    public class MimicryRecorder : global::PlayerRoles.PlayableScps.Subroutines.ScpStandardSubroutine<global::PlayerRoles.PlayableScps.Scp939.Scp939Role>
    {
        public readonly struct MimicryRecording
        {
            public readonly global::Footprinting.Footprint Owner;

            public readonly global::VoiceChat.Networking.PlaybackBuffer Buffer;

            public MimicryRecording(ReferenceHub owner, global::VoiceChat.Networking.PlaybackBuffer buffer)
            {
                Owner = new global::Footprinting.Footprint(owner);
                Buffer = buffer;
                Buffer.Reorganize();
            }
        }

        private readonly global::System.Collections.Generic.Dictionary<global::PlayerRoles.HumanRole, global::VoiceChat.Networking.PlaybackBuffer> _received = new global::System.Collections.Generic.Dictionary<global::PlayerRoles.HumanRole, global::VoiceChat.Networking.PlaybackBuffer>();

        private readonly global::System.Collections.Generic.HashSet<ReferenceHub> _serverSentVoices = new global::System.Collections.Generic.HashSet<ReferenceHub>();

        private readonly global::System.Collections.Generic.HashSet<ReferenceHub> _serverSentConfirmations = new global::System.Collections.Generic.HashSet<ReferenceHub>();

        private bool _wasLocal;

        private bool _syncMute;

        private ReferenceHub _syncPlayer;

        [global::UnityEngine.SerializeField]
        private int _maxDurationSamples;

        [global::UnityEngine.SerializeField]
        private int _minDurationSamples;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.GameObject _confirmationBox;

        public readonly global::System.Collections.Generic.List<global::PlayerRoles.PlayableScps.Scp939.Mimicry.MimicryRecorder.MimicryRecording> SavedVoices = new global::System.Collections.Generic.List<global::PlayerRoles.PlayableScps.Scp939.Mimicry.MimicryRecorder.MimicryRecording>();

        [field: global::UnityEngine.SerializeField]
        public int MaxRecordings { get; private set; }

        [field: global::UnityEngine.SerializeField]
        public global::PlayerRoles.PlayableScps.Scp939.Mimicry.MimicryPreviewPlayback PreviewPlayback { get; private set; }

        [field: global::UnityEngine.SerializeField]
        public global::PlayerRoles.PlayableScps.Scp939.Mimicry.MimicryTransmitter Transmitter { get; private set; }

        public bool SavedVoicesModified { get; internal set; }

        private void OnRoleChanged(ReferenceHub ply, global::PlayerRoles.PlayerRoleBase prevRole, global::PlayerRoles.PlayerRoleBase newRole)
        {
            if (prevRole is global::PlayerRoles.HumanRole role)
            {
                UnregisterRole(role);
            }
            if (newRole is global::PlayerRoles.HumanRole role2)
            {
                RegisterRole(role2);
            }
        }

        private void ServerRemoveClient(ReferenceHub ply)
        {
            _syncMute = true;
            _syncPlayer = ply;
            ServerSendRpc(toAll: false);
            RemoveRecordingsOfPlayer(ply);
        }

        private void RemoveRecordingsOfPlayer(ReferenceHub ply)
        {
            _serverSentVoices.Remove(ply);
            for (int i = 0; i < SavedVoices.Count; i++)
            {
                if (!(SavedVoices[i].Owner.Hub != ply))
                {
                    SavedVoices.RemoveAt(i--);
                    SavedVoicesModified = true;
                }
            }
        }

        private void OnAnyPlayerKilled(ReferenceHub ply, global::PlayerStatsSystem.DamageHandlerBase dh)
        {
            if (dh is global::PlayerRoles.PlayableScps.Scp939.Scp939DamageHandler scp939DamageHandler && !(scp939DamageHandler.Attacker.Hub != base.Owner) && !IsMuted(global::VoiceChat.VoiceChatMutes.GetFlags(ply)) && IsPrivacyAccepted(ply))
            {
                _syncPlayer = ply;
                _syncMute = false;
                if (base.Owner.isLocalPlayer)
                {
                    SaveRecording(ply);
                }
                else
                {
                    ServerSendRpc(toAll: false);
                }
                _serverSentVoices.Add(ply);
                _serverSentConfirmations.Remove(ply);
            }
        }

        private void OnPlayerMuteChanges(ReferenceHub ply, global::VoiceChat.VcMuteFlags flags)
        {
            if (IsMuted(flags) && _serverSentVoices.Remove(ply))
            {
                ServerRemoveClient(ply);
            }
        }

        private void OnPlayerPrivacyChanges(ReferenceHub ply)
        {
            if (!IsPrivacyAccepted(ply) && _serverSentVoices.Remove(ply))
            {
                ServerRemoveClient(ply);
            }
        }

        private bool IsMuted(global::VoiceChat.VcMuteFlags flags)
        {
            flags &= global::VoiceChat.VcMuteFlags.LocalRegular | global::VoiceChat.VcMuteFlags.GlobalRegular;
            return flags != global::VoiceChat.VcMuteFlags.None;
        }

        private bool IsPrivacyAccepted(ReferenceHub hub)
        {
            return global::VoiceChat.VoiceChatPrivacySettings.CheckUserFlags(hub, global::VoiceChat.VcPrivacyFlags.SettingsSelected | global::VoiceChat.VcPrivacyFlags.AllowMicCapture | global::VoiceChat.VcPrivacyFlags.AllowRecording);
        }

        private void UnregisterRole(global::PlayerRoles.HumanRole role)
        {
            if (_received.TryGetValue(role, out var value))
            {
                role.VoiceModule.OnSamplesReceived -= value.Write;
                _received.Remove(role);
            }
        }

        private void RegisterRole(global::PlayerRoles.HumanRole role)
        {
            global::VoiceChat.Networking.PlaybackBuffer playbackBuffer = new global::VoiceChat.Networking.PlaybackBuffer(_maxDurationSamples, endlessTapeMode: true);
            _received[role] = playbackBuffer;
            role.VoiceModule.OnSamplesReceived += playbackBuffer.Write;
        }

        private void SaveRecording(ReferenceHub ply)
        {
            if (ply.roleManager.CurrentRole is global::PlayerRoles.HumanRole key && _received.TryGetValue(key, out var value) && value.Length >= _minDurationSamples)
            {
                SavedVoices.Add(new global::PlayerRoles.PlayableScps.Scp939.Mimicry.MimicryRecorder.MimicryRecording(ply, value));
                SavedVoicesModified = true;
                ClientSendCmd();
                if (SavedVoices.Count > MaxRecordings)
                {
                    SavedVoices.RemoveAt(0);
                }
            }
        }

        public override void ServerWriteRpc(global::Mirror.NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            global::Mirror.NetworkWriterExtensions.WriteBool(writer, _syncMute);
            global::Utils.Networking.ReferenceHubReaderWriter.WriteReferenceHub(writer, _syncPlayer);
        }

        public override void ClientProcessRpc(global::Mirror.NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            _syncMute = global::Mirror.NetworkReaderExtensions.ReadBool(reader);
            _syncPlayer = global::Utils.Networking.ReferenceHubReaderWriter.ReadReferenceHub(reader);
            if (_syncPlayer == null)
            {
                return;
            }
            if (_syncPlayer.isLocalPlayer)
            {
                if (!global::PlayerRoles.PlayableScps.Scp939.Mimicry.MimicryConfirmationBox.Remember)
                {
                    global::UnityEngine.Object.Instantiate(_confirmationBox);
                }
            }
            else if (_syncMute)
            {
                RemoveRecordingsOfPlayer(_syncPlayer);
            }
            else
            {
                SaveRecording(_syncPlayer);
            }
        }

        public override void ClientWriteCmd(global::Mirror.NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            global::Utils.Networking.ReferenceHubReaderWriter.WriteReferenceHub(writer, _syncPlayer);
        }

        public override void ServerProcessCmd(global::Mirror.NetworkReader reader)
        {
            base.ServerProcessCmd(reader);
            ReferenceHub rh = global::Utils.Networking.ReferenceHubReaderWriter.ReadReferenceHub(reader);
            if (_serverSentVoices.Contains(rh) && _serverSentConfirmations.Add(rh))
            {
                ServerSendRpc((ReferenceHub x) => x == rh);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            global::PlayerStatsSystem.PlayerStats.OnAnyPlayerDied += OnAnyPlayerKilled;
            global::VoiceChat.VoiceChatMutes.OnFlagsSet += OnPlayerMuteChanges;
            global::VoiceChat.VoiceChatPrivacySettings.OnUserFlagsChanged += OnPlayerPrivacyChanges;
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            if (!base.Owner.isLocalPlayer)
            {
                return;
            }
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.roleManager.CurrentRole is global::PlayerRoles.HumanRole role)
                {
                    RegisterRole(role);
                }
            }
            _wasLocal = true;
            global::PlayerRoles.PlayerRoleManager.OnRoleChanged += OnRoleChanged;
            ReferenceHub.OnPlayerRemoved = (global::System.Action<ReferenceHub>)global::System.Delegate.Combine(ReferenceHub.OnPlayerRemoved, new global::System.Action<ReferenceHub>(RemoveRecordingsOfPlayer));
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _serverSentVoices.Clear();
            _serverSentConfirmations.Clear();
            if (_wasLocal)
            {
                _wasLocal = false;
                SavedVoices.Clear();
                _received.Clear();
                PreviewPlayback.StopPreview();
                global::PlayerRoles.PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
                ReferenceHub.OnPlayerRemoved = (global::System.Action<ReferenceHub>)global::System.Delegate.Remove(ReferenceHub.OnPlayerRemoved, new global::System.Action<ReferenceHub>(RemoveRecordingsOfPlayer));
            }
        }
    }
}
