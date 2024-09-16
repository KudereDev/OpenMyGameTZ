using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controlls UI, set up UI animations, set data for restart button, show number of turns and what level player is on
//Add UI animations of Biplane goint on Synosoid way with random data 
public class UIController : MonoBehaviour
{
    public static UIController Instance;

    private event Action OnRestartButtonPress = null;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
    }

    public void SetupUI(Action onRestartButtonPress) 
    {
        OnRestartButtonPress += onRestartButtonPress;
        restartButton.onClick.AddListener(OnRestartButtonPressHandler);
    }

    private void OnRestartButtonPressHandler() 
    {
        OnRestartButtonPress?.Invoke();
    }
}
