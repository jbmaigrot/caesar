using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Typeinput {OnHack, OnReadWord};

public enum Typeoutput {Repeat, SendMessage, TurnLightOn, TurnLightOff};

public struct Hackarrow
{
    public int target;
    public float waitTime;
};

public struct Hackinput
{
    public Typeinput typeOfInput;
    public string parameter;
    public List<Hackarrow> arrow;
};

public struct Hackoutput
{
    public Typeoutput typeOfOutput;
    public string parameter;
};

public class ProgrammableObjectsData : MonoBehaviour
{
    List<Hackinput> hackinput;
    List<Hackoutput> hackoutput;

    private void Start()
    {
        Hackinput voiture;
        voiture.typeOfInput = Typeinput.OnHack;
        voiture.parameter = "";
        voiture.arrow = null;
        hackinput.Add(voiture);

        Hackoutput velodrome;
        velodrome.typeOfOutput = Typeoutput.TurnLightOn;
        velodrome.parameter = "";

        Hackoutput velodrome2;
        velodrome2.typeOfOutput = Typeoutput.TurnLightOff;
        velodrome2.parameter = "";

        Hackoutput velodrome3;
        velodrome3.typeOfOutput = Typeoutput.Repeat;
        velodrome3.parameter = "";

    }

    private void Update()
    {

    }
}
