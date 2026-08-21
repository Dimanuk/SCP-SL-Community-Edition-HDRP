
using CursorManagement;
using UnityEngine;

public class DebugLogScreen : MonoBehaviour
{
	public GameObject Log;

	public GameObject Info;

	private void OnEnable()
	{
		Info.SetActive(true);
		CursorManager.SetLockMode(Log.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked);
	}

	private void OnDisable()
	{
		Info.SetActive(false);
		CursorManager.SetLockMode(CursorLockMode.Confined);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F4) && DebugLogReader.SuccesfullyInitialized())
		{
			Log.SetActive(!Log.activeSelf);
			CursorManager.SetLockMode(Log.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked);
		}
	}
}
