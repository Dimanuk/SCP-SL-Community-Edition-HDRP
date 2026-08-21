namespace UnityStandardAssets.Utility
{
	public class PlatformSpecificContent : global::UnityEngine.MonoBehaviour
	{
		private enum BuildTargetGroup
		{
			Standalone = 0,
			Mobile = 1
		}

		[global::UnityEngine.SerializeField]
		private global::UnityStandardAssets.Utility.PlatformSpecificContent.BuildTargetGroup m_BuildTargetGroup;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.GameObject[] m_Content = new global::UnityEngine.GameObject[0];

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.MonoBehaviour[] m_MonoBehaviours = new global::UnityEngine.MonoBehaviour[0];

		[global::UnityEngine.SerializeField]
		private bool m_ChildrenOfThisObject;

		private void OnEnable()
		{
			CheckEnableContent();
		}

		private void CheckEnableContent()
		{
			EnableContent(m_BuildTargetGroup != global::UnityStandardAssets.Utility.PlatformSpecificContent.BuildTargetGroup.Mobile);
		}

		private void EnableContent(bool enabled)
		{
			if (m_Content.Length != 0)
			{
				global::UnityEngine.GameObject[] content = m_Content;
				foreach (global::UnityEngine.GameObject gameObject in content)
				{
					if (gameObject != null)
					{
						gameObject.SetActive(enabled);
					}
				}
			}
			if (m_ChildrenOfThisObject)
			{
				foreach (global::UnityEngine.Transform item in base.transform)
				{
					item.gameObject.SetActive(enabled);
				}
			}
			if (m_MonoBehaviours.Length != 0)
			{
				global::UnityEngine.MonoBehaviour[] monoBehaviours = m_MonoBehaviours;
				for (int i = 0; i < monoBehaviours.Length; i++)
				{
					monoBehaviours[i].enabled = enabled;
				}
			}
		}
	}
}
