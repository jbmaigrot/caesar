﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
		if (!GameState.CLIENT) return; // replacement for preprocessor

		client = FindObjectOfType<Client>();
        mask = transform.Find("Mask").gameObject;
        text = transform.Find("Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

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
		if (!GameState.CLIENT) return; // replacement for preprocessor

		cooldown = 20;
        mask.SetActive(true);
    }
}
