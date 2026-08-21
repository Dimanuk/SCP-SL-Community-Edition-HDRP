public class MoreCast : global::UnityEngine.MonoBehaviour
{
    public static bool BeamCast(global::UnityEngine.Vector3 start, global::UnityEngine.Vector3 end, global::UnityEngine.Vector3 beamRadius, float beamStep, out global::System.Collections.Generic.List<global::UnityEngine.RaycastHit> hitInfo, int layerMask, bool any)
    {
        hitInfo = new global::System.Collections.Generic.List<global::UnityEngine.RaycastHit>();
        global::UnityEngine.Vector3 start2 = start;
        global::UnityEngine.Vector3 end2 = end;
        start2 -= beamRadius;
        end2 -= beamRadius;
        for (float num = 0f - beamRadius.x; num < beamRadius.x; num += beamStep)
        {
            start2.y = start.y;
            end2.y = end.y;
            start2.x += beamStep;
            end2.x += beamStep;
            for (float num2 = 0f - beamRadius.y; num2 < beamRadius.x; num2 += beamStep)
            {
                start2.z = start.z;
                end2.z = end.z;
                start2.y += beamStep;
                end2.y += beamStep;
                for (float num3 = 0f - beamRadius.x; num3 < beamRadius.x; num3 += beamStep)
                {
                    start2.z += beamStep;
                    end2.z += beamStep;
                    global::UnityEngine.RaycastHit hitInfo2;
                    bool flag = global::UnityEngine.Physics.Linecast(start2, end2, out hitInfo2, layerMask);
                    hitInfo.Add(hitInfo2);
                    if (any && flag)
                    {
                        return true;
                    }
                    if (!flag && !any)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public static bool BeamCast(global::UnityEngine.Vector3 start, global::UnityEngine.Vector3 end, global::UnityEngine.Vector3 beamRadius, float beamStep, int layerMask, bool any)
    {
        global::System.Collections.Generic.List<global::UnityEngine.RaycastHit> hitInfo;
        return BeamCast(start, end, beamRadius, beamStep, out hitInfo, layerMask, any);
    }
}
