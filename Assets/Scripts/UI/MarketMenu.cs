using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketMenu : MonoBehaviour
{
    public Market market;

    public GameResourceSpriteList gameResourceSpriteList;

    public TMP_Dropdown sellDropdown;
    public TMP_Dropdown buyDropdown;

    public TMP_Text availableValueText;

    public TMP_InputField sellValueInputField;
    public TMP_Text buyValueText;

    public Button tradeButton;

    public TMP_Text tradeStatusText;

    public Color availableColor = Color.white;
    public Color lockedColor = Color.red;


    private int currentSellItemIndex = 0;
    private int currentBuyItemIndex = 0;
    private float currentSellValue = 1f;
    private float currentBuyValue = 1f;

    private bool isMenuOpen = false;

    public void UpdateSellDropdown(int index)
    {
        currentSellItemIndex = index;

        UpdateUIValues();
    }

    public void UpdateBuyDropdown(int index)
    {
        currentBuyItemIndex = index;

        UpdateUIValues();
    }

    public void MultiplySellValue(int times)
    {
        currentSellValue *= times;

        sellValueInputField.text = Mathf.RoundToInt(currentSellValue).ToString();

        UpdateUIValues();
    }

    public void ResetSellValueToOne()
    {
        currentSellValue = 1f;

        sellValueInputField.text = "1";

        UpdateUIValues();
    }

    public void UpdateSellValueInputField(string text)
    {
        if (int.TryParse(text, out int sellValue))
        {
            currentSellValue = sellValue;
        }
        else
        {
            Debug.LogError("Cannot parse int value");
        }

        UpdateUIValues();
    }

    public void Trade()
    {
        // TODO: call trade from market
    }

    public void SetMenuToOpen()
    {
        isMenuOpen = true;
    }

    public void SetMenuToClose()
    {
        isMenuOpen = false;
    }


    private void UpdateUIValues()
    {
        float tradeRate = market.GetResourceTradeRate((GameResourceType) currentBuyItemIndex, (GameResourceType) currentSellItemIndex);
        currentBuyValue = currentSellValue * tradeRate;
        buyValueText.text = ((int) currentBuyValue).ToString();

        float availableValue = market.GetAvailableAmount((GameResourceType) currentBuyItemIndex);
        availableValueText.text = ((int) availableValue).ToString();

        if (currentBuyItemIndex == currentSellItemIndex)
        {
            tradeButton.interactable = false;
            tradeStatusText.text = "Sell item is equal to buy";
            tradeStatusText.color = lockedColor;
        }
        else if (currentBuyValue > availableValue)
        {
            tradeButton.interactable = false;
            tradeStatusText.text = "Too much resources";
            tradeStatusText.color = lockedColor;
        }
        else if (!market.IsTradeAvailable())
        {
            tradeButton.interactable = false;
            tradeStatusText.text = "Trade cool down";
            tradeStatusText.color = lockedColor;
        }
        else
        {
            tradeButton.interactable = true;
            tradeStatusText.text = "Trade available";
            tradeStatusText.color = availableColor;
        }
    }

    private void InitDropdown(TMP_Dropdown dropdown)
    {
        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (GameResourceSprite gameResourceSprite in gameResourceSpriteList.resourceSprites)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(gameResourceSprite.name, gameResourceSprite.sprite);
            options.Add(optionData);
        }

        dropdown.AddOptions(options);
        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }


    private void Start()
    {
        Utils.CheckFieldNotNullAndTryToSet(ref market, "Market");
        Utils.CheckFieldNotNull(gameResourceSpriteList, "Game Resource Sprite List");
        Utils.CheckFieldNotNull(sellDropdown, "Sell Dropdown");
        Utils.CheckFieldNotNull(buyDropdown, "Buy Dropdown");
        Utils.CheckFieldNotNull(availableValueText, "Available Value Text");
        Utils.CheckFieldNotNull(sellValueInputField, "Sell Value Input Field");
        Utils.CheckFieldNotNull(buyValueText, "Buy Value Text");
        Utils.CheckFieldNotNull(tradeButton, "Trade Button");
        Utils.CheckFieldNotNull(tradeStatusText, "Trade Status Text");

        InitDropdown(sellDropdown);
        InitDropdown(buyDropdown);
    }

    private void Update()
    {
        if (isMenuOpen)
        {
            UpdateUIValues();
        }
    }
}
