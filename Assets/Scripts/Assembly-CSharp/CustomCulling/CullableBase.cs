using Knife.DeferredDecals;
using PostProcessing;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCulling
{
    public class CullableBase : MonoBehaviour
    {
        public bool StaticObject;

        [SerializeField]
        internal List<Decal> Decals;

        [SerializeField]
        private List<Renderer> renderers;

        [SerializeField]
        private List<Behaviour> otherBehaviours;

        [SerializeField]
        private List<Light> lights;

        [SerializeField]
        private bool _isSerialized;

        protected bool _currentlyEnabled;

        private bool _cullEnabled;

        internal virtual bool CullEnabled
        {
            get => _cullEnabled;
            set => _cullEnabled = value;
        }

        internal List<Renderer> Renderers => renderers;
        internal List<Behaviour> OtherBehaviours => otherBehaviours;
        internal List<Light> Lights => lights;

        private void Awake()
        {
            if (!_isSerialized)
                GetChildren();

            _currentlyEnabled = true;
            CullEnabled = false;
            UpdateBehaviours();
            OnAwake();
        }

        protected virtual void OnAwake() { }

        private static HashSet<Transform> BuildNestedTransformSet(Transform root)
        {
            var nested = new HashSet<Transform>();

            foreach (CullableBase child in root.GetComponentsInChildren<CullableBase>(true))
            {
                if (child.transform == root)
                    continue;

                foreach (Transform t in child.GetComponentsInChildren<Transform>(true))
                    nested.Add(t);
            }

            return nested;
        }

        internal void GetChildren()
        {
            lights = new List<Light>();
            renderers = new List<Renderer>();
            otherBehaviours = new List<Behaviour>();
            Decals = new List<Decal>();

            HashSet<Transform> nested = BuildNestedTransformSet(transform);

            foreach (Light l in GetComponentsInChildren<Light>(true))
                if (!nested.Contains(l.transform))
                    lights.Add(l);

            foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
                if (!nested.Contains(r.transform) && !r.gameObject.name.StartsWith("RID"))
                    renderers.Add(r);

            foreach (Canvas c in GetComponentsInChildren<Canvas>(true))
                if (!nested.Contains(c.transform))
                    otherBehaviours.Add(c);

            foreach (CanvasScaler cs in GetComponentsInChildren<CanvasScaler>(true))
                if (!nested.Contains(cs.transform))
                    otherBehaviours.Add(cs);

            foreach (TextMeshPro tmp in GetComponentsInChildren<TextMeshPro>(true))
                if (!nested.Contains(tmp.transform))
                    otherBehaviours.Add(tmp);

            foreach (ReflectionProbe rp in GetComponentsInChildren<ReflectionProbe>(true))
                if (!nested.Contains(rp.transform))
                    otherBehaviours.Add(rp);

            foreach (Decal d in GetComponentsInChildren<Decal>(true))
                if (!nested.Contains(d.transform))
                    Decals.Add(d);
        }

        internal void UpdateBehaviours()
        {
            bool cull = CullEnabled;
            if (_currentlyEnabled == cull)
                return;

            _currentlyEnabled = cull;

            for (int i = otherBehaviours.Count - 1; i >= 0; i--)
            {
                var b = otherBehaviours[i];
                if (b == null) { otherBehaviours.RemoveAt(i); continue; }
                b.enabled = cull;
            }

            for (int i = lights.Count - 1; i >= 0; i--)
            {
                var l = lights[i];
                if (l == null) { lights.RemoveAt(i); continue; }
                l.enabled = cull && !CullingManager.AllLightsDisabled;
            }

            for (int i = renderers.Count - 1; i >= 0; i--)
            {
                var r = renderers[i];
                if (r == null) { renderers.RemoveAt(i); continue; }
                r.enabled = cull;
            }

            for (int i = Decals.Count - 1; i >= 0; i--)
            {
                var d = Decals[i];
                if (d == null) { Decals.RemoveAt(i); continue; }
                d.enabled = cull;
            }
        }
    }
}