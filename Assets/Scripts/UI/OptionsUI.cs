using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI moveUPText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAlternateText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Transform pressToRebindKeyTransform;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        soundEffectsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        closeButton.onClick.AddListener(() => { Hide(); });

        // 重新绑定按键
        moveUpButton.onClick.AddListener(() => { RebindBinding(GameInputManager.Binding.Move_Up); });
        moveDownButton.onClick.AddListener(() => { RebindBinding(GameInputManager.Binding.Move_Down); });
        moveLeftButton.onClick.AddListener(() => { RebindBinding(GameInputManager.Binding.Move_Left); });
        moveRightButton.onClick.AddListener(() => { RebindBinding(GameInputManager.Binding.Move_Right); });
        interactButton.onClick.AddListener(() => { RebindBinding(GameInputManager.Binding.Interact); });
        interactAlternateButton.onClick.AddListener(
            () => { RebindBinding(GameInputManager.Binding.InteractAlternate); });
        pauseButton.onClick.AddListener(() => { RebindBinding(GameInputManager.Binding.Pause); });
    }

    private void Start()
    {
        GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;

        UpdateVisual();

        HidePressToRebindKey();
        Hide();
    }

    private void GameManager_OnGameUnpaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        soundEffectsText.text = "音效: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "音乐: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        moveUPText.text = GameInputManager.Instance.GetBindingText(GameInputManager.Binding.Move_Up);
        moveDownText.text = GameInputManager.Instance.GetBindingText(GameInputManager.Binding.Move_Down);
        moveLeftText.text = GameInputManager.Instance.GetBindingText(GameInputManager.Binding.Move_Left);
        moveRightText.text = GameInputManager.Instance.GetBindingText(GameInputManager.Binding.Move_Right);
        interactText.text = GameInputManager.Instance.GetBindingText(GameInputManager.Binding.Interact);
        interactAlternateText.text =
            GameInputManager.Instance.GetBindingText(GameInputManager.Binding.InteractAlternate);
        pauseText.text = GameInputManager.Instance.GetBindingText(GameInputManager.Binding.Pause);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }

    private void HidePressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInputManager.Binding binding)
    {
        ShowPressToRebindKey();
        GameInputManager.Instance.RebindBinding(binding, () =>
        {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }
}