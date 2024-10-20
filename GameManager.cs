using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isHoverUI = false;
    public bool isPlacing = false;
    public Grid grid;

    public GameObject[] wallVariants; // Array to store wall variants
    public GameObject toPlace;

    private HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>(); // HashSet to store wall positions
    private Dictionary<Vector3Int, GameObject> wallObjects = new Dictionary<Vector3Int, GameObject>(); // Dictionary to store wall GameObjects


    public List<GameObject> constructionToDo; 


    //Dictionary of bitmasks and their corresponding wall variants names
    private Dictionary<int, string> wallVariantDict = new Dictionary<int, string>();

    public Inventory inventory;

    private void Awake()
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
        inventory = GetComponent<Inventory>();
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
            if (CheckValidPlacement(gridPos) && placementHolder.isColliding == false)
            {
                toPlace.GetComponent<SpriteRenderer>().color = Color.green;
            }
            else
            {
                toPlace.GetComponent<SpriteRenderer>().color = Color.red;
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
                // Place a new wall if there is no wall at the position
                PlaceWall(gridPos, finalPos , SetWallName);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            isPlacing = false;
        }
    }

    public string SetWallName;
    private PlacementHolder placementHolder;
    //Check placement validity
    bool CheckValidPlacement(Vector3Int gridPos)
    {
        // Check if the position is not onto another wall
        if (IsWall(gridPos) && !placementHolder.isColliding)
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
        //Debug path
        Debug.Log("Prefabs/Wall" + wallName + wallVariantDict[bitmask]);
        GameObject wall = Instantiate(wallPrefab, finalPos, Quaternion.identity);
        wallPositions.Add(gridPos);
        wallObjects.Add(gridPos, wall);
        constructionToDo.Add(wall);
        UpdateConstruction();
        // Update the bitmask and sprite of the new wall and its neighbors
        UpdateWallSprite(gridPos);
        UpdateWallSprite(gridPos + Vector3Int.up);
        UpdateWallSprite(gridPos + Vector3Int.right);
        UpdateWallSprite(gridPos + Vector3Int.down);
        UpdateWallSprite(gridPos + Vector3Int.left);
    }

    void RemoveWall(Vector3Int gridPos)
    {
        // Remove the wall position from the HashSet and Dictionary
        wallPositions.Remove(gridPos);
        Destroy(wallObjects[gridPos]);
        wallObjects.Remove(gridPos);
        UpdateConstruction();

        // Update the bitmask and sprite of the neighbors
        UpdateWallSprite(gridPos + Vector3Int.up);
        UpdateWallSprite(gridPos + Vector3Int.right);
        UpdateWallSprite(gridPos + Vector3Int.down);
        UpdateWallSprite(gridPos + Vector3Int.left);
    }


    //Update construction to do list
    public void UpdateConstruction()
    {  
        foreach (GameObject construction in constructionToDo)
        {
            if (construction == null)
            {
                constructionToDo.Remove(construction);
            }
        }
    }
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

    void UpdateWallSprite(Vector3Int gridPos)
    {
        if (wallObjects.ContainsKey(gridPos))
        {
            int bitmask = GetWallBitmask(gridPos);
            GameObject wall = wallObjects[gridPos];
            SpriteRenderer spriteRenderer = wall.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = wallVariants[bitmask].GetComponent<SpriteRenderer>().sprite;
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

    public void SpawnWood(Vector3 position)
    {
        GameObject wood = Instantiate(Resources.Load("Prefabs/Resource/Wood"), position, Quaternion.identity) as GameObject;
        wood.GetComponent<Items>().amount = Random.Range(3, 10);
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