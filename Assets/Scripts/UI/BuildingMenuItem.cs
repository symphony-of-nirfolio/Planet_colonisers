using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuItem : MonoBehaviour
{
    public Button button;
    public Image image;
    public TMP_Text nameText;


    public void SetCallBack(Action action)
    {
        button.onClick.AddListener(() => action());
    }
}
