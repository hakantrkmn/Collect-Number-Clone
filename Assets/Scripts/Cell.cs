using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI numberText;
    public Image background;


     public int number;
     public Vector2 cellIndex;

    private void Start()
    {
        UpdateText();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.gameState != GameStates.Idle) return;


        ClickAnimation();
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

    public void MoveUp(float yPoint)
    {
        transform.position = new Vector3(transform.position.x, yPoint, transform.position.z);
        number = EventManager.GetPuzzleSettings().possibleNumbers[Random.Range(0, EventManager.GetPuzzleSettings().possibleNumbers.Count)];
        UpdateText();
    }
    public Tween DropAnimation(Vector3 targetPos)
    {
        return transform.DOMove(targetPos,
            Vector3.Distance(targetPos, transform.position) / 800);
    }
    public Tween ClickAnimation()
    {
        DOTween.Complete(this);
        return background.transform.DOScale(1.3f, .2f).SetLoops(2, LoopType.Yoyo).SetId(this);
    }
}