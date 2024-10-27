using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEditor.Actions;
using System;
using UnityEditor.VersionControl;

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
    public AIDestinationSetter AID;

    private bool isOnAtask = false;
    [SerializeField]
    public List<InventoryItem> stuffCarried = new List<InventoryItem>();

    //Show in the inspector
    [SerializeField]
    private Task currentTask; // Backing field for CurrentTask

    public Task CurrentTask
    {
        get { return currentTask; }
        private set { currentTask = value; }
    }

    private TaskManager taskManager;
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
        //Debug CurrentTask
        if (CurrentTask != null)
        {
            Debug.LogWarning(CurrentTask.targetObject);
        }
        else
        {
            Debug.LogWarning("No task");
        }


        //Roam the world
        if (WorldGeneration.instance.isGenerating == true)
        {
            return;
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

    public void AssignTask(Task task)
    {
        if (task.targetObject == null)
        {
            return;
        }
        CurrentTask = task;
        ExecuteTask();
        AID.target = task.targetObject.transform;
    }

    private void ExecuteTask()
    {
        if (CurrentTask != null)
        {
            switch (CurrentTask.taskType)
            {
                case TaskType.GatherResource:
                    MoveTo(CurrentTask.targetPosition, () => InteractWith(CurrentTask.targetObject));
                    break;
                case TaskType.Build:
                    MoveTo(CurrentTask.targetPosition, () => InteractWith(CurrentTask.targetObject));
                    break;
                case TaskType.MoveToPosition:
                    MoveTo(CurrentTask.targetPosition, () => CurrentTask = null);
                    break;
            }
        }
    }

    private void InteractWith(GameObject targetObject)
    {
        IInteractable interactable = targetObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact(this);
        }
        // if completed the task, set the current task to null
        if (interactable is Tree)
        {
            Tree buildingObject = interactable as Tree;
            if (buildingObject.isCompleted)
            {
                CurrentTask = null;
            }
        }
        if (interactable is Walls)
        {
            Walls buildingObject = interactable as Walls;
            if (buildingObject.isCompleted)
            {
                CurrentTask = null;
            }
        }
    }
    
    public void MoveTo(Vector3 targetPosition, Action onArrival)
    {
        StartCoroutine(MoveToCoroutine(targetPosition, onArrival));
    }

    private IEnumerator MoveToCoroutine(Vector3 targetPosition, Action onArrival)
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.5f)
        {
            AID.target = CurrentTask.targetObject.transform;
            yield return null;
        }

        onArrival?.Invoke();
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
