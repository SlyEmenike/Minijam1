using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : GridObject {
    public enum WallPhase { selectPosition, ghostPlaced, building, build}
    public WallPhase currentPhase;

    public Player.PlayerNumber owner;

    public bool isGonnaBeRemoved;

    Renderer myRenderer;

    Color standardColor;
    [SerializeField]
    Color ghostBuildColor, deleteColor;

    [SerializeField]
    float dropHeight, dropSeconds;

    [SerializeField]
    float destroyTime = 2f;
    // Use this for initialization

    private void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        currentPhase = WallPhase.build;
        standardColor = myRenderer.material.color;
    }

    public override void Start()
    {
        if (owner == Player.PlayerNumber.Player1 || owner == Player.PlayerNumber.Player2 || owner == Player.PlayerNumber.Player3 || owner == Player.PlayerNumber.Player4)
        {

        }
        else
        {
            base.Start(); //Set Cell
            AddToCell();
        }
    }

    private void Update()
    {
        switch (currentPhase)
        {
            case WallPhase.selectPosition:
                myRenderer.material.color = ghostBuildColor;
                SnapToMouseCell();
                if (Input.GetMouseButtonDown(0))
                {
                    Place();
                }
                break;
            case WallPhase.ghostPlaced:
                break;
            case WallPhase.build:
                this.gameObject.SetActive(true);
                break;

        }
    }

    public void StartSetPosition()
    {
        currentPhase = WallPhase.selectPosition;
    }

    void Place()
    {
        //If current cell is available
        if (currentCell.currentWall == null && currentCell.currentCoin == null)
        {
            currentPhase = WallPhase.ghostPlaced;
            GameManager.Instance.EndBuildPhase();
        }
        else
        {
            Debug.Log("Wall onspot");
        }
    }

    public void BeingRemoved()
    {
        isGonnaBeRemoved = true;
        myRenderer.material.color = deleteColor;
    }

    public IEnumerator PlaceWall()
    {
        Debug.Log("placing wall");
        myRenderer.enabled = true;
        myRenderer.material.color = standardColor;
        currentPhase = Wall.WallPhase.build;
        if(GridManager.Instance.CellAtGrid(currentCell).currentWall == null)
        {
            yield return StartCoroutine(StartBuildAnimation());
            AddToCell();
        }
        else
        {
            //Wall on grid, this cant be placed
            Debug.Log("Destroying wall");
            Destroy(this.gameObject);
        }
        yield return null;
    }
    
    IEnumerator StartBuildAnimation()
    {
        Vector3 endPos = transform.position;
        transform.position += new Vector3(0, dropHeight, 0);
        Vector3 startPos = transform.position;
        float elapsedTime = 0;
        while (elapsedTime < dropSeconds)
        {
            transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime / dropSeconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPos;
        transform.position = GridManager.CellToWorldPos(currentCell);

        yield return null;
    }
    public IEnumerator RemoveWall()
    {
        yield return StartCoroutine(StartRemoveAnim());
        Destroy(this.gameObject);
    }
    IEnumerator StartRemoveAnim()
    {
        Vector3 endPos = transform.position + new Vector3(0,dropHeight,0);
        Vector3 startPos = transform.position;
        float elapsedTime = 0;
        Color c = myRenderer.material.color;
        while (elapsedTime < destroyTime)
        {
            c.a = Mathf.Lerp(255, 0, (elapsedTime / dropSeconds));
            myRenderer.material.color = c;
            transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime / dropSeconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    public void EndTurn()
    {
        if (isGonnaBeRemoved)
        {
            myRenderer.material.color = standardColor;
        }
        else
        {//being build
            myRenderer.enabled = false;
        }
    }

    void AddToCell()
    {
        currentCell.currentWall = this;
        GridManager.Instance.cellsWithoutWalls.Remove(GridManager.Instance.CellAtGrid(currentCell));
    }

    void RemoveFromCell()
    {
        GridManager.Instance.cellsWithoutWalls.Add(GridManager.Instance.CellAtGrid(currentCell));
    }

    private void OnDestroy()
    {
        RemoveFromCell();
    }
}
