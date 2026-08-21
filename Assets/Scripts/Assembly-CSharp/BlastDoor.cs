[global::UnityEngine.RequireComponent(typeof(global::Mirror.NetworkIdentity))]
public class BlastDoor : global::Mirror.NetworkBehaviour
{
    public static readonly global::System.Collections.Generic.HashSet<BlastDoor> Instances = new global::System.Collections.Generic.HashSet<BlastDoor>();

    private static readonly int _close = global::UnityEngine.Animator.StringToHash("Close");

    [global::Mirror.SyncVar(hook = nameof(SetClosed))]
    public bool isClosed;
    private void Start()
    {
        Instances.Add(this);
    }

    private void OnDestroy()
    {
        Instances.Remove(this);
    }

    public void SetClosed(bool prev, bool b)
    {
        isClosed = b;
        if (isClosed)
        {
            GetComponent<global::UnityEngine.Animator>().SetTrigger(_close);
        }
    }
}
