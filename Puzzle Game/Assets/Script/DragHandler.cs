using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DragHandler : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private BottomPanelManager bottomPanelManager;
    [SerializeField] private GridGenerator gridGenerator;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    private int score;
    private GameObject selectedColorCell;
    private List<GameObject> placedCells = new List<GameObject>();
    private List<GameObject> colorCellsInGrid = new List<GameObject>();
    private GameObject lastGridCell;

    public List<GameObject> ColorCellsInGrid {  get { return colorCellsInGrid; } }

    private void Start()
    {
        score = 0;
        scoreText.text = "Score: " + score.ToString();
        PlayerPrefs.SetInt("Score", score);
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseDown();
        }
        else if (Input.GetMouseButton(0) && selectedColorCell != null)
        {
            HandleMouseDrag();
        }
        else if (Input.GetMouseButtonUp(0) && selectedColorCell != null)
        {
            HandleMouseUp();
        }
    }

    void HandleMouseDown()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.CompareTag("Grid") && bottomPanelManager.HasColorCells())
            {
                selectedColorCell = bottomPanelManager.GetNextColorCell();
                if (selectedColorCell != null)
                {
                    MoveColorCellToGrid(hit.collider.gameObject);
                    lastGridCell = hit.collider.gameObject;
                }
            }
        }
    }

    void HandleMouseDrag()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.CompareTag("Grid"))
            {
                GameObject gridCell = hit.collider.gameObject;
                if (IsAdjacent(lastGridCell, gridCell) && !placedCells.Contains(gridCell) && bottomPanelManager.HasColorCells())
                {
                    selectedColorCell = bottomPanelManager.GetNextColorCell();
                    MoveColorCellToGrid(gridCell);
                    lastGridCell = gridCell;
                }
            }
        }
    }

    void HandleMouseUp()
    {
        if (bottomPanelManager.HasColorCells())
        {
            bottomPanelManager.ReturnPlacedCells(placedCells);
        }
        else
        {
            if (lastGridCell != null)
            {
                foreach(var cell in placedCells)
                {
                    CheckForMatches(cell);
                }                
            }
            colorCellsInGrid.AddRange(placedCells);
            placedCells.Clear();
            bottomPanelManager.PlaceRandomColorCells();
        }
        selectedColorCell = null;
        lastGridCell = null;
    }

    void MoveColorCellToGrid(GameObject gridCell)
    {
        selectedColorCell.transform.position = gridCell.transform.position + new Vector3(0, 0.1f, 0);
        placedCells.Add(selectedColorCell);
    }

    bool IsAdjacent(GameObject lastCell, GameObject newCell)
    {
        Vector3 lastPos = lastCell.transform.position;
        Vector3 newPos = newCell.transform.position;

        return (Mathf.Abs(lastPos.x - newPos.x) == 1f && lastPos.z == newPos.z) ||
               (Mathf.Abs(lastPos.z - newPos.z) == 1f && lastPos.x == newPos.x);
    }

    void CheckForMatches(GameObject startCell)
    {
        string colorTag = startCell.tag;
        HashSet<GameObject> matchedCells = new HashSet<GameObject>();
        FindMatches(startCell, colorTag, matchedCells);

        if (matchedCells.Count >= 4)
        {
            foreach (var cell in matchedCells)
            {
                cell.transform.DOScale(0, 0.5f).OnComplete(() =>
                {
                    cell.SetActive(false);
                    cell.transform.DOScale(1, 0.1f);
                    bottomPanelManager.ColorCellPool.Enqueue(cell);
                    colorCellsInGrid.Remove(cell);
                });
            }
            score += 10;
            scoreText.text = "Score: " + score.ToString();
            PlayerPrefs.SetInt("Score", score);
        }
    }

    void FindMatches(GameObject cell, string colorTag, HashSet<GameObject> matchedCells)
    {
        if (matchedCells.Contains(cell))
        {
            return;
        }

        matchedCells.Add(cell);

        Vector3[] directions = { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;
            if (Physics.Raycast(cell.transform.position, direction, out hit, 1f))
            {
                if (hit.collider != null && hit.collider.CompareTag(colorTag) && !matchedCells.Contains(hit.collider.gameObject))
                {
                    FindMatches(hit.collider.gameObject, colorTag, matchedCells);
                }
            }
        }
    }
}
