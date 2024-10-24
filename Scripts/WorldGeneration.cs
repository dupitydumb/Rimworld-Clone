using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGeneration : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public float scale = 20f;
    public float noiseScale = 1f; // Parameter to control the noise scale
    public int seed = 0; // Seed for random generation
    public Vector2 offset; // Offset for the noise
    public GameObject tilePrefab;
    public List<GameObject> tree; // List of plant prefabs
    public List<GameObject> bush; // List of plant prefabs
    public List<GameObject> grass;

    public GameObject pathPrefab;
    public static WorldGeneration instance;

    private Grid grid;

    public bool isGenerating = false;

    public Dictionary<Vector3Int, GameObject> tilemap = new Dictionary<Vector3Int, GameObject>();
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
        grid = GetComponent<Grid>();
        StartCoroutine(GenerateWorld());
    }

    void Update()
    {
        
    }


    IEnumerator GenerateWorld()
    {
        isGenerating = true;
        GeneratePerlinNoise();
        yield return new WaitForSeconds(0.1f);
        GenerateTrees();
        yield return new WaitForSeconds(0.1f);
        GenerateGrass();
        AstarPath.active.Scan();
    }

    void GeneratePerlinNoise()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale + offset.x;
                float yCoord = (float)y / height * scale + offset.y;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                float z = Mathf.PerlinNoise(xCoord, yCoord) * noiseScale;

                // Calculate the position of the center of the grid cell
                Vector3Int gridPosition = new Vector3Int(x, y, 0);
                Vector3 cellCenterPosition = grid.GetCellCenterWorld(gridPosition);
                GameObject tile = Instantiate(tilePrefab, cellCenterPosition, Quaternion.identity);
                tile.transform.SetParent(GameObject.FindWithTag("Noise").transform);
                tile.GetComponent<SpriteRenderer>().color = new Color(sample, sample, sample);
                tilemap.Add(gridPosition, tile);
            }
        }
        
    }


    void GenerateTrees()
    {
        // Collect positions where trees should be placed
        List<Vector3Int> treePositions = new List<Vector3Int>();

        foreach (KeyValuePair<Vector3Int, GameObject> tile in tilemap)
        {
            if (tile.Value.GetComponent<SpriteRenderer>().color.r < 0.5f)
            {
                treePositions.Add(tile.Key);
            }
        }

        // Define the minimum distance between trees
        int minDistance = 2; // Adjust this value as needed

        // Place trees at the collected positions
        foreach (Vector3Int gridPosition in treePositions)
        {
            // 60% chance of tree
            if (Random.Range(0, 100) < 60)
            {
                // Check if there are any trees within the minimum distance
                bool canPlaceTree = true;
                for (int x = -minDistance; x <= minDistance; x++)
                {
                    for (int y = -minDistance; y <= minDistance; y++)
                    {
                        Vector3Int checkPosition = gridPosition + new Vector3Int(x, y, 0);
                        if (tilemap.ContainsKey(checkPosition) && tilemap[checkPosition] != null && tilemap[checkPosition].CompareTag("Tree"))
                        {
                            canPlaceTree = false;
                            break;
                        }
                    }
                    if (!canPlaceTree)
                    {
                        break;
                    }
                }

                if (canPlaceTree)
                {
                    Vector3 cellCenterPosition = grid.GetCellCenterWorld(gridPosition);
                    GameObject treePrefab = tree[Random.Range(0, tree.Count)];
                    GameObject treeInstance = Instantiate(treePrefab, cellCenterPosition, Quaternion.identity);
                    // Set the order in layer to the y position of the tree
                    treeInstance.GetComponent<SpriteRenderer>().sortingOrder = -(gridPosition.y);
                    treeInstance.tag = "Tree"; // Set the tag to "Tree"
                    tilemap[gridPosition] = treeInstance;
                    //Set parent to the tree
                    treeInstance.transform.SetParent(this.transform);
                }
            }
        }
    }

    void GenerateGrass()
    {
        // Collect positions where trees should be placed
        List<Vector3Int> grassPositions = new List<Vector3Int>();

        foreach (KeyValuePair<Vector3Int, GameObject> tile in tilemap)
        {
            
            if (tile.Value.GetComponent<SpriteRenderer>().color.r < 0.8f)
            {
                grassPositions.Add(tile.Key);
            }
        }

        // Place trees at the collected positions
        foreach (Vector3Int gridPosition in grassPositions)
        {
            //60 % chance of grass
            if (Random.Range(0, 100) < 40)
            {
                Vector3 cellCenterPosition = grid.GetCellCenterWorld(gridPosition);
                // 90% chance of spawning a grass[0]
                GameObject grassPrefab;
                if (Random.Range(0, 100) < 90)
                {
                    grassPrefab = grass[0];
                }
                else
                {
                    grassPrefab = grass[Random.Range(1, grass.Count)];
                }
                GameObject grassInstance = Instantiate(grassPrefab, cellCenterPosition, Quaternion.identity);
                //Set the order in layer to the y position of the tree
                grassInstance.GetComponent<SpriteRenderer>().sortingOrder = gridPosition.y;
                tilemap[gridPosition] = grassInstance;
                grassInstance.transform.SetParent(this.transform);
            }
        }
    }
}