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
            else if (menu.menuName != menuName && menu.isOpen)
            {
                menu.Close();
            }
        }
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void CloseMenu(Menu.MenuName menuName)
    {
        foreach (Menu menu in menus)
        {
            if (menu.menuName == menuName && menu.isOpen)
            {
                menu.Close();
            }
        }
    }

    public void CloseAllMenus()
    {
        foreach (Menu menu in menus)
        {
            menu.Close();
        }
    }

    public bool IsOpenMenu(Menu menu)
    {
        return menu.isOpen;
    }

    public bool IsOpenMenu(Menu.MenuName menuName)
    {
        foreach (Menu menu in menus)
        {
            if (menu.menuName == menuName)
            {
                return menu.isOpen;
            }
        }
        return false;
    }


    private void Start()
    {
        bool[] used_menus = new bool[(int) Menu.MenuName.CountOf];

        foreach (Menu menu in menus)
        {
            int menuNameId = (int) menu.menuName;

            Debug.Assert(!used_menus[menuNameId], "Menu is already use");
            used_menus[menuNameId] = true;
        }
    }
}
