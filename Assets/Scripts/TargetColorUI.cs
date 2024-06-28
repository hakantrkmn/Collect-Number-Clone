using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetColorUI : MonoBehaviour
{
    public TextMeshProUGUI amountText;
    public Number info;
    public Image color;

    public void SetUI()
    {
        amountText.text = info.number.ToString();
        color.color = info.color;
    }
    
}
