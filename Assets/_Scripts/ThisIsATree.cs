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

    private AudioClip[] BeepBoop;
    private float timeBeep;
    private float timeFadeOut;
    private MeshRenderer[] meshRenderers;
    private float baseVolume;
    private AudioListener listener;
    private const float distanceAudioBeep=10f;
    // Start is called before the first frame update
    void Start()
    {
        BeepBoop = Resources.LoadAll<AudioClip>("Beep");
        isSoundOn = true;
        timeFadeOut = 0.5f;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        listener = FindObjectOfType<AudioListener>();
        baseVolume = Beep.volume;
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
                if (timeBeep < 0 && Vector3.Distance(listener.transform.position, this.transform.position)<distanceAudioBeep)
                {
                    Beep.volume = baseVolume;
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
#if SERVER
                GetComponentInParent<ProgrammableObjectsData>().OnInput("OnTurnOff");
#endif
                if (GetComponentInChildren<Light>())
                {
                    GetComponentInChildren<Light>().enabled = false;
                }
                foreach (MeshRenderer ryan in meshRenderers)
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
#if SERVER
                GetComponentInParent<ProgrammableObjectsData>().OnInput("OnTurnOn");
#endif
                if (GetComponentInChildren<Light>())
                {
                    GetComponentInChildren<Light>().enabled = true;
                }
                foreach (MeshRenderer ryan in meshRenderers)
                {
                    ryan.enabled = true;
                }
            }
        }
    }

            }
