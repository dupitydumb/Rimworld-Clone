using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public RectTransform selectionBox;
    private Vector2 startPosition;
    private List<GameObject> selectedObjects = new List<GameObject>();

    public Vector2 mouseOffset;
    void Update()
    {
        if (GameManager.instance.building.isPlacing)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0)) // Left Mouse Button
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                selectionBox.parent as RectTransform, 
                Input.mousePosition, 
                null, 
                out startPosition);
            selectionBox.gameObject.SetActive(true);
            selectionBox.anchoredPosition = startPosition;
            selectionBox.sizeDelta = Vector2.zero; // Reset the size
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 currentMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                selectionBox.parent as RectTransform, 
                Input.mousePosition, 
                null, 
                out currentMousePosition);
            UpdateSelectionBox(currentMousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectionBox.gameObject.SetActive(false);
            SelectObjects();
        }
    }

    void UpdateSelectionBox(Vector2 currentMousePosition)
    {
        Vector2 size = currentMousePosition - startPosition;
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
        selectionBox.anchoredPosition = startPosition + size / 2;
    }

    void SelectObjects()
    {


        //convert canvas space position to world space position 2d
        Vector2 min = selectionBox.TransformPoint(selectionBox.rect.min);
        Vector2 max = selectionBox.TransformPoint(selectionBox.rect.max);
        selectionBound = new Bounds((max + min) / 2, max - min);

        //Convert the selection bounds from screen space to world space
        selectionBound.size = Camera.main.ScreenToWorldPoint(selectionBound.size) - Camera.main.ScreenToWorldPoint(Vector3.zero);
        selectionBound.center = Camera.main.ScreenToWorldPoint(selectionBound.center);

        // Select all objects within the selection bounds on world space
        Collider2D[] colliders = Physics2D.OverlapAreaAll(selectionBound.min, selectionBound.max);
        foreach (var collider in colliders)
        {
            //Get ISelctable component from the collider
            ISelectable selectable = collider.GetComponent<ISelectable>();
            if (selectable != null)
            {
                selectable.Select();
            }
        }
    }

    private Bounds selectionBound;
    void OnDrawGizmos()
    {
        // Draw the selection bounds on world space
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(selectionBound.center, selectionBound.size);
        

    }
}