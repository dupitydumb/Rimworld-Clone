using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Grid grid;
    public List<GameObject> zoneList;
    public List<GameObject> constructionToDo; 
    public Building building;
    public Inventory inventory;
    public PawnSelectionManager pawnSelectionManager;
    //Event to update when the build type changes
    public UnityAction BuildChangeEvent;

    public BoundsInt bounds;
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
        building = GetComponent<Building>();
        inventory = GetComponent<Inventory>();     
        pawnSelectionManager = GetComponent<PawnSelectionManager>();   
    }

    void Update()
    {
        //Game Speed
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = 3;
        }
    }


    //Check placement validity
    

    public void SpawnWood(Vector3 position)
    {
        GameObject wood = Instantiate(Resources.Load("Prefabs/Resource/Wood"), position, Quaternion.identity) as GameObject;
        wood.GetComponent<Items>().item.amount = UnityEngine.Random.Range(3, 5);
        wood.GetComponent<Items>().item.itemGO = wood;
    }

    

    public void RemoveTask(GameObject task)
    {
        constructionToDo.Remove(task);
        GameObject marker = task.transform.Find("Marker").gameObject;
        if (marker != null)
        {
            Destroy(marker);
            return;
        }
        
    }
    public void AddTask(GameObject task)
    {
        if (constructionToDo.Contains(task))
        {
            return;
        }
        SpawnMarker(task);
        constructionToDo.Add(task);
    }

    public void SpawnMarker(GameObject target)
    {
        //Load the marker prefab from the resources folder
        //If object is wall do not spawn marker
        if (target.tag == "Wall")
        {
            return;
        }
        GameObject marker = Instantiate(Resources.Load("Prefabs/Marker", typeof(GameObject))) as GameObject;
        marker.name = "Marker";
        //Set the position of the marker to the position of the tree
        marker.transform.position = target.transform.position;
        //Set parent to the tree
        marker.transform.SetParent(target.transform);
    }

    public void SpawnVFX(GameObject target, float time)
    {
        //Load the marker prefab from the resources folder
        GameObject buildSfx = Instantiate(Resources.Load("Prefabs/Sfx/Spark", typeof(GameObject))) as GameObject;
        buildSfx.name = "BuildSfx";
        //Set the position of the marker to the position of the tree
        buildSfx.transform.position = target.transform.position;
        //Set parent to the tree
        buildSfx.transform.SetParent(target.transform);
        Destroy(buildSfx, time);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}