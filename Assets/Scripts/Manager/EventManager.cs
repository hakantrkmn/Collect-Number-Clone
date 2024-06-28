using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class EventManager 
{
    public static Action<Cell> CellClicked;
    
    public static Func<PuzzleSettings> GetPuzzleSettings;


    public static Action UpdateTargetUI;
    public static Action LevelCompleted;

}
