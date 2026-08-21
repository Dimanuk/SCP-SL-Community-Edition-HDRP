using System.Collections.Generic;
using UnityEngine;

public class Clutter : MonoBehaviour
{
    public GameObject holderObject;
    public List<GameObject> possiblePrefabs = new List<GameObject>();

    public Vector3 spawnOffset = Vector3.zero;
    public Vector3 clutterScale = new Vector3(0.72745f, 0.72745f, 0.72745f);

    public bool spawned;

    private const float OverallScale = 0.72745f;

    public void SpawnClutter()
    {
        GameCore.Console.AddDebugLog("MGCLTR",
            $"Spawning clutter component on object of name \"{gameObject.name}\"",
            MessageImportance.LeastImportant, true);

        spawned = true;

        if (holderObject == null)
            holderObject = gameObject;

        GameObject prefab = null;
        if (possiblePrefabs != null && possiblePrefabs.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, possiblePrefabs.Count);
            prefab = possiblePrefabs[index];
        }

        if (prefab == null)
            return;

        Transform holderTransform = holderObject.transform;

        Vector3 spawnPosition = holderTransform.position +
            holderTransform.rotation * spawnOffset;

        GameObject instance = Instantiate(prefab, spawnPosition,
            holderTransform.rotation, holderTransform.parent);

        if (instance != null)
        {
            instance.transform.localScale = clutterScale;

            if (instance.TryGetComponent<Clutter>(out var existing))
                Destroy(existing);

            instance.SetActive(true);
        }

        if (holderObject != null && holderObject != gameObject)
            Destroy(holderObject);

        if (instance != null)
        {
            var cullable = instance.AddComponent<CustomCulling.DynamicCullableBase>();
            if (cullable != null)
                cullable.StaticObject = true;
        }
    }

    public Clutter()
    {
        possiblePrefabs = new List<GameObject>();
        clutterScale = new Vector3(OverallScale, OverallScale, OverallScale);
    }
}