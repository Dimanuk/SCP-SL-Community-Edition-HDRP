namespace UnityStandardAssets.Utility
{
	public class TimedObjectActivator : global::UnityEngine.MonoBehaviour
	{
		public enum Action
		{
			Activate = 0,
			Deactivate = 1,
			Destroy = 2,
			ReloadLevel = 3,
			Call = 4
		}

		[global::System.Serializable]
		public class Entry
		{
			public global::UnityEngine.GameObject target;

			public global::UnityStandardAssets.Utility.TimedObjectActivator.Action action;

			public float delay;
		}

		[global::System.Serializable]
		public class Entries
		{
			public global::UnityStandardAssets.Utility.TimedObjectActivator.Entry[] entries;
		}

		public global::UnityStandardAssets.Utility.TimedObjectActivator.Entries entries = new global::UnityStandardAssets.Utility.TimedObjectActivator.Entries();

		private void Awake()
		{
			global::UnityStandardAssets.Utility.TimedObjectActivator.Entry[] array = entries.entries;
			foreach (global::UnityStandardAssets.Utility.TimedObjectActivator.Entry entry in array)
			{
				switch (entry.action)
				{
				case global::UnityStandardAssets.Utility.TimedObjectActivator.Action.Activate:
					StartCoroutine(Activate(entry));
					break;
				case global::UnityStandardAssets.Utility.TimedObjectActivator.Action.Deactivate:
					StartCoroutine(Deactivate(entry));
					break;
				case global::UnityStandardAssets.Utility.TimedObjectActivator.Action.Destroy:
					global::UnityEngine.Object.Destroy(entry.target, entry.delay);
					break;
				case global::UnityStandardAssets.Utility.TimedObjectActivator.Action.ReloadLevel:
					StartCoroutine(ReloadLevel(entry));
					break;
				}
			}
		}

		private global::System.Collections.IEnumerator Activate(global::UnityStandardAssets.Utility.TimedObjectActivator.Entry entry)
		{
			yield return new global::UnityEngine.WaitForSeconds(entry.delay);
			entry.target.SetActive(value: true);
		}

		private global::System.Collections.IEnumerator Deactivate(global::UnityStandardAssets.Utility.TimedObjectActivator.Entry entry)
		{
			yield return new global::UnityEngine.WaitForSeconds(entry.delay);
			entry.target.SetActive(value: false);
		}

		private global::System.Collections.IEnumerator ReloadLevel(global::UnityStandardAssets.Utility.TimedObjectActivator.Entry entry)
		{
			yield return new global::UnityEngine.WaitForSeconds(entry.delay);
			global::UnityEngine.SceneManagement.SceneManager.LoadScene(global::UnityEngine.SceneManagement.SceneManager.GetSceneAt(0).name);
		}
	}
}
