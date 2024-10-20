using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Items : MonoBehaviour, IInteractable
{
    public string itemName;
    public int amount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(Pawns pawns)
    {
        
    }

    public void CancelInteraction()
    {
        
    }

    void AddItem(string item, int amount)
    {
        GameManager.instance.inventory.AddItem(item, amount);
    }
}
