namespace UnityStandardAssets.Effects
{
	public class WaterHoseParticles : global::UnityEngine.MonoBehaviour
	{
		public static float lastSoundTime;

		public float force = 1f;

		private global::System.Collections.Generic.List<global::UnityEngine.ParticleCollisionEvent> m_CollisionEvents = new global::System.Collections.Generic.List<global::UnityEngine.ParticleCollisionEvent>();

		private global::UnityEngine.ParticleSystem m_ParticleSystem;

		private void Start()
		{
			m_ParticleSystem = GetComponent<global::UnityEngine.ParticleSystem>();
		}

		private void OnParticleCollision(global::UnityEngine.GameObject other)
		{
			int collisionEvents = global::UnityEngine.ParticlePhysicsExtensions.GetCollisionEvents(m_ParticleSystem, other, m_CollisionEvents);
			for (int i = 0; i < collisionEvents; i++)
			{
				if (global::UnityEngine.Time.time > lastSoundTime + 0.2f)
				{
					lastSoundTime = global::UnityEngine.Time.time;
				}
				global::UnityEngine.Rigidbody component = m_CollisionEvents[i].colliderComponent.GetComponent<global::UnityEngine.Rigidbody>();
				if (component != null)
				{
					global::UnityEngine.Vector3 velocity = m_CollisionEvents[i].velocity;
					component.AddForce(velocity * force, global::UnityEngine.ForceMode.Impulse);
				}
				other.BroadcastMessage("Extinguish", global::UnityEngine.SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
