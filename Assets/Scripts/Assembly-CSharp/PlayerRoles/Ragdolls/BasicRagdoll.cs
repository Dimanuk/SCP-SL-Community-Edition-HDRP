using PlayerRoles.Ragdolls;

public class BasicRagdoll : global::Mirror.NetworkBehaviour
{
    [global::Mirror.SyncVar]
    public RagdollData Info;

    public bool RunMagic;

    public global::DeathAnimations.DeathAnimation[] AllDeathAnimations;

    [global::UnityEngine.SerializeField]
    private global::UnityEngine.Transform _originPoint;

    [global::UnityEngine.SerializeField]
    private global::UnityEngine.Collider[] _serializedColliders;

    [global::UnityEngine.SerializeField]
    private global::UnityEngine.SkinnedMeshRenderer[] _serializedRenderers;

    private bool _hasCustomCenter;

    private bool _cleanedUp;

    public virtual global::UnityEngine.Transform CenterPoint
    {
        get
        {
            if (!_hasCustomCenter)
            {
                return base.transform;
            }
            return _originPoint;
        }
    }

    protected virtual void OnCleanup()
    {
        global::UnityEngine.Collider[] serializedColliders = _serializedColliders;
        for (int i = 0; i < serializedColliders.Length; i++)
        {
            serializedColliders[i].enabled = false;
        }
        global::UnityEngine.SkinnedMeshRenderer[] serializedRenderers = _serializedRenderers;
        for (int i = 0; i < serializedRenderers.Length; i++)
        {
            serializedRenderers[i].enabled = false;
        }
        _cleanedUp = true;
    }

    protected virtual void Awake()
    {
        _hasCustomCenter = _originPoint != null;
    }

    protected virtual void Start()
    {
        base.transform.SetPositionAndRotation(Info.StartPosition, Info.StartRotation);
        _cleanedUp = global::PlayerRoles.Ragdolls.RagdollManager.CleanupTime <= 0;
        Info.Handler.ProcessRagdoll(this);
        global::PlayerRoles.Ragdolls.RagdollManager.OnSpawnedRagdoll(this);
    }

    protected virtual void OnDestroy()
    {
        global::PlayerRoles.Ragdolls.RagdollManager.OnRemovedRagdoll(this);
    }

    protected virtual void Update()
    {
        UpdateCleanup();
    }

    private void UpdateCleanup()
    {
        if (!_cleanedUp && !(Info.ExistenceTime < (float)global::PlayerRoles.Ragdolls.RagdollManager.CleanupTime))
        {
            OnCleanup();
        }
    }
}
