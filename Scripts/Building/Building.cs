using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;
using UnityEngine.Events;

public class Building : MonoBehaviour
{
    public GameObject toPlace;
    private Grid grid;
    public string SetWallName;
    private PlacementHolder placementHolder;
    public bool isHoverUI = false;
    public bool isPlacing = false;
    public BuildType buildType;
    public HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>(); // HashSet to store wall positions
    private HashSet<Vector3Int> floorPositions = new HashSet<Vector3Int>(); // HashSet to store floor positions
    public Dictionary<Vector3Int, GameObject> objects = new Dictionary<Vector3Int, GameObject>(); // Dictionary to store wall GameObjects

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

    }

    // Update is called once per frame
    void Update()
    {
        if (!isHoverUI && SetWallName != null && isPlacing)
        {
            placementHolder.gameObject.SetActive(true);
            if (Input.GetMouseButtonDown(0)) // Left Mouse Button
            {
                isDragging = true;
                startGridPos = GetGridPositionFromMouse();
                currentGridPos = startGridPos;
    
                switch (buildType)
                {
                    case BuildType.Wall:
                        ShowWallPreview(currentGridPos);
                        break;
                    case BuildType.Floor:
                        ShowFloorPreview(startGridPos, currentGridPos);
                        break;
                }
    
            }
    
            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3Int newGridPos = GetGridPositionFromMouse();
                if (newGridPos != currentGridPos)
                {
                    currentGridPos = newGridPos;
                    switch (buildType)
                    {
                        case BuildType.Wall:
                            ShowWallPreview(currentGridPos);
                            break;
                        case BuildType.Floor:
                            ShowFloorPreview(startGridPos, currentGridPos);
                            break;
                    }
                }
            }
    
            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;
                switch (buildType)
                {
                    case BuildType.Wall:
                        PlaceWalls(startGridPos, currentGridPos);
                        break;
                    case BuildType.Floor:
                        PlaceFloor(startGridPos, currentGridPos);
                        break;
                }
                foreach (GameObject preview in currentPreview)
                {
                    Destroy(preview);
                }
            }
        }
        else
        {
            placementHolder.gameObject.SetActive(false);
        }
    }

    public Vector3Int GetGridPositionFromMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return grid.WorldToCell(mouseWorldPos);
    }

    void ShowWallPreview(Vector3Int gridPos)
    {
        Vector3 worldPos = grid.GetCellCenterWorld(gridPos);
        int bitmask = 0;
        GameObject preview = Instantiate(Resources.Load("Prefabs/Wall" + SetWallName + "Wall 0") as GameObject, worldPos, Quaternion.identity);
        // Set prevew z position to -1 so it is rendered behind other objects
        preview.transform.position = new Vector3(preview.transform.position.x, preview.transform.position.y, 3);
        currentPreview.Add(preview);

        // Destroy the preview object if there is already a wall at the position
        if (objects.ContainsKey(gridPos))
        {
            Destroy(preview);
        }

        
    }

    void ShowFloorPreview(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> positions = GetGridPositionsBetween(start, end);
        foreach (Vector3Int pos in positions)
        {
            if (!floorPositions.Contains(pos))
            {
                Vector3 worldPos = grid.GetCellCenterWorld(pos);
                //Get bitmask of the wall
                //Remove the / at the end of the name
                GameObject floor = Instantiate(Resources.Load("Prefabs/Floor" + SetWallName.Substring(0, SetWallName.Length - 1)) as GameObject, worldPos, Quaternion.identity);
                //Debug but warning
                floorPositions.Add(pos);
                objects.Add(pos, floor);
                gameManager.constructionToDo.Add(floor);
                floor.GetComponent<Floor>().GridPosition = pos;
                //Update the graph

            }
        }
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
                GameObject wall = Instantiate(Resources.Load("Prefabs/Wall" + SetWallName + "Wall 0") as GameObject, worldPos, Quaternion.identity);
                //Debug but warning
                wallPositions.Add(pos);
                objects.Add(pos, wall);
                gameManager.constructionToDo.Add(wall);
                wall.GetComponent<Walls>().GridPosition = pos;
                //Update the graph
                UpdateGraphNearObject(worldPos);

            }
        }
    }
    
    // Place floor

    void PlaceFloor(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> positions = GetGridPositionsBetween(start, end);
        foreach (Vector3Int pos in positions)
        {
            if (!floorPositions.Contains(pos))
            {
                Vector3 worldPos = grid.GetCellCenterWorld(pos);
                //Get bitmask of the wall
                //Remove the / at the end of the name
                GameObject floor = Instantiate(Resources.Load("Prefabs/Floor" + SetWallName.Substring(0, SetWallName.Length - 1)) as GameObject, worldPos, Quaternion.identity);
                //Debug but warning
                floorPositions.Add(pos);
                objects.Add(pos, floor);
                gameManager.constructionToDo.Add(floor);
                floor.GetComponent<Floor>().GridPosition = pos;
                //Update the graph
                UpdateGraphNearObject(worldPos);

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
    }

    void RemoveFloor(Vector3Int gridPos)
    {
        
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
