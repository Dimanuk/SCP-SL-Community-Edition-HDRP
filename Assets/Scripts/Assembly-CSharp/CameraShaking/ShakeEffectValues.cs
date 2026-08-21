using UnityEngine;

namespace CameraShaking
{
    public struct ShakeEffectValues
    {
        public Quaternion RootCameraRotation;
        public Quaternion ViewmodelCameraRotation;
        public Vector3 RootCameraPositionOffset;
        public float FovMultiplier;
        public float VerticalMouseLook;
        public float HorizontalMouseLook;

        public static ShakeEffectValues None = new(Quaternion.identity, Quaternion.identity, Vector3.zero);

        public ShakeEffectValues(Quaternion? rootRot = null, Quaternion? viewmodelRot = null, Vector3? posOffset = null, float fov = 1f, float vLook = 0f, float hLook = 0f)
        {
            RootCameraRotation = rootRot ?? Quaternion.identity;
            ViewmodelCameraRotation = viewmodelRot ?? Quaternion.identity;
            RootCameraPositionOffset = posOffset ?? Vector3.zero;
            FovMultiplier = fov;
            VerticalMouseLook = vLook;
            HorizontalMouseLook = hLook;
        }
    }
}