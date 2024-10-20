using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectingUI : MonoBehaviour
{
    public GameObject selectionBox;
    private Vector3 startMousePosition;
    private Vector3 currentMousePosition;
    private bool isSelecting = false;

    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = selectionBox.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Start selection
        if (Input.GetMouseButtonDown(0))
        {
            isSelecting = true;
            var mouseposX = Input.mousePosition.x;
            var mouseposY = Input.mousePosition.y;
            startMousePosition = new Vector3(mouseposX, mouseposY, 4);
            if (selectionBox != null)
            {
                selectionBox.SetActive(true);
            }
        }

        // Update selection
        if (isSelecting)
        {
            var mouseposX = Input.mousePosition.x;
            var mouseposY = Input.mousePosition.y;
            currentMousePosition = new Vector3(mouseposX, mouseposY, 4);
            UpdateSelectionBox();
        }

        // End selection
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
            if (selectionBox != null)
            {
                selectionBox.SetActive(false);
            }
            SelectObjects();
        }
    }

    void UpdateSelectionBox()
    {
        if (selectionBox == null) return;

        Vector3 startScreenPosition = Camera.main.ScreenToWorldPoint(startMousePosition);
        Vector3 endScreenPosition = Camera.main.ScreenToWorldPoint(currentMousePosition);

        // Lock the z-axis
        startScreenPosition.z = 0;
        endScreenPosition.z = 0;

        // Calculate the corners of the selection box
        Vector3 topLeft = Vector3.Min(startScreenPosition, endScreenPosition);
        Vector3 bottomRight = Vector3.Max(startScreenPosition, endScreenPosition);

        // Change the slice width and height to match the selection
        sr.size = new Vector2(bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
    }

    void SelectObjects()
    {
        // Convert screen positions to world positions
        Vector3 startWorldPosition = Camera.main.ScreenToWorldPoint(startMousePosition);
        Vector3 endWorldPosition = Camera.main.ScreenToWorldPoint(currentMousePosition);

        startMousePosition.z = 0;
        currentMousePosition.z = 0;
        // Create a rectangular area from the world positions
        Bounds selectionBounds = new Bounds();
        selectionBounds.SetMinMax(Vector3.Min(startWorldPosition, endWorldPosition), Vector3.Max(startWorldPosition, endWorldPosition));
        
        // Find all selectable objects within the selection bounds
        
    }
}