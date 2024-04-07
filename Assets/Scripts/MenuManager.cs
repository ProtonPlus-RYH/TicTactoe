using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoSingleton<MenuManager>
{
    public GameObject RuleIntroduceDialog;
    public GameObject ViewReplayDialog;
    public Transform ReplayGrid;
    public GameObject ReplayBar;
    public GameObject ModeSelection;
    public GameObject NoReplayTMP;

    public GameObject BgmPlayer;

    public Toggle SCLangToggle;
    public Toggle ENLangToggle;

    private void Start()
    {
        //≥ı ºªØBGM
        if (FindObjectOfType<BGMPlayer>() == null)
        {
            Instantiate(BgmPlayer, gameObject.transform.parent);
        }

        if (!PlayerPrefs.HasKey("SEVolume"))
        {
            PlayerPrefs.SetInt("SEVolume", 50);
        }

        if (LocalizationSettings.SelectedLocale.ToString() == "Chinese (Simplified) (zh-CN)")
        {
            SCLangToggle.isOn = true;
        }
        else if (LocalizationSettings.SelectedLocale.ToString() == "English (United Kingdom) (en-GB)")
        {
            ENLangToggle.isOn = true;
        }
    }

    public void GameStart()
    {
        ModeSelection.SetActive(true);
    }

    public void ModeSelect(int mode)
    {
        PlayerPrefs.SetInt("AISuccessRate", mode);
        SceneManager.LoadScene("Chess");
    }

    public void GoToSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void RuleIntrocduce()
    {
        RuleIntroduceDialog.SetActive(true);
    }

    public void ViewReplay(){
        ViewReplayDialog.SetActive(true);
        LoadReplay();
    }

    public void LoadReplay()
    {
        for (int i=0; i<ReplayGrid.childCount; i++)
        {
            Destroy(ReplayGrid.GetChild(i).gameObject);
        }
        foreach(string replayName in FileManager.Instance.ReplayFileNames)
        {
            GameObject replay = Instantiate(ReplayBar, ReplayGrid);
            replay.GetComponent<ReplayDisplay>().SetFileName(replayName);
        }
        if (FileManager.Instance.ReplayFileNames.Count == 0)
        {
            NoReplayTMP.SetActive(true);
        }
        else
        {
            NoReplayTMP.SetActive(false);
        }
    }

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
}
