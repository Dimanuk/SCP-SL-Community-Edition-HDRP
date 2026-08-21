namespace UnityStandardAssets.Utility
{
	public class AutoMobileShaderSwitch : global::UnityEngine.MonoBehaviour
	{
		[global::System.Serializable]
		public class ReplacementDefinition
		{
			public global::UnityEngine.Shader original;

			public global::UnityEngine.Shader replacement;
		}

		[global::System.Serializable]
		public class ReplacementList
		{
			public global::UnityStandardAssets.Utility.AutoMobileShaderSwitch.ReplacementDefinition[] items = new global::UnityStandardAssets.Utility.AutoMobileShaderSwitch.ReplacementDefinition[0];
		}

		[global::UnityEngine.SerializeField]
		private global::UnityStandardAssets.Utility.AutoMobileShaderSwitch.ReplacementList m_ReplacementList;

		private void OnEnable()
		{
		}
	}
}
