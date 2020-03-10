using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientMessage : MonoBehaviour
{
    public Vector3 sourcePosition = new Vector3(0,0,0);
    private CameraController cam;
    private Minimap minimap;

    //Start
    void Start()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		cam = Camera.main.GetComponent<CameraController>();
        minimap = FindObjectOfType<Minimap>();
    }

    //Click
    public void ShowMessageOnMap()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		minimap.ShowMessage(sourcePosition);
    }

	/*
    //Click
    public void MoveCamera()
    {
		if (!GameState.CLIENT) return; // replacement for preprocessor

        cam.cameraMode = 1;
        cam.transform.parent.position = sourcePosition;
    }*/
}
