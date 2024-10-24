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
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPlacing = !isPlacing;
        }
        if (!isPlacing)
        {
            toPlace.SetActive(false);
            return;
        }

        if (isPlacing)
        {
            //Place the wall on mouse cursor
            toPlace.SetActive(true);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Vector3Int gridPos = grid.WorldToCell(mousePos);
            Vector3 finalPos = grid.GetCellCenterWorld(gridPos);
            toPlace.transform.position = finalPos;

            //Check if the placement is valid
            if (CheckValidPlacement(gridPos))
            {
                placementHolder.spriteRenderer.color = Color.green;
            }
            else
            {
                placementHolder.spriteRenderer.color = Color.red;
            }
        }
        if (Input.GetMouseButtonDown(0) && !isHoverUI && placementHolder.isColliding == false)
        {
            //check if there is a wall at the position

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Vector3Int gridPos = grid.WorldToCell(mousePos);
            Vector3 finalPos = grid.GetCellCenterWorld(gridPos);

            if (IsWall(gridPos))
            {
                // Remove the wall if it already exists
                RemoveWall(gridPos);
            }
            else if (CheckValidPlacement(gridPos))
            {
                // Switch between build types
                switch (buildType)
                {
                    case BuildType.Wall:
                        PlaceWall(gridPos, finalPos, SetWallName);
                        break;
                    case BuildType.Zone:
                        PlaceFloor(gridPos, finalPos, SetWallName);
                        break;
                    case BuildType.Floor:
                        PlaceFloor(gridPos, finalPos, SetWallName);
                        break;
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            isPlacing = false;
        }
    }


    bool CheckValidPlacement(Vector3Int gridPos)
    {
        // Check if the position is not onto another wall
        if (IsWall(gridPos) && !placementHolder.isColliding)
        {
            return false;
        }
        if (placementHolder.isColliding)
        {
            return false;
        }

        // Check if the position is not occupied by another wall
        return !IsWall(gridPos);
    }
    void PlaceWall(Vector3Int gridPos, Vector3 finalPos, string wallName)
    {
        // Determine the facing condition using bitmasking
        int bitmask = GetWallBitmask(gridPos);
        //Wall Resources Folder
        GameObject wallPrefab = Resources.Load("Prefabs/Wall" + wallName + wallVariantDict[bitmask]) as GameObject;
        currentPreviewPrefab = wallPrefab;
        //Debug path
        Debug.Log("Prefabs/Wall" + wallName + wallVariantDict[bitmask]);
        GameObject wall = Instantiate(wallPrefab, finalPos, Quaternion.identity);
        wallPositions.Add(gridPos);
        objects.Add(gridPos, wall);
        gameManager.AddTask(wall);
        // Update the bitmask and sprite of the new wall and its neighbors
        UpdateWallSprite(gridPos);
        UpdateWallSprite(gridPos + Vector3Int.up);
        UpdateWallSprite(gridPos + Vector3Int.right);
        UpdateWallSprite(gridPos + Vector3Int.down);
        UpdateWallSprite(gridPos + Vector3Int.left);
    }
    public GameObject currentPreviewPrefab;
    void PlaceFloor(Vector3Int gridPos, Vector3 finalPos, string floorName)
    {
        // Delete the / at the end of the string
        floorName = floorName.Substring(0, floorName.Length - 1);
        GameObject floorPrefab = Resources.Load("Prefabs/Floor" + floorName) as GameObject;
        currentPreviewPrefab = floorPrefab;
        Debug.Log("Prefabs/Floor" + floorName);
        GameObject floor = Instantiate(floorPrefab, finalPos, Quaternion.identity);
        floorPositions.Add(gridPos);
        objects.Add(gridPos, floor);
        gameManager.constructionToDo.Add(floor);
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
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(worldPos, 0.1f);
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
