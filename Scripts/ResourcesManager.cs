using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ResourcesManager : MonoBehaviour
{
    public List<GameObject> resources;

    private TaskManager taskManager;
    void Start()
    {
        taskManager = FindObjectOfType<TaskManager>();
        taskManager.OnResourcesAdded += RemoveResource ;
    }


    public void AssignResourceTask(GameObject target, ResourceType resourceType)
    {
        if (resources.Count == 0)
        {
            return;
        }
        foreach (GameObject resource in resources)
        {
            if (resource.GetComponent<Items>().resourceType == resourceType)
            {
                resource.GetComponent<Items>().MoveResourceToConstruction(target);
                Task task = new Task(TaskType.GatherResource, resource.transform.position, resource);
                taskManager.AddTask(task);
            }
        }
    }

    public bool IsResoucesAvailable(ResourceType resourceType)
    {
        foreach (GameObject resource in resources)
        {
            if (resource.GetComponent<Items>().resourceType == resourceType)
            {
                return true;
            }
        }
        return false;
    }

    //Clear resources list if resource is destroyed
    public void RemoveResource()
    {
        foreach (GameObject resource in resources)
        {
            if (resource == null)
            {
                resources.Remove(resource);
            }
        }
    }

    public GameObject GetResource(ResourceType resourceType)
    {
        foreach (GameObject resource in resources)
        {
            if (resource.GetComponent<Items>().resourceType == resourceType)
            {
                return resource;
            }
        }
        return null;
    }

}
