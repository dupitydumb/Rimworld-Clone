using UnityEngine;

public class Floor : BuildingObject, IInteractable
{
    public Pawns worker;
    public bool isComplete = false;
    public ResourceType resourceType;
    public int resourceAmount;
    private Color32 originalColor;
    public float constructionTime = 5f;

    public GameObject floorPrefab;
    public GameObject floorPreviewPrefab;
    private GameObject currentPreview;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store the original color
        originalColor = GetComponent<SpriteRenderer>().color;
        // Set collision to false
        // GetComponent<BoxCollider2D>().enabled = false;
        // Set color to transparent. set the alpha only
        GetComponent<SpriteRenderer>().color = new Color32(originalColor.r, originalColor.g, originalColor.b, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (worker != null)
        {
            // Check if the worker is close to the wall
            if (Vector2.Distance(worker.transform.position, transform.position) < 0.8f)
            {
                BuildFloor();
            }
        }
    }

    public override void Place(Vector3Int gridPosition, Grid grid, GameManager gameManager)
    {
        Vector3 worldPos = grid.GetCellCenterWorld(gridPosition);
        GameObject floor = Instantiate(floorPrefab, worldPos, Quaternion.identity);
        GridPosition = gridPosition;
        gameManager.constructionToDo.Add(floor);
    }

    public override void ShowPreview(Vector3Int gridPosition, Grid grid)
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }
        Vector3 worldPos = grid.GetCellCenterWorld(gridPosition);
        currentPreview = Instantiate(floorPreviewPrefab, worldPos, Quaternion.identity);
    }
    bool isVfxSpawned = false;
    public void BuildFloor()
    {
        if (!isVfxSpawned)
        {
            isVfxSpawned = true;
            // Spawn the vfx
            GameManager.instance.SpawnVFX(this.gameObject, constructionTime);
        }
        constructionTime -= Time.deltaTime;
        if (constructionTime <= 0)
        {
            isComplete = true;
            GetComponent<SpriteRenderer>().color = originalColor;
            // GetComponent<BoxCollider2D>().enabled = true;
            //Remove task from the list
            worker = null;
            GameManager.instance.RemoveTask(gameObject);

        }
    }

    public void Interact(Pawns pawns)
    {
        if (worker == null)
        {
            worker = pawns;
            worker.task = this.gameObject;
        }
    }

    public void CancelInteraction()
    {
        Debug.Log("Cancelling interaction with floor");
    }
    public void Hover()
    {
        Debug.Log("Hovering over floor");
    }

    Pawns IInteractable.GetWorker()
    {
        return worker;
    }
}
