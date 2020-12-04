using TMPro;
using UnityEngine;

public class HexCellInfoMenu : MonoBehaviour
{
    public RectTransform panelRectTransform;
    public TMP_Text typeValueText;
    public TMP_Text extraValueText;


    public void Show(WorldGenerator.HexCellInfo hexCellInfo)
    {
        panelRectTransform.gameObject.SetActive(true);

        typeValueText.text = hexCellInfo.hexType.ToString();
        extraValueText.text = hexCellInfo.shortDescription;

        Vector3 mousePosition = Input.mousePosition;
        Vector3 panelPosition = mousePosition;

        Vector2 pivot = panelRectTransform.pivot;
        if (panelPosition.x < Screen.width / 2f)
        {
            if (panelPosition.x < panelRectTransform.rect.width)
            {
                pivot.x = 0f;
            }
            else
            {
                pivot.x = 1f;
            }
        }
        else
        {
            if (panelPosition.x + panelRectTransform.rect.width > Screen.width)
            {
                pivot.x = 1f;
            }
            else
            {
                pivot.x = 0f;
            }
        }

        if (panelPosition.y + panelRectTransform.rect.height > Screen.height)
        {
            pivot.y = 1f;
        }
        else
        {
            pivot.y = 0f;
        }

        panelRectTransform.pivot = pivot;
        panelRectTransform.position = panelPosition;
    }

    public void Hide()
    {
        panelRectTransform.gameObject.SetActive(false);
    }


    private void Start()
    {
        Debug.Assert(panelRectTransform, "Panel Rect Transform doesn't set");
        Debug.Assert(typeValueText, "Type Value Text doesn't set");
        Debug.Assert(extraValueText, "Extra Value Text doesn't set");
    }
}
