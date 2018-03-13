using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// This handles the grid and its cells. 
/// It checks if a cell is empty or occupied by a wall or player.
/// </summary>
public class GridManager : MonoBehaviour {

    [SerializeField]
    public int rows, columns;
    [SerializeField] [Range(0.1f, 5f)]
    float gridSize = 1f;

    [SerializeField]
    Grid gridPlane;

    public GridCell[,] grid { get; set; }
    public GridHighlighter topHL, rightHL, downHL, leftHL;

    public List<GridCell> cellsWithoutCoins;
    public List<GridCell> cellsWithoutWalls;

    private static GridManager _instance;
    public static GridManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GridManager();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else if (_instance != this) Destroy(gameObject);

        grid = new GridCell[columns, rows];
        cellsWithoutCoins = new List<GridCell>();
        cellsWithoutWalls = new List<GridCell>();

        Debug.Log("Columns: " + grid.GetLength(0) + " Rows: " + grid.GetLength(1));
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = new GridCell();
                grid[i, j].columnPos = i;
                grid[i, j].rowPos = j;
                cellsWithoutCoins.Add(grid[i, j]);
                cellsWithoutWalls.Add(grid[i, j]);
            }
        }
    }

    void Start () {        
        SetGridPlane();
    }

    void SetGridPlane()
    {
        //Size: 1 = 10 cells
        float rowSize = rows/10f;
        float columnSize = columns/10f;

        gridPlane.transform.localScale = new Vector3(columnSize, 1, rowSize);
        gridPlane.transform.position = new Vector3(columns / 2, 0, -(rows / 2));
    }
    /// <summary>
    /// Get all walkable celss adjacent from player p
    /// </summary>
    /// <param name="p"></param>
    /// <returns> A list<GridCell> with walkable adjacent cells </GridCell></returns>
    public List<GridCell> WalkableCellsFromPlayer(Player p)
    {
        List<GridCell> walkableCells = new List<GridCell>();
        GridCell topCell = null, rightCell = null, downCell = null, leftCell = null;
        try
        {
            topCell = grid[p.cellToSelectFrom.columnPos, p.cellToSelectFrom.rowPos - 1];
        }
        catch (Exception e)
        {
            topCell = null;
        }

        try
        {
            rightCell = grid[p.cellToSelectFrom.columnPos + 1, p.cellToSelectFrom.rowPos];
        }
        catch (Exception e)
        {
            rightCell = null;
        }
        try
        {
            downCell = grid[p.cellToSelectFrom.columnPos, p.cellToSelectFrom.rowPos + 1];
        }
        catch (Exception e)
        {
            downCell = null;
        }
        try
        {
            leftCell = grid[p.cellToSelectFrom.columnPos - 1, p.cellToSelectFrom.rowPos];
        }
        catch (Exception e)
        {
            leftCell = null;
        }
        //Top

        if (topCell != null)
        {
            topHL.gameObject.SetActive(true);
            topHL.transform.position = CellToWorldPos(topCell);
            walkableCells.Add(topCell);
        }
        else topHL.gameObject.SetActive(false);

        if (rightCell != null)
        {
            rightHL.gameObject.SetActive(true);
            rightHL.transform.position = CellToWorldPos(rightCell);
            walkableCells.Add(rightCell);
        }
        else GridManager.Instance.rightHL.gameObject.SetActive(false);

        if (downCell != null)
        {
            downHL.gameObject.SetActive(true);
            downHL.transform.position = CellToWorldPos(downCell);
            walkableCells.Add(downCell);
        }
        else GridManager.Instance.downHL.gameObject.SetActive(false);

        if (leftCell != null)
        {
            leftHL.gameObject.SetActive(true);
            leftHL.transform.position = CellToWorldPos(leftCell);
            walkableCells.Add(leftCell);
        }
        else GridManager.Instance.leftHL.gameObject.SetActive(false);

        return walkableCells;
    }

    public GridCell WorldPosToGridCell(Vector3 pos)
    {
        int rowPos = ((int)Mathf.Floor(pos.z))*-1;
        int columnPos = (int)Mathf.Floor(pos.x);


        GridCell cellAtPos = GridManager.Instance.grid[columnPos, rowPos-1];
        return cellAtPos;
    }

    public static Vector3 CellToWorldPos(int colPos, int rowPos)
    {
        float xPos = colPos + 0.475f;
        float zPos = rowPos + 0.525f;

        return new Vector3(xPos,0,zPos);
    }
    public static Vector3 CellToWorldPos(GridCell gc)
    {
        float xPos = gc.columnPos + 0.475f;
        float zPos = -(gc.rowPos + 0.525f);

        return new Vector3(xPos, 0, zPos);
    }

    public GridCell CellAtGrid(GridCell gc)
    {
        return grid[gc.columnPos, gc.rowPos];
    }
}
