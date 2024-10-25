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
    
    private Grid grid;
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
        grid = GameManager.instance.grid;
    }

    void Update()
    {
        if (GameManager.instance.building.isPlacing)
        {

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Vector3Int gridPos = grid.WorldToCell(mousePos);
            Vector3 finalPos = grid.GetCellCenterWorld(gridPos);
            this.transform.position = finalPos;

            if (isColliding)
            {
                spriteRenderer.color = new Color32(255, 0, 0, 100);
            }
            else
            {
                spriteRenderer.color = new Color32(0, 255, 0, 100);
            }
        }
        else
        {
            spriteRenderer.color = new Color32(255, 255, 255, 0);
        }

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