using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Main Grid")]
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private int numRows = 5;
    [SerializeField] private int numCols = 5;
    [SerializeField] private float cellSize = 1f;

    private Vector3 startPosition;
    private GameObject[,] gridCells;

    public GameObject[,] GridCells {  get { return gridCells; } }

    void Start()
    {
        startPosition = transform.position;
        gridCells = new GameObject[numRows, numCols];
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                Vector3 cellPosition = startPosition + new Vector3(col * cellSize, 0, row * cellSize);
                GameObject gridCell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform);
                gridCell.name = gridCell.tag;
                gridCells[row, col] = gridCell;
            }
        }
    }
}
