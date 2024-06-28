using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class PuzzleSettings : ScriptableObject
{
    public List<Number> numbers;
    public int width;
    public int height;
    public float spacing;
    [HideInInspector]public List<int> possibleNumbers;
    public GameObject cellPrefab;
    public List<FixedValue> matrixValues;

    

[Button]
    public void CreateGrid()
    {
        possibleNumbers.Clear();
        foreach (var numb in numbers)
        {
            possibleNumbers.Add(numb.number);
        }
        EventManager.SetPuzzle(this);
    }


}
