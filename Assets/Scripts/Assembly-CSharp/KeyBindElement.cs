using UnityEngine;

public class KeyBindElement : MonoBehaviour
{
	public ActionName Action;
	public void Click()
	{
		ChangeKeyBinding parent = GetComponentInParent<ChangeKeyBinding>();
		if (parent != null)
		{
			parent.ChangeKey(Action);
		}
	}
}