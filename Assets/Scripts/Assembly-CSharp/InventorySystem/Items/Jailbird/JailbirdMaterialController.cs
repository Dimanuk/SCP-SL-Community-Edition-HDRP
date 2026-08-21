using System.Collections.Generic;

using UnityEngine;

namespace InventorySystem.Items.Jailbird
{
	public class JailbirdMaterialController : MonoBehaviour
	{
        private static readonly global::System.Collections.Generic.HashSet<ushort> AlmostDepletedJailbirds = new global::System.Collections.Generic.HashSet<ushort>();

        private ushort _serial;

		[SerializeField]
		private Material _almostDepletedMat;

		[SerializeField]
		private Material _normalMat;

		[SerializeField]
		private Renderer _emissionRend;

        private void Update()
        {
            bool flag = AlmostDepletedJailbirds.Contains(_serial);
            _emissionRend.sharedMaterial = (flag ? _almostDepletedMat : _normalMat);
        }

        public void SetSerial(ushort serial)
		{
			_serial = serial;
		}

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::InventorySystem.Items.Jailbird.JailbirdItem.OnRpcReceived += delegate (ushort serial, global::InventorySystem.Items.Jailbird.JailbirdMessageType rpc)
            {
                if (rpc == global::InventorySystem.Items.Jailbird.JailbirdMessageType.AlmostDepleted)
                {
                    AlmostDepletedJailbirds.Add(serial);
                }
            };
            CustomNetworkManager.OnClientReady += AlmostDepletedJailbirds.Clear;
        }
    }
}
