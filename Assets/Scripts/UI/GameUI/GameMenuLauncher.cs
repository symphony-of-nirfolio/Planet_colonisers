using UnityEngine;

public class GameMenuLauncher : MonoBehaviour
{
    public Freezer freezer;

    public MenuManager menuManager;


    private void Start()
    {
        Debug.Assert(menuManager, "Menu Manager doesn't set");
        Debug.Assert(freezer, "Freezer doesn't set");

        menuManager.CloseAllMenus();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!freezer.IsInteractionFreeze)
            {
                menuManager.OpenMenu(Menu.MenuName.BuildingMenu);
                freezer.InteractionFreeze();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuManager.IsOpenMenu(Menu.MenuName.BuildingMenu))
            {
                if (!freezer.IsFullFreeze)
                {
                    menuManager.CloseMenu(Menu.MenuName.BuildingMenu);
                    freezer.InteractionUnfreeze();
                }
            }
        }
    }
}
