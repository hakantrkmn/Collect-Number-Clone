using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour , IPointerClickHandler
{
    public TextMeshProUGUI numberText;

    public int number;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        number++;
        numberText.text = number.ToString();
        EventManager.CellClicked?.Invoke(this);
    }
}
