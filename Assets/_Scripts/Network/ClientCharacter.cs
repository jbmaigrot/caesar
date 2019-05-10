using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCharacter : MonoBehaviour
{

    public MeshRenderer Body;
    public MeshRenderer Lens;

#if CLIENT
    public Vector3 speed = new Vector3(0, 0, 0);
    public int number;
    public bool isTacle;

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

    private MaterialPropertyBlock block_default;
    private MaterialPropertyBlock block_nonempty_nonstun;
    private MaterialPropertyBlock block_stun;

    private bool isDataEmpty;

    //Start
    private void Start()
    {
        client = FindObjectOfType<Client>();
        hackinterface = FindObjectOfType<HackInterface>();

        floatingFreq = Random.Range(0.3f, 0.4f);
        mesh = transform.Find("Mesh").transform;
        startingY = transform.localPosition.y;

        stunAnimator = this.transform.Find("StunLightning").GetComponent<Animator>();

        block_default = new MaterialPropertyBlock();
        block_default.SetFloat("_IsShining", 0f);
        block_default.SetColor("_EmissiveColor", new Color(0, 0, 0));
        block_default.SetFloat("_WaveSpeed", 0.0f);

        block_nonempty_nonstun = new MaterialPropertyBlock();
        block_nonempty_nonstun.SetFloat("_IsShining", 1f);
        block_nonempty_nonstun.SetColor("_EmissiveColor", new Color(255, 236, 0));
        block_nonempty_nonstun.SetFloat("_WaveSpeed", 3.0f);

        block_stun = new MaterialPropertyBlock();
        block_stun.SetFloat("_IsShining", 1f);
        block_stun.SetColor("_EmissiveColor", new Color(255, 0, 9));
        block_stun.SetFloat("_WaveSpeed", 8.0f);

        isDataEmpty = true;
    }

    // Update is called once per frame
    void Update()
    {
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
                    Body.material.SetColor("_EmissiveColor", new Color(255, 236, 0));
                    Body.material.SetFloat("_WaveSpeed", 3.0f);
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
                    MeshRenderer ryan = this.gameObject.GetComponentInChildren<MeshRenderer>();
                    Body.material.SetFloat("_IsShining", 1f);
                    Body.material.SetFloat("_IsShining", 0f);
                    Body.material.SetColor("_EmissiveColor", new Color(0, 0, 0));
                    Body.material.SetFloat("_WaveSpeed", 0.0f);
                }
            }
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
                SpriteRenderer lightning = this.transform.Find("StunLightning").GetComponent<SpriteRenderer>();
                lightning.enabled = true;
                stunAnimator.Play(0);
                Body.material.SetFloat("_IsShining", 1f);
                Body.material.SetColor("_EmissiveColor", new Color(255, 0, 9));
                Body.material.SetFloat("_WaveSpeed", 8.0f);
            }

        }
        else
        {
            if (isTacle)
            {
                isTacle = false;
                SpriteRenderer lightning = this.transform.Find("StunLightning").GetComponent<SpriteRenderer>();
                lightning.enabled = false;
                if (isDataEmpty)
                {
                    Body.material.SetFloat("_IsShining", 0f);
                    Body.material.SetColor("_EmissiveColor", new Color(0, 0, 0));
                    Body.material.SetFloat("_WaveSpeed", 0.0f);
                }
                else
                {
                    Body.material.SetFloat("_IsShining", 1f);
                    Body.material.SetColor("_EmissiveColor", new Color(255, 236, 0));
                    Body.material.SetFloat("_WaveSpeed", 3.0f);
                }
                
            }

        }
    }
#endif
}

