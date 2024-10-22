using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBox : MonoBehaviour
{

    public BoxCollider2D boxCollider;
    // Start is called before the first frame update

    public List<ISelectable> selectedObjects = new List<ISelectable>();


    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void OnTriggerStay2D(Collider2D other)
    {
        ISelectable selectable = other.GetComponent<ISelectable>();
        if (selectable != null)
        {
            Debug.Log("Object name: " + other.name);
            if (!selectedObjects.Contains(selectable))
            {
                selectedObjects.Add(selectable);
                selectable.Select();
            }
        }
    }

    void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Vector2 center = (Vector2)transform.position + boxCollider.offset;
            Gizmos.DrawWireCube(center, boxCollider.size);
        }
    }
}
