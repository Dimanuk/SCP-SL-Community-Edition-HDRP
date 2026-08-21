public class ParticleCollision : global::UnityEngine.MonoBehaviour
{
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
            ExtinguishableFire component = m_CollisionEvents[i].colliderComponent.GetComponent<ExtinguishableFire>();
            if (component != null)
            {
                component.Extinguish();
            }
        }
    }
}
