namespace InventorySystem.Items.Usables.Scp244
{
    public static class Scp244Utils
    {
        public static bool CheckVisibility(global::UnityEngine.Vector3 observer, global::UnityEngine.Vector3 target)
        {
            if ((observer - target).sqrMagnitude <= 73.96f)
            {
                return true;
            }
            foreach (global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup instance in global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup.Instances)
            {
                if (instance.IntersectRay(observer, target))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IntersectRay(this global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup scp244, global::UnityEngine.Vector3 observer, global::UnityEngine.Vector3 target)
        {
            if (scp244.State == global::InventorySystem.Items.Usables.Scp244.Scp244State.Idle || scp244.CurrentSizePercent < 0.55f)
            {
                return false;
            }
            global::UnityEngine.Vector3 vector = target - observer;
            float magnitude = vector.magnitude;
            global::UnityEngine.Vector3 vector2 = vector / magnitude;
            global::UnityEngine.Ray ray = new global::UnityEngine.Ray(observer, vector2);
            if (!scp244.CurrentBounds.IntersectRay(ray, out var distance) || distance > magnitude)
            {
                return false;
            }
            global::UnityEngine.Vector3 position = scp244.transform.position;
            float num = scp244.CurrentDiameter / 2f;
            float value = global::UnityEngine.Vector3.Dot(position - observer, vector2);
            global::UnityEngine.Vector3 vector3 = observer + vector2 * global::UnityEngine.Mathf.Clamp(value, 0f, magnitude);
            return (position - vector3).sqrMagnitude < num * num;
        }
    }
}
