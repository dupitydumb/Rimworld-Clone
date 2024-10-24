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
        selectedObjects.Clear();
        Rect selectionRect = new Rect(selectionBox.anchoredPosition - selectionBox.sizeDelta / 2, selectionBox.sizeDelta);
        
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Tree"))
        {
            if (selectionRect.Contains(Camera.main.WorldToScreenPoint(obj.transform.position)))
            {
                selectedObjects.Add(obj);
                // Optionally change the color or highlight the selected object
            }
        }
    }
}