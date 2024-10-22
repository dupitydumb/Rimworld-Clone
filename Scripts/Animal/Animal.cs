using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
public class Animal : MonoBehaviour
{
    private Direction direction;
    public string Name { get; set; }
    public int Age { get; set; }
    AIDestinationSetter AID;
    public List<Transform> destinations = new List<Transform>();
    SpriteRenderer spriteRenderer;

    public Sprite[] spritesDirection;
    // 0 is North, 1 is East, 2 is South
    public Animal(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public void Start()
    {
        //Find child sprite renderer
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        AID = GetComponent<AIDestinationSetter>();
    }
    public virtual void Speak()
    {
        Debug.Log("The animal makes a sound.");
    }

    private Vector2 coordinates;
    public void MoveToDestination()
    {
        if (WorldGeneration.instance.isGenerating == true)
        {
            return;
        }

        if (destinations.Count == 0)
        {
            return;
        }

        if (destinations.Count > 0)
        {
            AID.target = destinations[0];
        }
        
        if (Vector2.Distance(transform.position, destinations[0].position) < 0.1f)
        {
            //Destroy the destination object
            Destroy(destinations[0].gameObject);
            destinations.RemoveAt(0);
        }

        coordinates = destinations[0].position;
        //Change direction
        if (Mathf.Abs(coordinates.x - transform.position.x) > Mathf.Abs(coordinates.y - transform.position.y))
        {
            if (coordinates.x > transform.position.x)
            {
                direction = Direction.Right;
            }
            else if (coordinates.x < transform.position.x)
            {
                direction = Direction.Left;
            }
        }
        else
        {
            if (coordinates.y > transform.position.y)
            {
                direction = Direction.Up;
            }
            else if (coordinates.y < transform.position.y)
            {
                direction = Direction.Down;
            }
        }

        // Change direction based on where the pawn is facing
        switch (direction)
        {
            case Direction.Up:
                spriteRenderer.sprite = spritesDirection[0];
                break;
            case Direction.Right:
                spriteRenderer.flipX = false;
                spriteRenderer.sprite = spritesDirection[1];
                break;
            case Direction.Down:
                spriteRenderer.sprite = spritesDirection[2];
                break;
            case Direction.Left:
                spriteRenderer.flipX = true;
                spriteRenderer.sprite = spritesDirection[1];
                break;
        }
    }

    public void CreateNewDestination()
    {
        if (destinations.Count > 0)
        {
            return;
        }
        if (WorldGeneration.instance.isGenerating == true)
        {
            return;
        }
        Vector2 randomPosition = new Vector2(Random.Range(-WorldGeneration.instance.width / 2, WorldGeneration.instance.width/ 2), Random.Range(-WorldGeneration.instance.height / 2, WorldGeneration.instance.height / 2));
        // Convert Vector2 to transform
        Transform newDestination = new GameObject().transform;
        newDestination.position = randomPosition;
        destinations.Add(newDestination);
        newDestination.gameObject.name = "Destination";
        //Set parent to the world generation object
        newDestination.parent = GameObject.Find("Unused").transform;
    }
}