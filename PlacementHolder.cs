using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementHolder : MonoBehaviour
{
    public bool isColliding = false;
    BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called when the collider is bound to another collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isColliding = true;
    }

    // Called when the collider exits another collider
    private void OnTriggerExit2D(Collider2D collision)
    {
        isColliding = false;
    }
}