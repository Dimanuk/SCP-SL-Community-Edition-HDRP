using System;
using UnityEngine;

namespace InventorySystem.Items.Radio
{
	[Serializable]
	public struct RadioRangeMode
	{
		public string ShortName;

		public string FullName;

		public Texture SignalTexture;

		public float MinuteCostWhenIdle;

		public int MinuteCostWhenTalking;

		public int MaximumRange;
	}
}
