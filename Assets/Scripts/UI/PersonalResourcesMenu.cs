using TMPro;
using UnityEngine;

public class PersonalResourcesMenu : MonoBehaviour
{
    public TMP_Text[] resourceCountTexts;

    public int letterAmountTillCropping = 5;

    public float thresholdFroBillionth = 900_000_000f;
    public float thresholdFroMillionth = 900_000f;
    public float thresholdFroThousandth = 900f;


    public void UpdateResources(GameResourcesStorage storage)
    {
        for (int i = 0; i < GameResourcesStorage.resourceCount; ++i)
        {
            float resourceAmount = storage.getResourceAmount((GameResourceType) i);

            resourceCountTexts[i].text = ValueToText(resourceAmount);
        }
    }


    private string ValueToText(float value)
    {
        string suffix = "";
        if (value > thresholdFroBillionth)
        {
            value /= 1000_000_000f;
            suffix = " B";
        }
        else if (value > thresholdFroMillionth)
        {
            value /= 1000_000f;
            suffix = " M";
        }
        else if (value > thresholdFroThousandth)
        {
            value /= 1000f;
            suffix = " k";
        }

        string valueText = value.ToString();
        if (valueText.Length + suffix.Length > letterAmountTillCropping)
        {
            valueText = valueText.Substring(0, letterAmountTillCropping - suffix.Length);
            if (valueText[valueText.Length - 1] == '.')
            {
                valueText = valueText.Substring(0, valueText.Length - 1);
            }
        }

        return valueText;
    }
}
