using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum ResourceType
{
    Wood,
    Stone,
    Iron
}
public class Walls : MonoBehaviour, IInteractable
{

    public ResourceType resourceType;
    public int resourceAmount;
    private Color32 originalColor;
    public float updateRadius = 5f;
    public float constructionTime = 5f;

    public Pawns worker;
    // Start is called before the first frame update
    void Start()
    {
        // Store the original color
        originalColor = GetComponent<SpriteRenderer>().color;
        // Set collision to false
        GetComponent<BoxCollider2D>().enabled = false;
        // Set color to transparent. set the alpha only
        GetComponent<SpriteRenderer>().color = new Color32(originalColor.r, originalColor.g, originalColor.b, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (worker != null)
        {
            // Check if the worker is close to the wall
            if (Vector2.Distance(worker.transform.position, transform.position) < 0.8f)
            {
                BuildWall();
            }
        }
    }

    public void BuildWall()
    {
        constructionTime -= Time.deltaTime;
        if (constructionTime <= 0)
        {
            // Find a safe position for the worker
            Vector3 safePosition = FindSafePosition(worker.transform.position);

            // Move the worker to the safe position
            worker.transform.position = safePosition;

            // Enable collision
            worker = null;
            GetComponent<BoxCollider2D>().enabled = true;
            // Set color to opaque
            GetComponent<SpriteRenderer>().color = originalColor;
            GameManager.instance.constructionToDo.Remove(gameObject);
            // Update the graph near the wall
            UpdateGraphNearWall();
        }
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
        if (worker != null)
        {
            worker.destinations.Remove(transform);
            worker = null;
            GameManager.instance.constructionToDo.Remove(gameObject);
        }
    }

    
    void UpdateGraphNearWall()
    {
        Vector3 wallPosition = transform.position;
        Bounds updateBounds = new Bounds(wallPosition, new Vector3(updateRadius * 2, updateRadius * 2, updateRadius * 2));

        GraphUpdateObject guo = new GraphUpdateObject(updateBounds);
        AstarPath.active.UpdateGraphs(guo);
    }

    Vector3 FindSafePosition(Vector3 currentPosition)
    {
        Vector3[] directions = {
            Vector3.up,
            Vector3.right,
            Vector3.down,
            Vector3.left
        };

        foreach (Vector3 direction in directions)
        {
            Vector3 newPosition = currentPosition + direction;
            if (!IsPositionOccupied(newPosition))
            {
                return newPosition;
            }
        }

        // If no safe position is found, return the current position
        return currentPosition;
    }

    bool IsPositionOccupied(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.GetComponent<Walls>() != null)
            {
                return true;
            }
        }
        return false;
    }

}
