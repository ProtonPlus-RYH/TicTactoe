using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LineOnBoard
{
    public int[][] points;
    public LineOnBoard(int[][] linePoints)
    {
        if(linePoints.Length==3 && linePoints[0].Length == 2)
        {
            points = linePoints;
        }
        else
        {
            Debug.Log("添加错误");
        }
    }

    public int GetLineValue()
    {
        int result = 0;
        for(int i=0; i<3; i++)
        {
            result += ChessGameManager.Instance.ChessBoard[points[i][0]][points[i][1]];
        }
        return result;
    }

    public int[] GetFirstFreeBlock()
    {
        int[] result = null;
        foreach(var point in points)
        {
            if (ChessGameManager.Instance.ChessBoard[point[0]][point[1]]==0)
            {
                result = point;
                break;
            }
        }
        return result;
    }

    public int[] GetRandomFreeBlock()
    {
        List<int[]> freeBlocks = new List<int[]>();
        foreach (var point in points)
        {
            if (ChessGameManager.Instance.ChessBoard[point[0]][point[1]] == 0)
            {
                freeBlocks.Add(point);
            }
        }
        if (freeBlocks.Count > 0)
        {
            return freeBlocks[UnityEngine.Random.Range(0, freeBlocks.Count)];
        }
        else
        {
            return null;
        }
    }

    public bool IncludePoint(int[] point)
    {
        foreach(var myPoint in points)
        {
            if (myPoint.SequenceEqual(point))
            {
                return true;
            }
        }
        return false;
    }
}


public class VersusAI : MonoBehaviour
{
    LineOnBoard[] lines;
    int IQ;

    void Start()
    {
        InitializeLines();
        if (PlayerPrefs.HasKey("AISuccessRate"))
        {
            IQ = PlayerPrefs.GetInt("AISuccessRate");
        }
        else
        {
            IQ = 100;
        }
        ChessGameManager.Instance.CallAI += BasicCheck;
    }

    public void InitializeLines()
    {
        lines = new LineOnBoard[8];
        for (int i = 0; i < 3; i++)
        {
            int[][] lineToAdd = new int[3][];
            for (int j = 0; j < 3; j++)
            {
                int[] pointInLine = new int[2] { i, j };
                lineToAdd[j] = pointInLine;
            }
            lines[i] = new LineOnBoard(lineToAdd);
        }
        for (int i = 0; i < 3; i++)
        {
            int[][] lineToAdd1 = new int[3][];
            for (int j = 0; j < 3; j++)
            {
                int[] pointInLine = new int[2] { j, i };
                lineToAdd1[j] = pointInLine;
            }
            lines[i + 3] = new LineOnBoard(lineToAdd1);
        }
        int[][] LineToAdd = new int[3][];
        for (int i = 0; i < 3; i++)
        {
            LineToAdd[i] = new int[2] { i, i };
        }
        lines[6] = new LineOnBoard(LineToAdd);
        int[][] LineToAdd1 = new int[3][];
        for (int i = 0; i < 3; i++)
        {
            LineToAdd1[i] = new int[2] { i, 2 - i };
        }
        lines[7] = new LineOnBoard(LineToAdd1);
    }

    public void BasicCheck(object sender, EventArgs e)
    {
        bool ifHaveResult = false;
        foreach(var line in lines)//能不能胜利
        {
            if(line.GetLineValue() == -2)
            {
                ifHaveResult = true;
                int[] point = line.GetFirstFreeBlock();
                ChessGameManager.Instance.AddChess(false, point[0], point[1]);
                break;
            }
        }
        if (!ifHaveResult)//对手能不能胜利
        {
            foreach (var line in lines)
            {
                if (line.GetLineValue() == 2)
                {
                    ifHaveResult = true;
                    int[] point = line.GetFirstFreeBlock();
                    ChessGameManager.Instance.AddChess(false, point[0], point[1]);
                    break;
                }
            }
        }
        if (!ifHaveResult && IQCheck())
        {
            Check();
        }else if (!ifHaveResult)
        {
            int[] point = GetRandomFreePoint();
            ChessGameManager.Instance.AddChess(false, point[0], point[1]);
        }
    }

