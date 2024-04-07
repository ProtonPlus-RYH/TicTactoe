using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chess : MonoBehaviour
{
    public Image ChessPattern;
    public Sprite pattern1;
    public Sprite pattern2;
    public Sprite pattern3;
    public Sprite pattern4;
    public Sprite pattern5;

    public AudioClip ChessSE;
    public AudioSource ChessSESource;

    public bool ifPlaySE;

    private void Start()
    {
        if (ifPlaySE)
        {
            ChessSESource.clip = ChessSE;
            ChessSESource.volume = 0.01f * PlayerPrefs.GetInt("SEVolume");
            ChessSESource.Play();
        }
    }

    public void ChangePattern(int num)
    {
        switch (num)
        {
            case 1:
                ChessPattern.sprite = pattern1;
                break;
            case 2:
                ChessPattern.sprite = pattern2;
                break;
            case 3:
                ChessPattern.sprite = pattern3;
                break;
            case 4:
                ChessPattern.sprite = pattern4;
                break;
            case 5:
                ChessPattern.sprite = pattern5;
                break;
        }
    }

}
