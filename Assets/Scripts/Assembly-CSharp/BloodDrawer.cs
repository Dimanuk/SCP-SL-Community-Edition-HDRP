using Mirror;
using UnityEngine;

public class BloodDrawer : NetworkBehaviour
{
    private static Collider[] cachedColliders = new Collider[3];

    public void DrawBlood(RaycastHit hit, float randomMin = 0.75f, float randomMax = 1.25f)
    {
        if (!Knife.DeferredDecals.DeferredDecalsSystem.Singleton.EnableDecals)
            return;

        TestOverlapBlood(hit.point);

        if (Knife.DeferredDecals.DecalPoolManager.Singleton.TrySpawnDecal(Knife.DeferredDecals.DecalPoolType.Blood, out var decal))
        {
            Knife.DeferredDecals.Decal.SetTransformAlongSurface(decal.transform, hit);
            float randomScale = UnityEngine.Random.Range(randomMin, randomMax);
            decal.transform.localScale = Vector3.one * randomScale;
            decal.transform.Rotate(0f, UnityEngine.Random.Range(0f, 360f), 0f);
            decal.transform.SetParent(hit.transform, true);
        }
    }

    private void TestOverlapBlood(Vector3 position)
    {
        int num = Physics.OverlapSphereNonAlloc(position, 0.5f, cachedColliders);
        for (int i = 0; i < num; i++)
        {
            if (cachedColliders[i] == null) continue;

            BloodController component = cachedColliders[i].GetComponent<BloodController>();
            if (component != null && component.BloodAgeTimer > 0f)
                component.DestroyDecal();
        }
    }

    public void PlaceUnderneath(Vector3 pos, float amountMultiplier = 1f)
    {
        if (!Knife.DeferredDecals.DeferredDecalsSystem.Singleton.EnableDecals)
            return;

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 10f))
        {
            TestOverlapBlood(hit.point);

            if (Knife.DeferredDecals.DecalPoolManager.Singleton.TrySpawnDecal(Knife.DeferredDecals.DecalPoolType.Blood, out var decal))
            {
                decal.transform.SetPositionAndRotation(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                float randomScale = UnityEngine.Random.Range(0.75f, 1.25f) * amountMultiplier;
                decal.transform.localScale = Vector3.one * randomScale;
                decal.transform.Rotate(0f, UnityEngine.Random.Range(0f, 360f), 0f);
                decal.transform.SetParent(hit.transform, true);
            }
        }
    }
}