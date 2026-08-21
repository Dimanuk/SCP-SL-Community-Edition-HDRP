using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Overcons
{
    public class StandardOvercon : OverconBase
    {
        [SerializeField]
        private AnimationCurve _scaleOverDistance;

        [SerializeField]
        protected SpriteRenderer TargetSprite;

        private const float SurfaceSizeScale = 2.5f;

        public static readonly Color HighlightedColor = new Color(1f, 1f, 1f, 1f);
        public static readonly Color NormalColor = new Color(1f, 1f, 1f, 0.27f);

        public override bool IsHighlighted
        {
            get => base.IsHighlighted;
            internal set
            {
                TargetSprite.color = value ? HighlightedColor : NormalColor;
                base.IsHighlighted = value;
            }
        }

        protected virtual void Awake()
        {
            TargetSprite.color = HighlightedColor;
        }

        public void Rescale(Scp079Camera cam)
        {
            Rescale(cam, Vector3.Distance(cam.Position, transform.position));
        }

        public void Rescale(Scp079Camera cam, float dis)
        {
            transform.LookAt(cam.Position);
            float scale = _scaleOverDistance.Evaluate(dis);

            if (cam.Room.Zone == MapGeneration.FacilityZone.Surface)
                scale *= SurfaceSizeScale;

            transform.localScale = Vector3.one * scale;
        }
    }
}