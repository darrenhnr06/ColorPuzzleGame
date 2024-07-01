using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BottomPanelManager : MonoBehaviour
{
    [Header("Bottom Grid")]
    [SerializeField] private GameObject bottomCellPrefab;
    [SerializeField] private int numCols = 4;
    [SerializeField] private float cellSize = 1f;

    [Header("Color Cell")]
    [SerializeField] private GameObject colorCellPrefab;
    [SerializeField] private Material[] colorCellMaterial;
    [SerializeField] private string[] colorCellTags;

    [Header("Script References")]
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private DragHandler dragHandler;

    private Vector3 startPosition;
    private List<GameObject> bottomGridCells = new List<GameObject>();
    private Queue<GameObject> colorCellQueue = new Queue<GameObject>();
    private Queue<GameObject> colorCellPool = new Queue<GameObject>();

    public Queue<GameObject> ColorCellPool { get { return colorCellPool; } set { colorCellPool = value; } }

    void Start()
    {
        startPosition = transform.position;
        GenerateBottomGrid();
    }

    void GenerateBottomGrid()
    {
        for (int col = 0; col < numCols; col++)
        {
            Vector3 cellPosition = startPosition + new Vector3(col * cellSize, 0, 0);

            GameObject gridCell = Instantiate(bottomCellPrefab, cellPosition, Quaternion.identity, transform);
            gridCell.name = gridCell.tag;
            bottomGridCells.Add(gridCell);
        }
        PlaceRandomColorCells();
    }

    public void PlaceRandomColorCells()
    {
        int numberOfCellsToPlace = Random.Range(1, bottomGridCells.Count); 

        for (int i = 0; i < numberOfCellsToPlace; i++)
        {
            int colorIndex = Random.Range(0, colorCellMaterial.Length);
            Vector3 cellPosition = bottomGridCells[i].transform.position + new Vector3(0, 0.1f, 0.05f);
            GameObject colorCell;

            if (colorCellPool.Count > 0)
            {
                colorCell = colorCellPool.Dequeue();
                colorCell.SetActive(true);
                colorCell.transform.position = cellPosition;
            }
            else
            {
                colorCell = Instantiate(colorCellPrefab, cellPosition, Quaternion.identity, transform);
            }

            colorCell.GetComponent<MeshRenderer>().material = colorCellMaterial[colorIndex];
            colorCell.name = colorCell.tag = colorCellTags[colorIndex];
            colorCellQueue.Enqueue(colorCell);
        }
        if ((gridGenerator.GridCells.Length - dragHandler.ColorCellsInGrid.Count) < colorCellQueue.Count)
        {
            EndGame();
        }

    }

    private void EndGame()
    {
        GamePlayAttemptStat.gamePlayAttempted = true;
        SceneManager.LoadScene("HomeScene");
    }

    public bool HasColorCells()
    {
        return colorCellQueue.Count > 0;
    }

    public GameObject GetNextColorCell()
    {
        if (colorCellQueue.Count > 0)
        {
            return colorCellQueue.Dequeue();
        }
        return null;
    }

    public void ReturnPlacedCells(List<GameObject> placedCells)
    {
        int colorIndex = 0;
        List<GameObject> tempQueue = new List<GameObject>(colorCellQueue);

        foreach (GameObject cell in placedCells)
        {
            cell.transform.position = bottomGridCells[colorIndex].transform.position + new Vector3(0, 0.1f, 0.05f);
            tempQueue.Insert(0, cell);
            colorIndex++;
        }

        colorCellQueue = new Queue<GameObject>(tempQueue);
        placedCells.Clear();
    }
}
