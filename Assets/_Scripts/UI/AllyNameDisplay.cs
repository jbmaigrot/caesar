using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllyNameDisplay : MonoBehaviour
{
#if CLIENT
    public CameraController cameraController;
    public Text allyNameText;
    float baseTextScale;
    // Start is called before the first frame update
    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();

        baseTextScale = allyNameText.rectTransform.localScale.x;
        Canvas canvas = this.GetComponent<Canvas>();
        if (canvas != null)
        {
            GetComponent<RectTransform>().rotation = Quaternion.Euler(50.0f, -45.0f, 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(transform.position + cameraController.transform.rotation * Vector3.forward, cameraController.transform.rotation * Vector3.up);

        allyNameText.GetComponent<RectTransform>().localScale = new Vector3(baseTextScale * cameraController.zoomFactor, baseTextScale * cameraController.zoomFactor, 1.0f);
    }
#endif
}
