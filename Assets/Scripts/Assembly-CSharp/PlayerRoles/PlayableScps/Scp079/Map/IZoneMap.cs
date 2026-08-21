using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Map
{
	public interface IZoneMap
	{
		void Generate();

		bool TryGetCenterTransform(Scp079Camera curCam, out Vector3 center);

		bool TrySetTeammateIndicator(ReferenceHub ply, RectTransform indicator, bool exact);

		void UpdateOpened(Scp079Camera curCam);

		bool TryGetCamera(out Scp079Camera target);
	}
}
