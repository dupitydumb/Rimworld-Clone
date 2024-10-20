using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public bool isGenerating = false;

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
        StartCoroutine(GenerateWorld());
    }

    void Update()
    {
        
    }


    IEnumerator GenerateWorld()
    {
        isGenerating = true;
        // Destroy all children if there are any
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        System.Random prng = new System.Random(seed);
        float offsetX = prng.Next(-100000, 100000) + offset.x;
        float offsetY = prng.Next(-100000, 100000) + offset.y;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale * noiseScale + offsetX;
                float yCoord = (float)y / height * scale * noiseScale + offsetY;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                float tileSize = 0.528f;
                Vector3 position = new Vector3(x * tileSize, y * tileSize, 0);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                // Set tile parent to this object
                tile.transform.SetParent(GameObject.FindWithTag("Noise").transform);
                tile.GetComponent<SpriteRenderer>().color = new Color(sample, sample, sample); // Adjust color based on noise value

                // switch the sprite based on the noise value
                if (sample < 0.5)
                {
                    //70% chance to spawn a grass no 0
                    if (prng.Next(0, 100) < 80)
                    {
                        //instantiate a 3-4 more grass around the bush
                        for (int i = 0; i < prng.Next(1, 2); i++)
                        {
                            GameObject plant = grass[0];
                            //random offset to place the plant
                            float xOff = prng.Next(-1, 1);
                            float yOff = prng.Next(-1, 1);
                            Vector3 plantPosition = new Vector3(x * tileSize + xOff, y * tileSize + yOff, 0);
                            GameObject go = Instantiate(plant, plantPosition, Quaternion.identity);
                            //Set the parent to this object
                            go.transform.SetParent(this.transform);
                        }
                    }
                    // 20% chance to spawn a grass no 1 - max
                    else if (prng.Next(0, 100) < 20)
                    {
                        //instantiate a 3-4 more grass around the bush
                        for (int i = 0; i < prng.Next(1, 2); i++)
                        {
                            GameObject plant = grass[prng.Next(1, grass.Count)];
                            //random offset to place the plant
                            float xOff = prng.Next(-1, 1);
                            float yOff = prng.Next(-1, 1);
                            Vector3 plantPosition = new Vector3(x * tileSize + xOff, y * tileSize + yOff, 0);
                            GameObject go = Instantiate(plant, plantPosition, Quaternion.identity);
                            //Set the parent to this object
                            go.transform.SetParent(this.transform);
                        }
                    }
                }
                else if (sample < 0.6)
                {
                    // 30% chance to spawn a tree
                    if (prng.Next(0, 100) < 20)
                    {
                        GameObject plant = tree[prng.Next(0, tree.Count)];
                        GameObject treeGO = Instantiate(plant, position, Quaternion.identity);
                        //Set the parent to this object
                        treeGO.transform.SetParent(this.transform);
                        treeGO.GetComponent<SpriteRenderer>().sortingOrder = -(int)treeGO.transform.position.y;


                        //spawn 4-6 more trees around the tree
                        for (int i = 0; i < prng.Next(1, 1); i++)
                        {
                            GameObject plant2 = tree[prng.Next(0, tree.Count)];
                            //random offset to place the plant
                            float xOff = prng.Next(-3, 3);
                            float yOff = prng.Next(-3, 3);
                            Vector3 plantPosition = new Vector3(x * tileSize + xOff, y * tileSize + yOff, 0);
                            GameObject go = Instantiate(plant2, plantPosition, Quaternion.identity);
                            //Set the parent to this object
                            go.transform.SetParent(this.transform);
                            go.GetComponent<SpriteRenderer>().sortingOrder = -(int)go.transform.position.y;
                            Debug.Log(go.name + " / " + go.GetComponent<SpriteRenderer>().sortingOrder);
                        }
                    }

                    // 30% chance to spawn a grass
                    else if (prng.Next(0, 100) < 30)
                    {
                        GameObject plant = grass[prng.Next(0, grass.Count)];
                        GameObject grassGO = Instantiate(plant, position, Quaternion.identity);
                        //Set the parent to this object
                        grassGO.transform.SetParent(this.transform);
                        grassGO.GetComponent<SpriteRenderer>().sortingOrder = -(int)grassGO.transform.position.y;
                    }
                    
                }

                // Yield control back to the main thread to avoid freezing
                if (y % 10 == 0)
                {
                    yield return null;
                }
            }
            // Yield control back to the main thread to avoid freezing
            yield return null;
        }

        // once the generation is done, set the flag to false
        isGenerating = false;

        AstarPath.active.Scan();
    }
}