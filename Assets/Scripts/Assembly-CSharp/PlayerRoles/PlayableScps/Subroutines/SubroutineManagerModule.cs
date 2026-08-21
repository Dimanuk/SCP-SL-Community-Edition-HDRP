using Mirror;
using PlayerRoles.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Subroutines
{
    public class SubroutineManagerModule : global::UnityEngine.MonoBehaviour
    {
        public global::PlayerRoles.PlayableScps.Subroutines.SubroutineBase[] AllSubroutines;

        private void OnValidate()
        {
            AllSubroutines = GetComponentsInChildren<global::PlayerRoles.PlayableScps.Subroutines.SubroutineBase>();
        }

        public bool TryGetSubroutine<T>(out T subroutine) where T : global::PlayerRoles.PlayableScps.Subroutines.SubroutineBase
        {
            for (int i = 0; i < AllSubroutines.Length; i++)
            {
                if (AllSubroutines[i] is T val)
                {
                    subroutine = val;
                    return true;
                }
            }
            subroutine = null;
            return false;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += delegate
            {
                NetworkServer.ReplaceHandler<SubroutineMessage>(delegate (NetworkConnectionToClient conn, SubroutineMessage msg)
                {
                    msg.ServerApplyTrigger(conn);
                });

                NetworkClient.ReplaceHandler<SubroutineMessage>(delegate (SubroutineMessage msg)
                {
                    msg.ClientApplyConfirmation();
                });
            };
        }
    }
}