    public void Check()
    {
        switch (ChessGameManager.Instance.turnCount) {
            case 0:
                //角最优，边，中心其次
                int selectionTemp = UnityEngine.Random.Range(0, 10);
                if (selectionTemp < 6)
                {
                    ChessGameManager.Instance.AddChess(false, UnityEngine.Random.Range(0, 2)*2, UnityEngine.Random.Range(0, 2) * 2);//角
                }else if(selectionTemp < 8)
                {
                    ChessGameManager.Instance.AddChess(false, 1, 1);//中心
                }
                else
                {
                    int[] point = GetRandomSide();
                    ChessGameManager.Instance.AddChess(false, point[0], point[1]);//边
                }
                break;
            case 1:
                int[] lastPoint = new int[2];
                for(int i=0; i<3; i++)
                {
                    for(int j=0; j<3; j++)
                    {
                        if (ChessGameManager.Instance.ChessBoard[i][j]==1)
                        {
                            lastPoint = new int[2] {i, j};
                        }
                    }
                }
                if (Math.Abs(lastPoint[0] - lastPoint[1]) == 1)//边
                {
                    List<int[]> possiblePoints = new List<int[]>();
                    for(int i=0; i<3; i++)
                    {
                        if (i != lastPoint[0])
                        {
                            possiblePoints.Add(new int[2] { i, lastPoint[1] });
                        }
                        if (i != lastPoint[1]){
                            possiblePoints.Add(new int[2] { lastPoint[0], i });
                        }
                    }
                    int[] point = possiblePoints[UnityEngine.Random.Range(0, possiblePoints.Count)];
                    ChessGameManager.Instance.AddChess(false, point[0], point[1]);
                }
                else if (lastPoint[0]==1 && lastPoint[1] == 1)//中心
                {
                    ChessGameManager.Instance.AddChess(false, UnityEngine.Random.Range(0, 2) * 2, UnityEngine.Random.Range(0, 2) * 2);
                }
                else//角落
                {
                    ChessGameManager.Instance.AddChess(false, 1, 1);
                }
                break;
            case 2:
                int[] aiPoint = new int[2];
                int[] playerPoint = new int[2];
                for (int i = 0; i < 3; i++)//取得双方下棋位置
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (ChessGameManager.Instance.ChessBoard[i][j] == 1)
                        {
                            playerPoint = new int[2] { i, j };
                        }
                        else if (ChessGameManager.Instance.ChessBoard[i][j] == -1)
                        {
                            aiPoint = new int[2] { i, j };
                        }
                    }
                }
                if (aiPoint[0]==1 && aiPoint[1] == 1)//中心
                {
                    if (Math.Abs(playerPoint[0] - playerPoint[1]) == 1)
                    {
                        List<int[]> possiblePoints = new List<int[]>
                        {
                            new int[2] { 1 + Math.Abs(1 - playerPoint[1]), 1 + Math.Abs(1 - playerPoint[0]) },
                            new int[2] { 1 - Math.Abs(1 - playerPoint[1]), 1 - Math.Abs(1 - playerPoint[0]) }
                        };
                        int[] randomPoint = possiblePoints[UnityEngine.Random.Range(0, possiblePoints.Count)];
                        ChessGameManager.Instance.AddChess(false, randomPoint[0], randomPoint[1]);
                    }
                    else//看情况
                    {
                        AdvancedCheck();
                    }
                }
                else if (Math.Abs(aiPoint[0] - aiPoint[1]) == 1)//边
                {
                    if ((Math.Abs(aiPoint[0] - playerPoint[0]) == 1 && Math.Abs(aiPoint[1] - playerPoint[1]) == 2) || (Math.Abs(aiPoint[0] - playerPoint[0]) == 2 && Math.Abs(aiPoint[1] - playerPoint[1]) == 1))
                    {
                        int xDifference = Math.Abs(aiPoint[0] - playerPoint[0]);
                        if (xDifference==1)
                        {
                            ChessGameManager.Instance.AddChess(false, playerPoint[0], aiPoint[1]);
                        }
                        else
                        {
                            ChessGameManager.Instance.AddChess(false, aiPoint[0], playerPoint[1]);
                        }
                    }
                    else if ((Math.Abs(aiPoint[0] - playerPoint[0]) == 1 && Math.Abs(aiPoint[1] - playerPoint[1]) == 1))
                    {
                        ChessGameManager.Instance.AddChess(false, 1, 1);
                    }
                    else//随机,但不能走上次AI下的边对面的边
                    {
                        List<int[]> possiblePoints = GetFreePoints();
                        int[] oppositeOfLastStep = new int[2] { Math.Abs(aiPoint[0]-2), Math.Abs(aiPoint[1] - 2) };
                        for(int i=0; i<possiblePoints.Count; i++)
                        {
                            if (oppositeOfLastStep.SequenceEqual(possiblePoints[i]))
                            {
                                possiblePoints.RemoveAt(i);
                                break;
                            }
                        }
                        int[] randomPoint = possiblePoints[UnityEngine.Random.Range(0, possiblePoints.Count)];
                        ChessGameManager.Instance.AddChess(false, randomPoint[0], randomPoint[1]);
                    }
                }
                else//角
                {
                    if (playerPoint[0]!=1 || playerPoint[1] != 1)
                    {
                        if (aiPoint[1] == playerPoint[1])
                        {
                            ChessGameManager.Instance.AddChess(false, aiPoint[0], Math.Abs(aiPoint[1] - 2));
                        }
                        else if (aiPoint[0] == playerPoint[0])
                        {
                            ChessGameManager.Instance.AddChess(false , Math.Abs(aiPoint[0] - 2), aiPoint[1]);
                        }
                        else
                        {
                            int randomNum = UnityEngine.Random.Range(0, 2);
                            ChessGameManager.Instance.AddChess(false, Math.Abs(aiPoint[0] - 2 * (randomNum)), Math.Abs(aiPoint[1] - 2 * (1 - randomNum)));
                        }
                    }
                    else//看情况
                    {
                        AdvancedCheck();
                    }
                }
                break;
            case 7:
                int[] point7 = GetRandomFreePoint();
                ChessGameManager.Instance.AddChess(false, point7[0], point7[1]);
                break;
            case 8:
                int[] point8 = GetRandomFreePoint();
                ChessGameManager.Instance.AddChess(false, point8[0], point8[1]);
                break;
            case 9:
                Debug.Log("棋盘已满");
                break;
            default:
                if (ChessGameManager.Instance.turnCount <= 9)
                {
                    AdvancedCheck();
                }
                break;
        }
    }

    public void AdvancedCheck()
    {
        bool ifPointSelected = false;
        List<int[]> possiblePoint = GetPointCanConstructTwoChoices(true);
        if (possiblePoint.Count!=0)//AI能做两条2子线
        {
            ifPointSelected = true;
            ChessGameManager.Instance.AddChess(false, possiblePoint[0][0], possiblePoint[0][1]);
        }
        else//判断玩家是不是能做两条2子线
        {
            possiblePoint = GetPointCanConstructTwoChoices(false);
            if (possiblePoint.Count == 1)//只能做一组
            {
                ifPointSelected = true;
                ChessGameManager.Instance.AddChess(false, possiblePoint[0][0], possiblePoint[0][1]);
            }
            else if (possiblePoint.Count > 1)//能做两组,AI先做2子线压制
            {
                List<LineOnBoard> possibleLines = GetPossibleLines();
                int playerPointCount;
                int[] tempPoint = null;
                List<int[]> noDangerousPointRecord = new List<int[]>();
                foreach (var line in possibleLines)
                {
                    playerPointCount = 0;
                    foreach (var point in possiblePoint)
                    {
                        if (line.IncludePoint(point))
                        {
                            playerPointCount++;
                            tempPoint = point;
                        }
                    }
                    if (playerPointCount == 1 && tempPoint != null)//当前线上只有一个危险点
                    {
                        ifPointSelected = true;
                        ChessGameManager.Instance.AddChess(false, tempPoint[0], tempPoint[1]);
                        break;
                    }else if (playerPointCount == 0)
                    {
                        noDangerousPointRecord.Add(line.GetFirstFreeBlock());
                    }
                }
                if (!ifPointSelected && noDangerousPointRecord.Count>0)
                {
                    int[] point = noDangerousPointRecord[UnityEngine.Random.Range(0, noDangerousPointRecord.Count)];
                    ifPointSelected = true;
                    ChessGameManager.Instance.AddChess(false, point[0], point[1]);
                }
                else if(!ifPointSelected)
                {
                    int[] point = possibleLines[UnityEngine.Random.Range(0, possibleLines.Count)].GetFirstFreeBlock();
                    ifPointSelected = true;
                    ChessGameManager.Instance.AddChess(false, point[0], point[1]);
                }
            }
        }
        if (!ifPointSelected)//双方均无双2子线
        {
            int[] point = GetRandomFreePoint();
            List<LineOnBoard> AIPossibleLine = GetPossibleLines();
            if (AIPossibleLine.Count > 0)
            {
                point = AIPossibleLine[UnityEngine.Random.Range(0, AIPossibleLine.Count)].GetRandomFreeBlock();
            }
            else
            {
                AIPossibleLine = GetLinesUnusedByPlayer();
                if(AIPossibleLine.Count > 0)
                {
                    point = AIPossibleLine[UnityEngine.Random.Range(0, AIPossibleLine.Count)].GetRandomFreeBlock();
                }
            }
            ChessGameManager.Instance.AddChess(false, point[0], point[1]);
        }
    }

    #region Gets

    public List<int[]> GetFreePoints()
    {
        List<int[]> freePoints = new List<int[]>();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (ChessGameManager.Instance.ChessBoard[x][y] == 0)
                {
                    freePoints.Add(new int[2] { x, y });
                }
            }
        }
        return freePoints;
    }

    public int[] GetRandomFreePoint()
    {
        int[] result = null;
        List<int[]> freePoints = GetFreePoints();
        if (freePoints.Count == 0)
        {
            Debug.Log("无未被占用的格子");
        }
        else
        {
            result = freePoints[UnityEngine.Random.Range(0, freePoints.Count)];
        }
        return result;
    }

    public List<int[]> GetPointCanConstructTwoChoices(bool ifAI)
    {
        List<int[]> result = new List<int[]>();
        switch (ifAI)
        {
            case true:
                List<LineOnBoard> possibleLines = GetPossibleLines();
                for(int pivot = 0; pivot < possibleLines.Count; pivot++) {
                    for(int pivot1 = pivot + 1; pivot1<possibleLines.Count; pivot1++)
                    {
                        int[] intersectionPoint = GetIntersectPoint(possibleLines[pivot], possibleLines[pivot1]);
                        if (intersectionPoint!=null && ChessGameManager.Instance.ChessBoard[intersectionPoint[0]][intersectionPoint[1]]==0)
                        {
                            result.Add(intersectionPoint);
                        }
                    }
                }
                break;
            case false:
                List<LineOnBoard> possibleLines_Player = GetPlayerPossibleLines();
                for (int pivot = 0; pivot < possibleLines_Player.Count; pivot++)
                {
                    for (int pivot1 = pivot + 1; pivot1 < possibleLines_Player.Count; pivot1++)
                    {
                        int[] intersectionPoint = GetIntersectPoint(possibleLines_Player[pivot], possibleLines_Player[pivot1]);
                        if (intersectionPoint != null && ChessGameManager.Instance.ChessBoard[intersectionPoint[0]][intersectionPoint[1]] == 0)
                        {
                            result.Add(intersectionPoint);
                        }
                    }
                }
                break;
        }
        return result;
    }

    public int[] GetRandomNeighbor(int[] inPut)
    {
        int x = inPut[0];
        int y = inPut[1];
        List<int[]> freePoints = new List<int[]>();
        for(int i=-1; i<=1; i+=2)
        {
            for(int j=-1; j<=1; j += 2)
            {
                if (x + i >= 0 &&  x + i <= 2 && y + j >= 0 && y + j <= 2)
                {
                    if (ChessGameManager.Instance.ChessBoard[x + i][y + j] == 0)
                    {
                        freePoints.Add(new int[2] { x + i, y + j });
                    }
                }
            }
        }
        int[] result = null;
        if (freePoints.Count != 0)
        {
            result = freePoints[UnityEngine.Random.Range(0, freePoints.Count)];
        }
        else
        {
            Debug.Log("无未被占用的格子");
        }
        return result;
    }

    public int[] GetRandomSide()
    {
        int randomNum = UnityEngine.Random.Range(0, 4);
        int[] result = null;
        switch (randomNum)
        {
            case 0:
                result = new int[2] { 0, 1 };
                break;
            case 1:
                result = new int[2] { 1, 2 };
                break;
            case 2:
                result = new int[2] { 2, 1 };
                break;
            case 3:
                result = new int[2] { 1, 0 };
                break;
            default:
                Debug.Log("选取边时随机数越界");
                break;
        }
        return result;
    }

    public List<LineOnBoard> GetLinesUnusedByPlayer()
    {
        List<LineOnBoard> result = new List<LineOnBoard>();
        foreach(var line in lines)
        {
            bool ifThisLineUsed = false;
            for(int i=0; i<3; i++)
            {
                if (ChessGameManager.Instance.ChessBoard[line.points[i][0]][line.points[i][1]]==1)
                {
                    ifThisLineUsed = true;
                    break;
                }
            }
            if (!ifThisLineUsed)
            {
                result.Add(line);
            }
        }
        return result;
    }

    public List<LineOnBoard> GetLinesUsedByPlayer()
    {
        List<LineOnBoard> result = new List<LineOnBoard>();
        foreach (var line in lines)
        {
            for (int i = 0; i < 3; i++)
            {
                if (ChessGameManager.Instance.ChessBoard[line.points[i][0]][line.points[i][1]] == 1)
                {
                    result.Add(line);
                    break;
                }
            }
        }
        return result;
    }

    public List<LineOnBoard> GetPlayerPossibleLines()
    {
        List<LineOnBoard> result = GetLinesUsedByPlayer();
        for (int a=0; a<result.Count; a++)
        {
            for (int i = 0; i < 3; i++)
            {
                if (ChessGameManager.Instance.ChessBoard[result[a].points[i][0]][result[a].points[i][1]] == -1)
                {
                    result.RemoveAt(a);
                    a--;
                    break;
                }
            }
        }
        return result;
    }

    public List<LineOnBoard> GetPossibleLines()
    {
        List<LineOnBoard> result = GetLinesUnusedByPlayer();
        for(int a = 0; a<result.Count; a++)
        {
            bool ifThisLineUsed = false;
            for (int i = 0; i < 3; i++)
            {
                if (ChessGameManager.Instance.ChessBoard[result[a].points[i][0]][result[a].points[i][1]] == -1)
                {
                    ifThisLineUsed = true;
                    break;
                }
            }
            if (!ifThisLineUsed)
            {
                result.Remove(result[a]);
                a--;
            }
        }
        return result;
    }

    public int[] GetIntersectPoint(LineOnBoard line1, LineOnBoard line2)
    {
        int[] result = null;
        foreach(var point in line1.points)
        {
            if (line2.IncludePoint(point))
            {
                result = point;
                break;
            }
        }
        return result;
    }

    #endregion

    public bool IQCheck()
    {
        bool result = false;
        if(UnityEngine.Random.Range(0f, 100f) < IQ)
        {
            result = true;
        }
        return result;
    }
}
