using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    private event Action OnRestartButtonPress = null;
    private event Action OnContinueButtonPress = null;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject winscreenPanel;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
    }

    public void SetupUI(Action onRestartButtonPress, Action onContinueButtonPress) 
    {
        OnRestartButtonPress += onRestartButtonPress;
        restartButton.onClick.AddListener(OnRestartButtonPressHandler);

        OnContinueButtonPress += onContinueButtonPress;
        continueButton.onClick.AddListener(OnContinueButtonPressHandler);
    }

    public void ShowWinScreen() 
    {
        winscreenPanel.SetActive(true);
    }

    public void HideWinScreen() 
    {
        winscreenPanel.SetActive(false);
    }

    private void OnRestartButtonPressHandler() 
    {
        OnRestartButtonPress?.Invoke();
    }

    private void OnContinueButtonPressHandler() 
    {
        OnContinueButtonPress?.Invoke();
    }
}
