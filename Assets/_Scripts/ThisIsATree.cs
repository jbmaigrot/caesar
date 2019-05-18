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
    }
#endif
}
