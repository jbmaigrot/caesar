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
    public Vector3 inputPos = new Vector3();
    public Vector3 outputPos = new Vector3();

    public Arrow()
    {

    }
    public Arrow(int _input, int _output, float _transmitTime, List<float> _timeBeforeTransmit)
    {
        input = _input;
        output = _output;
        transmitTime = _transmitTime;
        timeBeforeTransmit = _timeBeforeTransmit;
    }
}