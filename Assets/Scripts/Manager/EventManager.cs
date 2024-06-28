using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager 
{
    public static Action<Cell> CellClicked;
    
    public static Func<PuzzleSettings> GetPuzzleSettings;


    public static Action UpdateTargetUI;
    public static Action LevelCompleted;


    public static Action<PuzzleSettings> SetPuzzle;

}
