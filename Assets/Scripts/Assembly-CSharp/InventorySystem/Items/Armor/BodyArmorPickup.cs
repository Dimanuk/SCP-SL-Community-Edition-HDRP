namespace InventorySystem.Items.Armor
{
    public class BodyArmorPickup : global::InventorySystem.Items.Pickups.ItemPickupBase
    {
        private static readonly global::UnityEngine.RigidbodyConstraints StartConstraints = (global::UnityEngine.RigidbodyConstraints)80;

        private static readonly global::UnityEngine.Quaternion StartRotation = global::UnityEngine.Quaternion.Euler(0f, 0f, -90f);

        private readonly global::System.Collections.Generic.HashSet<ushort> _alreadyMovedPickups = new global::System.Collections.Generic.HashSet<ushort>();

        private const float ReleaseVelocity = 0.1f;

        private const float ReleaseDelay = 0.15f;

        private const float DotProductThreshold = -0.8f;

        private const float HeightOffset = 0.16f;

        private const float WeightLimit = 2.1f;

        private const int PickupLayer = 9;

        private float _remainingReleaseTime;

        private bool _released;

        private bool IsAffected
        {
            get
            {
                if (!_released && global::Mirror.NetworkServer.active)
                {
                    return PreviousOwner.IsSet;
                }
                return false;
            }
        }

        protected override void Start()
        {
            base.Start();
            if (IsAffected && !(PreviousOwner.Hub == null))
            {
                _remainingReleaseTime = 0.15f;
                Rb.rotation = PreviousOwner.Hub.transform.rotation * StartRotation;
                Rb.constraints = StartConstraints;
            }
        }

        private void OnTriggerStay(global::UnityEngine.Collider other)
        {
            if (other.gameObject.layer == 9 && !(global::UnityEngine.Vector3.Dot(global::UnityEngine.Vector3.up, base.transform.right) > -0.8f) && other.transform.root.TryGetComponent<global::InventorySystem.Items.Pickups.ItemPickupBase>(out var component) && !(component.Info.Weight > 2.1f) && _alreadyMovedPickups.Add(component.Info.Serial) && global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(component.Info.ItemId, out var value) && value.Category != ItemCategory.Armor)
            {
                float num = base.transform.position.y - component.transform.position.y;
                component.transform.position += global::UnityEngine.Vector3.up * (num * 2f + 0.16f);
                component.RefreshPositionAndRotation();
            }
        }

        private void Update()
        {
            if (IsAffected && !(global::UnityEngine.Mathf.Abs(Rb.linearVelocity.y) > 0.1f))
            {
                _remainingReleaseTime -= global::UnityEngine.Time.deltaTime;
                if (_remainingReleaseTime <= 0f)
                {
                    _released = true;
                    Rb.constraints = global::UnityEngine.RigidbodyConstraints.None;
                }
            }
        }
    }
}
