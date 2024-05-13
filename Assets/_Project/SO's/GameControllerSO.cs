using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameControllerSO", menuName = "Scriptable Objects/GameControllerSO")]
public class GameControllerSO : ScriptableObject
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject enterNamePanel;
    public GameObject blankPanel;
    public GameObject leaderboardPanel;
    public GameObject gameplayPanel;
    public GameObject panelsParent;

    [Space]
    public Shape activeShape;

    [Header("Settings")]
    public WaitForSeconds dropTimer, moveTimer, removeRowTimer, ghostTimer;
    public float dropTimerValue = 0.7f, dropTimerSpeedMultiplier = 1, ghostTimerValue = 0.2f, removeRowTimerValue = 0.2f;
    [Range(0.02f, 0.2f)] public float moveTimerValue = 0.07f;

    public int score;

    public bool didLevelUp = false;

    public bool rotateClockwise = true;

    public bool isPaused = true;

    public bool isGameOver;

    public void Init()
    {
        ResetAllPanels();
        isGameOver = true;
        isPaused = true;
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    public void ResetAllPanels()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (enterNamePanel) enterNamePanel.SetActive(false);
        if (leaderboardPanel) leaderboardPanel.SetActive(false);
        if (gameplayPanel) gameplayPanel.SetActive(false);
        if (blankPanel) blankPanel.SetActive(false);
    }
}
