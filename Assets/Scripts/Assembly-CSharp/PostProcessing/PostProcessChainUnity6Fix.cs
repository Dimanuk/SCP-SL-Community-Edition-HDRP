using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

namespace PostProcessing
{
    public static class PostProcessChainUnity6Fix
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            SceneManager.sceneLoaded += (_, __) => Apply();
            Apply();
        }

        public static void Apply()
        {
            var character = GameObject.Find("CharacterCamera");
            var viewmodel = GameObject.Find("ViewmodelCamera");
            var first = GameObject.Find("FirstProcessCam");
            var final = GameObject.Find("FinalProcessCam");
            var effect = GameObject.Find("PlayerEffectCam");

            if (character == null || viewmodel == null || first == null || final == null || effect == null)
                return;

            var charLayer = character.GetComponent<PostProcessLayer>();
            var vmLayer = viewmodel.GetComponent<PostProcessLayer>();
            var firstLayer = first.GetComponent<PostProcessLayer>();
            var finalLayer = final.GetComponent<PostProcessLayer>();
            var effectLayer = effect.GetComponent<PostProcessLayer>();

            if (charLayer == null || vmLayer == null)
                return;

            if (firstLayer != null)
                charLayer.volumeLayer |= firstLayer.volumeLayer;

            if (finalLayer != null)
                vmLayer.volumeLayer |= finalLayer.volumeLayer;

            if (effectLayer != null)
            {
                // PlayerEffectCam carries the full-screen brightness grade (BrightnessPostProcessVolume,
                // layer 23). It must be composited by the world camera (CharacterCamera: SolidColor clear,
                // depth 0, full-scene cullingMask), NOT the viewmodel overlay camera (Depth-only clear,
                // viewmodel-only cullingMask) — an overlay pass under Unity 6's built-in RP does not
                // reliably grade the whole visible world, which is why the brightness slider looked dead.
                charLayer.volumeLayer |= effectLayer.volumeLayer;

                charLayer.antialiasingMode = effectLayer.antialiasingMode;
            }

            DisableCamera(first);
            DisableCamera(final);
            DisableCamera(effect);

            Debug.Log("[PPChainFix] PP-слои пустых камер слиты: CharacterCamera mask="
                      + charLayer.volumeLayer.value + ", ViewmodelCamera mask=" + vmLayer.volumeLayer.value);
        }

        private static void DisableCamera(GameObject go)
        {
            var cam = go.GetComponent<Camera>();
            if (cam != null)
                cam.enabled = false;

            var layer = go.GetComponent<PostProcessLayer>();
            if (layer != null)
                layer.enabled = false;
        }

        // The 079 HUD stacks six cameras by depth: CharacterCamera(0) -> FirstProcessCam(1)
        // -> "Cam Overcon"/"Cam HUD"(3, draws the on-screen frame/level/text, no PP of its own)
        // -> FinalProcessCam(6) -> PlayerEffectCam(10). FinalProcessCam/PlayerEffectCam run AFTER
        // the UI cameras, which is how the shipped game grades grain/vignette/nightvision over the
        // HUD text too. Collapsing everything onto CharacterCamera (which runs BEFORE the UI) would
        // strip post-processing off the HUD entirely, so we fold FirstProcessCam forward into
        // CharacterCamera (both pre-UI) but fold FinalProcessCam backward into PlayerEffectCam
        // (both post-UI) and keep PlayerEffectCam alive as that deferred pass — mirroring how
        // Apply() above keeps two active passes (CharacterCamera + ViewmodelCamera) instead of one.
        public static void ApplyToScp079Hud(Transform hudRoot)
        {
            if (hudRoot == null)
                return;

            PostProcessLayer character = null;
            PostProcessLayer first = null;
            PostProcessLayer final = null;
            PostProcessLayer effect = null;

            foreach (var layer in hudRoot.GetComponentsInChildren<PostProcessLayer>(true))
            {
                switch (layer.gameObject.name)
                {
                    case "CharacterCamera": character = layer; break;
                    case "FirstProcessCam": first = layer; break;
                    case "FinalProcessCam": final = layer; break;
                    case "PlayerEffectCam": effect = layer; break;
                }
            }

            if (character == null)
                return;

            if (first != null)
                character.volumeLayer |= first.volumeLayer;

            if (effect != null && final != null)
                effect.volumeLayer |= final.volumeLayer;
            else if (final != null && effect == null)
                character.volumeLayer |= final.volumeLayer;

            if (first != null)
                DisableCamera(first.gameObject);
            if (final != null)
                DisableCamera(final.gameObject);

            Debug.Log("[PPChainFix] SCP-079 HUD PP-цепочка: CharacterCamera(pre-UI) mask="
                      + character.volumeLayer.value
                      + (effect != null ? ", PlayerEffectCam(post-UI) mask=" + effect.volumeLayer.value : string.Empty));
        }
    }
}
