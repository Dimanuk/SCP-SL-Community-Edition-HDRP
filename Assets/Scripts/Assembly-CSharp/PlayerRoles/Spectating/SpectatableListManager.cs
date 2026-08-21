using System;
using System.Collections.Generic;
using GameObjectPools;
using UnityEngine;
using UnityEngine.UI;
using ToggleableMenus;

namespace PlayerRoles.Spectating
{
    public class SpectatableListManager : MonoBehaviour
    {
        [SerializeField]
        private SpectatableListElementDefinition[] _definedPairs;

        [SerializeField]
        private float _targetHeight;

        [SerializeField]
        private VerticalLayoutGroup _layoutGroup;

        private int _lastTargetId;

        private readonly List<SpectatableListSpawnedElement> _spawnedTargets = new List<SpectatableListSpawnedElement>();

        private static bool _initialized;

        private static KeyCode _nextKey;

        private static KeyCode _prevKey;

        private static readonly Dictionary<SpectatableListElementType, SpectatableListElementDefinition> Definitions
            = new Dictionary<SpectatableListElementType, SpectatableListElementDefinition>();

        private void OnEnable()
        {
            SpectatorTargetTracker.OnTargetChanged += RefreshSize;

            SpectatableModuleBase.OnAdded += AddTarget;
            SpectatableModuleBase.OnRemoved += RemoveTarget;

            if (!_initialized)
            {
                foreach (var value in _definedPairs)
                {
                    Definitions[value.Type] = value;
                    PoolManager.Singleton.TryAddPool(value.FullSize);
                    PoolManager.Singleton.TryAddPool(value.Compact);
                }

                RefreshKeybinds();
                _initialized = true;
            }

            RefreshAllTargets();
        }

        private void OnDisable()
        {
            SpectatorTargetTracker.OnTargetChanged -= RefreshSize;

            SpectatableModuleBase.OnAdded -= AddTarget;
            SpectatableModuleBase.OnRemoved -= RemoveTarget;
        }

        private void LateUpdate()
        {
            int count = _spawnedTargets.Count;
            if (count == 0)
            {
                _lastTargetId = 0;
                return;
            }

            int lastTargetId = _lastTargetId;

            if (Input.GetKeyDown(_prevKey))
                lastTargetId--;
            if (Input.GetKeyDown(_nextKey))
                lastTargetId++;

            if (lastTargetId != _lastTargetId)
            {
                if (ToggleableMenuController.AnyEnabled)
                    return;

                if (SpectatorGuiElement.AnyHighlighted)
                    return;

                // Wrap robustly. _lastTargetId can drift several steps out of range
                // after spectated targets are removed (it's only re-synced when the
                // current target is still in the list), so a single +/-count wrap is
                // not enough — use modulo to guarantee a valid index (count > 0 here).
                lastTargetId = ((lastTargetId % count) + count) % count;

                _lastTargetId = lastTargetId;
                SpectatorTargetTracker.CurrentTarget = _spawnedTargets[lastTargetId].Target;
            }
        }

        private void AddTarget(SpectatableModuleBase target)
        {
            int orderPriority = GetOrderPriority(target.MainRole);
            int insertIndex = _spawnedTargets.Count;

            for (int i = 0; i < _spawnedTargets.Count; i++)
            {
                if (_spawnedTargets[i].Priority > orderPriority)
                {
                    insertIndex = i;
                    break;
                }
            }

            if (Definitions.TryGetValue(target.ListElementType, out var value)
                && value.TryGetFromPools(base.transform, out var full, out var compact))
            {
                SpectatableListSpawnedElement item = new SpectatableListSpawnedElement
                {
                    Priority = orderPriority,
                    Compact = compact,
                    FullSize = full,
                    Target = target
                };

                SetupNewTarget(item.Compact, target, insertIndex * 2);
                SetupNewTarget(item.FullSize, target, insertIndex * 2 + 1);
                _spawnedTargets.Insert(insertIndex, item);
                RefreshSize();
            }
        }

        private void RemoveTarget(SpectatableModuleBase target)
        {
            for (int i = 0; i < _spawnedTargets.Count; i++)
            {
                if (_spawnedTargets[i].Compact.Target == target)
                {
                    _spawnedTargets[i].ReturnToPool();
                    _spawnedTargets.RemoveAt(i);
                    break;
                }
            }
            RefreshSize();
        }

        private void RefreshAllTargets()
        {
            _spawnedTargets.ForEach(delegate(SpectatableListSpawnedElement x)
            {
                x.ReturnToPool();
            });

            _spawnedTargets.Clear();

            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.roleManager.CurrentRole is ISpectatableRole spectatableRole)
                {
                    AddTarget(spectatableRole.SpectatorModule);
                }
            }
        }

        private void SetupNewTarget(SpectatableListElementBase element, SpectatableModuleBase module, int order)
        {
            Transform obj = element.transform;
            obj.localScale = Vector3.one;
            obj.localPosition = Vector3.zero;
            obj.localRotation = Quaternion.identity;
            obj.SetSiblingIndex(order);

            element.Index = order;

            if (element.Target != module)
                element.Target = module;
        }

        private void RefreshSize()
        {
            int count = _spawnedTargets.Count;
            if (count == 0)
                return;

            float compactTotal = _layoutGroup.spacing * (count - 1);
            float fullTotal = compactTotal;

            for (int i = 0; i < count; i++)
            {
                compactTotal += _spawnedTargets[i].Compact.Height;
                fullTotal += _spawnedTargets[i].FullSize.Height;

                if (_spawnedTargets[i].Target == SpectatorTargetTracker.CurrentTarget)
                    _lastTargetId = i;
            }

            float t = Mathf.InverseLerp(compactTotal, fullTotal, _targetHeight);
            int start, end;

            if (t < 1f)
            {
                int visibleCount = Mathf.FloorToInt(t * count) - 1;

                if (visibleCount > 12)
                    visibleCount = 12;
                else if (visibleCount % 2 != 0)
                    visibleCount--;

                int center = Mathf.Clamp(_lastTargetId, 0, count - 1);
                start = center - visibleCount / 2;
                end = center + visibleCount / 2;

                if (start < 0)
                    end -= start;
                if (end >= count)
                    start += 1 + count - end;
            }
            else
            {
                start = 0;
                end = count;
            }

            for (int j = 0; j < count; j++)
            {
                bool active = j >= start && j <= end;
                _spawnedTargets[j].FullSize.gameObject.SetActive(active);
                _spawnedTargets[j].Compact.gameObject.SetActive(!active);
            }
        }

        private static void RefreshKeybinds()
        {
            _nextKey = NewInput.GetKey(ActionName.Shoot);
            _prevKey = NewInput.GetKey(ActionName.Zoom);
        }

        private static int GetOrderPriority(PlayerRoleBase prb)
        {
            int num = (int)prb.Team.GetFaction() * 65535;
            int num2 = (int)prb.Team * 255;
            int roleTypeId = (int)prb.RoleTypeId;
            return num + num2 + roleTypeId;
        }
    }
}