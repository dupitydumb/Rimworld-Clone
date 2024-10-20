using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Pawns : MonoBehaviour
{
    
    public Direction direction;
    public Sprite[] spritesHead;
    public Sprite[] spritesBody;
    public GameObject head;
    public GameObject body;
    public float Speed = 1f;
    private Vector2 destination;
    public GameObject pathprefab;
    AIDestinationSetter AID;
    public List<Transform> destinations = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        //Find the head and body objects in the children of this object
        head = transform.Find("Head").gameObject;
        body = transform.Find("Body").gameObject;

        AID = GetComponent<AIDestinationSetter>();

        
    }

    // Update is called once per frame
    void Update()
    {
        //Roam the world
        if (WorldGeneration.instance.isGenerating == true)
        {
            return;
        }
        

        // Move to mouse click
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     pathprefab.transform.position = mousePosition;
            
        // }
    }

    //Get task interval
    private void FixedUpdate()
    {
        FixPathNotFound();
        if (tempPath)
        {
            return;
        }
        if (GameManager.instance.constructionToDo.Count > 0)
        {
            GetTask();
        }
        else
        {
            RoamTheWorld();
        }
    }
    void RoamTheWorld()
    {
        //randomize path prefab position
        if (destinations.Count == 0)
        {
            Vector2 randomPosition = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
            GameObject path = Instantiate(pathprefab, randomPosition, Quaternion.identity);
            destinations.Add(path.transform);
        }

    }

    private bool tempPath = false;
    void FixPathNotFound()
    {
        //If in the pawn not moving for a while, add random destination nearby
        if (Vector2.Distance(transform.position, destination) < 0.1f)
        {
            tempPath = true;
            Vector2 randomPosition = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
            GameObject path = Instantiate(pathprefab, randomPosition, Quaternion.identity);
            destinations.Add(path.transform);
        }
        if (tempPath)
        {
            if (Vector2.Distance(transform.position, destination) < 0.1f)
            {
                tempPath = false;
                destinations.RemoveAt(0);
            }
        }
    }
    void GetTask()
    {
        if (GameManager.instance.constructionToDo.Count > 0)
        {
            //Get the first task
            GameObject task = GameManager.instance.constructionToDo[0];
            //Set the destination
            AID.target = task.transform;
            // AID.target = task.transform;
            task.GetComponent<Walls>().worker = this;
        }
    }
    void Move(Vector2 coordinates)
    {
        //Move the pawn to the new position
        transform.position = Vector2.MoveTowards(transform.position, coordinates, Speed * Time.deltaTime);
        //Change direction based on the movement
        // Change direction based on the movement
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
                head.GetComponent<SpriteRenderer>().sprite = spritesHead[2];
                body.GetComponent<SpriteRenderer>().sprite = spritesBody[0];
                break;
            case Direction.Down:
                head.GetComponent<SpriteRenderer>().sprite = spritesHead[0];
                body.GetComponent<SpriteRenderer>().sprite = spritesBody[0];
                break;
            case Direction.Left:
                head.GetComponent<SpriteRenderer>().sprite = spritesHead[1];
                body.GetComponent<SpriteRenderer>().sprite = spritesBody[1];
                //flip the body and head
                head.transform.localScale = new Vector3(-1, 1, 1);
                body.transform.localScale = new Vector3(-1, 1, 1);
                break;
            case Direction.Right:
                head.GetComponent<SpriteRenderer>().sprite = spritesHead[1];
                body.GetComponent<SpriteRenderer>().sprite = spritesBody[1];
                head.transform.localScale = new Vector3(1, 1, 1);
                body.transform.localScale = new Vector3(1, 1, 1);
                break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.6f);
    }
    
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
