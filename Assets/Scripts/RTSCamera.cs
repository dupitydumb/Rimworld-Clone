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

    public float minX = -50f;
    public float maxX = 50f;
    public float minY = -50f;
    public float maxY = 50f;

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

        // Call the PanAtEdge method to handle panning at screen edges
        // PanAtEdge(ref pos);

        // Clamp the camera position within the defined boundaries
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

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