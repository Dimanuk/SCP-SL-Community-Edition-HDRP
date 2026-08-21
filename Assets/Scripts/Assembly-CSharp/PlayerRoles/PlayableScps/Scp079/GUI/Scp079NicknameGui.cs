using System.Collections.Generic;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.PlayableScps;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using TMPro;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.GUI
{
	public class Scp079NicknameGui : Scp079GuiElementBase
	{
		private const float DefaultDistance = 17f;
		private const float AdditionalSize = 3f;
		private const float HeadSize = 270f;

		[SerializeField]
		private Camera _cam;

		[SerializeField]
		private RectTransform _fullscreenRect;

		[SerializeField]
		private RectTransform _template;

		private Scp079CurrentCameraSync _camSync;

		private readonly Queue<RectTransform> _pool = new Queue<RectTransform>();
		private readonly HashSet<RectTransform> _instances = new HashSet<RectTransform>();

		private void LateUpdate()
		{
			Redraw();
		}

		private void Redraw()
		{
			ClearAll();
			foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
			{
				if (allHub.IsHuman() && allHub.roleManager.CurrentRole is IFpcRole fpcRole && fpcRole.FpcModule.CharacterModelInstance is HumanCharacterModel model)
				{
					TryDrawPlayer(model, allHub.nicknameSync.DisplayName);
				}
			}
		}

		private void TryDrawPlayer(HumanCharacterModel model, string nickname)
		{
			Transform currentCamera = MainCameraController.CurrentCamera;
			float currentZoom = _camSync.CurrentCamera.ZoomAxis.CurrentZoom;
			float num = DefaultDistance * currentZoom;
			float num2 = num * num;
			HitboxIdentity[] hitboxes = model.Hitboxes;
			foreach (HitboxIdentity hitboxIdentity in hitboxes)
			{
				if (hitboxIdentity.HitboxType != HitboxType.Headshot)
				{
					continue;
				}
				if (hitboxIdentity.TargetColliders.Length != 0 && hitboxIdentity.TargetColliders[0] is CapsuleCollider capsuleCollider)
				{
					Vector3 vector = hitboxIdentity.transform.TransformPoint(capsuleCollider.center);
					if (!((vector - currentCamera.position).sqrMagnitude > num2) && !Physics.Linecast(vector, currentCamera.position, VisionInformation.VisionLayerMask))
					{
						DrawRectangle(vector, currentZoom, nickname);
					}
				}
				break;
			}
		}

		private void DrawRectangle(Vector3 targetPos, float zoom, string nickname)
		{
			Vector2 sizeDelta = _fullscreenRect.sizeDelta;
			Vector2 vector = new Vector2(sizeDelta.x / Screen.width, sizeDelta.y / Screen.height);
			Vector3 vector2 = targetPos - _camSync.CurrentCamera.Position;
			float magnitude = vector2.magnitude;
			if (magnitude != 0f && !(Vector3.Dot(MainCameraController.CurrentCamera.forward, vector2 / magnitude) < 0f))
			{
				if (!CollectionExtensions.TryDequeue(_pool, out var element))
				{
					element = Instantiate(_template, _template.parent);
				}
				Vector3 vector3 = _cam.WorldToScreenPoint(targetPos);
				float num = HeadSize * zoom / magnitude + AdditionalSize;
				element.anchoredPosition = (Vector2)vector3 * vector;
				element.sizeDelta = num * Vector2.one;
				element.gameObject.SetActive(true);
				bool flag = new Bounds(vector3, Vector3.one * num).Contains(Input.mousePosition);
				element.GetComponentInChildren<TextMeshProUGUI>().text = flag ? nickname : string.Empty;
				_instances.Add(element);
			}
		}

		private void ClearAll()
		{
			foreach (RectTransform instance in _instances)
			{
				instance.gameObject.SetActive(false);
				_pool.Enqueue(instance);
			}
			_instances.Clear();
		}

		internal override void Init(Scp079Role role, ReferenceHub owner)
		{
			base.Init(role, owner);
			role.SubroutineModule.TryGetSubroutine(out _camSync);
		}
	}
}
