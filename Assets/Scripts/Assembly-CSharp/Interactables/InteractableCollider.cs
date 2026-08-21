using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
    public class InteractableCollider : MonoBehaviour
    {
        [Header("Linked Object")]
        public MonoBehaviour Target;

        [Header("Settings")]
        public byte ColliderId;

        public Vector3 VerificationOffset;

        public static readonly Dictionary<IInteractable, Dictionary<byte, InteractableCollider>> AllInstances =
            new();

        protected virtual void Awake()
        {
            if (Target is IInteractable key)
            {
                if (!AllInstances.ContainsKey(key))
                {
                    AllInstances[key] = new Dictionary<byte, InteractableCollider>();
                }

                AllInstances[key][ColliderId] = this;
            }
            else
            {
                Debug.LogError($"Fatal error: '{Target?.name ?? "NULL"}' is not IInteractable.");
            }
        }

        public static bool TryGetCollider(IInteractable target, byte colliderId, out InteractableCollider res)
        {
            if (AllInstances.TryGetValue(target, out var collidersDict) && collidersDict.TryGetValue(colliderId, out res))
            {
                res = collidersDict[colliderId];
                return true;
            }

            res = null;
            return false;
        }
    }
}