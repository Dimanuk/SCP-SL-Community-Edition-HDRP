using TMPro;
using UnityEngine;

namespace LightContainmentZoneDecontamination
{
    public class DecontaminationScreen : MonoBehaviour
    {
        public Animator AnimationController;
        public TextMeshPro CountdownText;

        public bool InsideLCZ = true;

        private int _animHash;
        private int _lczHash;
        private bool _singletonSet;

        private void Start()
        {
            _animHash = Animator.StringToHash("Time");
            _lczHash = Animator.StringToHash("InsideLCZ");
        }

        private void Update()
        {
            if (!_singletonSet)
            {
                if (DecontaminationController.Singleton == null)
                    return;

                _singletonSet = true;
            }

            var controller = DecontaminationController.Singleton;
            if (controller == null)
                return;

            if (controller.ClientTimer != null && !controller.ClientTimer.enabled)
            {
                enabled = false;
            }

            if (AnimationController != null)
            {
                if (controller.DecontaminationOverride == DecontaminationController.DecontaminationStatus.Forced)
                {
                    AnimationController.SetFloat(_animHash, 0f);
                }
                else
                {
                    AnimationController.SetBool(_lczHash, InsideLCZ);
                    AnimationController.SetFloat(_animHash, DecontaminationClientTimer.RemainingTimeInSeconds);
                }
            }

            if (CountdownText != null)
            {
                CountdownText.text = DecontaminationClientTimer.ScreenTimeString;
            }
        }
    }
}
