using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum ResourceType
{
    Wood,
    Stone,
    Iron
}
public class Walls : BuildingObject, IInteractable
{
    public TaskType taskType;
    private Grid grid;
    private Dictionary<int, string> wallVariantDict = new Dictionary<int, string>();
    public bool isComplete = false;
    private Color32 originalColor;
    public float updateRadius = 5f;
    public float constructionTime = 5f;
    private HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>(); // HashSet to store wall positions
    public GameObject wallPrefab;
    public GameObject wallPreviewPrefab;
    private GameObject currentPreview;

    private Dictionary<Vector3Int, GameObject> objects = new Dictionary<Vector3Int, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        grid = GameManager.instance.grid;
        wallPositions = GameManager.instance.building.wallPositions;
        objects = GameManager.instance.building.objects;
        // Store the original color
        originalColor = GetComponent<SpriteRenderer>().color;
        // Set collision to false
        GetComponent<BoxCollider2D>().enabled = false;
        // Set color to transparent. set the alpha only
        GetComponent<SpriteRenderer>().color = new Color32(originalColor.r, originalColor.g, originalColor.b, 100);

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

        UpdateWallSprite(GridPosition);
        UpdateWallSprite(GridPosition + Vector3Int.up);
        UpdateWallSprite(GridPosition + Vector3Int.right);
        UpdateWallSprite(GridPosition + Vector3Int.down);
        UpdateWallSprite(GridPosition + Vector3Int.left);

    }

    // Update is called once per frame

    private bool isAddedToPending = false;
    void Update()
    {
        
        if (Vector2.Distance(worker.transform.position, transform.position) < 2f && !isComplete)
        {
            if (resourceAmount >= requiredResources && worker != null && !isComplete)
            {
                BuildWall();
                Debug.LogWarning("Building wall");
            }

            else if (resourceAmount < requiredResources && worker != null && !isComplete)
            {
                GameObject resource = GameManager.instance.resourcesManager.GetResource(resourceType);
                if (Vector2.Distance(worker.transform.position, this.transform.position) < 2f)
                {
                    Debug.LogWarning("Gathering resources");
                    if (worker.stuffCarried.Count > 0)
                    {
                        foreach (InventoryItem item in worker.stuffCarried)
                        {
                            Debug.LogWarning(item.itemName + "/ " + resourceType.ToString());
                            if (item.itemName == resourceType.ToString())
                            {
                                //Add needed resources to the construction if the worker is carrying the needed resources
                                int neededAmount = requiredResources - resourceAmount;
                                if (item.amount >= neededAmount)
                                {
                                    resourceAmount += neededAmount;
                                    item.amount -= neededAmount;
                                }
                                else
                                {
                                    resourceAmount += item.amount;
                                    item.amount = 0;
                                }

                                if (item.amount <= 0)
                                {
                                    worker.stuffCarried.Remove(item);
                                }
                                break;
                            }
                        }
                    }
                    else if (worker.stuffCarried.Count <= 0 && resource != null && !isResourcesAssigned)
                    {
                        worker.AID.target = resource.transform;
                        resource.GetComponent<Items>().MoveResourceToConstruction(this.gameObject);
                        //Items IInteractable.GetWorker();
                        resource.GetComponent<IInteractable>().Interact(worker);
                    }
                    else
                    {
                        GameManager.instance.taskManager.RemoveTask(this.gameObject);
                        GameManager.instance.taskManager.pendingTasks.Add(new Task(taskType, gameObject.transform.position, this.gameObject));
                        isAddedToPending = true;
                        worker.CurrentTask.targetObject = null;
                        worker = null;
                    }
                }
                
            }
        }
        
        // if (resourceAmount >= requiredResources && worker != null && !isComplete)
        // {
        //     if (Vector2.Distance(worker.transform.position, transform.position) < 0.8f)
        //     {
        //         BuildWall();
        //         Debug.LogWarning("Building wall");
        //     }
        // }
        // if (resourceAmount < requiredResources && worker != null && !isComplete)
        // {
        //     GameObject resource = GameManager.instance.resourcesManager.GetResource(resourceType);
        //     if (resource != null && !isResourcesAssigned)
        //     {
        //         worker.AID.target = resource.transform;

        //         if (Vector2.Distance(worker.transform.position, resource.transform.position) < 0.8f)
        //         {
        //             Items items = resource.GetComponent<Items>();
        //             resource.GetComponent<Items>().AddItem(resourceType.ToString(), items.item.amount, worker);
        //             items.item.amount = 0;
        //             Destroy(resource);
        //             worker.AID.target = this.transform;
        //             isResourcesAssigned = true;
        //         }
        //     }
        //     if (Vector2.Distance(worker.transform.position, this.transform.position) < 0.8f)
        //     {
        //         Debug.LogWarning("Gathering resources");
        //         if (worker.stuffCarried.Count > 0)
        //         {
        //             foreach (InventoryItem item in worker.stuffCarried)
        //             {
        //                 Debug.LogWarning(item.itemName + "/ " + resourceType.ToString());
        //                 if (item.itemName == resourceType.ToString())
        //                 {
        //                     Debug.LogWarning("Resource ADDED");
        //                     resourceAmount += item.amount;
        //                     worker.stuffCarried.Remove(item);
        //                     break;
        //                 }
        //             }
        //         }
        //     }
        // }
        
    }
    bool isPending = false;
    bool isResourcesAssigned = false;
    
    public void UpdateWallSprite(Vector3Int gridPos)
    {
        // if not wall, return
        if (!IsWall(gridPos)) return;
        if (objects.ContainsKey(gridPos))
        {
            string SetWallName = GameManager.instance.building.SetWallName;
            int bitmask = GetWallBitmask(gridPos);
            GameObject wall = objects[gridPos];
            // Replace the game object with the new wall variant
            GameObject newWall = Instantiate(Resources.Load("Prefabs/Wall" + SetWallName + wallVariantDict[bitmask]) as GameObject, grid.GetCellCenterWorld(gridPos), Quaternion.identity);
            wall.GetComponent<SpriteRenderer>().sprite = newWall.GetComponent<SpriteRenderer>().sprite;
            Destroy(newWall);
        }
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
    public override void Place(Vector3Int gridPosition, Grid grid, GameManager gameManager)
    {
        
    }

    public override void ShowPreview(Vector3Int gridPosition, Grid grid)
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }
        Vector3 worldPos = grid.GetCellCenterWorld(gridPosition);
        currentPreview = Instantiate(wallPreviewPrefab, worldPos, Quaternion.identity);
    }

    bool isVfxSpawned = false;
    public void BuildWall()
    {
        if (!isVfxSpawned)
        {
            isVfxSpawned = true;
            GameManager.instance.SpawnVFX(this.gameObject, constructionTime);
        }
        constructionTime -= Time.deltaTime;
        if (constructionTime <= 0)
        {
            GameManager.instance.RemoveTask(gameObject);
            Vector3 safePosition = FindSafePosition(worker.transform.position);
            worker.transform.position = safePosition;
            GetComponent<BoxCollider2D>().enabled = true;
            // Set color to opaque
            GetComponent<SpriteRenderer>().color = originalColor;
            isComplete = true;
            worker.CurrentTask.targetObject = null;
            worker = null;
            Debug.LogWarning("Wall built");
            // Update the graph near the wall
            UpdateGraphNearWall();
        }
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
    public void Interact(Pawns pawns)
    {
        worker = pawns;
    }


    public void CancelInteraction()
    {
        if (worker != null)
        {
            worker = null;
            GameManager.instance.constructionToDo.Remove(gameObject);
        }
    }

    
    void UpdateGraphNearWall()
    {
        Vector3 wallPosition = transform.position;
        Bounds updateBounds = new Bounds(wallPosition, new Vector3(updateRadius * 2, updateRadius * 2, updateRadius * 2));

        GraphUpdateObject guo = new GraphUpdateObject(updateBounds);
        AstarPath.active.UpdateGraphs(guo);
    }

    Vector3 FindSafePosition(Vector3 currentPosition)
    {
        Vector3[] directions = {
            Vector3.up,
            Vector3.right,
            Vector3.down,
            Vector3.left
        };

        foreach (Vector3 direction in directions)
        {
            Vector3 newPosition = currentPosition + direction;
            if (!IsPositionOccupied(newPosition))
            {
                return newPosition;
            }
        }

        // If no safe position is found, return the current position
        return currentPosition;
    }

    bool IsPositionOccupied(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.GetComponent<Walls>() != null)
            {
                return true;
            }
        }
        return false;
    }

    Pawns IInteractable.GetWorker()
    {
        return worker;
    }

}
