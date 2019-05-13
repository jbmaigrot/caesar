using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceAnimator : MonoBehaviour
{
#if CLIENT
    public int state = 0; // 0: off, 1: active, 2: giving
    public float lowSpeed = 2;
    public float highSpeed = 5;

    private int prevState = 0;
    private float currentSpeed = 0;
    private float lerpT = 0;
    private Transform cube;
    private Renderer emissive;
    private Color emissiveColor = new Color(191,191,0);
    private float emission = 0;
    private Light pointLight;
    private float startingY;
    private float floatingRange = 20;
    private float floatingFreq = 0.3f;
    private float floatingT = 0;


    // Start is called before the first frame update
    void Start()
    {
        cube = transform.Find("Cube");
        emissive = cube.Find("Source 1").GetComponent<Renderer>();
        pointLight = cube.Find("Point Light").GetComponent<Light>();
        startingY = cube.localPosition.y;
        pointLight.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (prevState != state)
        {
            lerpT = 0f;
        }
        lerpT += Time.deltaTime;

        switch (state)
        {
            case 0:
                currentSpeed = Mathf.Lerp(currentSpeed, 0, lerpT);
                emission = Mathf.Lerp(emission, 0, lerpT);
                pointLight.intensity = Mathf.Lerp(pointLight.intensity, 0, lerpT);
                floatingT += Time.deltaTime * (1 - Mathf.Min(lerpT, 1));
                break;

            case 1:
                currentSpeed = Mathf.Lerp(currentSpeed, lowSpeed, lerpT);
                emission = Mathf.Lerp(emission, 0, lerpT);
                pointLight.intensity = Mathf.Lerp(pointLight.intensity, 0, lerpT);
                floatingT += Time.deltaTime * Mathf.Min(lerpT, 1);
                break;

            case 2:
                currentSpeed = Mathf.Lerp(currentSpeed, lowSpeed, lerpT);
                emission = Mathf.Lerp(emission, 0.5f, lerpT);
                pointLight.intensity = Mathf.Lerp(pointLight.intensity, 1000, lerpT);
                floatingT += Time.deltaTime * Mathf.Min(lerpT, 1);
                break;

            case 3:
                currentSpeed = Mathf.Lerp(currentSpeed, highSpeed, lerpT);
                emission = Mathf.Lerp(emission, 0.5f, lerpT);
                pointLight.intensity = Mathf.Lerp(pointLight.intensity, 1000, lerpT);
                floatingT += Time.deltaTime * Mathf.Min(lerpT, 1);
                break;

            default:
                break;
        }
        
        // Floating animation
        cube.localPosition = new Vector3(0, startingY + floatingRange * (Mathf.Sin(floatingT * 2 * Mathf.PI * floatingFreq)), 0);

        // Rotation
        cube.Rotate(currentSpeed * Vector3.up, Space.World);
        prevState = state;

        // Emission
        emissive.material.SetColor("_EmissionColor", emissiveColor * emission);
    }
#endif
}
