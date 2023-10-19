using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public TMP_Text starScoreText;
    public TMP_Text levelScoreText;
    public Image starBackground;
    public Image starSign;
    public TMP_Text levelText;
    public Button playButton;
    
    public TMP_Text timerText;
    public TMP_Text levelPanelText;
    public Image timerBackground;
    public Image timerSign;
    public Image levelPanelBackground;
    public Image stackTile;
    public Button pauseButton;
    public TMP_Text rewardText;
    public Button settingButton;
    public Slider musicSlider;
    public Slider sfxSlider;
    
    public Image dialogBackground;
    public GameObject winDialog;
    public GameObject loseDialog;
    public GameObject pauseDialog;
    public GameObject settingDialog;

    private GameManager _gameManager;


    public void Initialize(GameManager gameManager)
    {
        _gameManager = gameManager;
        InitializeAction();
        InitializeData();
    }

    public void SetData(int starScore, int levelScore)
    {
        starScoreText.text = starScore.ToString();
        levelScoreText.text = levelScore.ToString();
        levelPanelText.text = "lv." + levelScore;
        musicSlider.value = GetMappedAudioValue(_gameManager.audioManager.GetMusicVolume());
        sfxSlider.value = GetMappedAudioValue(_gameManager.audioManager.GetSfxVolume());
    }
    
    public void UpdateStarScore(int starScore)
    {
        starScoreText.text = starScore.ToString();
    }
    
    public void UpdateLevel(int levelScore)
    {
        levelPanelText.text = "lv." + levelScore;
        levelScoreText.text = levelScore.ToString();
    }
    
    public void UpdateTimer(float time)
    {
        //convert time to minutes and seconds
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        timerText.text = niceTime;
    }

    private void InitializeAction()
    {
        playButton.onClick.AddListener(ClickPlayButton);
        pauseButton.onClick.AddListener(ClickPauseButton);
    }

    private void InitializeData()
    {
        starScoreText.text = "0";
        levelScoreText.text = "1";
    }
    
    private float GetMappedAudioValue(float value)
    {
        float mappedValue = Mathf.Lerp(0f, 100f, (value + 80f) / 80f);
        return Mathf.Clamp(mappedValue, 0f, 100f);
    }

    private void ClickPlayButton()
    {
        _gameManager.audioManager.PlayClickButton();
        _gameManager.OnGameStart?.Invoke();
    }
    
    private void ClickPauseButton()
    {
        _gameManager.audioManager.PlayClickButton();
        Time.timeScale = 0;
        _gameManager.GameStateChangedAction?.Invoke(GameState.NonGame);
        if(pauseDialog != null)
            pauseDialog.SetActive(true);
        if(dialogBackground != null)
            dialogBackground.gameObject.SetActive(true);
        //_gameManager.BackToMenu?.Invoke();
    }

    private void HideMenuUI()
    {
        levelScoreText.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(false);
    }

    public void ShowMenuUI(int starScoreValue)
    {
        HideGameUI();
        levelScoreText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
        playButton.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(true);
        UpdateStarScore(starScoreValue);
    }
    
    public void ShowGameUI()
    {
        HideMenuUI();
        timerText.gameObject.SetActive(true);
        levelPanelText.gameObject.SetActive(true);
        timerBackground.gameObject.SetActive(true);
        timerSign.gameObject.SetActive(true);
        levelPanelBackground.gameObject.SetActive(true);
        stackTile.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(true);
    }
    
    private void HideGameUI()
    {
        timerText.gameObject.SetActive(false);
        levelPanelText.gameObject.SetActive(false);
        timerBackground.gameObject.SetActive(false);
        timerSign.gameObject.SetActive(false);
        levelPanelBackground.gameObject.SetActive(false);
        stackTile.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
    }
    
    public void OnSetting()
    {
        _gameManager.audioManager.PlayClickButton();
        if(dialogBackground != null)
            dialogBackground.gameObject.SetActive(true);
        if (settingDialog != null) settingDialog.SetActive(true);
    }

    public void OnReturnToMenu()
    {
        _gameManager.audioManager.PlayClickButton();
        dialogBackground.gameObject.SetActive(false);
        _gameManager.BackToMenu?.Invoke();
    }
    
    public void OnNextLevel()
    {
        _gameManager.audioManager.PlayClickButton();
        dialogBackground.gameObject.SetActive(false);
        _gameManager.OnGameStart?.Invoke();
    }
    
    public void OnAgain()
    {
        _gameManager.audioManager.PlayClickButton();
        dialogBackground.gameObject.SetActive(false);
        _gameManager.OnGameStart?.Invoke();
    }

    public void OnExitGame()
    {
        _gameManager.audioManager.PlayClickButton();
        Application.Quit();
    }

    public void OnResumeGame()
    {
        _gameManager.audioManager.PlayClickButton();
        dialogBackground.gameObject.SetActive(false);
        _gameManager.GameStateChangedAction?.Invoke(GameState.Game);
        Time.timeScale = 1;
    }
    
    public void ShowWinDialog(int starScore)
    {
        Time.timeScale = 0;
        if (dialogBackground != null) dialogBackground.gameObject.SetActive(true);
        if (winDialog != null) winDialog.SetActive(true);
        if (rewardText != null) rewardText.text = "Star + " + starScore;
    }
    
    public void ShowLoseDialog()
    {
        Time.timeScale = 0;
        if (dialogBackground != null) dialogBackground.gameObject.SetActive(true);
        if (loseDialog != null) loseDialog.SetActive(true);
    }
    
    public void OnMusicSliderChange(float value)
    {
        _gameManager.audioManager.ChangeAudioValue("Music Volume", value);
    }
    
    public void OnSfxSliderChange(float value)
    {
        _gameManager.audioManager.ChangeAudioValue("SFX Volume", value);
    }
}
