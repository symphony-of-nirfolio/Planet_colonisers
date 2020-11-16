using UnityEngine;

public class MainMenuLauncher : MonoBehaviour
{
    public MenuManager menuManager;


    public void Quit()
    {
        Application.Quit();
    }


    private void Start()
    {
        Debug.Assert(menuManager, "Menu Manager doesn't set");

        menuManager.OpenMenu(Menu.MenuName.Main);
    }
}
