using System;
using UnityEngine;

namespace Tooltips
{
	[Serializable]
	public struct TooltipData
	{
		public GameObject Key;

		[Multiline]
		public string Text;
	}
}
