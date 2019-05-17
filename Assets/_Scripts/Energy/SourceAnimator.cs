using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceAnimator : MonoBehaviour
{
    public AudioSource Nappe;
    public AudioSource Beep;
    public AudioSource Boop;
    public float timeBeepBoopMin = 0.05f;
    public float timeBeepBoopMax = 0.5f;

    public int state = 0; // 0: off, 1: empty, 2: not empty, 3: giving
    public float lowSpeed = 2;
    public float highSpeed = 5;
    public float lerpLength = 1;

#if CLIENT
    private int prevState = 0;
    private float currentSpeed = 0;
    private float lerpT = 0;
    private Transform cube;
    private Renderer emissive;
    private Color emissiveColor = new Color(191/255f,191/255f,0f);
    private float emission = 0;
    private float maxEmission = 3;
    private Light pointLight;
    private float maxIntensity;
    private float startingY;
    private float floatingRange = 0.2f;
    private float floatingFreq = 0.3f;
    private float floatingT = 0;
    private AudioClip[] BeepBoop;
    private float timeBeep;
    private float timeBoop;

    // Start is called before the first frame update
    void Start()
    {
        cube = transform.Find("Cube");
        emissive = cube.Find("Source 1").GetComponent<Renderer>();
        pointLight = cube.Find("Point Light").GetComponent<Light>();
        maxIntensity = pointLight.intensity;
        startingY = cube.localPosition.y;
        pointLight.intensity = 0;

        BeepBoop = Resources.LoadAll<AudioClip>("Beep");
    }

    // Update is called once per frame
    void Update()
    {
        if (prevState != state)
        {
            lerpT = 0f;
        }
        lerpT += Time.deltaTime / lerpLength;

        switch (state)
        {
            case 0: /*Eteint don't rotate*/
                currentSpeed = Mathf.Lerp(currentSpeed, 0, lerpT);
                emission = Mathf.Lerp(emission, 0, lerpT);
                pointLight.intensity = Mathf.Lerp(pointLight.intensity, 0, lerpT);
                floatingT += Time.deltaTime * (1 - Mathf.Min(lerpT, 1));
                if (Nappe.isPlaying)
                {
                    Nappe.Stop();
                }
                timeBeep = Random.Range(timeBeepBoopMin, timeBeepBoopMax);
                timeBoop = Random.Range(timeBeepBoopMin, timeBeepBoopMax);
                Beep.Stop();
                Boop.Stop();
                break;

            case 1: /*Eteint rotate*/
                currentSpeed = Mathf.Lerp(currentSpeed, lowSpeed, lerpT);
                emission = Mathf.Lerp(emission, 1, lerpT);
                pointLight.intensity = Mathf.Lerp(pointLight.intensity, 0, lerpT);
                floatingT += Time.deltaTime * Mathf.Min(lerpT, 1);
                if (!Nappe.isPlaying)
                {
                    Nappe.Play();
                }
                if (!Beep.isPlaying)
                {
                    timeBeep -= Time.deltaTime;
                    if (timeBeep < 0)
                    {
                        Beep.clip = BeepBoop[Random.Range(0, BeepBoop.Length)];
                        Beep.Play();
                        timeBeep = Random.Range(timeBeepBoopMin, timeBeepBoopMax);
                    }
                    
                }
                if (!Boop.isPlaying)
                {
                    timeBoop -= Time.deltaTime;
                    if (timeBoop < 0)
                    {
                        Boop.clip = BeepBoop[Random.Range(0, BeepBoop.Length)];
                        Boop.Play();
                        timeBoop = Random.Range(timeBeepBoopMin, timeBeepBoopMax);
                    }

                }
                break;

            case 2: /*Allumé rotate*/
                currentSpeed = Mathf.Lerp(currentSpeed, lowSpeed, lerpT);
                emission = Mathf.Lerp(emission, maxEmission, lerpT);
                pointLight.intensity = Mathf.Lerp(pointLight.intensity, maxIntensity, lerpT);
                floatingT += Time.deltaTime * Mathf.Min(lerpT, 1);
                if (!Nappe.isPlaying)
                {
                     Nappe.Play();
                }
                if (!Beep.isPlaying)
                {
                    timeBeep -= Time.deltaTime;
                    if (timeBeep < 0)
                    {
                        Beep.clip = BeepBoop[Random.Range(0, BeepBoop.Length)];
                        Beep.Play();
                        timeBeep = Random.Range(timeBeepBoopMin, timeBeepBoopMax);
                    }

                }
                if (!Boop.isPlaying)
                {
                    timeBoop -= Time.deltaTime;
                    if (timeBoop < 0)
                    {
                        Boop.clip = BeepBoop[Random.Range(0, BeepBoop.Length)];
                        Boop.Play();
                        timeBoop = Random.Range(timeBeepBoopMin, timeBeepBoopMax);
                    }

                }
                break;

            case 3: /*Allumé rotate rotate*/
                currentSpeed = Mathf.Lerp(currentSpeed, highSpeed, lerpT);
                emission = Mathf.Lerp(emission, maxEmission, lerpT);
                pointLight.intensity = Mathf.Lerp(pointLight.intensity, maxIntensity, lerpT);
                floatingT += Time.deltaTime * Mathf.Min(lerpT, 1);
                if (!Nappe.isPlaying)
                {
                    Nappe.Play();
                }
                if (!Beep.isPlaying)
                {
                    timeBeep -= Time.deltaTime;
                    if (timeBeep < 0)
                    {
                        Beep.clip = BeepBoop[Random.Range(0, BeepBoop.Length)];
                        Beep.Play();
                        timeBeep = Random.Range(timeBeepBoopMin, timeBeepBoopMax);
                    }

                }
                if (!Boop.isPlaying)
                {
                    timeBoop -= Time.deltaTime;
                    if (timeBoop < 0)
                    {
                        Boop.clip = BeepBoop[Random.Range(0, BeepBoop.Length)];
                        Boop.Play();
                        timeBoop = Random.Range(timeBeepBoopMin, timeBeepBoopMax);
                    }

                }
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
        emissive.material.SetColor("_EmissionColor", emissiveColor * Mathf.GammaToLinearSpace(emission));
    }
#endif
}
