using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSelectButton : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    public void OnChangeValue()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (dropdown == null)
            return;

        string[] menuScenes = SimpleMenu.MenuSceneNames;
        int selectedIndex = dropdown.value;

        if (selectedIndex >= menuScenes.Length)
            return;

        string selectedScene = menuScenes[selectedIndex];

        if (!string.Equals(currentScene, selectedScene))
        {
            SimpleMenu.ChangeMode(selectedIndex);
        }
    }

    private void Start()
    {
        if (dropdown == null)
            return;

        int savedMode = PlayerPrefsSl.Get("menumode", 1);
        dropdown.value = savedMode;
    }
}