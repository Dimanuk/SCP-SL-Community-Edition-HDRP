using System;
using PlayerRoles;
using UnityEngine;

namespace InventorySystem.Items
{
    public class SharedHandsController : MonoBehaviour
    {
        [Serializable]
        private struct GlovesForRole
        {
            public RoleTypeId Role;

            public GameObject Gloves;
        }

        [Serializable]
        private struct SleevesForRole
        {
            public RoleTypeId Role;

            public Material Sleeves;
        }

        public static SharedHandsController Singleton { get; private set; }

        public Animator Hands;

        private Transform _trackedPosition;

        private static bool _eventAssigned;

        private static bool _singletonSet;

        [SerializeField]
        private Renderer[] _sleevesRenderers;

        [SerializeField]
        private SleevesForRole[] _sleevesForRole;

        [SerializeField]
        private Material _defaultSleeves;

        [SerializeField]
        private GlovesForRole[] _glovesForRole;

        [SerializeField]
        private GameObject _defaultGloves;

        private void Awake()
        {
            Singleton = this;
            _singletonSet = true;

            if (Hands != null)
            {
                Hands.fireEvents = false;
                AnimatedViewmodelBase.OnSwayUpdated += UpdateTrackedPosition;

                if (!_eventAssigned)
                {
                    PlayerRoleManager.OnRoleChanged += RoleChanged;
                    _eventAssigned = true;
                }
            }
        }

        private void OnDestroy()
        {
            AnimatedViewmodelBase.OnSwayUpdated -= UpdateTrackedPosition;
            _singletonSet = false;
        }

        private void LateUpdate()
        {
            UpdateTrackedPosition();
        }

        private void UpdateTrackedPosition()
        {
            if (_trackedPosition == null)
                return;

            Transform handsTransform = Hands.transform;
            handsTransform.localScale = _trackedPosition.localScale;

            _trackedPosition.GetPositionAndRotation(out Vector3 pos, out Quaternion rot);
            handsTransform.SetPositionAndRotation(pos, rot);
        }

        public static void UpdateInstance(ItemViewmodelBase ivb)
        {
            if (!_singletonSet)
                return;

            AnimatedViewmodelBase animatedIvb = ivb as AnimatedViewmodelBase;

            if (animatedIvb == null || ivb == null || animatedIvb.DisableSharedHands)
            {
                if (Singleton != null && Singleton.Hands != null)
                    Singleton.Hands.gameObject.SetActive(false);
                return;
            }

            if (Singleton == null || Singleton.Hands == null)
                return;

            Singleton.Hands.gameObject.SetActive(true);
            Singleton.Hands.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            Singleton.Hands.avatar = animatedIvb.AnimatorAvatar;
            Singleton.Hands.runtimeAnimatorController = animatedIvb.AnimatorRuntimeController;
            Singleton._trackedPosition = animatedIvb.AnimatorTransform;
            Singleton.Hands.Rebind();
            return;
        }

        private static void RoleChanged(ReferenceHub hub, PlayerRoleBase oldRole, PlayerRoleBase newRole)
        {
            if (hub == null)
                return;

            if (!hub.isLocalPlayer)
                return;

            if (Singleton == null)
                return;

            if (newRole == null)
                return;

            SetRoleGloves(newRole.RoleTypeId);
        }

        public static void SetRoleGloves(RoleTypeId id)
        {
            if (Singleton == null)
                return;

            Material targetSleeves = Singleton._defaultSleeves;

            foreach (SleevesForRole entry in Singleton._sleevesForRole)
            {
                if (entry.Role == id)
                {
                    targetSleeves = entry.Sleeves;
                    break;
                }
            }

            foreach (Renderer r in Singleton._sleevesRenderers)
            {
                if (r != null)
                    r.material = targetSleeves;
            }

            if (Singleton._defaultGloves != null)
                Singleton._defaultGloves.SetActive(true);

            foreach (GlovesForRole entry in Singleton._glovesForRole)
            {
                if (entry.Gloves != null)
                    entry.Gloves.SetActive(false);
            }

            foreach (GlovesForRole entry in Singleton._glovesForRole)
            {
                if (entry.Role == id && entry.Gloves != null)
                {
                    entry.Gloves.SetActive(true);

                    if (Singleton._defaultGloves != null)
                        Singleton._defaultGloves.SetActive(false);

                    break;
                }
            }
        }
    }
}