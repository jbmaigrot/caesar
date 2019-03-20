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

    [Header("0 : character, 1 : free")]
    public int cameraMode;

    [Header("Default value are X = 28, Y = 50, Z = 27.5.")]
    public Vector3 characterOffset = new Vector3(28.0f, 50.0f, 27.5f);

    [Header("Default value are X = 50, Y = 225, Z = 0.")]
    public Vector3 defaultCameraRotation = new Vector3(50.0f, 225.0f, 0.0f);
    

    public float smoothTime = 0.6F;
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

        transform.rotation = Quaternion.Euler(defaultCameraRotation);
    }

    // LateUpdate for the movement to happen after all updates
    void Update()
    {
        Vector3 targetPosition = transform.position;

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

        switch (cameraMode) {
            case MODE_CHARA:
                targetPosition = characterToFollow.transform.position + characterOffset;
                break;
            case MODE_FREE:

                if (isInsideFreeModeBorder)
                {
                    Ray ray = cam.ScreenPointToRay(mousePosition);
                    float enter = 0.0f;
                    if (floorPlane.Raycast(ray, out enter))
                    {
                        targetPosition = ray.GetPoint(enter) + characterOffset;
                    }
                }
                break;
            default:
                //
                break;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
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
}
