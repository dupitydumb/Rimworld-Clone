using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour, IInteractable
{
    public List<InventoryItem> storageItems = new List<InventoryItem>();
    public ZoneType zoneType;

    public List<Pawns> workersInZone = new List<Pawns>();
    public List<Pawns> workers = new List<Pawns>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //If pawn is near the zone
        if (workersInZone.Count > 0)
        {
            foreach (Pawns p in workersInZone)
            {
                if (p.stuffCarried.Count > 0)
                {
                    foreach (InventoryItem item in p.stuffCarried)
                    {
                        item.itemGO.GetComponent<Items>().isCarried = false;
                        //Place the item in the storage
                        //Random child of the zone
                        item.itemGO.GetComponent<Items>().worker = null;
                        GameObject storageLocation = transform.GetChild(Random.Range(0, transform.childCount)).gameObject;
                        item.itemGO.transform.position = storageLocation.transform.position;
                        AddItem(item.itemName, item.amount, p);
                        GameManager.instance.RemoveTask(item.itemGO);
                        GameManager.instance.RemoveTask(this.gameObject);
                    }
                    p.stuffCarried.Clear();
                }
            }
        }

        //Check the workers distance to the zone
        if (workers.Count > 0)
        {
            foreach (Pawns p in workers)
            {
                if (Vector2.Distance(p.transform.position, transform.position) < 0.8f)
                {
                    if (!workersInZone.Contains(p))
                    {
                        workersInZone.Add(p);
                    }
                }
            }
        }
    }
    

    public void Interact(Pawns pawns)
    {
        if (!workers.Contains(pawns))
        {
            workers.Add(pawns);
        }
    }

    public void CancelInteraction()
    {
        workers.Clear();
    }

    public void AddItem(string item, int amount, Pawns pawn)
    {
        // Move the stuff from the pawn to the storage
        InventoryItem inventoryItem = storageItems.Find(i => i.itemName == item);
        if (inventoryItem != null)
        {
            inventoryItem.amount += amount;
        }
        else
        {
            InventoryItem newItem = new InventoryItem();
            newItem.itemName = item;
            newItem.amount = amount;
            storageItems.Add(newItem);
        }
    }

    public void RemoveItem(string item, int amount, Pawns pawn)
    {
        InventoryItem inventoryItem = storageItems.Find(i => i.itemName == item);
        if (inventoryItem != null)
        {
            inventoryItem.amount -= amount;
            if (inventoryItem.amount <= 0)
            {
                pawn.stuffCarried.Add(inventoryItem);
                storageItems.Remove(inventoryItem);
            }
        }
    }
}


public enum ZoneType
{
    Storage,

    Production

}