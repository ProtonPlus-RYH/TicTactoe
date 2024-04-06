using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    private int SEVolume;
    public TextMeshProUGUI SE_TMP;
    public Slider SESlider;

    private bool ifPlayerGoFirst;
    private bool ifAIGoFirst;
    public Toggle PlayerToggle;
    public Toggle AIToggle;

    public Toggle SCLangToggle;
    public Toggle ENLangToggle;

    void Start()
    {
        //音量初始化
        if (PlayerPrefs.HasKey("SEVolume"))
        {
            SESlider.value = PlayerPrefs.GetInt("SEVolume");
        }
        else
        {
            SESlider.value = 50;
        }

        //先后手初始化
        ifPlayerGoFirst = false;
        ifAIGoFirst = false;
        if(PlayerPrefs.HasKey("FirstPlayerSet") && PlayerPrefs.GetInt("FirstPlayerSet") > 0)
        {
            PlayerToggle.isOn = true;
        }
        else if(PlayerPrefs.HasKey("FirstPlayerSet") && PlayerPrefs.GetInt("FirstPlayerSet") < 0)
        {
            AIToggle.isOn = true;
        }
        else
        {
            PlayerToggle.isOn = false;
            AIToggle.isOn = false;
        }

        //语言初始化
        if (LocalizationSettings.SelectedLocale.ToString() == "Chinese (Simplified) (zh-CN)")
        {
            SCLangToggle.isOn = true;
        }else if (LocalizationSettings.SelectedLocale.ToString() == "English (United Kingdom) (en-GB)")
        {
            ENLangToggle.isOn = true;
        }
    }

    #region SE volume settings

    public void SetSEVolume()
    {
        SEVolume = (int)SESlider.value;
        SE_TMP.text = SEVolume.ToString();
        PlayerPrefs.SetInt("SEVolume", SEVolume);
    }

    #endregion


    #region First player settings

    public void SetIfPlayerGoFirst()
    {
        ifPlayerGoFirst = PlayerToggle.isOn;
        if (ifPlayerGoFirst)
        {
            PlayerPrefs.SetInt("FirstPlayerSet", 1);
        }
        else
        {
            if (!ifAIGoFirst)
            {
                PlayerPrefs.SetInt("FirstPlayerSet", 0);
            }
        }
    }

    public void SetIfAIGoFirst()
    {
        ifAIGoFirst = AIToggle.isOn;
        if (ifAIGoFirst)
        {
            PlayerPrefs.SetInt("FirstPlayerSet", -1);
        }
        else
        {
            if (!ifPlayerGoFirst)
            {
                PlayerPrefs.SetInt("FirstPlayerSet", 0);
            }
        }
    }

    #endregion


    #region Language settings

    public void SetLanguage()
    {
        if (SCLangToggle.isOn)
        {
            LanguageManager.Instance.languageSelect(0);
        }
        if (ENLangToggle.isOn)
        {
            LanguageManager.Instance.languageSelect(1);
        }
    }

    #endregion

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
