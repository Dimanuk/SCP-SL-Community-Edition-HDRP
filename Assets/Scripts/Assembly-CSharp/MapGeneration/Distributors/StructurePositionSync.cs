using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

namespace MapGeneration.Distributors
{
    public class StructurePositionSync : NetworkBehaviour
    {
        private const float ConversionRate = 5.625f;

        [SyncVar]
        private sbyte _rotationY;

        [SyncVar]
        private Vector3 _position;

        private void Start()
        {
            if (!NetworkServer.active)
                return;

            _position = base.transform.position;
            _rotationY = (sbyte)Mathf.RoundToInt(base.transform.rotation.eulerAngles.y / ConversionRate);
            base.enabled = false;
        }

        private void Update()
        {
            if (_position != Vector3.zero)
            {
                base.transform.position = _position;
                base.transform.rotation = Quaternion.Euler(Vector3.up * _rotationY * 5.625f);
                base.enabled = false;
            }
        }
    }
}
