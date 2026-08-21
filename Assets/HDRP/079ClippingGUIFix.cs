using UnityEngine;

[RequireComponent(typeof(Camera))]
public class InfiniteClippingCamera : MonoBehaviour
{
    [Header("Режимы отключения clipping planes")]
    [Tooltip("Использовать бесконечную дальнюю плоскость (рекомендуется)")]
    public bool useInfiniteFar = true;

    [Tooltip("Значение ближней плоскости (не может быть 0, минимально 0.001)")]
    public float nearClip = 0.001f;

    [Tooltip("Принудительно обновлять матрицу каждый кадр (необходимо для HDRP)")]
    public bool applyEveryFrame = true;

    private Camera cam;
    private Matrix4x4 originalProjection;
    private bool isOrthographic;

    void Awake()
    {
        cam = GetComponent<Camera>();
        originalProjection = cam.projectionMatrix;
        isOrthographic = cam.orthographic;
    }

    void OnEnable()
    {
        ApplyProjection();
    }

    void OnDisable()
    {
        // Восстанавливаем исходную матрицу при отключении скрипта
        cam.projectionMatrix = originalProjection;
        // Если использовали экстремальные значения через стандартные параметры – сбрасываем их
        cam.nearClipPlane = 0.3f;   // значение по умолчанию
        cam.farClipPlane = 1000f;   // значение по умолчанию
    }

    void Update()
    {
        if (applyEveryFrame)
            ApplyProjection();
    }

    void ApplyProjection()
    {
        if (useInfiniteFar)
        {
            // Устанавливаем кастомную проекционную матрицу с бесконечной дальностью
            if (isOrthographic)
                cam.projectionMatrix = BuildInfiniteOrthographicMatrix();
            else
                cam.projectionMatrix = BuildInfinitePerspectiveMatrix();
        }
        else
        {
            // Экстремальные значения (простой способ, но может вызвать z-fighting на больших расстояниях)
            cam.nearClipPlane = Mathf.Max(nearClip, 0.001f);
            cam.farClipPlane = 1e9f;
            // Если ранее была переопределена projectionMatrix – сбрасываем её
            cam.ResetProjectionMatrix();
        }
    }

    /// <summary>
    /// Строит бесконечную матрицу для перспективной камеры (far = ∞)
    /// </summary>
    Matrix4x4 BuildInfinitePerspectiveMatrix()
    {
        float fov = cam.fieldOfView;
        float aspect = cam.aspect;
        float near = Mathf.Max(nearClip, 0.001f);

        float f = 1.0f / Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
        Matrix4x4 mat = new Matrix4x4();
        mat.m00 = f / aspect;
        mat.m11 = f;
        mat.m22 = -1f;
        mat.m23 = -2f * near;
        mat.m32 = -1f;
        mat.m33 = 0f;
        // Остальные элементы (m01, m02, m03, m10, m12, m13, m20, m21, m30, m31) равны 0
        return mat;
    }

    /// <summary>
    /// Строит бесконечную матрицу для ортографической камеры (far = ∞)
    /// </summary>
    Matrix4x4 BuildInfiniteOrthographicMatrix()
    {
        float near = Mathf.Max(nearClip, 0.001f);
        float size = cam.orthographicSize;
        float aspect = cam.aspect;

        float left = -size * aspect;
        float right = size * aspect;
        float bottom = -size;
        float top = size;

        Matrix4x4 mat = new Matrix4x4();
        mat.m00 = 2f / (right - left);
        mat.m11 = 2f / (top - bottom);
        mat.m22 = 0f;           // при far = ∞
        mat.m23 = -1f;          // при far = ∞
        mat.m33 = 1f;
        // Остальные элементы обнулены
        return mat;
    }

    // Опционально: при изменении параметров камеры (размер окна, FOV) пересчитываем матрицу
    void OnValidate()
    {
        if (cam != null && applyEveryFrame == false)
            ApplyProjection();
    }
}