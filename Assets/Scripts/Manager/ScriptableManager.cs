using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableManager : MonoBehaviour
{
    public PuzzleSettings puzzleSettings;
    private void OnEnable()
    {
        EventManager.GetPuzzleSettings += () => puzzleSettings;
    }
}
