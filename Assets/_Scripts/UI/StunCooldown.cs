using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if CLIENT
public class StunCooldown : MonoBehaviour
{
    private Client client;
    private GameObject mask;
    private Text text;
    private float cooldown = 0;
    private bool hastoplayasound = false;

    // Start
    private void Start()
    {
        client = FindObjectOfType<Client>();
        mask = transform.Find("Mask").gameObject;
        text = transform.Find("Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown > 0)
        {

            hastoplayasound = true;
            text.text = "" + Mathf.FloorToInt(cooldown);
            cooldown -= Time.deltaTime;
        }
        else
        {
            if (hastoplayasound)
            {
                hastoplayasound = false;
                GetComponent<AudioSource>().Play();
            }
            
            text.text = "";
            mask.SetActive(false);
        }
    }

    public void StartCooldown()
    {
        cooldown = 20;
        mask.SetActive(true);
    }
}
#endif