using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuLauncher : MonoBehaviour
{
    public Freezer freezer;

    public MenuManager menuManager;

    public Builder builder;


    public void ExitToMainMenu()
    {
        freezer.FullUnfreeze();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ResumeGame()
    {
        menuManager.CloseMenu(Menu.MenuName.Pause);
        freezer.FullUnfreeze();
    }


    private void Start()
    {
        Debug.Assert(freezer, "Freezer doesn't set");
        Debug.Assert(menuManager, "Menu Manager doesn't set");
        Debug.Assert(builder, "Builder doesn't set");

        menuManager.CloseAllMenus();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!freezer.IsInteractionFreeze)
            {
                menuManager.OpenMenu(Menu.MenuName.Building);
                freezer.InteractionFreeze();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuManager.IsOpenMenu(Menu.MenuName.Building))
            {
                if (!freezer.IsFullFreeze)
                {
                    menuManager.CloseMenu(Menu.MenuName.Building);
                    freezer.InteractionUnfreeze();
                }
            }
            else if (menuManager.IsOpenMenu(Menu.MenuName.Settings))
            {
                menuManager.OpenMenu(Menu.MenuName.Pause);
            }
            else if (menuManager.IsOpenMenu(Menu.MenuName.Pause))
            {
                ResumeGame();
            }
            else if (builder.IsActiveBuilder())
            {
                builder.DeactivateBuilder();
            }
            else
            {
                menuManager.OpenMenu(Menu.MenuName.Pause);
                freezer.FullFreeze();
            }
        }
    }
}
