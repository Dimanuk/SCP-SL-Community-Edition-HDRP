using UnityEngine;

public class RagdollPart : MonoBehaviour
{
    public BasicRagdoll ParentRagdoll;

    private void Reset()
    {
        ParentRagdoll = GetComponentInParent<BasicRagdoll>();
    }
}