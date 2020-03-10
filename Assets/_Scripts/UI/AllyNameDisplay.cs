using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllyNameDisplay : MonoBehaviour
{

    public CameraController cameraController;
    public Text allyNameText;
    //float baseTextScale;
    public RectTransform canvasRT;
    float baseCanvasScale;
    // Start is called before the first frame update
    void Start()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		cameraController = FindObjectOfType<CameraController>();

        //baseTextScale = allyNameText.rectTransform.localScale.x;
        
        canvasRT = this.GetComponent<RectTransform>();
        if (canvasRT != null)
        {
            canvasRT.rotation = Quaternion.Euler(50.0f, -45.0f, 0.0f);

            if (FindObjectOfType<Client>().team == 1 )
                canvasRT.rotation = Quaternion.Euler(50.0f, 135.0f, 0.0f);

            baseCanvasScale = canvasRT.localScale.x;
        }
    }

    // Update is called once per frame
    void Update()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		//transform.LookAt(transform.position + cameraController.transform.rotation * Vector3.forward, cameraController.transform.rotation * Vector3.up);

		//allyNameText.GetComponent<RectTransform>().localScale = new Vector3(baseTextScale * cameraController.zoomFactor, baseTextScale * cameraController.zoomFactor, 1.0f);

		canvasRT.localScale = new Vector3(baseCanvasScale * cameraController.zoomFactor, baseCanvasScale * cameraController.zoomFactor, 1.0f);
    }
}
