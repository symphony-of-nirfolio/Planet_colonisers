using UnityEngine;

public class Menu : MonoBehaviour
{
    public enum MenuName : byte
    {
        MainMenu,
        None
    }


    public MenuName menuName = MenuName.None;

    [HideInInspector]
    public bool isOpen = false;


    public void Open()
    {
        gameObject.SetActive(true);
        isOpen = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        isOpen = false;
    }


    private void Awake()
    {
        isOpen = gameObject.activeInHierarchy;
    }
}
