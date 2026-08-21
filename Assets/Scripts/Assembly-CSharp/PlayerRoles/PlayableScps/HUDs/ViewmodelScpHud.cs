using System.Runtime.CompilerServices;
using UnityEngine;

namespace PlayerRoles.PlayableScps.HUDs
{
    public abstract class ViewmodelScpHud : ScpHudBase, IViewmodelRole
    {
        [field: SerializeField]
        public ScpViewmodelBase Viewmodel { get; private set; }

        public bool TryGetViewmodelFov(out float fov)
        {
            ScpViewmodelBase viewmodel = this.Viewmodel;
            
            if (viewmodel == null)
            {
                fov = 0f;
                return false;
            }

            fov = viewmodel.CamFOV;
            return true;
        }
    }
}