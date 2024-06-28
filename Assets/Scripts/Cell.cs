using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI numberText;
    public Image background;


    [HideInInspector] public int number;
    [HideInInspector] public Vector2 cellIndex;

    private void Start()
    {
        UpdateText();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.gameState != GameStates.Idle) return;


        Animation();
        number++;
        UpdateText();
        EventManager.CellClicked?.Invoke(this);
    }

    public void UpdateText()
    {
        numberText.text = number.ToString();
        if (Application.isPlaying)
        {
            if (EventManager.GetPuzzleSettings().numbers.Exists(x=> x.number == number))
            {
                background.color = EventManager.GetPuzzleSettings().numbers.First(x => x.number == number).color;
            }
        }
    }

    public Tween Animation()
    {
        DOTween.Complete(this);
        return background.transform.DOScale(1.3f, .2f).SetLoops(2, LoopType.Yoyo).SetId(this);
    }
}