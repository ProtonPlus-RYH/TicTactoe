using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayDisplay : MonoBehaviour
{
    public TextMeshProUGUI NameTMP;
    private string replayName;
    [HideInInspector]
    public string fileName;
    public GameObject DialogWindow;

    void Start()
    {
        ShowReplay();
    }

    /*private void Update()
    {
    }*/

    public void ShowReplay()
    {
        NameTMP.text = replayName;
    }

    public void SetFileName(string name)
    {
        fileName = name;
        string[] times = name.Split("_");
        replayName = times[0] + "." + times[1] + "." + times[2] + " " + times[3] + ":" + times[4] + ":" + times[5];
        ShowReplay();
    }

    public void PlayButton()
    {
        GameObject dialog = Instantiate(DialogWindow, transform.parent.parent.parent.parent);
        dialog.GetComponent<DialogManager>().SetTitle(LanguageManager.Instance.GetLocalizedString("AskPlayReplay") + " " + replayName + " ?");
        dialog.GetComponent<DialogManager>().ConfirmButtonClick += PlayReplay;
    }
    public void PlayReplay(object sender, EventArgs e)
    {
        PlayerPrefs.SetString("SelectedReplay", fileName);
        SceneManager.LoadScene("Replay");
    }

    public void DeleteReplayButton()
    {
        GameObject dialog = Instantiate(DialogWindow, transform.parent.parent.parent.parent);
        dialog.GetComponent<DialogManager>().SetTitle(LanguageManager.Instance.GetLocalizedString("AskDelete") + " " + replayName + " ?");
        dialog.GetComponent<DialogManager>().ConfirmButtonClick += DeleteReplay;
    }
    public void DeleteReplay(object sender, EventArgs e)
    {
        FileManager.Instance.DeleteReplay(fileName);
        MenuManager.Instance.LoadReplay();
    }
}
