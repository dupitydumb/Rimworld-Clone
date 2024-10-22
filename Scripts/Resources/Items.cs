using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Items : MonoBehaviour, IInteractable
{

    public InventoryItem item;

    public Pawns worker;

    public bool isCarried = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (worker != null && !isCarried)
        {
            // Check if the worker is close to the wall

            if (GameManager.instance.zoneList == null)
            {
                //Gizmos Add label

                UnityEditor.Handles.Label(transform.position, "No zone to haul to");

                return;
            }
            if (Vector2.Distance(worker.transform.position, transform.position) < 0.8f)
            {
                worker.stuffCarried.Add(item);
                HaulToZone(GameManager.instance.zoneList[0], worker);
                GameManager.instance.constructionToDo.Remove(this.gameObject);
                isCarried = true;
            }
        }

        if (isCarried)
        {
            //Destroy the object
            transform.position = worker.transform.position;
        }

        //Mouse click add task
        
    }

    //OnMouseStay is called every frame the mouse is over the object
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.instance.AddTask(this.gameObject);
            }
        }
        Debug.Log("Mouse over");
    }

    public void Interact(Pawns pawns)
    {
        if (worker == null)
        {
            worker = pawns;
        }
    }

    public void CancelInteraction()
    {
        
    }

    void OnDrawGizmos()
    {
        
    }

    

    public void HaulToZone(GameObject zone, Pawns pawns)
    {
        if (zone != null && !isCarried)
        {
            GameManager.instance.AddTask(zone);
        }
    }
    void AddItem(string item, int amount, Pawns pawns)
    {
        InventoryItem inventoryItem = pawns.stuffCarried.Find(i => i.itemName == item);
        if (inventoryItem != null)
        {
            inventoryItem.amount += amount;
        }
        else
        {
            pawns.stuffCarried.Add(new InventoryItem { itemName = item, amount = amount, itemGO = this.gameObject });
        }
    }
}
