using UnityEngine;
using UnityEngine.UI;

public class MainMenuLauncher : MonoBehaviour
{
    public MenuManager menuManager;


    public Button temp;

    private void Start()
    {
        menuManager.OpenMenu(Menu.MenuName.MainMenu);

        temp.onClick.AddListener(() => Debug.Log("Click"));
    }
}
