using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThisIsATree : MonoBehaviour
{
    public AudioSource Beep;
    public float timeBeepBoopMin = 0.05f;
    public float timeBeepBoopMax = 0.5f;
    public AnimationCurve FadeOutCurve;
    public bool isSoundOn;

    public AudioSource HoloSound;
    public AudioClip HoloOn;
    public AudioClip HoloOff;

    private const float timeForDisappearing = 0.1f;
    private const float timeForAppearing = 0.1f;
    private float timeBeforeDisappearing;
    private float timeBeforeAppearing;
#if CLIENT
    private AudioClip[] BeepBoop;
    private float timeBeep;
    private float timeFadeOut;
    // Start is called before the first frame update
    void Start()
    {
        BeepBoop = Resources.LoadAll<AudioClip>("Beep");
        isSoundOn = true;
        timeFadeOut = 0.5f;
    }

    public void TurnOn()
    {
        if (!isSoundOn)
        {
            timeBeforeAppearing = timeForAppearing;
            isSoundOn = true;
            if (!HoloSound.isPlaying)
            {
                HoloSound.PlayOneShot(HoloOn);
            }
            
        }
    }

    public void TurnOff()
    {
        if (isSoundOn)
        {

            timeBeforeDisappearing = timeForDisappearing;
            isSoundOn = false;

            if (!HoloSound.isPlaying)
            {
                HoloSound.PlayOneShot(HoloOff);
            }
            
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        if (isSoundOn)
        {
            timeFadeOut = 0f;
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
        }
        else
        {
            if (timeFadeOut >= FadeOutCurve.keys[FadeOutCurve.length - 1].time)
            {
                timeBeep = Random.Range(timeBeepBoopMin, timeBeepBoopMax);
                Beep.volume = 0;
            }
            else
            {
                timeFadeOut += Time.deltaTime;
                Beep.volume = FadeOutCurve.Evaluate(timeFadeOut);
            }
        }

        if (!isSoundOn && timeBeforeDisappearing>0)
        {
            timeBeforeDisappearing -= Time.deltaTime;
            if (timeBeforeDisappearing <= 0)
            {
                if (GetComponentInChildren<Light>())
                {
                    GetComponentInChildren<Light>().enabled = false;
                }
                foreach (MeshRenderer ryan in GetComponentsInChildren<MeshRenderer>())
                {
                    ryan.enabled = false;
                }
            }
        }

        if (isSoundOn && timeBeforeAppearing>0)
        {
            timeBeforeAppearing -= Time.deltaTime;
            if (timeBeforeAppearing <= 0)
            {
                if (GetComponentInChildren<Light>())
                {
                    GetComponentInChildren<Light>().enabled = true;
                }
                foreach (MeshRenderer ryan in GetComponentsInChildren<MeshRenderer>())
                {
                    ryan.enabled = true;
                }
            }
        }
    }
#endif
}
