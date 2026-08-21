public class ExtinguishableFire : global::UnityEngine.MonoBehaviour
{
    public global::UnityEngine.ParticleSystem fireParticleSystem;

    public global::UnityEngine.ParticleSystem smokeParticleSystem;

    protected bool m_isExtinguished;

    private const float m_FireStartingTime = 2f;

    private void Start()
    {
        m_isExtinguished = true;
        smokeParticleSystem.Stop();
        fireParticleSystem.Stop();
        StartCoroutine(StartingFire());
    }

    public void Extinguish()
    {
        if (!m_isExtinguished)
        {
            m_isExtinguished = true;
            StartCoroutine(Extinguishing());
        }
    }

    private global::System.Collections.IEnumerator Extinguishing()
    {
        fireParticleSystem.Stop();
        smokeParticleSystem.time = 0f;
        smokeParticleSystem.Play();
        for (float elapsedTime = 0f; elapsedTime < 2f; elapsedTime += global::UnityEngine.Time.deltaTime)
        {
            float num = global::UnityEngine.Mathf.Max(0f, 1f - elapsedTime / 2f);
            fireParticleSystem.transform.localScale = global::UnityEngine.Vector3.one * num;
            yield return null;
        }
        yield return new global::UnityEngine.WaitForSeconds(2f);
        smokeParticleSystem.Stop();
        fireParticleSystem.transform.localScale = global::UnityEngine.Vector3.one;
        yield return new global::UnityEngine.WaitForSeconds(4f);
        StartCoroutine(StartingFire());
    }

    private global::System.Collections.IEnumerator StartingFire()
    {
        smokeParticleSystem.Stop();
        fireParticleSystem.time = 0f;
        fireParticleSystem.Play();
        for (float elapsedTime = 0f; elapsedTime < 2f; elapsedTime += global::UnityEngine.Time.deltaTime)
        {
            float num = global::UnityEngine.Mathf.Min(1f, elapsedTime / 2f);
            fireParticleSystem.transform.localScale = global::UnityEngine.Vector3.one * num;
            yield return null;
        }
        fireParticleSystem.transform.localScale = global::UnityEngine.Vector3.one;
        m_isExtinguished = false;
    }
}
