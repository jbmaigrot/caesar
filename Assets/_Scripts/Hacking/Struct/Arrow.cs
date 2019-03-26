using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Arrow
{
    public int input;
    public int output;
    public float transmitTime = 0.2f;
    public List<float> timeBeforeTransmit = new List<float>();
}