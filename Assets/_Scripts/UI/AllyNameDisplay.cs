using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyNameDisplay : MonoBehaviour
{
#if CLIENT
    public CameraController cameraController;
    public TextMesh allyNameText;
    float baseCharacterSize;
    // Start is called before the first frame update
    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
        baseCharacterSize = allyNameText.characterSize;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + cameraController.transform.rotation * Vector3.forward, cameraController.transform.rotation * Vector3.up);
        allyNameText.characterSize = baseCharacterSize * cameraController.zoomFactor;
    }
#endif
}
