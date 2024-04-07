using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Setting_InGame : MonoBehaviour
{

    private BGMPlayer BgmPlayer;
    private int BGMVolume;
    public TextMeshProUGUI BGM_TMP;
    public Slider BGMSlider;

    private int SEVolume;
    public TextMeshProUGUI SE_TMP;
    public Slider SESlider;

    public Toggle SCLangToggle;
    public Toggle ENLangToggle;
    void Start()
    {
        //音量初始化
        if (!PlayerPrefs.HasKey("BGMVolume"))
        {
            PlayerPrefs.SetInt("BGMVolume", 50);
        }
        BGMSlider.value = PlayerPrefs.GetInt("BGMVolume");
        SetBGMVolume();
        BgmPlayer = FindObjectOfType<BGMPlayer>();
        if (BgmPlayer != null)
        {
            BgmPlayer.GetComponent<AudioSource>().volume = BGMVolume * 0.01f;
        }
        if (!PlayerPrefs.HasKey("SEVolume"))
        {
            PlayerPrefs.SetInt("SEVolume", 50);
        }
        SESlider.value = PlayerPrefs.GetInt("SEVolume");
        SetSEVolume();

        //语言初始化
        if (LocalizationSettings.SelectedLocale.ToString() == "Chinese (Simplified) (zh-CN)")
        {
            SCLangToggle.isOn = true;
        }
        else if (LocalizationSettings.SelectedLocale.ToString() == "English (United Kingdom) (en-GB)")
        {
            ENLangToggle.isOn = true;
        }
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

}
