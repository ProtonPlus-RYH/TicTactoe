using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    private BGMPlayer BgmPlayer;
    private int BGMVolume;
    public TextMeshProUGUI BGM_TMP;
    public Slider BGMSlider;

    private int SEVolume;
    public TextMeshProUGUI SE_TMP;
    public Slider SESlider;

    private bool ifPlayerGoFirst;
    private bool ifAIGoFirst;
    public Toggle PlayerToggle;
    public Toggle AIToggle;

    public Toggle SCLangToggle;
    public Toggle ENLangToggle;

    public TMP_Dropdown PlayerPatternDropDown;
    public TMP_Dropdown AIPatternDropDown;

    void Start()
    {
        //音量初始化
        if (!PlayerPrefs.HasKey("BGMVolume"))
        {
            PlayerPrefs.SetInt("BGMVolume",50);
        }
        BGMSlider.value = PlayerPrefs.GetInt("BGMVolume");
        SetBGMVolume();
        BgmPlayer = FindObjectOfType<BGMPlayer>();
        if (BgmPlayer!=null)
        {
            BgmPlayer.GetComponent<AudioSource>().volume = BGMVolume * 0.01f;
        }
        if (!PlayerPrefs.HasKey("SEVolume"))
        {
            PlayerPrefs.SetInt("SEVolume", 50);
        }
        SESlider.value = PlayerPrefs.GetInt("SEVolume");
        SetSEVolume();

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

        //棋子样式选择初始化
        if (!PlayerPrefs.HasKey("PlayerChessPattern"))
        {
            PlayerPrefs.SetInt("PlayerChessPattern", 4);
        }
        PlayerPatternDropDown.SetValueWithoutNotify(PlayerPrefs.GetInt("PlayerChessPattern")-1);
        if (!PlayerPrefs.HasKey("AIChessPattern"))
        {
            PlayerPrefs.SetInt("AIChessPattern", 3);
        }
        AIPatternDropDown.SetValueWithoutNotify(PlayerPrefs.GetInt("AIChessPattern")-1);
    }

    #region Volume settings

    public void SetBGMVolume()
    {
        BGMVolume = (int)BGMSlider.value;
        BGM_TMP.text = BGMVolume.ToString();
        PlayerPrefs.SetInt("BGMVolume", BGMVolume);
        if (BgmPlayer != null)
        {
            BgmPlayer.GetComponent<AudioSource>().volume = BGMVolume * 0.01f;
            if (BgmPlayer.Audio.volume == 0)
            {
                BgmPlayer.Audio.Pause();
            }
            else
            {
                BgmPlayer.Audio.UnPause();
            }
        }
    }


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


    #region Pattern setting

    public void SetPlayerPattern()
    {
        PlayerPrefs.SetInt("PlayerChessPattern", PlayerPatternDropDown.value + 1);
    }

    public void SetAIPattern()
    {
        PlayerPrefs.SetInt("AIChessPattern", AIPatternDropDown.value + 1);
    }

    #endregion

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
