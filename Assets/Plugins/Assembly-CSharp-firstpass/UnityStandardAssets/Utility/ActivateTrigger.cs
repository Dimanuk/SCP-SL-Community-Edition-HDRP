namespace UnityStandardAssets.Utility
{
	public class ActivateTrigger : global::UnityEngine.MonoBehaviour
	{
		public enum Mode
		{
			Trigger = 0,
			Replace = 1,
			Activate = 2,
			Enable = 3,
			Animate = 4,
			Deactivate = 5
		}

		public global::UnityStandardAssets.Utility.ActivateTrigger.Mode action = global::UnityStandardAssets.Utility.ActivateTrigger.Mode.Activate;

		public global::UnityEngine.Object target;

		public global::UnityEngine.GameObject source;

		public int triggerCount = 1;

		public bool repeatTrigger;

		private void DoActivateTrigger()
		{
			triggerCount--;
			if (triggerCount != 0 && !repeatTrigger)
			{
				return;
			}
			global::UnityEngine.Object obj = target ?? base.gameObject;
			global::UnityEngine.Behaviour behaviour = obj as global::UnityEngine.Behaviour;
			global::UnityEngine.GameObject gameObject = obj as global::UnityEngine.GameObject;
			if (behaviour != null)
			{
				gameObject = behaviour.gameObject;
			}
			switch (action)
			{
			case global::UnityStandardAssets.Utility.ActivateTrigger.Mode.Trigger:
				if (gameObject != null)
				{
					gameObject.BroadcastMessage("DoActivateTrigger");
				}
				break;
			case global::UnityStandardAssets.Utility.ActivateTrigger.Mode.Replace:
				if (source != null && gameObject != null)
				{
					global::UnityEngine.Object.Instantiate(source, gameObject.transform.position, gameObject.transform.rotation);
					global::UnityEngine.Object.DestroyObject(gameObject);
				}
				break;
			case global::UnityStandardAssets.Utility.ActivateTrigger.Mode.Activate:
				if (gameObject != null)
				{
					gameObject.SetActive(value: true);
				}
				break;
			case global::UnityStandardAssets.Utility.ActivateTrigger.Mode.Enable:
				if (behaviour != null)
				{
					behaviour.enabled = true;
				}
				break;
			case global::UnityStandardAssets.Utility.ActivateTrigger.Mode.Animate:
				if (gameObject != null)
				{
					gameObject.GetComponent<global::UnityEngine.Animation>().Play();
				}
				break;
			case global::UnityStandardAssets.Utility.ActivateTrigger.Mode.Deactivate:
				if (gameObject != null)
				{
					gameObject.SetActive(value: false);
				}
				break;
			}
		}

		private void OnTriggerEnter(global::UnityEngine.Collider other)
		{
			DoActivateTrigger();
		}
	}
}
