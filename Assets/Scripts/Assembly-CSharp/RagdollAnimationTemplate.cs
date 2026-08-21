public class RagdollAnimationTemplate : global::UnityEngine.MonoBehaviour
{
    [global::System.Serializable]
    private struct RagdollBone
    {
        public global::UnityEngine.Vector3 PositionOffset;

        public global::UnityEngine.Quaternion RotationOffset;

        public global::UnityEngine.Vector3 StartVelocity;
    }

    [global::UnityEngine.SerializeField]
    private RagdollAnimationTemplate.RagdollBone[] _bones;

    [global::UnityEngine.SerializeField]
    private global::UnityEngine.Quaternion _overallRotation;

    public void ProcessRagdoll(BasicRagdoll rg)
    {
        if (rg is global::PlayerRoles.Ragdolls.DynamicRagdoll dynamicRagdoll)
        {
            int num = global::UnityEngine.Mathf.Min(_bones.Length, dynamicRagdoll.LinkedRigidbodies.Length);
            rg.transform.rotation *= _overallRotation;
            for (int i = 0; i < num; i++)
            {
                global::UnityEngine.Rigidbody obj = dynamicRagdoll.LinkedRigidbodies[i];
                RagdollAnimationTemplate.RagdollBone ragdollBone = _bones[i];
                global::UnityEngine.Transform obj2 = obj.transform;
                obj2.localRotation = ragdollBone.RotationOffset;
                obj2.position = rg.Info.StartPosition + rg.Info.StartRotation * ragdollBone.PositionOffset;
                obj.linearVelocity = rg.Info.StartRotation * ragdollBone.StartVelocity;
            }
        }
    }
}
