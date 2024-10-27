using UnityEngine;

public abstract class BuildingObject : MonoBehaviour
{

    public ResourceType resourceType;
    public int resourceAmount = 0;
    public int requiredResources = 5; // Example resource requirement
    public Pawns worker;
    public bool isGathering = false;
    public bool isCompleted;
    public Vector3Int GridPosition { get; set; }
    public abstract void Place(Vector3Int gridPosition, Grid grid, GameManager gameManager);
    public abstract void ShowPreview(Vector3Int gridPosition, Grid grid);

}