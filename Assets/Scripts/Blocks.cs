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
        ChessGameManager.Instance.AddChess(ChessGameManager.Instance.ifPlayerOperating, x, y);
    }
}
