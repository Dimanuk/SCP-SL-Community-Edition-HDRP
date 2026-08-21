using MapGeneration;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Overcons
{
    public class TeslaOverconRenderer : PooledOverconRenderer
    {
        private static readonly Vector3 Offset = new Vector3(0f, 2.2f, 0f);

        internal override void SpawnOvercons(Scp079Camera newCamera)
        {
            ReturnAll();

            foreach (TeslaGate teslaGate in TeslaGateController.Singleton.TeslaGates)
            {
                Vector3 position = teslaGate.transform.position;

                if (RoomIdUtils.IsTheSameRoom(newCamera.Position, position))
                {
                    TeslaOvercon overcon = GetFromPool<TeslaOvercon>();
                    overcon.transform.position = position + Offset;
                    overcon.Rescale(newCamera);
                }
            }
        }
    }
}