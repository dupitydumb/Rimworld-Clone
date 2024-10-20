using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class Tree : MonoBehaviour, IInteractable
{

    public float cutTime;
    public Pawns worker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (worker != null)
        {
            if (Vector2.Distance(worker.transform.position, transform.position) < 0.8f)
            {
                CutDownTree();
            }
        }
    }


    //OnMouseStay is called every frame the mouse is over the object
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AddTask();
        }
        Debug.Log("Mouse over");
    }
    void OnMouseExit()
    {
        Debug.Log("Mouse exit");
    }

    void SpawnMarker()
    {
        //Load the marker prefab from the resources folder
        GameObject marker = Instantiate(Resources.Load("Prefabs/Marker", typeof(GameObject))) as GameObject;
        //Set the position of the marker to the position of the tree
        marker.transform.position = transform.position;
        marker.transform.position = new Vector3(marker.transform.position.x, marker.transform.position.y, -0.3f);
        //Set the parent of the marker to the tree
        marker.transform.parent = transform;
    }

    void AddTask()
    {
        if (GameManager.instance.constructionToDo.Contains(gameObject))
        {
            return;
        }
        GameManager.instance.constructionToDo.Add(gameObject);
        SpawnMarker();
    }
    public void Interact(Pawns pawns)
    {
        if (worker == null)
        {
            worker = pawns;
            worker.destinations.Add(transform);
        }
    }

    public void CancelInteraction()
    {
        worker = null;
    }
    void CutDownTree()
    {
        if (worker != null)
        {
            cutTime -= Time.deltaTime;
            if (cutTime <= 0)
            {
                GameManager.instance.SpawnWood(transform.position);
                //Disable the collider
                GetComponent<BoxCollider2D>().enabled = false;
                //Update the graph
                GameManager.instance.UpdateGraphNearObject(transform.position);
                GameManager.instance.constructionToDo.Remove(gameObject);
                Destroy(gameObject);
            }
        }
        Debug.Log("Cutting down tree");
    }

}
