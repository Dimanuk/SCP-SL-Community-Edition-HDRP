using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Overcons
{
    public class DoorOvercon : StandardOvercon
    {
        [SerializeField]
        private Sprite _openSprite;

        [SerializeField]
        private Sprite _closedSprite;

        private SphereCollider _col;

        public DoorVariant Target { get; internal set; }

        private bool IsInvisible
        {
            get
            {
                if (Target is IDamageableDoor damageableDoor && damageableDoor.IsDestroyed)
                    return true;

                if (Target is CheckpointDoor checkpointDoor)
                {
                    if (!checkpointDoor.TargetState)
                        return checkpointDoor.GetExactState() > 0f;
                    
                    return true;
                }

                return false;
            }
        }

        private void LateUpdate()
        {
            TargetSprite.sprite = Target.TargetState ? _openSprite : _closedSprite;

            bool visible = !IsInvisible;
            TargetSprite.enabled = visible;
            _col.enabled = visible;
        }

        protected override void Awake()
        {
            base.Awake();
            
            TargetSprite.color = HighlightedColor;
            
            _col = GetComponent<SphereCollider>();
        }
    }
}
