using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : GridObject {

    [SerializeField]
    public int value;

    [SerializeField]
    float removeSpeed, removeHeight;

	// Use this for initialization
	public override void Start () {
        base.Start();
        AddToCell();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PickedUp()
    {
        
        GridCell nextCell = GetNewCell();
        if (GridManager.Instance.cellsWithoutWalls.Contains(nextCell))
        {
            nextCell = null;
        }
        RemoveFromCell();
        

        //Start animation
        StartCoroutine(RemoveAnimation());
    }

    IEnumerator RemoveAnimation()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + new Vector3(0, removeHeight, 0);

        float elapsedTime = 0;
        while (elapsedTime < removeSpeed)
        {
            transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime / removeSpeed));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPos;
        yield return null;
    }

    IEnumerator ReplaceAnimation()
    {
        //TO DO:L REPLACE LOGIC
        yield return null;
    }
    void AddToCell()
    {
        currentCell.currentCoin = this;
        GridManager.Instance.cellsWithoutCoins.Remove(GridManager.Instance.CellAtGrid(currentCell));
    }

    void RemoveFromCell()
    {
        currentCell.currentCoin = null;
        GridManager.Instance.cellsWithoutCoins.Add(GridManager.Instance.CellAtGrid(currentCell));
    }

    GridCell GetNewCell()
    {
        GridCell nextCell = GridManager.Instance.cellsWithoutCoins[Random.Range(0, GridManager.Instance.cellsWithoutCoins.Count)];

        return nextCell;
    }

}
