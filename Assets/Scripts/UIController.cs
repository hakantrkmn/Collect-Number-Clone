using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject levelCompletedPanel;

    private void OnEnable()
    {
        EventManager.LevelCompleted += LevelCompleted;
    }

    private void OnDisable()
    {
        EventManager.LevelCompleted -= LevelCompleted;
    }
    
    void LevelCompleted()
    {
        levelCompletedPanel.SetActive(true);
    }
}
