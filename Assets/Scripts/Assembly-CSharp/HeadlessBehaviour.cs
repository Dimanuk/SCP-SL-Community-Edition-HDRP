using UnityEngine;

public class HeadlessBehaviour : MonoBehaviour
{
	public void NullifyCamera(Camera camera)
	{
		int num = 0;
		camera.enabled = (byte)num != 0;
	}
}
