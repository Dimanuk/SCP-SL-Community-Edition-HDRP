using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Spawnpoints;
using PlayerRoles.PlayableScps.HUDs;
using PlayerRoles.Spectating;
using UnityEngine;

namespace PlayerRoles.PlayableScps
{
	public class FpcStandardScp : FpcStandardRoleBase
	{
		private static readonly Color DarkenedAmbientColor = new Color(0.25f, 0.25f, 0.25f);

		private RoomRoleSpawnpoint _cachedSpawnpoint;

		[SerializeField]
		private RoleTypeId _roleTypeId;

		[SerializeField]
		private int _maxHealth;

		[SerializeField]
		private RoomRoleSpawnpoint _roomSpawnpoint;

		[SerializeField]
		private bool _disableSpawnpoint;

		public override RoleTypeId RoleTypeId => _roleTypeId;

		public override Team Team => Team.SCPs;

		public override Color RoleColor => Color.red;

		public override float MaxHealth => _maxHealth;

		public override ISpawnpointHandler SpawnpointHandler
		{
			get
			{
				if (_disableSpawnpoint)
					return null;

				if (_cachedSpawnpoint == null)
				{
					_cachedSpawnpoint = new RoomRoleSpawnpoint(_roomSpawnpoint);
				}

				return _cachedSpawnpoint;
			}
		}

		public override Color AmbientLight
		{
			get
			{
				if (!InsufficientLight)
					return base.AmbientLight;

				return DarkenedAmbientColor;
			}
		}

		public override bool TryGetViewmodelFov(out float fov)
		{
			if (this is IHudScp && ScpHudController.CurInstance is IViewmodelRole vmRole)
			{
				return vmRole.TryGetViewmodelFov(out fov);
			}

			return base.TryGetViewmodelFov(out fov);
		}
	}
}