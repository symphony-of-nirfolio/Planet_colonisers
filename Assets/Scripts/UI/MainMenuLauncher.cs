using UnityEngine;
using UnityEngine.UI;

public class MainMenuLauncher : MonoBehaviour
{
    public MenuManager menuManager;


    public void Quit()
    {
        Application.Quit();
    }


    private void Start()
    {
        menuManager.OpenMenu(Menu.MenuName.Main);
    }
}
