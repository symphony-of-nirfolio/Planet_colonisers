using UnityEngine;

public class GameMenuLauncher : MonoBehaviour
{
    public MenuManager menuManager;


    private void Start()
    {
        menuManager.CloseAllMenus();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            menuManager.OpenMenu(Menu.MenuName.BuildingMenu);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuManager.IsOpenMenu(Menu.MenuName.BuildingMenu))
            {
                menuManager.CloseMenu(Menu.MenuName.BuildingMenu);
            }
        }
    }
}
