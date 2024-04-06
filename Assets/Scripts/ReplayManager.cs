using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayManager : MonoBehaviour
{
    [HideInInspector]
    public int[][] ChessBoard;
    public Transform[][] ChessTransform;
    [HideInInspector]
    public bool ifFirstPlayerOperating;

    private int stepCount;
    public TextMeshProUGUI StepCountTMP;

    public GameObject FirstChess;
    public GameObject SecondChess;

    private List<string> GameReplay;

    public Transform TurnPlayerTransform;
    public TextMeshProUGUI HintTMP;

    public GameObject SettingsDialog;

    void Start()
    {
        InitializingGame();
    }

    void InitializingGame()
    {
        InitializeBoard();
        GameReplay = FileManager.Instance.ReadReplay(PlayerPrefs.GetString("SelectedReplay"));
        ifFirstPlayerOperating = true;
        RefreshPlayerTMP();
    }

    public void InitializeBoard()
    {
        if (ChessTransform != null)
        {
            foreach (var array in ChessTransform)
            {
                foreach (var block in array)
                {
                    if (block.childCount > 0)
                    {
                        Destroy(block.GetChild(0).GameObject());
                    }
                }
            }
            for (int i = 0; i < 9; i++)
            {
                ChessBoard[i / 3][i % 3] = 0;
            }
        }
        else
        {
            ChessTransform = new Transform[3][];
            ChessBoard = new int[3][];
            Transform[] blocks = GetComponentsInChildren<Transform>();
            for (int i = 0; i < 9; i++)
            {
                if (i % 3 == 0)
                {
                    ChessTransform[i / 3] = new Transform[3];
                    ChessBoard[i / 3] = new int[3];
                }
                ChessTransform[i / 3][i % 3] = blocks[i + 1];
                ChessBoard[i / 3][i % 3] = 0;
            }
        }
        stepCount = 0;
        StepCountTMP.text = "0";
    }

    public void AddChess(bool ifFirstPlayer, int xPosition, int yPosition)
    {
        if (ChessBoard[xPosition][yPosition] == 0)
        {
            if (ifFirstPlayer)
            {
                Instantiate(FirstChess, ChessTransform[xPosition][yPosition]);
                ChessBoard[xPosition][yPosition] = 1;
            }
            else
            {
                Instantiate(SecondChess, ChessTransform[xPosition][yPosition]);
                ChessBoard[xPosition][yPosition] = -1;
            }
        }
    }

    public void RefreshPlayerTMP()
    {
        switch (ifFirstPlayerOperating)
        {
            case true:
                TurnPlayerTransform.localPosition = new Vector3(TurnPlayerTransform.localPosition.x, Math.Abs(TurnPlayerTransform.localPosition.y), TurnPlayerTransform.localPosition.z);
                break;
            case false:
                TurnPlayerTransform.localPosition = new Vector3(TurnPlayerTransform.localPosition.x, Math.Abs(TurnPlayerTransform.localPosition.y) * (-1), TurnPlayerTransform.localPosition.z);
                break;
        }
    }


    #region Button functions

    public void ReturnToLastStep()
    {
        if (stepCount > 0)
        {
            StepCountChange(-1);
            if (GameReplay[stepCount] != "Return")
            {
                string[] xyPositionStr = GameReplay[stepCount].Split(",");
                if (xyPositionStr.Length == 2)
                {
                    if (int.TryParse(xyPositionStr[0], out int x) && int.TryParse(xyPositionStr[1], out int y))
                    {
                        int[] lastPosition = new int[2] { x, y };
                        if (ChessBoard[lastPosition[0]][lastPosition[1]] != 0)
                        {
                            Transform returnBlock = ChessTransform[lastPosition[0]][lastPosition[1]];
                            if (returnBlock.childCount > 0)
                            {
                                Destroy(returnBlock.GetChild(0).GameObject());
                            }
                            ChessBoard[lastPosition[0]][lastPosition[1]] = 0;
                            ifFirstPlayerOperating = !ifFirstPlayerOperating;
                        }
                    }
                }
            }
            else
            {
                string[] lastPositionStr;
                int returnCount = 0;
                for (int i = stepCount-1; i >= 0; i--)
                {
                    if (GameReplay[i] != "Return")
                    {
                        if (returnCount == 0)
                        {
                            lastPositionStr = GameReplay[i].Split(",");
                            if (int.TryParse(lastPositionStr[0], out int x) && int.TryParse(lastPositionStr[1], out int y))
                            {
                                if (ChessBoard[x][y] == 0)
                                {
                                    AddChess(ifFirstPlayerOperating, x, y);
                                    ifFirstPlayerOperating = !ifFirstPlayerOperating;
                                }
                            }
                        }
                        else{
                            returnCount--;
                        }
                    }
                    else{
                        returnCount++;
                    }
                }
            }
            RefreshPlayerTMP();
        }
        else
        {
            Hint(LanguageManager.Instance.GetLocalizedString("IsReplayStart"));
        }
    }

    public void GoToNextStep()
    {
        if (stepCount<GameReplay.Count)
        {
            if (GameReplay[stepCount] != "Return")
            {
                string[] xyPositionStr = GameReplay[stepCount].Split(",");
                if (xyPositionStr.Length == 2)
                {
                    if (int.TryParse(xyPositionStr[0], out int x) && int.TryParse(xyPositionStr[1], out int y))
                    {
                        AddChess(ifFirstPlayerOperating, x, y);
                        ifFirstPlayerOperating = !ifFirstPlayerOperating;
                    }
                }
            }
            else
            {
                string[] lastPositionStr;
                for (int i = stepCount - 1; i >= 0; i--)
                {
                    if (GameReplay[i] != "Return")
                    {
                        lastPositionStr = GameReplay[i].Split(",");
                        if (int.TryParse(lastPositionStr[0], out int x) && int.TryParse(lastPositionStr[1], out int y))
                        {
                            int[] lastPosition = new int[2] { x, y };
                            if (ChessBoard[lastPosition[0]][lastPosition[1]] != 0)
                            {
                                Hint(LanguageManager.Instance.GetLocalizedString("ReturnStep"));
                                Transform returnBlock = ChessTransform[lastPosition[0]][lastPosition[1]];
                                if (returnBlock.childCount > 0)
                                {
                                    Destroy(returnBlock.GetChild(0).GameObject());
                                }
                                ChessBoard[lastPosition[0]][lastPosition[1]] = 0;
                                ifFirstPlayerOperating = !ifFirstPlayerOperating;
                                break;
                            }
                        }
                    }

                }
            }
            RefreshPlayerTMP();
            StepCountChange(1);
        }
        else
        {
            Hint(LanguageManager.Instance.GetLocalizedString("ReplayOver"));
        }
    }

    public void GoToSettings()
    {
        SettingsDialog.SetActive(true);
    }


    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    #region Support functions

    public void Hint(string hintText)
    {
        HintTMP.text = hintText;
        HintTMP.gameObject.SetActive(true);
        Invoke(nameof(HintTMPDisappear), 1.0f);
    }
    public void HintTMPDisappear()
    {
        HintTMP.gameObject.SetActive(false);
    }

    public void StepCountChange(int num)
    {
        stepCount += num;
        StepCountTMP.text = stepCount.ToString();
    }

    #endregion
}
