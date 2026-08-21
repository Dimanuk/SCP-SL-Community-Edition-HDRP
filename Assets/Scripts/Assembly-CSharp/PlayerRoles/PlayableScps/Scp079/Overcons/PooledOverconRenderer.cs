using System.Collections.Generic;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Overcons
{
    public abstract class PooledOverconRenderer : OverconRendererBase
    {
        [SerializeField]
        private OverconBase _template;

        private readonly Queue<OverconBase> _queue = new Queue<OverconBase>();
        private readonly HashSet<OverconBase> _spawned = new HashSet<OverconBase>();

        protected T GetFromPool<T>() where T : OverconBase
        {
            if (!CollectionExtensions.TryDequeue(_queue, out OverconBase element))
                element = Object.Instantiate(_template);

            element.gameObject.SetActive(true);
            _spawned.Add(element);
            return element as T;
        }

        protected void ReturnToPool(OverconBase instance)
        {
            _spawned.Remove(instance);
            _queue.Enqueue(instance);
            instance.gameObject.SetActive(false);
        }

        protected void ReturnAll()
        {
            foreach (OverconBase item in _spawned)
            {
                if (item != null)
                {
                    item.gameObject.SetActive(false);
                    _queue.Enqueue(item);
                }
            }
            _spawned.Clear();
        }
    }
}