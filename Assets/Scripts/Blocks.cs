using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Blocks : MonoBehaviour, IPointerDownHandler
{
    public int x;
    public int y;
    

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!ChessGameManager.Instance.ifVersusAI || ChessGameManager.Instance.ifPlayerOperating)
        ChessGameManager.Instance.AddChess(ChessGameManager.Instance.ifPlayerOperating, x, y);
    }
}
