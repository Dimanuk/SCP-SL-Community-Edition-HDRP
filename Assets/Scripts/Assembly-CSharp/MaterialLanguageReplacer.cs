using UnityEngine;

public class MaterialLanguageReplacer : MonoBehaviour
{
    public Material englishVersion;

    public void Start()
    {
        GetComponent<Renderer>().material = englishVersion;
    }

    public void OnDestroy()
    {
        Object.Destroy(GetComponent<Renderer>().material);
    }
}