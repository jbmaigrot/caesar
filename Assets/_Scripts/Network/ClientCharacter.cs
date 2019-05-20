using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCharacter : MonoBehaviour
{

    public MeshRenderer Body;
    public MeshRenderer Lens;

    public AudioSource NappeDataAudioSource;
    public AnimationCurve NappeDataVolume;
    public float NappeDataMinCurveSpeed;
    public float NappeDataMedCurveSpeed;
    public float NappeDataMaxCurveSpeed;

    public AudioSource NappeMoveAudioSource;
    public AnimationCurve NappeMoveVolume;
    public AnimationCurve NappeMovePitch;


    public AudioSource StunAudioSource;

    public AudioSource StunQAudioSource;

    public AnimationCurve FadeOutCurve;

    private const float MaxSpeed = 5f;

#if CLIENT
    public Vector3 speed = new Vector3(0, 0, 0);
    public int number;
    public bool isTacle;
    public float TimeBeforeEndOfTacle;

    private Client client;
    private HackInterface hackinterface;

    private float floatingRange = 0.1f;
    private float floatingFreq = 0;
    public Transform mesh;
    private float startingY;

    public bool isAlly = false;
    public bool isKnownAsAlly = false;
    public string playerName;

    private Animator stunAnimator;
    
    private bool isDataEmpty;

    private float NappeDataCurveTime;
    private AudioClip[] StunClip;
    private AudioClip[] StunQClip;
    private float DelayBetweenStartOfNextClipAndEndOfPresentOne;
    SpriteRenderer lightning;


    //Start
    private void Start()
    {
        client = FindObjectOfType<Client>();
        hackinterface = FindObjectOfType<HackInterface>();

        floatingFreq = Random.Range(0.3f, 0.4f);
        mesh = transform.Find("Mesh").transform;
        startingY = mesh.transform.localPosition.y;

        stunAnimator = this.transform.Find("StunLightning").GetComponent<Animator>();

        StunClip= Resources.LoadAll<AudioClip>("Stun");
        StunQClip = Resources.LoadAll<AudioClip>("StunQ");

        isDataEmpty = true;
        DelayBetweenStartOfNextClipAndEndOfPresentOne = Random.Range(0.15f,0.25f);
        lightning = this.transform.Find("StunLightning").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        NappeMoveAudioSource.volume = NappeMoveVolume.Evaluate(speed.magnitude / MaxSpeed);
        NappeMoveAudioSource.pitch = NappeMovePitch.Evaluate(speed.magnitude / MaxSpeed);
        /*
        if (client.characters[client.playerIndex] == this)
        {
            NappeDataCurveTime = (NappeDataCurveTime + Time.deltaTime * (this.GetComponent<ServerCarrier>().clientCharge < 0.05 ? NappeDataMinCurveSpeed  : this.GetComponent<ServerCarrier>().clientCharge < 0.95 ? NappeDataMedCurveSpeed : NappeDataMaxCurveSpeed)) % 1f;
            NappeDataAudioSource.volume = NappeDataVolume.Evaluate(NappeDataCurveTime);
        }
        else
        {*/
            NappeDataAudioSource.volume = 0;
        /*}*/
        

        transform.position = transform.position + speed * Time.deltaTime;

        // Floating animation
        mesh.localPosition = new Vector3(0, startingY - floatingRange / 2 + floatingRange * (Mathf.Sin(Time.time * 2 * Mathf.PI * floatingFreq)), 0);

        if(this.GetComponent<ServerCarrier>().clientCharge > 0.05)
        {
            if (isDataEmpty)
            {
                isDataEmpty = false;
                if (!isTacle)
                {
                    Body.material.SetFloat("_IsShining", 1f);
                    Body.material.SetColor("_EmissiveColor", new Color(1f, 236f/255f, 0f,1f));
                    Body.material.SetFloat("_WaveSpeed", 3.0f);
                    Lens.material.SetColor("_EmissionColor", new Color(1f, 236f / 255f, 0f, 1f));
                }
            }
        }
        else
        {
            if (!isDataEmpty)
            {
                isDataEmpty = true;
                if (!isTacle)
                {
                    Body.material.SetFloat("_IsShining", 0f);
                    Body.material.SetColor("_EmissiveColor", new Color(0f, 0f, 0f,1f));
                    Body.material.SetFloat("_WaveSpeed", 0.0f);
                    Lens.material.SetColor("_EmissionColor", new Color(146f/255f, 214f/255f, 1f,1f));
                    
                }
            }
        }

        if (isTacle&&stunAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime<1)
        {
            if (StunAudioSource.isPlaying)
            {
                if(!StunQAudioSource.isPlaying && StunAudioSource.clip.length - StunAudioSource.time < DelayBetweenStartOfNextClipAndEndOfPresentOne)
                {
                    StunQAudioSource.clip = StunQClip[Random.Range(0,StunQClip.Length)];
                    StunQAudioSource.Play();
                    DelayBetweenStartOfNextClipAndEndOfPresentOne = Random.Range(0f, 0.25f);
                }
            }
            else
            {
                if (!StunQAudioSource.isPlaying || StunQAudioSource.clip.length - StunQAudioSource.time < DelayBetweenStartOfNextClipAndEndOfPresentOne)
                {
                    StunAudioSource.clip = StunClip[Random.Range(0, StunClip.Length)];
                    StunAudioSource.Play();
                    DelayBetweenStartOfNextClipAndEndOfPresentOne = Random.Range(0.15f, 0.25f);

                }
                
            }
            if(stunAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime> 1- (0.5f/ stunAnimator.GetCurrentAnimatorStateInfo(0).length))
            {
                StunAudioSource.volume = FadeOutCurve.Evaluate(0.5f - TimeBeforeEndOfTacle);
                StunQAudioSource.volume = FadeOutCurve.Evaluate(0.5f - TimeBeforeEndOfTacle);
            }
            else
            {
                StunAudioSource.volume = 1;
                StunQAudioSource.volume = 1;
            }
        }
        else
        {
            StunAudioSource.volume = 0;
            StunQAudioSource.volume = 0;
        }
    }

    // Tacle
    public void OnMouseOver()
    {
        if (!Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1) && number != client.playerIndex && !hackinterface.GetComponent<CanvasGroup>().blocksRaycasts) //Control+click is for energy-related actions
        {
            client.Tacle(number);
        }
    }

    public void GetTacled(bool OnNotOff)
    {
        if (OnNotOff)
        {
            if (!isTacle)
            {
                isTacle = true;
                
                lightning.enabled = true;
                stunAnimator.Play(0);
                Body.material.SetFloat("_IsShining", 1f);
                Body.material.SetColor("_EmissiveColor", new Color(1f, 0f, 9f/255f,1f));
                Body.material.SetFloat("_WaveSpeed", 8.0f);
                Lens.material.SetColor("_EmissionColor", new Color(1f,0f,9f/255f,1f));
            }

        }
        else
        {
            if (isTacle)
            {
                isTacle = false;
                lightning.enabled = false;
                if (isDataEmpty)
                {
                    Body.material.SetFloat("_IsShining", 0f);
                    Body.material.SetColor("_EmissiveColor", new Color(0f, 0f, 0f, 1f));
                    Body.material.SetFloat("_WaveSpeed", 0.0f);
                    Lens.material.SetColor("_EmissionColor", new Color(146f/255f, 214/255f, 1f, 1f));
                }
                else
                {
                    Body.material.SetFloat("_IsShining", 1f);
                    Body.material.SetColor("_EmissiveColor", new Color(1f, 236f/255f, 0f,1f));
                    Body.material.SetFloat("_WaveSpeed", 3.0f);
                    Lens.material.SetColor("_EmissionColor", new Color(1f, 236f/255f,0f,1f));
                }
                
            }

        }
    }
#endif
}

