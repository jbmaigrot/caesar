using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientMessage : MonoBehaviour
{
    public Vector3 sourcePosition = new Vector3(0,0,0);
    private CameraController cam;
    private Minimap minimap;
#if CLIENT
    //Start
    void Start()
    {
        cam = Camera.main.GetComponent<CameraController>();
        minimap = FindObjectOfType<Minimap>();
    }

    //Click
    public void ShowMessageOnMap()
    {
        minimap.ShowMessage(sourcePosition);
    }

    /*
    //Click
    public void MoveCamera()
    {
        cam.cameraMode = 1;
        cam.transform.parent.position = sourcePosition;
    }*/
#endif
}
