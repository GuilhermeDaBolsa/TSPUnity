using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_movement : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    public float zoomStep;
    public float minCamSize;
    public float maxCamSize;

    private Vector3 dragOrigin;


    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        PanCameraHandle();
        ZoomCameraHandle();
    }


    private void PanCameraHandle() {
        if (Input.GetMouseButtonDown(0))
            dragOrigin = CurrentMousePosition();

        if (Input.GetMouseButton(0)) {
            Vector3 delta = dragOrigin - CurrentMousePosition();

            camera.transform.position += delta;
        }
    }


    private void ZoomCameraHandle() {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0)
            ZoomOut();
        else if (scrollInput < 0)
            ZoomIn();
    }


    public void ZoomIn() {
        float newCamSize = camera.orthographicSize + zoomStep;

        camera.orthographicSize = Mathf.Clamp(newCamSize, minCamSize, maxCamSize);
    }


    public void ZoomOut() {
        float newCamSize = camera.orthographicSize - zoomStep;

        camera.orthographicSize = Mathf.Clamp(newCamSize, minCamSize, maxCamSize);
    }


    private Vector3 CurrentMousePosition() {
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }
}