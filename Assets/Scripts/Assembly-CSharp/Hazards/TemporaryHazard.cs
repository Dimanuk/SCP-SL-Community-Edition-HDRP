namespace Hazards
{
    [global::UnityEngine.RequireComponent(typeof(global::Elevators.TransformElevatorFollower))]
    public abstract class TemporaryHazard : global::Hazards.EnvironmentalHazard
    {
        private bool _destroyed;

        private float _elapsed;

        public override bool IsActive => !_destroyed;

        protected abstract float HazardDuration { get; }

        protected virtual float DecaySpeed { get; } = 1f;

        [global::Mirror.Server]
        public virtual void ServerDestroy()
        {
            if (!global::Mirror.NetworkServer.active)
            {
                global::UnityEngine.Debug.LogWarning("[Server] function 'System.Void Hazards.TemporaryHazard::ServerDestroy()' called when server was not active");
                return;
            }
            _destroyed = true;
            base.AffectedPlayers.ToArray().ForEach(OnExit);
        }

        protected override void Update()
        {
            base.Update();
            if (global::Mirror.NetworkServer.active && IsActive)
            {
                if (_elapsed > HazardDuration)
                {
                    ServerDestroy();
                }
                else
                {
                    _elapsed += DecaySpeed * global::UnityEngine.Time.deltaTime;
                }
            }
        }
    }
}
