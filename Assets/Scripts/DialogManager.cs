using System;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public TextMeshProUGUI TitleTMP;
    public string title;
    void Start()
    {
        //SetTitle("Title there");
    }

    public void ShowDialog()
    {
        TitleTMP.text = title;
    }

    public void SetTitle(string titleToSet)
    {
        title = titleToSet;
        ShowDialog();
    }

    public event EventHandler<EventArgs> ConfirmButtonClick;
    public event EventHandler<EventArgs> CancelButtonClick;

    public void OnConfirmButtonClick()
    {
        ConfirmButtonClick?.Invoke(this, new EventArgs());
        Invoke(nameof(CloseDialog), 0.05f);
    }

    public void OnCancelButtonClick()
    {
        CancelButtonClick?.Invoke(this, new EventArgs());
        Invoke(nameof(CloseDialog), 0.05f);
    }

    public void CloseDialog()
    {
        Destroy(gameObject);
    }
}
