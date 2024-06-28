using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetUI : MonoBehaviour
{
    public GameObject prefab;
    public List<TargetColorUI> targetColors;


    private void Start()
    {
        var targets = GameManager.Instance.targets;
        var data = EventManager.GetPuzzleSettings();

        for (int i = 0; i < targets.Count; i++)
        {
            var target = Instantiate(prefab, transform).GetComponent<TargetColorUI>();
            targetColors.Add(target);
            target.info = data.numbers.First(x => x.number == targets[i].number);
            target.SetUI();
        }
    }

    private void OnEnable()
    {
        EventManager.UpdateTargetUI += UpdateTargetUI;
    }

    private void OnDisable()
    {
        EventManager.UpdateTargetUI -= UpdateTargetUI;
    }

    void UpdateTargetUI()
    {
        var targets = GameManager.Instance.targets;

        for (int i = 0; i < targetColors.Count; i++)
        {
            if (targets[i].number == targetColors[i].info.number)
            {
                targetColors[i].amountText.text = targets[i].amount.ToString();
            }
        }
    }
}