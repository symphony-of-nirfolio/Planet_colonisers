using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Menu[] menus;


    public void OpenMenu(Menu menu)
    {
        OpenMenu(menu.menuName);
    }

    public void OpenMenu(Menu.MenuName menuName)
    {
        foreach (Menu menu in menus)
        {
            if (menu.menuName == menuName && !menu.isOpen)
            {
                menu.Open();
            }
            else if (menu.menuName != menuName &&  menu.isOpen)
            {
                menu.Close();
            }
        }
    }
}
