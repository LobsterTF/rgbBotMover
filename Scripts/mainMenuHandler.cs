using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class mainMenuHandler : MonoBehaviour
{
    [SerializeField] private Toggle cbToggle, musicToggle;
    void Start()
    {
        
        try
        {
            int val = PlayerPrefs.GetInt("firstTime");
            if (val == 0)
            {
                colorBlind(true);
                music(true);
                PlayerPrefs.SetInt("firstTime", 1);
            }
        }
        catch (Exception e)
        {
            colorBlind(true);
            music(true);
            PlayerPrefs.SetInt("firstTime", 1);
        }
        setCBModeInit();
        setMusicInit();
    }
    public void selectLevel(int selection)
    {
        PlayerPrefs.SetInt("CurrentLevel", selection);
        SceneManager.LoadScene("playScene", LoadSceneMode.Single);

    }
    void setCBModeInit()
    {
        int sel = PlayerPrefs.GetInt("colorblind");
        bool active = false;
        if (sel == 0) { active = false; } else { active = true; }
        cbToggle.isOn = active;
    }
    void setMusicInit()
    {
        int sel = PlayerPrefs.GetInt("music");
        bool active = false;
        if (sel == 0) { active = false; } else { active = true; }
        musicToggle.isOn = active;
    }
    public void colorBlind(bool active)
    {
        int sel = 0;
        if (active) { sel = 1; } else { sel = 0; }
        PlayerPrefs.SetInt("colorblind", sel);

    }
    public void music(bool active)
    {
        int sel = 0;
        if (active) { sel = 1; } else { sel = 0; }
        PlayerPrefs.SetInt("music", sel);

    }
    public void continueGame()
    {
        SceneManager.LoadScene("playScene", LoadSceneMode.Single);

    }
    [SerializeField] private GameObject infoGraphic;
    private bool infoGraphicActive = false;
    public void toggleInfographic()
    {
        infoGraphicActive = !infoGraphicActive;
        infoGraphic.SetActive(infoGraphicActive);
    }
    [SerializeField] private GameObject levelSelectHolder, mainMenuHolder;
    private bool levelSelectActive = false;
    public void toggleLevelSelect()
    {
        levelSelectActive = !levelSelectActive;
        levelSelectHolder.SetActive(levelSelectActive);
        mainMenuHolder.SetActive(!levelSelectActive);

    }
}
