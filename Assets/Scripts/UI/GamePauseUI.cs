using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() => { GameManager.Instance.TogglePauseGame(); });
        mainMenuButton.onClick.AddListener(() => { Loader.Load(Loader.Scene.MainMenuScene); });
        optionsButton.onClick.AddListener(() => { OptionsUI.Instance.Show(); });
    }

    private void Start()
    {
        GameManager.Instance.OnGamePaused += KitchenGameManager_OnGamePaused;
        GameManager.Instance.OnGameUnpaused += KitchenGameManager_OnGameUnpaused;
        Hide();
    }

    private void KitchenGameManager_OnGamePaused(object sender, EventArgs e)
    {
        Show();
    }

    private void KitchenGameManager_OnGameUnpaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}