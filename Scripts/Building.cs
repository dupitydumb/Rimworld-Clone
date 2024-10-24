using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;
using UnityEngine.Events;

public class Building : MonoBehaviour
{
    public GameObject toPlace;
    private Dictionary<int, string> wallVariantDict = new Dictionary<int, string>();
    private Grid grid;
    public string SetWallName;
    private PlacementHolder placementHolder;
    public bool isHoverUI = false;
    public bool isPlacing = false;
    public BuildType buildType;
    private HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>(); // HashSet to store wall positions
    private HashSet<Vector3Int> floorPositions = new HashSet<Vector3Int>(); // HashSet to store floor positions
    private Dictionary<Vector3Int, GameObject> objects = new Dictionary<Vector3Int, GameObject>(); // Dictionary to store wall GameObjects

    GameManager gameManager;
    private Vector3Int startGridPos;
    private Vector3Int currentGridPos;
    private bool isDragging = false;
    private List<GameObject> currentPreview = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameManager.instance;
        grid = gameManager.grid;
        placementHolder = toPlace.GetComponent<PlacementHolder>();
        // Initialize the dictionary
        wallVariantDict.Add(0, "Wall 12");
        wallVariantDict.Add(1, "Wall 13");
        wallVariantDict.Add(2, "Wall 14");
        wallVariantDict.Add(3, "Wall 15");
        wallVariantDict.Add(4, "Wall 8");
        wallVariantDict.Add(5, "Wall 9");
        wallVariantDict.Add(6, "Wall 10");
        wallVariantDict.Add(7, "Wall 11");
        wallVariantDict.Add(8, "Wall 4");
        wallVariantDict.Add(9, "Wall 5");
        wallVariantDict.Add(10, "Wall 6");
        wallVariantDict.Add(11, "Wall 7");
        wallVariantDict.Add(12, "Wall 0");
        wallVariantDict.Add(13, "Wall 1");
        wallVariantDict.Add(14, "Wall 2");
        wallVariantDict.Add(15, "Wall 3");
    }

    // Update is called once per frame
     void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left Mouse Button
        {
            isDragging = true;
            startGridPos = GetGridPositionFromMouse();
            currentGridPos = startGridPos;
            ShowWallPreview(startGridPos);
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3Int newGridPos = GetGridPositionFromMouse();
            if (newGridPos != currentGridPos)
            {
                currentGridPos = newGridPos;
                ShowWallPreview(currentGridPos);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            PlaceWalls(startGridPos, currentGridPos);
            foreach (GameObject preview in currentPreview)
            {
                Destroy(preview);
            }
        }
    }

    Vector3Int GetGridPositionFromMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return grid.WorldToCell(mouseWorldPos);
    }

    void ShowWallPreview(Vector3Int gridPos)
    {
        Vector3 worldPos = grid.GetCellCenterWorld(gridPos);
        int bitmask = GetWallBitmask(gridPos);
        GameObject preview = Instantiate(Resources.Load("Prefabs/Wall" + SetWallName + wallVariantDict[bitmask]) as GameObject, worldPos, Quaternion.identity);
        // Set prevew z position to -1 so it is rendered behind other objects
        preview.transform.position = new Vector3(preview.transform.position.x, preview.transform.position.y, 3);
        currentPreview.Add(preview);

        UpdateWallSprite(gridPos + Vector3Int.up);
        UpdateWallSprite(gridPos + Vector3Int.right);
        UpdateWallSprite(gridPos + Vector3Int.down);
        UpdateWallSprite(gridPos + Vector3Int.left);
    }

    void PlaceWalls(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> positions = GetGridPositionsBetween(start, end);
        foreach (Vector3Int pos in positions)
        {
            if (!wallPositions.Contains(pos))
            {
                Vector3 worldPos = grid.GetCellCenterWorld(pos);
                //Get bitmask of the wall
                int bitmask = GetWallBitmask(pos);
                GameObject wall = Instantiate(Resources.Load("Prefabs/Wall" + SetWallName + wallVariantDict[bitmask]) as GameObject, worldPos, Quaternion.identity);
                //Debug but warning
                Debug.LogWarning("Wall bitmask: " + bitmask + " Wall name: " + wallVariantDict[bitmask]);
                wallPositions.Add(pos);
                objects.Add(pos, wall);
                UpdateWallSprite(pos);
                gameManager.constructionToDo.Add(wall);

                //Update the graph
                UpdateGraphNearObject(worldPos);

                //Update bitmask
                UpdateWallSprite(pos + Vector3Int.up);
                UpdateWallSprite(pos + Vector3Int.right);
                UpdateWallSprite(pos + Vector3Int.down);
                UpdateWallSprite(pos + Vector3Int.left);
                UpdateWallSprite(pos);
            }
        }
    }

    List<Vector3Int> GetGridPositionsBetween(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        int xDir = end.x > start.x ? 1 : -1;
        int yDir = end.y > start.y ? 1 : -1;

        for (int x = start.x; x != end.x + xDir; x += xDir)
        {
            for (int y = start.y; y != end.y + yDir; y += yDir)
            {
                positions.Add(new Vector3Int(x, y, 0));
            }
        }
        return positions;
    }

    void RemoveWall(Vector3Int gridPos)
    {
        // Remove the wall position from the HashSet and Dictionary
        wallPositions.Remove(gridPos);
        Destroy(objects[gridPos]);
        objects.Remove(gridPos);

        // Update the bitmask and sprite of the neighbors
        UpdateWallSprite(gridPos + Vector3Int.up);
        UpdateWallSprite(gridPos + Vector3Int.right);
        UpdateWallSprite(gridPos + Vector3Int.down);
        UpdateWallSprite(gridPos + Vector3Int.left);
    }

    void RemoveFloor(Vector3Int gridPos)
    {
        
    }


    //Update construction to do list
    int GetWallBitmask(Vector3Int gridPos)
    {
        int bitmask = 0;

        // Check neighboring cells and set bits accordingly
        if (IsWall(gridPos + Vector3Int.up)) bitmask |= 1;       // Top
        if (IsWall(gridPos + Vector3Int.right)) bitmask |= 2;    // Right
        if (IsWall(gridPos + Vector3Int.down)) bitmask |= 4;     // Bottom
        if (IsWall(gridPos + Vector3Int.left)) bitmask |= 8;     // Left

        return bitmask;
    }

    bool IsWall(Vector3Int gridPos)
    {
        // Check if the HashSet contains the grid position
        return wallPositions.Contains(gridPos);
    }

    bool IsFloor(Vector3Int gridPos)
    {
        // Check if the HashSet contains the grid position
        return wallPositions.Contains(gridPos);
    }

    void UpdateWallSprite(Vector3Int gridPos)
    {
        if (objects.ContainsKey(gridPos))
        {
            int bitmask = GetWallBitmask(gridPos);
            GameObject wall = objects[gridPos];
            // Replace the game object with the new wall variant
            GameObject newWall = Instantiate(Resources.Load("Prefabs/Wall" + SetWallName + wallVariantDict[bitmask]) as GameObject, grid.GetCellCenterWorld(gridPos), Quaternion.identity);
            wall.GetComponent<SpriteRenderer>().sprite = newWall.GetComponent<SpriteRenderer>().sprite;
            Destroy(newWall);
        }
    }
    
    void OnDrawGizmos()
    {
        if (wallPositions != null)
        {
            foreach (var pos in wallPositions)
            {
                int bitmask = GetWallBitmask(pos);
                Vector3 worldPos = grid.GetCellCenterWorld(pos);
                UnityEditor.Handles.Label(worldPos, bitmask.ToString());
            }
        }
    }

    public float updateRadius = 5f;
    public void UpdateGraphNearObject(Vector3 transform)
    {
        Vector3 wallPosition = transform;
        Bounds updateBounds = new Bounds(wallPosition, new Vector3(updateRadius * 2, updateRadius * 2, updateRadius * 2));

        GraphUpdateObject guo = new GraphUpdateObject(updateBounds);
        AstarPath.active.UpdateGraphs(guo);

    }
}
