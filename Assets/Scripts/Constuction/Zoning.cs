using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoning : MonoBehaviour
{
    public GameObject zonePrefab; // Prefab for the zone
    public GameObject previewZonePrefab; // Prefab for the preview zone
    public Grid grid; // Reference to the grid
    private Dictionary<Vector2Int, GameObject> zones;
    private List<GameObject> previewZones;
    private Vector2Int dragStart;
    private bool isDragging = false;

    private bool isZoning = false;

    void Start()
    {
        grid = FindObjectOfType<Grid>();
        zones = new Dictionary<Vector2Int, GameObject>();
        previewZones = new List<GameObject>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            isZoning = !isZoning;
        }

        if (!isZoning)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = grid.WorldToCell(mousePosition);
            dragStart = new Vector2Int(cellPosition.x, cellPosition.y);
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = grid.WorldToCell(mousePosition);
            Vector2Int currentDragPosition = new Vector2Int(cellPosition.x, cellPosition.y);
            UpdatePreviewZones(dragStart, currentDragPosition);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = grid.WorldToCell(mousePosition);
            Vector2Int dragEnd = new Vector2Int(cellPosition.x, cellPosition.y);
            CreateZones(dragStart, dragEnd);
            ClearPreviewZones();
            isDragging = false;
        }
    }

    void UpdatePreviewZones(Vector2Int start, Vector2Int end)
    {
        ClearPreviewZones();

        int minX = Mathf.Min(start.x, end.x);
        int maxX = Mathf.Max(start.x, end.x);
        int minY = Mathf.Min(start.y, end.y);
        int maxY = Mathf.Max(start.y, end.y);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                GameObject previewZone = Instantiate(previewZonePrefab, grid.GetCellCenterWorld(cellPosition), Quaternion.identity);
                // Set Z axis to 0
                previewZone.transform.position = new Vector3(previewZone.transform.position.x, previewZone.transform.position.y, 0);
                previewZones.Add(previewZone);
            }
        }
    }

    void ClearPreviewZones()
    {
        foreach (GameObject previewZone in previewZones)
        {
            Destroy(previewZone);
        }
        previewZones.Clear();
    }

    void CreateZones(Vector2Int start, Vector2Int end)
    {
        int minX = Mathf.Min(start.x, end.x);
        int maxX = Mathf.Max(start.x, end.x);
        int minY = Mathf.Min(start.y, end.y);
        int maxY = Mathf.Max(start.y, end.y);

        //Create empty game objects for each zone

        GameObject Czone = new GameObject();
        Czone.name = "Zone";
        GameManager.instance.zoneList.Add(Czone);
        Czone.AddComponent<Zone>();
        //Move the zone to the center of the grid
        Czone.transform.position = grid.GetCellCenterWorld(new Vector3Int((minX + maxX) / 2, (minY + maxY) / 2, 0));
        //Scale the zone to cover the selected area
        Czone.transform.localScale = new Vector3((maxX - minX) + 1, (maxY - minY) + 1, 1);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int zonePosition = new Vector2Int(x, y);
                if (!zones.ContainsKey(zonePosition))
                {
                    Vector3Int cellPosition = new Vector3Int(x, y, 0);
                    GameObject zone = Instantiate(zonePrefab, grid.GetCellCenterWorld(cellPosition), Quaternion.identity);
                    // Set Z axis to 0
                    zone.transform.position = new Vector3(zone.transform.position.x, zone.transform.position.y, 0);
                    zones.Add(zonePosition, zone);
                    // Set the parent of the zone to the Zone object
                    zone.transform.SetParent(Czone.transform);
                    
                    //set alpa to 1
                    Color color = zone.GetComponent<SpriteRenderer>().color;
                    color.a = 1;
                }
            }
        }
    }
}


