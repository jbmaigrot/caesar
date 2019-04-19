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

    //Start
    private void Start()
    {
        client = FindObjectOfType<Client>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + speed * Time.deltaTime;
    }

    // Tacle
    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)&& number != client.playerIndex)
        {
            client.Tacle(number);
        }
    }
#endif
}

