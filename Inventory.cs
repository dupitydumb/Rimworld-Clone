using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public string itemName;
    public int amount;
}

public class Inventory : MonoBehaviour
{
    [SerializeField]
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(string item, int amount)
    {
        InventoryItem inventoryItem = items.Find(i => i.itemName == item);
        if (inventoryItem != null)
        {
            inventoryItem.amount += amount;
        }
        else
        {
            items.Add(new InventoryItem { itemName = item, amount = amount });
        }
    }

    public void RemoveItem(string item, int amount)
    {
        InventoryItem inventoryItem = items.Find(i => i.itemName == item);
        if (inventoryItem != null)
        {
            inventoryItem.amount -= amount;
            if (inventoryItem.amount <= 0)
            {
                items.Remove(inventoryItem);
            }
        }
    }
}