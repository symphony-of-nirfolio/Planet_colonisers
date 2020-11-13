using TMPro;
using UnityEngine;

public class PersonalResourcesMenu : MonoBehaviour
{
    // TODO: replace with original
    // Temporary class to functional check
    public class GameResources
    {
        public int resource1 = 0;
        public int resource2 = 0;
        public int resource3 = 0;
        public int resource4 = 0;

        public int resource5 = 0;
        public int resource6 = 0;
        public int resource7 = 0;
        public int resource8 = 0;
        
        public int resource9 = 0;
        public int resource10 = 0;
        public int resource11 = 0;
        public int resource12 = 0;
    }

    public TMP_Text resource1;
    public TMP_Text resource2;
    public TMP_Text resource3;
    public TMP_Text resource4;

    public TMP_Text resource5;
    public TMP_Text resource6;
    public TMP_Text resource7;
    public TMP_Text resource8;
    
    public TMP_Text resource9;
    public TMP_Text resource10;
    public TMP_Text resource11;
    public TMP_Text resource12;


    public void UpdateResources(GameResources gameResources)
    {
        resource1.text = gameResources.resource1.ToString();
        resource2.text = gameResources.resource2.ToString();
        resource3.text = gameResources.resource3.ToString();
        resource4.text = gameResources.resource4.ToString();

        resource5.text = gameResources.resource5.ToString();
        resource6.text = gameResources.resource6.ToString();
        resource7.text = gameResources.resource7.ToString();
        resource8.text = gameResources.resource8.ToString();
        
        resource9.text = gameResources.resource9.ToString();
        resource10.text = gameResources.resource10.ToString();
        resource11.text = gameResources.resource11.ToString();
        resource12.text = gameResources.resource12.ToString();
    }

    // TODO: remove it
    // Temporary variable to functional check
    private GameResources gameResources = new GameResources();

    // TODO: remove it
    // Temporary function to functional check
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            int index = Random.Range(0, 3);

            if (index == 0)
            {
                ++gameResources.resource3;
            }
            else if (index == 1)
            {
                ++gameResources.resource6;
            }
            else if (index == 2)
            {
                ++gameResources.resource10;
            }

            UpdateResources(gameResources);
        }
    }
}
