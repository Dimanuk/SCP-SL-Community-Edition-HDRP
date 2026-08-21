namespace IESLights.Demo
{
    public class ToggleIES : global::UnityEngine.MonoBehaviour
    {
        private global::System.Collections.Generic.Dictionary<global::UnityEngine.Light, global::UnityEngine.Texture> _spotsToCookies = new global::System.Collections.Generic.Dictionary<global::UnityEngine.Light, global::UnityEngine.Texture>();

        private void Start()
        {
            global::UnityEngine.Light[] componentsInChildren = GetComponentsInChildren<global::UnityEngine.Light>();
            foreach (global::UnityEngine.Light light in componentsInChildren)
            {
                _spotsToCookies.Add(light, light.cookie);
            }
        }

        private void Update()
        {
            if (!global::UnityEngine.Input.GetKeyDown(global::UnityEngine.KeyCode.Space))
            {
                return;
            }
            foreach (global::UnityEngine.Light key in _spotsToCookies.Keys)
            {
                if (key.cookie == null)
                {
                    key.cookie = _spotsToCookies[key];
                    key.intensity = 0.7f;
                }
                else
                {
                    key.cookie = null;
                    key.intensity = 0.4f;
                }
            }
        }
    }
}
