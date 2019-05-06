using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCharacter : MonoBehaviour
{
#if CLIENT
    public Vector3 speed = new Vector3(0, 0, 0);
    public int number;
    public bool isTacle;

    private Client client;
    private HackInterface hackinterface;

    private float floatingRange = 0.1f;
    private float floatingFreq = 0;
    private Transform mesh;
    private float startingY;

    public bool isAlly = false;
    public string playerName;

    private Animator stunAnimator;

    //Start
    private void Start()
    {
        client = FindObjectOfType<Client>();
        hackinterface = FindObjectOfType<HackInterface>();

        floatingFreq = Random.Range(0.3f, 0.4f);
        mesh = transform.Find("Mesh").transform;
        startingY = transform.localPosition.y;

        stunAnimator = this.transform.Find("StunLightning").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + speed * Time.deltaTime;

        // Floating animation
        mesh.localPosition = new Vector3(0, startingY - floatingRange / 2 + floatingRange * (Mathf.Sin(Time.time * 2 * Mathf.PI * floatingFreq)), 0);
    }

    // Tacle
    public void OnMouseOver()
    {
        if (!Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1) && number != client.playerIndex&&!hackinterface.GetComponent<CanvasGroup>().blocksRaycasts) //Control+click is for energy-related actions
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
                foreach (MeshRenderer ryan in this.gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    ryan.material.color = new Color(ryan.material.color.r * 0.8f, ryan.material.color.g * 1.5625f, ryan.material.color.b * 0.8f);
                }
            }
            
        }
        else
        {
            if (isTacle)
            {
                isTacle = false;
                SpriteRenderer lightning = this.transform.Find("StunLightning").GetComponent<SpriteRenderer>();
                lightning.enabled = false;
                foreach (MeshRenderer ryan in this.gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    ryan.material.color = new Color(ryan.material.color.r * 1.25f, ryan.material.color.g * 0.64f, ryan.material.color.b * 1.25f);
                }
            }
            
        }
    }
#endif
}

