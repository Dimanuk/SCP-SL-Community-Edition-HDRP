using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering.PostProcessing;
#if PPS
using UnityEditor.Rendering.PostProcessing;
#endif

namespace SCPE
{
#if PPS
    public sealed class TransitionEditor : Editor {} }
#else
    [PostProcessEditor(typeof(Transition))]
    public sealed class TransitionEditor : PostProcessEffectEditor<Transition>
    {
        SerializedParameterOverride gradientTex;
        SerializedParameterOverride progress;

        public override void OnEnable()
        {
            gradientTex = FindParameterOverride(x => x.gradientTex);
            progress = FindParameterOverride(x => x.progress);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("transition");

            SCPE_GUI.DisplaySetupWarning<TransitionRenderer>();

            PropertyField(gradientTex);

            if (gradientTex.overrideState.boolValue && gradientTex.value.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign a gradient texture (pre-made textures can be found in the \"_Samples\" package", MessageType.Info);
            }

            PropertyField(progress);
        }
    }
}
#endif