using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


public class Items : MonoBehaviour, IInteractable
{
    public TaskType taskType;
    public ResourceType resourceType;
    public InventoryItem item;

    public Pawns worker;

    public bool isCarried = false;

    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        TMP_Text text = GetComponentInChildren<TMP_Text>();
        text.text = item.amount.ToString();

        GameManager.instance.resourcesManager.resources.Add(gameObject);
        GameManager.instance.taskManager.OnResourcesAdded();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //OnMouseStay is called every frame the mouse is over the object
    void OnMouseOver()
    {
        
    }

    public void Interact(Pawns pawns)
    {
        worker = pawns;

        if (!isCarried)
        {
            AddItem(item.itemName, item.amount, pawns);
            GameManager.instance.AddTask(target);
            GameManager.instance.resourcesManager.resources.Remove(gameObject);
            Destroy(gameObject);
        }

        if (target != null)
        {
            worker.AID.target = target.transform;
        }
    }

    public void CancelInteraction()
    {
        
    }
    bool isWillMove = false;
    public void MoveResourceToConstruction(GameObject target)
    {
        isWillMove = true;
        this.target = target;
    }

    void OnDrawGizmos()
    {
        
    }

    Pawns IInteractable.GetWorker()
    {
        return worker;
    }

    public void AddItem(string item, int amount, Pawns pawns)
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
        isCarried = true;
    }
}
