using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class Tree : MonoBehaviour, IInteractable, ISelectable
{

    public float cutTime;
    public Pawns worker;
    
    public bool isCompleted = false;
    public TaskType taskType;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (worker != null)
        {
            if (Vector2.Distance(worker.transform.position, transform.position) < 1f)
            {
                CutDownTree();
            }
        }
        if (worker == null)
        {
            cutTime = 2;
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
        gameManager.AddTask(gameObject);
        Debug.Log("Selected");
    }

    public void Deselect()
    {
        
        Debug.Log("Deselected");
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
    

    public void Interact(Pawns pawns)
    {
        worker = pawns;
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
                gameManager.building.UpdateGraphNearObject(transform.position);
                gameManager.RemoveTask(gameObject);
                isCompleted = true;
                Destroy(gameObject);
            }
        }
        Debug.Log("Cutting down tree");
    }

    Pawns IInteractable.GetWorker()
    {
        return worker;
    }

}
