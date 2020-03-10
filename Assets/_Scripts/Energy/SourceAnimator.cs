using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceAnimator : MonoBehaviour
{
    public AudioSource Nappe;
    public AnimationCurve NappeWindowSpeedCurve;
    public AnimationCurve NappeLowpassFrequencyCurve;
    public AnimationCurve NappePitchCurve;
    public AnimationCurve NappeVolumeCurve;
    public AudioSource Beep;
    public AnimationCurve timeBeepCurve;
    public AnimationCurve BeeplowpassFrequencyCurve;

    public AnimationCurve FadeOutCurve;

    public int state = 0; // 0: off, 1: empty, 2: not empty, 3: giving
    public float lowSpeed = 2;
    public float highSpeed = 5;
    public float lerpLength = 1;
    
    private int prevState = 0;
    private float currentSpeed = 0;
    private float lerpT = 0;
    private Transform cube;
    private Renderer emissive;
    private Color emissiveColor = new Color(191 / 255f, 191 / 255f, 0f);
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
    private float timeLivingAsADataPool;
    private float timeFadeOut;
    private float timeNappeWindows;


    private float[] timeBeforeEndOfBeeps = new float[3] { 0,0,0};
    private bool canBeepPlay;

    // Start is called before the first frame update
    void Start()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		cube = transform.Find("Cube");
        emissive = cube.Find("Source 1").GetComponent<Renderer>();
        pointLight = cube.Find("Point Light").GetComponent<Light>();
        maxIntensity = pointLight.intensity;
        startingY = cube.localPosition.y;
        pointLight.intensity = 0;

        BeepBoop = Resources.LoadAll<AudioClip>("Beep");
        timeFadeOut = 0.5f;
    }

    // Update is called once per frame
    void Update()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		canBeepPlay = false;
        for(int i =0; i < 3; i++)
        {
            timeBeforeEndOfBeeps[i] -= Time.deltaTime;
            if (timeBeforeEndOfBeeps[i] <= 0)
            {
                timeBeforeEndOfBeeps[i] = 0;
                canBeepPlay = true;
            }
        }
        if (prevState != state)
        {
            lerpT = 0f;
        }
        lerpT += Time.deltaTime / lerpLength;
        timeNappeWindows = (timeNappeWindows+NappeWindowSpeedCurve.Evaluate(timeLivingAsADataPool)*Time.deltaTime)% 1f;
        switch (state)
        {
            case 0: /*Eteint don't rotate*/
                currentSpeed = Mathf.Lerp(currentSpeed, 0, lerpT);
                emission = Mathf.Lerp(emission, 0, lerpT);
                pointLight.intensity = Mathf.Lerp(pointLight.intensity, 0, lerpT);
                floatingT += Time.deltaTime * (1 - Mathf.Min(lerpT, 1));
                timeLivingAsADataPool = 0;
                if (timeFadeOut >= FadeOutCurve.keys[FadeOutCurve.length-1].time)
                {
                    timeBeep = 0;
                    Beep.volume = 0;
                    if (Nappe.isPlaying)
                    {
                        Nappe.Stop();
                    }
                }
                else
                {
                    timeFadeOut += Time.deltaTime;
                    Beep.volume = FadeOutCurve.Evaluate(timeFadeOut);
                    Nappe.volume = FadeOutCurve.Evaluate(timeFadeOut)*NappeVolumeCurve.Evaluate(timeNappeWindows);
                    Nappe.pitch = NappePitchCurve.Evaluate(timeNappeWindows);
                    Nappe.GetComponent<AudioLowPassFilter>().cutoffFrequency = NappeLowpassFrequencyCurve.Evaluate(timeNappeWindows);
                }
                
                break;

            case 1: /*Eteint rotate*/
                currentSpeed = Mathf.Lerp(currentSpeed, lowSpeed, lerpT);
                emission = Mathf.Lerp(emission, 1, lerpT);
                pointLight.intensity = Mathf.Lerp(pointLight.intensity, 0, lerpT);
                floatingT += Time.deltaTime * Mathf.Min(lerpT, 1);
                timeFadeOut = 0f;
                timeLivingAsADataPool += Time.deltaTime;
                if (!Nappe.isPlaying)
                {
                    Nappe.Play();
                    Nappe.volume = 1;
                }
                Nappe.volume = FadeOutCurve.Evaluate(timeFadeOut) * NappeVolumeCurve.Evaluate(timeNappeWindows);
                Nappe.pitch = NappePitchCurve.Evaluate(timeNappeWindows);
                Nappe.GetComponent<AudioLowPassFilter>().cutoffFrequency = NappeLowpassFrequencyCurve.Evaluate(timeNappeWindows);
                timeBeep += Time.deltaTime;
                Beep.volume = 1;
                if (timeBeep > timeBeepCurve.Evaluate(GetComponent<ServerCarrier>().clientCharge))
                {
                    timeBeep -= timeBeepCurve.Evaluate(GetComponent<ServerCarrier>().clientCharge);
                    if (canBeepPlay)
                    {
                        AudioClip RandomBeep = BeepBoop[Random.Range(0, BeepBoop.Length)];
                        Beep.PlayOneShot(RandomBeep);
                        bool isTheTimeInTheArray = false;
                        for (int i = 0; i < 3; i++)
                        {
                            if(timeBeforeEndOfBeeps[i] <= 0 && !isTheTimeInTheArray)
                            {
                                isTheTimeInTheArray = true;
                                timeBeforeEndOfBeeps[i] = RandomBeep.length;
                            }
                        }
                    }
                }
                
                
                break;

            case 2: /*Allumé rotate*/
                currentSpeed = Mathf.Lerp(currentSpeed, lowSpeed, lerpT);
                emission = Mathf.Lerp(emission, maxEmission, lerpT);
                pointLight.intensity = Mathf.Lerp(pointLight.intensity, maxIntensity, lerpT);
                floatingT += Time.deltaTime * Mathf.Min(lerpT, 1);
                timeFadeOut = 0f;
                timeLivingAsADataPool += Time.deltaTime;
                if (!Nappe.isPlaying)
                {
                     Nappe.Play();
                    Nappe.volume = 1;
                }
                Nappe.volume = FadeOutCurve.Evaluate(timeFadeOut) * NappeVolumeCurve.Evaluate(timeNappeWindows);
                Nappe.pitch = NappePitchCurve.Evaluate(timeNappeWindows);
                Nappe.GetComponent<AudioLowPassFilter>().cutoffFrequency = NappeLowpassFrequencyCurve.Evaluate(timeNappeWindows);
                timeBeep += Time.deltaTime;
                Beep.volume = 1;
                if (timeBeep > timeBeepCurve.Evaluate(GetComponent<ServerCarrier>().clientCharge))
                {
                    timeBeep -= timeBeepCurve.Evaluate(GetComponent<ServerCarrier>().clientCharge);
                    if (canBeepPlay)
                    {
                        AudioClip RandomBeep = BeepBoop[Random.Range(0, BeepBoop.Length)];
                        Beep.PlayOneShot(RandomBeep);
                        bool isTheTimeInTheArray = false;
                        for (int i = 0; i < 3; i++)
                        {
                            if (timeBeforeEndOfBeeps[i] <= 0 && !isTheTimeInTheArray)
                            {
                                isTheTimeInTheArray = true;
                                timeBeforeEndOfBeeps[i] = RandomBeep.length;
                            }
                        }
                    }
                }

                break;

            case 3: /*Allumé rotate rotate*/
                currentSpeed = Mathf.Lerp(currentSpeed, highSpeed, lerpT);
                emission = Mathf.Lerp(emission, maxEmission, lerpT);
                pointLight.intensity = Mathf.Lerp(pointLight.intensity, maxIntensity, lerpT);
                floatingT += Time.deltaTime * Mathf.Min(lerpT, 1);
                timeFadeOut = 0f;
                timeLivingAsADataPool += Time.deltaTime;
                if (!Nappe.isPlaying)
                {
                    Nappe.Play();
                    Nappe.volume = 1;
                }
                Nappe.volume = FadeOutCurve.Evaluate(timeFadeOut) * NappeVolumeCurve.Evaluate(timeNappeWindows);
                Nappe.pitch = NappePitchCurve.Evaluate(timeNappeWindows);
                Nappe.GetComponent<AudioLowPassFilter>().cutoffFrequency = NappeLowpassFrequencyCurve.Evaluate(timeNappeWindows);
                timeBeep += Time.deltaTime;
                Beep.volume = 1;
                if (timeBeep > timeBeepCurve.Evaluate(GetComponent<ServerCarrier>().clientCharge))
                {
                    timeBeep -= timeBeepCurve.Evaluate(GetComponent<ServerCarrier>().clientCharge);
                    if (canBeepPlay)
                    {
                        AudioClip RandomBeep = BeepBoop[Random.Range(0, BeepBoop.Length)];
                        Beep.PlayOneShot(RandomBeep);
                        bool isTheTimeInTheArray = false;
                        for (int i = 0; i < 3; i++)
                        {
                            if (timeBeforeEndOfBeeps[i] <= 0 && !isTheTimeInTheArray)
                            {
                                isTheTimeInTheArray = true;
                                timeBeforeEndOfBeeps[i] = RandomBeep.length;
                            }
                        }
                    }
                }

                break;

            default:
                break;
        }
        Beep.GetComponent<AudioLowPassFilter>().cutoffFrequency = BeeplowpassFrequencyCurve.Evaluate(timeLivingAsADataPool);
        
        // Floating animation
        cube.localPosition = new Vector3(0, startingY + floatingRange * (Mathf.Sin(floatingT * 2 * Mathf.PI * floatingFreq)), 0);

        // Rotation
        cube.Rotate(currentSpeed * Vector3.up, Space.World);
        prevState = state;

        // Emission
        emissive.material.SetColor("_EmissionColor", emissiveColor * Mathf.GammaToLinearSpace(emission));
    }
}
