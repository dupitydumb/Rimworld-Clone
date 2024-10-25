using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    public float panSpeed = 20f; // Speed of camera movement
    public float panBorderThickness = 10f; // Thickness of the border that triggers camera movement
    public float scrollSpeed = 20f; // Speed of camera zoom
    public float minSize = 4f; // Minimum size of the camera
    public float maxSize = 12f; // Maximum size of the camera

    private BoundsInt bounds; // Bounds of the camera movement


    void Start()
    {
        // Set the bounds of the camera movement
        bounds = GameManager.instance.bounds;
    }
    void Update()
    {
        Vector3 pos = transform.position;

        // Panning with WASD keys
        if (Input.GetKey("w"))
        {
            pos.y += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        //if middle mouse button is pressed
        if (Input.GetMouseButton(2))
        {
            //Get the direction of the mouse movement
            Vector3 move = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            //Move the camera
            pos -= move * panSpeed * Time.deltaTime;
        }

        // Call the PanAtEdge method to handle panning at screen edges
        // PanAtEdge(ref pos);

        // make sure the camera stays within the bounds
        pos.x = Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x);
        pos.y = Mathf.Clamp(pos.y, bounds.min.y, bounds.max.y);

        // Zooming with mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize -= scroll * scrollSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minSize, maxSize);
        transform.position = pos;
    }

    void PanAtEdge(ref Vector3 pos)
    {
        // Panning with mouse at screen edges
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.y += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= panBorderThickness)
        {
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
    }
}