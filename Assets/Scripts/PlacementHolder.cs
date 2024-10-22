using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlacementHolder : MonoBehaviour
{  
    public static PlacementHolder instance;
    public bool isColliding = false;
    BoxCollider2D boxCollider;
    public SpriteRenderer spriteRenderer;
    public Event onBuildChange;
    
    
    void Awake()
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
    
    // Start is called before the first frame update
    
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {

    }
    // Update the preview of the placement holder


    // Called when the collider is bound to another collider
    private void OnTriggerStay2D(Collider2D collision)
    {
        isColliding = true;

        if (collision.gameObject.CompareTag("Grass"))
        {
            isColliding = false;
        }

    }

    // Called when the collider exits another collider
    private void OnTriggerExit2D(Collider2D collision)
    {
        isColliding = false;
    }
}