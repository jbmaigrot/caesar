using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private const int MODE_CHARA = 0;
    private const int MODE_FREE = 1;

    public Button cameraModeToggleButton;
    public GameObject characterToFollow;

    [Header("0 : character, 1 : free")]
    public int cameraMode;

    [Header("Default value are X = 25, Y = 50, Z = 25.5.")]
    public Vector3 characterOffset;

    [Header("Default value are X = 50, Y = 225, Z = 0.")]
    public Vector3 defaultCameraRotation = new Vector3(50.0f, 225.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        if (cameraModeToggleButton == null)
        {
            Debug.Log("Please reference the Camera mode toggle button in the inspector.");
        }
        if (characterToFollow == null)
        {
            Debug.Log("There was no character referenced for the camera in the inspector, deactivating character camera mode.");
            cameraMode = MODE_FREE;
        } else
        {
            cameraMode = MODE_CHARA;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (cameraMode) {
            case MODE_CHARA:
                transform.position = characterToFollow.transform.position + characterOffset;
                break;
            case MODE_FREE:
                //
                break;
            default:
                //
                break;
        }
    }
}
