using PlayerRoles.RoleAssign;
using UnityEngine;

namespace OperationalGuide
{
    public class PannableObject : MonoBehaviour
    {
        private const float RotationSpeed = 400f;
        private const float IdleRotationSpeed = 25f;
        private const float InertiaDecay = 0.994f;

        public bool UserInput;

        public float XAxisModifer = -1f;
        public float YAxisModifer = 1f;

        private float _verticalInertia;
        private float _horizontalInertia;

        /* ==========================================================
           Lifecycle
           ========================================================== */

        private void Start()
        {
            OnPannableStart();
        }

        private void OnEnable()
        {
            UserInput = false;
            OnPannableEnable();
        }

        private void OnDisable()
        {
            UserInput = false;
            OnPannableDisable();
        }

        /* ==========================================================
           Virtual hooks для наследников
           ========================================================== */

        public virtual void OnPannableStart() { }
        public virtual void OnPannableEnable() { }
        public virtual void OnPannableDisable() { }
        public virtual void OnUpdate() { }

        private void Update()
        {
            if (ScpPreferenceSlider.AnyHighlighted) 
                return;

            Transform t = transform;

            if (Input.GetMouseButton(0))
            {
                UserInput = true;

                float mouseY = Input.GetAxis("Mouse Y");
                _verticalInertia = mouseY * RotationSpeed * Time.deltaTime * YAxisModifer * SensitivitySettings.SensMultiplier;

                float mouseX = Input.GetAxis("Mouse X");
                _horizontalInertia = mouseX * RotationSpeed * Time.deltaTime * XAxisModifer * SensitivitySettings.SensMultiplier;

                if (t != null)
                    t.Rotate(_verticalInertia, _horizontalInertia, 0f, Space.Self);
            }
            else
            {
                _verticalInertia *= InertiaDecay;
                _horizontalInertia *= InertiaDecay;

                if (t != null)
                    t.Rotate(_verticalInertia, _horizontalInertia, 0f, Space.Self);
            }

            if (!UserInput)
            {
                Transform idleTransform = transform;
                if (idleTransform != null)
                {
                    idleTransform.Rotate(0f, IdleRotationSpeed * Time.deltaTime, 0f, Space.Self);
                }
            }

            OnUpdate();
        }
    }
}
