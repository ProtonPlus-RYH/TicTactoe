using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ChessGameManager : MonoSingleton<ChessGameManager>
{
    [HideInInspector]
    public int[][] ChessBoard;
    public Transform[][] ChessTransform;
    [HideInInspector]
    public bool ifPlayerOperating;
    [HideInInspector]
    public bool ifPause;
    [HideInInspector]
    public int turnCount;
    public GameObject ChessObj;
    public GameObject Chess1Image;
    public GameObject Chess2Image;

    private List<string> GameReplay;

    public Transform TurnPlayerTransform;
    public TextMeshProUGUI HintTMP;

    public GameObject SettingsDialog;

    public GameObject GameOverDialog;
    public TextMeshProUGUI GameResultTMP;

    [HideInInspector]
    public bool ifVersusAI;
    public GameObject TTTAI;

    public GameObject pointer;
    private int[] pointerPosition;

    void Start()
    {
        DoPause();

        if (!PlayerPrefs.HasKey("AIChessPattern"))
        {
            PlayerPrefs.SetInt("AIChessPattern", 3);
        }
        if (!PlayerPrefs.HasKey("PlayerChessPattern"))
        {
            PlayerPrefs.SetInt("PlayerChessPattern", 4);
        }
        Chess1Image.GetComponent<Chess>().ChangePattern(PlayerPrefs.GetInt("PlayerChessPattern"));
        Chess2Image.GetComponent<Chess>().ChangePattern(PlayerPrefs.GetInt("AIChessPattern"));

        if (PlayerPrefs.HasKey("AISuccessRate"))
        {
            if (PlayerPrefs.GetInt("AISuccessRate") == 0)//线下对战
            {
                ifVersusAI = false;
                InitializingGame();
            }
            else//AI对战
            {
                ifVersusAI = true;
                Instantiate(TTTAI, transform.parent);
                Hint(LanguageManager.Instance.GetLocalizedString("Coining"));
                Invoke(nameof(InitializingGame), 1.0f);
            }
        }
        else//AI对战
        {
            ifVersusAI = true;
            Instantiate(TTTAI, transform.parent);
            Hint(LanguageManager.Instance.GetLocalizedString("Coining"));
            Invoke(nameof(InitializingGame), 1.0f);
        }
    }

    void InitializingGame()
    {
        InitializeBoard();
        GameReplay = new List<string>();
        if (ifVersusAI)
        {
            if (PlayerPrefs.GetInt("FirstPlayerSet") > 0)//玩家先手
            {
                ifPlayerOperating = true;
            }
            else if (PlayerPrefs.GetInt("FirstPlayerSet") < 0)//AI先手
            {
                ifPlayerOperating = false;
            }
            else
            {
                ifPlayerOperating = Coin();
            }
        }
        else
        {
            ifPlayerOperating = Coin();
        }
        if (ifPlayerOperating && ifVersusAI)
        {
            Hint(LanguageManager.Instance.GetLocalizedString("YouGoingFirst"));
        }
        else if(ifVersusAI)
        {
           Hint(LanguageManager.Instance.GetLocalizedString("OpponentGoingFirst"));
        }
        else
        {
            Hint(LanguageManager.Instance.GetLocalizedString("GameStart"));
        }
        ifReplaySaved = false;
        TurnPlayerTransform.gameObject.SetActive(true);
        RefreshPlayerTMP();
        DoStopPause();
        Invoke(nameof(CallAIEvent), 1.0f);
    }

    public void InitializeBoard()
    {
        if (ChessTransform!=null)
        {
            foreach(var array in ChessTransform)
            {
                foreach(var block in array)
                {
                    if (block.childCount > 0)
                    {
                        for (int i = 0; i < block.childCount; i++)
                        {
                            Destroy(block.GetChild(i).GameObject());
                        }
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
        pointerPosition = null;
        turnCount = 0;
    }

    public void AddChess(bool ifSelf, int xPosition, int yPosition)
    {
        if (!ifPause)
        {
            if (ChessBoard[xPosition][yPosition] == 0)
            {
                if (ifSelf)
                {
                    GameObject playerChess = Instantiate(ChessObj, ChessTransform[xPosition][yPosition]);
                    playerChess.GetComponent<Chess>().ChangePattern(PlayerPrefs.GetInt("PlayerChessPattern"));
                    ChessBoard[xPosition][yPosition] = 1;
                }
                else
                {
                    GameObject aiChess = Instantiate(ChessObj, ChessTransform[xPosition][yPosition]);
                    aiChess.GetComponent<Chess>().ChangePattern(PlayerPrefs.GetInt("AIChessPattern"));
                    ChessBoard[xPosition][yPosition] = -1;
                }
                if (pointerPosition != null && ChessTransform[pointerPosition[0]][pointerPosition[1]].childCount == 2)
                {
                    Destroy(ChessTransform[pointerPosition[0]][pointerPosition[1]].GetChild(1).GameObject());
                }
                pointerPosition = new int[2] {xPosition, yPosition};
                Instantiate(pointer, ChessTransform[xPosition][yPosition]);
                GameReplay.Add(xPosition.ToString() + "," + yPosition.ToString());

                if (CheckResult(xPosition, yPosition))
                {
                    GameWin(ifPlayerOperating);
                }
                else
                {
                    turnCount++;
                    if (turnCount == 9)
                    {
                        GameDraw();
                    }
                    else
                    {
                        ifPlayerOperating = !ifSelf;
                        RefreshPlayerTMP();
                        Invoke(nameof(CallAIEvent), 0.2f);
                    }
                }
            }
        }
    }
    public event EventHandler<EventArgs> CallAI;
    public void CallAIEvent()
    {
        if (!ifPlayerOperating)
        {
            CallAI?.Invoke(this, new EventArgs());
        }
    }

    public void RefreshPlayerTMP()
    {
        switch (ifPlayerOperating)
        {
            case true:
                TurnPlayerTransform.localPosition = new Vector3(TurnPlayerTransform.localPosition.x, Math.Abs(TurnPlayerTransform.localPosition.y), TurnPlayerTransform.localPosition.z);
                break;
            case false:
                TurnPlayerTransform.localPosition = new Vector3(TurnPlayerTransform.localPosition.x, Math.Abs(TurnPlayerTransform.localPosition.y) * (-1), TurnPlayerTransform.localPosition.z);
                break;
        }
    }

    public bool CheckResult(int x, int y)
    {
        bool result = false;
        int resultCount = 0;
        foreach(var chess in ChessBoard[x])
        {
            resultCount += chess;
        }
        if (Math.Abs(resultCount) == 3)
        {
            result = true;
        }
        else
        {
            resultCount = 0;
            foreach(var row in ChessBoard)
            {
                resultCount += row[y];
            }
            if (Math.Abs(resultCount) == 3)
            {
                result = true;
            }
            resultCount = 0;
            if(!result && x == y)
            {
                for (int i=0; i<3; i++)
                {
                    resultCount += ChessBoard[i][i];
                }
                if (Math.Abs(resultCount) == 3)
                {
                    result = true;
                }
            }
            resultCount = 0;
            if(!result && x + y == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    resultCount += ChessBoard[i][2-i];
                }
                if (Math.Abs(resultCount) == 3)
                {
                    result = true;
                }
            }
        }
        return result;
    }

    public void GameWin(bool ifSelfWin)
    {
        if (ifVersusAI)
        {
            if (ifSelfWin)
            {
                GameResultTMP.text = LanguageManager.Instance.GetLocalizedString("Win_Self");
            }
            else
            {
                GameResultTMP.text = LanguageManager.Instance.GetLocalizedString("Win_Opponent");
            }
        }
        else
        {
            GameResultTMP.text = LanguageManager.Instance.GetLocalizedString("GameOver");
        }
        TurnPlayerTransform.gameObject.SetActive(false);
        GameOverDialog.SetActive(true);
        DoPause();
    }

    public void GameDraw()
    {
        GameResultTMP.text = LanguageManager.Instance.GetLocalizedString("GameDraw");
        TurnPlayerTransform.gameObject.SetActive(false);
        GameOverDialog.SetActive(true);
        DoPause();
    }



    #region Button functions

    public void ReturnToLastStep()
    {
        int currentReturnCount = 1;
        SettingsDialog.SetActive(false);
        string[] lastPositionStr = null;
        for(int i=GameReplay.Count-1; i>=0; i--)
        {
            if (GameReplay[i] != "Return" && currentReturnCount==0)
            {
                lastPositionStr = GameReplay[i].Split(",");
                break;
            }
            else if(GameReplay[i] != "Return")
            {
                currentReturnCount--;
            }else
            {
                currentReturnCount++;
            }
        }
        if (pointerPosition != null)
        {
            Transform returnBlock = ChessTransform[pointerPosition[0]][pointerPosition[1]];
            if (returnBlock.childCount > 0)
            {
                for(int i=0; i<returnBlock.childCount; i++)
                {
                    Destroy(returnBlock.GetChild(i).GameObject());
                }
            }
            ChessBoard[pointerPosition[0]][pointerPosition[1]] = 0;
            if (lastPositionStr != null)
            {
                int[] lastPosition = new int[2] { int.Parse(lastPositionStr[0]), int.Parse(lastPositionStr[1]) };
                pointerPosition = new int[2] { lastPosition[0], lastPosition[1] };
                Instantiate(pointer, ChessTransform[lastPosition[0]][lastPosition[1]]);
            }
            ifPlayerOperating = !ifPlayerOperating;
            turnCount--;
            GameReplay.Add("Return");
            RefreshPlayerTMP();
            if(!ifPlayerOperating && ifVersusAI)
            {
                ReturnToLastStep();
            }
        }
        else
        {
            Hint(LanguageManager.Instance.GetLocalizedString("IsStartState"));
        }
    }

    private bool ifReplaySaved;
    public void SaveReplay()
    {
        if (!ifReplaySaved)
        {
            DateTime now = DateTime.Now;
            string name = now.Year.ToString() + "_" + now.Month.ToString() + "_" +now.Day.ToString() + "_" +now.Hour.ToString() + "_" + now.Minute.ToString() + "_" + now.Second.ToString();
            FileManager.Instance.WriteReplay(GameReplay, name);
            ifReplaySaved = true;
        }
        Hint(LanguageManager.Instance.GetLocalizedString("Hint_AlreadySaved"));
    }

    public void GoToSettings()
    {
        SettingsDialog.SetActive(true);
    }

    public void Surrender()
    {
        SettingsDialog.SetActive(false);
        GameWin(false);
    }

    public void PlayAgain()
    {
        GameOverDialog.SetActive(false);
        InitializingGame();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    #region Support functions

    public void DoPause()
    {
        ifPause = true;
    }

    public void DoStopPause()
    {
        ifPause = false;
    }

    public void Hint(string hintText)
    {
        HintTMP.text = hintText;
        HintTMP.gameObject.SetActive(true);
        Invoke(nameof(FirstTMPDisappear), 1.0f);
    }

    public void FirstTMPDisappear()
    {
        HintTMP.gameObject.SetActive(false);
    }

    public bool Coin()
    {
        if (UnityEngine.Random.Range(0, 2)==0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}