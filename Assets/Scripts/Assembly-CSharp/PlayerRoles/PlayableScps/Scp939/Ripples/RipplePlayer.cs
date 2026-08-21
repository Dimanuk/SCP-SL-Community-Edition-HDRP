using System.Collections.Generic;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939.Ripples
{
    public class RipplePlayer : SubroutineBase
    {
        [SerializeField]
        private RippleInstance _rippleTemplate;

        private Scp939FocusAbility _focus;

        private int _poolCount;

        private readonly Queue<RippleInstance> _pool = new Queue<RippleInstance>();

        public void Play(Vector3 position, Color color)
        {
            bool found = false;
            RippleInstance instance = null;

            while (_poolCount != 0 && !(found = (instance = _pool.Peek()) != null))
            {
                _pool.Dequeue();
                _poolCount--;
            }

            if (found && !instance.InUse)
            {
                _pool.Dequeue();
                _poolCount--;
            }
            else
            {
                instance = Object.Instantiate(_rippleTemplate);
            }

            instance.Set(position, (_focus.State < 1f) ? Color.red : color);

            _pool.Enqueue(instance);
            _poolCount++;
        }

        public void Play(HumanRole human)
        {
            Play(human.FpcModule.Position, human.RoleColor);
        }


        protected override void Awake()
        {
            base.Awake();
            (base.Role as ISubroutinedScpRole).SubroutineModule.TryGetSubroutine(out _focus);
        }
    }
}