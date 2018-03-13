using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell{

    public int rowPos, columnPos;

    public Player currentPlayer;
    public Wall currentWall { get; set; }
    public Coin currentCoin { get; set; }

    public bool isEmpty { get { return currentPlayer == null && currentWall == null && currentCoin == null; } }
    public Vector3 snapPosition;

}
