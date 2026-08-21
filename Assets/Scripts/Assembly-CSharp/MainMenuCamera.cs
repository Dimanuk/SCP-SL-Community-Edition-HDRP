public class MainMenuCamera : global::UnityEngine.MonoBehaviour
{
    public float borderWidthPercent;

    private float rotSpeed;

    private void Update()
    {
        float num = (float)global::UnityEngine.Screen.width * (borderWidthPercent / 100f);
        global::UnityEngine.Vector3 zero = global::UnityEngine.Vector3.zero;
        global::UnityEngine.Vector3 mousePosition = global::UnityEngine.Input.mousePosition;
        if (mousePosition.x < num && base.transform.localRotation.eulerAngles.y > 41f)
        {
            zero += global::UnityEngine.Vector3.down;
        }
        if (mousePosition.x > (float)global::UnityEngine.Screen.width - num && base.transform.localRotation.eulerAngles.y < 74f)
        {
            zero += global::UnityEngine.Vector3.up;
        }
        if (zero == global::UnityEngine.Vector3.zero)
        {
            rotSpeed = 0f;
        }
        else
        {
            rotSpeed += global::UnityEngine.Time.deltaTime * 200f;
            rotSpeed = global::UnityEngine.Mathf.Clamp(rotSpeed, 0f, 120f);
        }
        zero.Normalize();
        base.transform.localRotation = global::UnityEngine.Quaternion.Euler(base.transform.localRotation.eulerAngles + global::UnityEngine.Time.deltaTime * rotSpeed * zero);
        if (global::UnityEngine.Input.GetKeyDown(global::UnityEngine.KeyCode.Mouse0))
        {
            Raycast();
        }
    }

    private void Raycast()
    {
        if (global::UnityEngine.Physics.Raycast(GetComponent<global::UnityEngine.Camera>().ScreenPointToRay(global::UnityEngine.Input.mousePosition), out var hitInfo))
        {
            ElementChoosen(hitInfo.transform.name);
        }
    }

    public void ElementChoosen(string id)
    {
        if (!(id == "EXIT"))
        {
            if (id == "PLAY")
            {
                global::UnityEngine.Object.FindFirstObjectByType<NetManagerValueSetter>().HostGame();
            }
        }
        else
        {
            global::UnityEngine.Debug.Log("Application closed by the user.");
            Shutdown.Quit();
        }
    }
}
