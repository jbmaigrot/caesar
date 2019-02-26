using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class arrow
{
    public int input;
    public int output;
    public float transmitTime;
    public List<float> timeBeforeTransmit;
};
public class InputHack
{
    public string inputcode;
    public int parameter_int;
    public string parameter_string;

    public InputHack()
    {
        inputcode = "OnHack";
        parameter_int = 0;
        parameter_string = "";
    }
}

public class OutputHack
{
    public string outputcode;
    public int parameter_int;
    public string parameter_string;

    public OutputHack()
    {
        outputcode = "TurnLightOff";
        parameter_int = 0;
        parameter_string = "";
    }
}

public class ProgrammableObjectsData : MonoBehaviour
{
    public HackingAssetScriptable HackingAsset;
    public GameObject HackInterface;
    public List<string> accessibleInputCode;
    public List<string> accessibleOutputCode;

    public List<InputHack> inputCodes=new List<InputHack>();
    public List<OutputHack> outputCodes=new List<OutputHack>();
    public List<arrow> graph= new List<arrow>();

    void Start()
    {
        foreach(arrow a in graph)
        {
            a.timeBeforeTransmit.Clear();
        }
    }

    void OnMouseDown()
    {
        ExecuteEvents.Execute<ISelectObject>(HackInterface, null, (x, y) => x.SelectedProgrammableObject(this.gameObject));
    }

}
