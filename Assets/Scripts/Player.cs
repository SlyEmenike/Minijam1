using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GridObject {
    public enum PlayerNumber { Player1, Player2, Player3, Player4, None }
    public PlayerNumber playerNumber;

    public List<GridCell> walkPath;
    List<GridCell> walkableCells;
    [SerializeField]
    WalkIndicator walkIndicator;
    List<WalkIndicator> walkIndicators;

    public GridCell cellToSelectFrom;

    [SerializeField]
    float moveSpeedInSeconds;

    int ordersAvailable;


    public int currentPoints { get; set; }

    // Use this for initialization
    public override void Start () {
        base.Start();
        walkPath = new List<GridCell>();
        walkIndicators = new List<WalkIndicator>();
        ordersAvailable = GameManager.Instance.walkOrders;

    }
	
	// Update is called once per frame
	void Update () {

	}

    public void startWalkPhase()
    {
        walkPath.Clear();
        walkPath.Add(currentCell);
        cellToSelectFrom = currentCell;
        Debug.Log("cellToSelectFrom:" + cellToSelectFrom.columnPos + "/ " + cellToSelectFrom.rowPos);
        walkableCells = GridManager.Instance.WalkableCellsFromPlayer(this);
    }

    public void WaitForNextWalkCell()
    {
        //highlight walkable cells
        
        GridCell mouseOnCell;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 40f, 1 << LayerMask.NameToLayer("Grid")))
        {
            // some point of the plane was hit - get its coordinates
            Vector3 point = hit.point;
            mouseOnCell = GridManager.Instance.WorldPosToGridCell(point);
            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < walkableCells.Count; i++)
                {
                    if(walkableCells[i] == mouseOnCell)
                    {
                        SelectCell(mouseOnCell);
                        
                    }
                }
            }
        }
        //If clicked on walkable cell go to next or end
    }

    void SelectCell(GridCell cell)
    {
        walkPath.Add(cell);
        WalkIndicator w = Instantiate(walkIndicator);
        w.Init(cellToSelectFrom, cell);
        walkIndicators.Add(w);

        if(walkPath.Count > ordersAvailable)
        {
            EndSelect();
        }
        else
        {
            cellToSelectFrom = cell;
            walkableCells.Clear();
            walkableCells = GridManager.Instance.WalkableCellsFromPlayer(this);
        }
    }

    //If player is done with selecting his path
    void EndSelect()
    {
        GameManager.Instance.EndMovePhase();
    }

    public IEnumerator MoveAlongPath()
    {
        for (int i = 0; i < walkIndicators.Count; i++)
        {
            walkIndicators[i].gameObject.SetActive(true);
        }
        //Check if cell ahead is free
        for (int i = 0; i < walkPath.Count; i++)
        {

            currentCell.currentPlayer = null;
            if (walkPath[i].currentWall == null && walkPath[i].currentPlayer == null)
            {
                yield return StartCoroutine(MoveToNextCell(walkPath[i]));
                currentCell.currentPlayer = this;
            }
            else
            {
                currentCell.currentPlayer = this;
                break;
            }
        }
        yield return null;
    }

    IEnumerator MoveToNextCell(GridCell gc)
    {
        bool checkedIfCoin = false;
        Vector3 endPos = GridManager.CellToWorldPos(gc);
        Vector3 startPos = transform.position;
        currentCell = GridManager.Instance.WorldPosToGridCell(endPos);

        float elapsedTime = 0;
        while (elapsedTime < moveSpeedInSeconds)
        {
            if(elapsedTime >= moveSpeedInSeconds / 2f && !checkedIfCoin)
            {
                checkedIfCoin = true;
                //Check if cell containts coin
                if (currentCell.currentCoin != null)
                {
                    //add point
                    currentPoints += currentCell.currentCoin.value;
                    currentCell.currentCoin.PickedUp();
                }
            }
            transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime / moveSpeedInSeconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPos;
        
        yield return null;
    }
    public void EndTurn()
    {
        for (int i = 0; i < walkIndicators.Count; i++)
        {
            walkIndicators[i].gameObject.SetActive(false);
        }
    }
    public void OnEndRound()
    {
        walkPath.Clear();
        for (int i = 0; i < walkIndicators.Count; i++)
        {
            Destroy(walkIndicators[i].gameObject);
        }
        walkIndicators.Clear();
    }
}
