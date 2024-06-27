using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour , IPointerClickHandler
{
    public TextMeshProUGUI numberText;

    public int number;
    public Image background;
    
    public Vector2 cellIndex;
    public void OnPointerClick(PointerEventData eventData)
    {
        number++;
        numberText.text = number.ToString();
        EventManager.CellClicked?.Invoke(this);
    }

    public void UpdateText()
    {
        numberText.text = number.ToString();
    }

    public Tween Animation()
    {
        return background.DOColor(Color.red, .5f).SetLoops(2, LoopType.Yoyo);
    }
}
