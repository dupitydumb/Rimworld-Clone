using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Pawns : MonoBehaviour, ISelectable
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

    private bool isOnAtask = false;
    public List<InventoryItem> stuffCarried = new List<InventoryItem>();

    public GameObject task;
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
        
        if (coorObject != null)
        {
            if (Vector2.Distance(transform.position, coorObject.transform.position) < 0.5f)
            {
                Destroy(coorObject);
            }
        }
    
        // Check if the pawn is stationary
        if (Vector2.Distance(transform.position, lastPosition) < 0.01f)
        {
            stationaryTime += Time.deltaTime;
        }
        else
        {
            stationaryTime = 0f;
            lastPosition = transform.position;
        }
        FixPathNotFound();
    }


    bool isNotMovingForAWhile()
    {
        // Detect if the pawn is not moving for a while
        return stationaryTime >= stationaryThreshold;
    }
    //Get task interval
    private void FixedUpdate()
    {
        
        if (tempPath)
        {
            return;
        }
        if (GameManager.instance.constructionToDo.Count > 0 && task == null)
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
        BoundsInt bounds = GameManager.instance.bounds;
    }
    private Vector2 lastPosition;
    private float stationaryTime = 0f;
    private float stationaryThreshold = 5f;
    private bool tempPath = false;
    private float stuckTimer = 0f;
    private float stuckThreshold = 2f;

    private GameObject tempPathObject;
    void FixPathNotFound()
    {
        // If the pawn is not moving for a while, add a random destination nearby
        if (isNotMovingForAWhile())
        {
            
            
        }

        if (tempPath)
        {
            
        }
    }
    void GetTask()
    {
        //Get task without pawns assigned
        foreach (GameObject task in GameManager.instance.constructionToDo)
        {
            //Check if the task is not assigned to any pawn
            if (task.GetComponent<IInteractable>().GetWorker() != null)
            {
                continue;
            }
            if (task.GetComponent<IInteractable>().GetWorker() == null)
            {
                //Set the destination
                AID.target = task.transform;
                destination = task.transform.position;
                // Call The Interact Method
                if (Vector2.Distance(transform.position, destination) < 0.5f)
                {
                    IInteractable interactable = task.GetComponent<IInteractable>();
                    interactable.Interact(this);
                }
                return;
            }
        }


        // if (GameManager.instance.constructionToDo.Count > 0)
        // {
        //     //Get the first task
        //     GameObject task = GameManager.instance.constructionToDo[0];
        //     //Set the destination
        //     AID.target = task.transform;
        //     destination = task.transform.position;
        //     // Call The Interact Method

        //     if (Vector2.Distance(transform.position, destination) < 0.5f)
        //     {
        //         IInteractable interactable = task.GetComponent<IInteractable>();
        //         interactable.Interact(this);
        //     }
        //     // IInteractable interactable = task.GetComponent<IInteractable>();
        //     // interactable.Interact(this);
        // }
    }

    private GameObject coorObject;
    public void SetDestination(Vector2 coordinates)
    {
        if (coorObject != null)
        {
            Destroy(coorObject);
        }
        //Set the destination of the pawn
        coorObject = Instantiate(Resources.Load("Prefabs/Target", typeof(GameObject))) as GameObject;
        coorObject.transform.position = coordinates;
        AID.target = coorObject.transform;
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


    public void HighLight()
    {
        // Change the color of the pawn when highlighted
        head.GetComponent<SpriteRenderer>().color = Color.yellow;
        body.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void DeHighLight()
    {
        // Change the color of the pawn when dehighlighted
        head.GetComponent<SpriteRenderer>().color = Color.white;
        body.GetComponent<SpriteRenderer>().color = Color.white;
    }
    public void Select()
    {
        // Change the color of the pawn when selected
        GameManager.instance.pawnSelectionManager.pawns.Add(this.gameObject);
        GameObject highlighted = transform.Find("Highlight").gameObject;
        highlighted.SetActive(true);
    }

    public void Deselect()
    {
        GameManager.instance.pawnSelectionManager.pawns.Remove(this.gameObject);
        GameObject highlighted = transform.Find("Highlight").gameObject;
        highlighted.SetActive(false);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
