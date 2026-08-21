using PlayerRoles.PlayableScps.Scp079.GUI;
using PlayerRoles.PlayableScps.Scp079.Overcons;

namespace PlayerRoles.PlayableScps.Scp079.Cameras
{
    public class Scp079OverconCameraSelector : Scp079DirectionalCameraSelector
    {
        private OverconBase CurOvercon => OverconManager.Singleton.HighlightedOvercon;

        public override bool IsVisible
        {
            get
            {
                if (Scp079CursorManager.LockCameras)
                    return false;
                
                if (CurOvercon is not CameraOvercon cameraOvercon)
                    return false;
                
                return cameraOvercon != null;
            }
        }

        protected override bool TryGetCamera(out Scp079Camera targetCamera)
        {
            if (!IsVisible)
            {
                targetCamera = null;
                return false;
            }
            
            targetCamera = (CurOvercon as CameraOvercon).Target;
            return true;
        }
    }
}