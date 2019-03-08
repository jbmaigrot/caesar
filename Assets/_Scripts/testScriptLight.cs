using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class testScriptLight : MonoBehaviour, IMessageReceiver
{
    // Start is called before the first frame update
    public void ChatInstruction(string instruction)
    {/*
        if (instruction == "light")
        {
            this.GetComponent<Light>().intensity = 100;
        }
        if (instruction == "dark")
        {
            this.GetComponent<Light>().intensity = 0;
        }*/
    }
}
