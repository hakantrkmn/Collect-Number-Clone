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
    public int amount;
    public void SetUI()
    {
        amountText.text = amount.ToString();
        color.color = info.color;
    }
    
}
