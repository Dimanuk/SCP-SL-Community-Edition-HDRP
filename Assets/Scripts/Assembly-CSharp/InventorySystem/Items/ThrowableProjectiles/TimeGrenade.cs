
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.ThrowableProjectiles
{
	public abstract class TimeGrenade : ThrownProjectile
	{
		[SerializeField]
		private float _fuseTime;

		private bool _alreadyDetonated;

		public float TargetTime { get; protected set; }

		[ClientRpc]
		private void RpcSetTime(float time)
		{
            TargetTime = global::UnityEngine.Time.timeSinceLevelLoad + time;
        }

		protected abstract void ServerFuseEnd();

        public override void ServerActivate()
        {
            RpcSetTime(_fuseTime);
        }

        protected override void Update()
        {
            base.Update();
            if (global::Mirror.NetworkServer.active && !_alreadyDetonated && TargetTime != 0f && !(global::UnityEngine.Time.timeSinceLevelLoad < TargetTime))
            {
                ServerFuseEnd();
                _alreadyDetonated = true;
            }
        }
	}
}
