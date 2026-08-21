using System;
using UnityEngine;

[Serializable]
public struct SeasonalMaterialStruct
{
	public string editorDescriptor;

	public Material[] initialMaterial;

	public Material replaceMaterial;

	public Holidays condition;
}
