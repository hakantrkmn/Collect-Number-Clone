using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake() 
    { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public GameStates gameState;
    public List<Target> targets;


    public void ColorPopped(int number)
    {
        if (targets.Exists(x=>x.number == number))
        {
            var target = targets.First(x => x.number == number);
            if (target.amount != 0)
            {
                targets.First(x=>x.number == number).amount--;
                EventManager.UpdateTargetUI();
            }
            CheckWin();
        }
       
    }
    
    void CheckWin()
    {
        if (targets.All(x => x.amount == 0))
        {
            gameState = GameStates.GameOver;
            EventManager.LevelCompleted();
        }
    }
   
}