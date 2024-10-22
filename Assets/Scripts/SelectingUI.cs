using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectingUI : MonoBehaviour
{
    public static SelectingUI instance;
    public GameObject selectionBoxPrefab; // Prefab for the selection box
    private GameObject selectionBox;
    private Vector2Int dragStart;
    public bool isDragging = false;
    private Vector3 cellSize;

    private Grid grid;
    private List<GameObject> previewZones;
    

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        grid = FindObjectOfType<Grid>();
        previewZones = new List<GameObject>();
        selectionBox = selectionBoxPrefab;


        // Get the size of a single cell in the grid
        cellSize = grid.cellSize;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = grid.WorldToCell(mousePosition);
            dragStart = new Vector2Int(cellPosition.x, cellPosition.y);
            isDragging = true;

            // Activate the selection box
            selectionBox.SetActive(true);
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = grid.WorldToCell(mousePosition);
            Vector2Int currentDragPosition = new Vector2Int(cellPosition.x, cellPosition.y);
            UpdateSelectionBox(dragStart, currentDragPosition);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;

            // Deactivate the selection box
            selectionBox.SetActive(false);
        }
    }

    void UpdateSelectionBox(Vector2Int start, Vector2Int end)
    {
        // Calculate the size and position of the selection box
        Vector2Int size = new Vector2Int(Mathf.Abs(end.x - start.x) + 1, Mathf.Abs(end.y - start.y) + 1);
        Vector3 position = new Vector3((start.x + end.x) / 2f, (start.y + end.y) / 2f, 0);
        Vector3 scale = new Vector3(size.x * cellSize.x, size.y * cellSize.y, 1);

        // Update the selection box's position and scale
        selectionBox.transform.position = grid.CellToWorld(new Vector3Int(start.x, start.y, 0)) + new Vector3(cellSize.x / 2, cellSize.y / 2, 0);
        selectionBox.transform.localScale = scale;
    }
}