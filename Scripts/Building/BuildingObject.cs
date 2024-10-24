using UnityEngine;

public abstract class BuildingObject : MonoBehaviour
{
    public Vector3Int GridPosition { get; set; }
    public abstract void Place(Vector3Int gridPosition, Grid grid, GameManager gameManager);
    public abstract void ShowPreview(Vector3Int gridPosition, Grid grid);
}