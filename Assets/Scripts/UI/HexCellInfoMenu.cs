using System.Collections;
using TMPro;
using UnityEngine;

public class HexCellInfoMenu : MonoBehaviour
{
    public RectTransform panelRectTransform;
    public TMP_Text typeValueText;
    public TMP_Text extraValueText;
    public TMP_Text amountText;
    public TMP_Text amountValueText;


    private Coroutine resourceAmountUpdater;

    public void Show(WorldGenerator.HexCellInfo hexCellInfo)
    {
        panelRectTransform.gameObject.SetActive(true);

        typeValueText.text = (hexCellInfo.hexType & ~WorldGenerator.HexType.Building).ToString();
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

        if ((hexCellInfo.hexType & WorldGenerator.HexType.AllResources) != WorldGenerator.HexType.None)
        {
            amountText.gameObject.SetActive(true);
            amountValueText.gameObject.SetActive(true);

            amountValueText.text = Mathf.Round(hexCellInfo.resourceDeposit.AvailableResource()).ToString();
            StartCoroutine(ResourceAmountUpdate(hexCellInfo.resourceDeposit));
        }
        else
        {
            amountText.gameObject.SetActive(false);
            amountValueText.gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        if (resourceAmountUpdater != null)
        {
            StopCoroutine(resourceAmountUpdater);
            resourceAmountUpdater = null;
        }
        panelRectTransform.gameObject.SetActive(false);
    }


    private IEnumerator ResourceAmountUpdate(ResourceDeposit resourceDeposit)
    {
        while (true)
        {
            amountValueText.text = Mathf.Round(resourceDeposit.AvailableResource()).ToString();
            yield return new WaitForFixedUpdate();
        }
    }


    private void Start()
    {
        Debug.Assert(panelRectTransform, "Panel Rect Transform doesn't set");
        Debug.Assert(typeValueText, "Type Value Text doesn't set");
        Debug.Assert(extraValueText, "Extra Value Text doesn't set");
        Debug.Assert(amountText, "Amount Text doesn't set");
        Debug.Assert(amountValueText, "Amount Value Text doesn't set");
    }
}
