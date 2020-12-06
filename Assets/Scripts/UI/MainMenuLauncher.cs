using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLauncher : MonoBehaviour
{
    public MenuManager menuManager;


    public void Quit()
    {
        Application.Quit();
    }

    public void StartGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }


    private void Start()
    {
        Utils.CheckFieldNotNullAndTryToSet(ref menuManager, "Menu Manager");

        menuManager.OpenMenu(Menu.MenuName.Main);
    }
}
