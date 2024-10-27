using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System;

public enum TaskType
{
    GatherResource,
    Build,
    MoveToPosition
}

[System.Serializable]
public class Task
{
    public TaskType taskType;
    public Vector3 targetPosition;
    public GameObject targetObject;

    public Task(TaskType taskType, Vector3 targetPosition, GameObject targetObject = null)
    {
        this.taskType = taskType;
        this.targetPosition = targetPosition;
        this.targetObject = targetObject;
    }
}
public class TaskManager : MonoBehaviour
{
    [SerializeField]
    public Queue<Task> taskQueue = new Queue<Task>();
    private List<Pawns> pawns = new List<Pawns>();

    public List<Task> pendingTasks = new List<Task>();

    public Action OnResourcesAdded;

    void Start()
    {
        // Initialize pawns list
        pawns = new List<Pawns>(FindObjectsOfType<Pawns>());
        OnResourcesAdded += AddPendingTasks;
    }

    void Update()
    {
        AssignTasks();
    }

    public void AddTask(Task task)
    {
        Debug.LogWarning("Task added");
        taskQueue.Enqueue(task);
    }

    private void AssignTasks()
    {
        foreach (Pawns pawn in pawns)
        {
            if (pawn.CurrentTask.targetObject == null && taskQueue.Count > 0)
            {
                Task task = taskQueue.Dequeue();
                pawn.AssignTask(task);
            }
        }
    }

    public void RemoveTask(GameObject task)
    {
        // Remove task where game object is the target object
        Task taskToRemove = taskQueue.FirstOrDefault(t => t.targetObject == task);
        if (taskToRemove != null)
        {
            taskQueue = new Queue<Task>(taskQueue.Where(t => t != taskToRemove));
        }
    }

    //Add Pending Tasks to the task queue
    public void AddPendingTasks()
    {
        foreach (Task task in pendingTasks)
        {
            taskQueue.Enqueue(task);
        }
        pendingTasks.Clear();
    }
}


[CustomEditor(typeof(TaskManager))]
public class TaskManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TaskManager taskManager = (TaskManager)target;

        EditorGUILayout.LabelField("Task Queue", EditorStyles.boldLabel);

        if (taskManager.taskQueue.Count == 0)
        {
            EditorGUILayout.LabelField("No tasks in queue");
        }
        else
        {
            foreach (Task task in taskManager.taskQueue)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Task Type", task.taskType.ToString());
                EditorGUILayout.LabelField("Target Position", task.targetPosition.ToString());
                if (task.targetObject != null)
                {
                    EditorGUILayout.ObjectField("Target Object", task.targetObject, typeof(GameObject), true);
                }
                EditorGUILayout.EndVertical();
            }
        }
    }

    //Add a button to refresh the task queue

}