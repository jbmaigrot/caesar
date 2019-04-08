using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientMessage : MonoBehaviour
{
    private Vector3 sourcePosition;
    private CameraController camera;

    //Constructors
    public ClientMessage() { }

    public ClientMessage(Vector3 pos)
    {
        sourcePosition = pos;
    }

    //
    void Start()
    {
        camera = Camera.main.GetComponent<CameraController>();
    }

    //Click
    public void MoveCamera()
    {
        camera.cameraMode = 1;
        camera.transform.parent.position = sourcePosition;
    }
}
