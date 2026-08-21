using UnityEngine;

public class ToggleableLight : MonoBehaviour
{
    public GameObject[] allLights;
    public bool isAlarm;

    public void SetLights(bool b)
    {
        for (int i = 0; i < allLights.Length; i++)
        {
            allLights[i].SetActive(isAlarm ? b : !b);
        }
    }
}