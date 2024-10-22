using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class Tree : MonoBehaviour, IInteractable, ISelectable
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
        
    }
    void OnMouseExit()
    {
        Debug.Log("Mouse exit");
    }

    public void Select()
    {
        GameManager.instance.AddTask(gameObject);
        Debug.Log("Selected");
    }

    public void Deselect()
    {
        
        Debug.Log("Deselected");
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
