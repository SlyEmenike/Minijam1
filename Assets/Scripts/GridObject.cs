using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public GridCell currentCell;

    //Snap to current cell

    public virtual void Start()
    {
        currentCell = GridManager.Instance.WorldPosToGridCell(transform.position);
        transform.position = GridManager.CellToWorldPos(currentCell);
    }
    
    public void SnapToMouseCell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        float rowPos = transform.position.z;
        float colPos = transform.position.x;
        if (Physics.Raycast(ray, out hit,40f, 1 << LayerMask.NameToLayer("Grid")))
        {
            // some point of the plane was hit - get its coordinates
            Vector3 point = hit.point;
            rowPos = Mathf.Floor(point.z) + 0.475f;
            colPos = Mathf.Floor(point.x) + 0.525f;
        }
        
        transform.position = new Vector3(colPos, 0, rowPos);
        currentCell = GridManager.Instance.WorldPosToGridCell(transform.position);
    }

    public void SetToCellPosition(GridCell gc)
    {
        currentCell = gc;
        transform.position = GridManager.CellToWorldPos(currentCell);
    }
}
