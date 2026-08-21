public class DetectorController : global::UnityEngine.MonoBehaviour
{
    public float detectionProgress;

    public float viewRange = 30f;

    public float fov = -0.75f;

    public global::UnityEngine.GameObject[] detectors;

    private void Start()
    {
        InvokeRepeating(nameof(RefreshDetectorsList), 10f, 10f);
    }

    public void RefreshDetectorsList()
    {
        detectors = global::UnityEngine.GameObject.FindGameObjectsWithTag("Detector");
    }

    private void Update()
    {
        if (detectors.Length == 0)
        {
            return;
        }
        bool flag = false;
        global::UnityEngine.GameObject[] array = detectors;
        foreach (global::UnityEngine.GameObject gameObject in array)
        {
            if (global::UnityEngine.Vector3.Distance(gameObject.transform.position, base.transform.position) > viewRange)
            {
                global::UnityEngine.Vector3 normalized = (base.transform.position - gameObject.transform.position).normalized;
                if (global::UnityEngine.Vector3.Dot(gameObject.transform.forward, normalized) < fov && global::UnityEngine.Physics.Raycast(gameObject.transform.position, normalized, out var hitInfo) && hitInfo.transform.CompareTag("Detector"))
                {
                    flag = true;
                    break;
                }
            }
        }
        detectionProgress += global::UnityEngine.Time.deltaTime * (flag ? 0.3f : (-0.5f));
        detectionProgress = global::UnityEngine.Mathf.Clamp01(detectionProgress);
    }
}
