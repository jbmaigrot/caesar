using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryAnimator : MonoBehaviour
{
    public AudioSource Nappe;
    public AudioSource Beep;
    public AudioSource Boop;
    public float timeBeepBoopMin = 0.05f;
    public float timeBeepBoopMax = 0.5f;
    public AnimationCurve FadeOutCurve;

    public int state = 0; // 0: off, 1: receiving
    public float smallRingSpeed = 2;
    public float mediumRingSpeed = -2;
    public float largeRingSpeed = 2;
    public Transform smallRing;
    public Transform mediumRing;
    public Transform largeRing;

#if CLIENT
    private int prevState = 0;
    private float currentSmallRingSpeed = 0;
    private float currentMediumRingSpeed = 0;
    private float currentLargeRingSpeed = 0;
    private float lerpT = 0;
    private AudioClip[] BeepBoop;
    private float timeBeep;
    private float timeBoop;
    private float timeFadeOut;

    // Start is called before the first frame update
    void Start()
    {
        BeepBoop = Resources.LoadAll<AudioClip>("Beep");
        timeFadeOut = 0.5f;
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
            case 0: /*Off, don't rotate*/
                currentSmallRingSpeed = Mathf.Lerp(currentSmallRingSpeed, 0, lerpT);
                currentMediumRingSpeed = Mathf.Lerp(currentMediumRingSpeed, 0, lerpT);
                currentLargeRingSpeed = Mathf.Lerp(currentLargeRingSpeed, 0, lerpT);
                
                /*if (timeFadeOut >= FadeOutCurve.keys[FadeOutCurve.length - 1].time)
                {
                    timeBeep = Random.Range(timeBeepBoopMin, timeBeepBoopMax);
                    timeBoop = Random.Range(timeBeepBoopMin, timeBeepBoopMax);
                    Beep.volume = 0;
                    Boop.volume = 0;
                    if (Nappe.isPlaying)
                    {
                        Nappe.Stop();
                    }
                }
                else
                {
                    timeFadeOut += Time.deltaTime;
                    Beep.volume = FadeOutCurve.Evaluate(timeFadeOut);
                    Boop.volume = FadeOutCurve.Evaluate(timeFadeOut);
                    Nappe.volume = FadeOutCurve.Evaluate(timeFadeOut);
                }*/

                break;

            case 1: /*Receiving, rotate*/
                currentSmallRingSpeed = Mathf.Lerp(currentSmallRingSpeed, smallRingSpeed, lerpT);
                currentMediumRingSpeed = Mathf.Lerp(currentMediumRingSpeed, mediumRingSpeed, lerpT);
                currentLargeRingSpeed = Mathf.Lerp(currentLargeRingSpeed, largeRingSpeed, lerpT);

                /*timeFadeOut = 0f;
                if (!Nappe.isPlaying)
                {
                    Nappe.Play();
                    Nappe.volume = 1;
                }
                if (!Beep.isPlaying)
                {
                    timeBeep -= Time.deltaTime;
                    if (timeBeep < 0)
                    {
                        Beep.volume = 1;
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
                        Boop.volume = 1;
                        Boop.clip = BeepBoop[Random.Range(0, BeepBoop.Length)];
                        Boop.Play();
                        timeBoop = Random.Range(timeBeepBoopMin, timeBeepBoopMax);
                    }
                }*/

                break;

            default:
                break;
        }

        // Rotation
        smallRing.Rotate(currentSmallRingSpeed * Vector3.right, Space.World);
        mediumRing.Rotate(currentMediumRingSpeed * (Vector3.right + Vector3.forward), Space.World);
        largeRing.Rotate(currentLargeRingSpeed * Vector3.forward, Space.World);

        prevState = state;
    }
#endif
}