using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private const int MODE_CHARA = 0;
    private const int MODE_FREE = 1;

    public Button cameraModeButton;
    public GameObject characterToFollow;
    public GameObject cameraParent;

    [Header("0 : character, 1 : free")]
    public int cameraMode;

    [Header("Default value is 225. Apply to the camera's parent.")]
    public float defaultCameraYRotation = 225.0f;

    [Header("Default value is 50. Apply to the camera itself.")]
    public float defaultCameraXRotation = 50.0f;

    public float smoothTime = 0.6F;
    public float keyboardSpeed = 10.0f;
    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    public Plane floorPlane;

    [Header("Portion of the screen that activates free mode.")]
    public float freeModeBorder = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();

        if (cameraModeButton == null)
        {
            Debug.Log("Please reference the Camera mode toggle button in the inspector.");
        } else
        {
            cameraModeButton.onClick.AddListener(CameraModeButtonOnClick);
        }
        if (characterToFollow == null)
        {
            Debug.Log("There was no character referenced for the camera in the inspector, deactivating character camera mode.");
            cameraMode = MODE_FREE;
        } else
        {
            cameraMode = MODE_CHARA;
        }

        floorPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));

        cameraParent.transform.rotation = Quaternion.Euler(new Vector3(0.0f, defaultCameraYRotation, 0.0f));
        transform.localRotation = Quaternion.Euler(new Vector3(defaultCameraXRotation, 0.0f, 0.0f));
    }

    void Update()
    {
        Vector3 parentTargetPosition = cameraParent.transform.position;

        //Check if we are in the borders
        Vector3 mousePosition = Input.mousePosition;
        bool isInsideFreeModeBorder;

        Rect screen = new Rect(0, 0, Screen.width, Screen.height);
        if (IsInsideFreeModeBorder(mousePosition, screen, freeModeBorder))
        {
            cameraMode = MODE_FREE;
            isInsideFreeModeBorder = true;
        } else
        {
            isInsideFreeModeBorder = false;
        }

        //Check if we have keyboard input
        Vector2 axesInput = ReadKeyboardInput();
        if (axesInput != Vector2.zero)
        {
            cameraMode = MODE_FREE;
        }

        switch (cameraMode) {
            case MODE_CHARA:
                parentTargetPosition = characterToFollow.transform.position;
                break;
            case MODE_FREE:

                if (isInsideFreeModeBorder)
                {
                    Ray ray = cam.ScreenPointToRay(mousePosition);
                    float enter = 0.0f;
                    if (floorPlane.Raycast(ray, out enter))
                    {
                        parentTargetPosition = ray.GetPoint(enter);
                    }
                }

                parentTargetPosition = TargetPositionFromKeyboardInput(axesInput, parentTargetPosition);
                break;
            default:
                //
                break;
        }
        
        cameraParent.transform.position = Vector3.SmoothDamp(cameraParent.transform.position, parentTargetPosition, ref velocity, smoothTime);
    }

    void CameraModeButtonOnClick()
    {
        cameraMode = MODE_CHARA;
    }

    bool IsInsideFreeModeBorder(Vector2 mousePosition, Rect screen, float freeModeBorder)
    {
        Rect innerBorder = new Rect(screen.width * freeModeBorder / 2, screen.height * freeModeBorder / 2, screen.width * (1 - freeModeBorder), screen.height * (1 - freeModeBorder));

        if (!innerBorder.Contains(mousePosition) && screen.Contains(mousePosition))
        {
            return true;
        } else
        {
            return false;
        }
    }

    Vector2 ReadKeyboardInput ()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 axesInput = new Vector2(h, v);
        return axesInput;
    }

    Vector3 TargetPositionFromKeyboardInput(Vector2 axesInput, Vector3 curPosition)
    {
        Vector3 targetPosition = curPosition 
            + (axesInput.x * cameraParent.transform.right + axesInput.y * cameraParent.transform.forward) * keyboardSpeed;
        
        return targetPosition;
    }
}
