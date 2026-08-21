using System.Collections.Generic;
using UnityEngine;
using GameCore;
using MapGeneration;

public class SeasonalMaterialReplacer : MonoBehaviour
{
    public List<SeasonalMaterialStruct> replacers = new List<SeasonalMaterialStruct>();

    private void Start()
    {
        SeedSynchronizer.OnMapGenerated += Festivize;
    }

    private void OnDestroy()
    {
        SeedSynchronizer.OnMapGenerated -= Festivize;
    }

    public void Festivize()
    {
        if (!TryGetComponent<Renderer>(out var renderer))
        {
            Console.AddDebugLog("MGCLTR", "Detected no renderer; skipping", MessageImportance.LessImportant, true);
            return;
        }

        Console.AddDebugLog("MGCLTR", $"Checking material replacers on object: \"{gameObject.name}\"", MessageImportance.LeastImportant, true);
        Console.AddDebugLog("MGCLTR", " Searching structs...", MessageImportance.LeastImportant, true);

        Material[] materials = renderer.materials;
        bool anyChange = false;

        foreach (var replacer in replacers)
        {
            if (!ClutterSpawner.IsHolidayActive(replacer.condition))
                continue;

            Console.AddDebugLog("MGCLTR", $"  Searching for material: \"{replacer.editorDescriptor}\"", MessageImportance.LeastImportant, true);

            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i];
                if (mat == null) continue;

                bool match = false;
                if (replacer.initialMaterial != null)
                {
                    foreach (var initial in replacer.initialMaterial)
                    {
                        if (mat == initial)
                        {
                            match = true;
                            break;
                        }
                    }
                }

                if (match)
                {
                    Console.AddDebugLog("MGCLTR", $"  Found material of name: \"{mat.name}\"", MessageImportance.LeastImportant, true);
                    Console.AddDebugLog("MGCLTR", $" Firing material replacer: \"{replacer.editorDescriptor}\"", MessageImportance.LessImportant, true);
                    materials[i] = replacer.replaceMaterial;
                    anyChange = true;
                }
                else
                {
                    Console.AddDebugLog("MGCLTR", "   Material does not match", MessageImportance.LeastImportant, true);
                }
            }
        }

        if (anyChange)
            renderer.materials = materials;
    }
}