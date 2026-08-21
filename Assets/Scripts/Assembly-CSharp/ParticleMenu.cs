public class ParticleMenu : global::UnityEngine.MonoBehaviour
{
    public ParticleExamples[] particleSystems;

    public global::UnityEngine.GameObject gunGameObject;

    private int currentIndex;

    private global::UnityEngine.GameObject currentGO;

    public global::UnityEngine.Transform spawnLocation;

    private void Start()
    {
        Navigate(0);
        currentIndex = 0;
    }

    public void Navigate(int i)
    {
        currentIndex = (particleSystems.Length + currentIndex + i) % particleSystems.Length;
        if (currentGO != null)
        {
            global::UnityEngine.Object.Destroy(currentGO);
        }
        currentGO = global::UnityEngine.Object.Instantiate(particleSystems[currentIndex].particleSystemGO, spawnLocation.position + particleSystems[currentIndex].particlePosition, global::UnityEngine.Quaternion.Euler(particleSystems[currentIndex].particleRotation));
        gunGameObject.SetActive(particleSystems[currentIndex].isWeaponEffect);
    }
}
