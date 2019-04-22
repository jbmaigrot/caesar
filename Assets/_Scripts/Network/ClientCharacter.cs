﻿using System.Collections;
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

    //Start
    private void Start()
    {
        client = FindObjectOfType<Client>();
        hackinterface = FindObjectOfType<HackInterface>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + speed * Time.deltaTime;
    }

    // Tacle
    public void OnMouseOver()
    {
        if (!Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1) && number != client.playerIndex&&!hackinterface.GetComponent<CanvasGroup>().blocksRaycasts) //Control+click is for energy-related actions
        {
            client.Tacle(number);
        }
    }
#endif
}

