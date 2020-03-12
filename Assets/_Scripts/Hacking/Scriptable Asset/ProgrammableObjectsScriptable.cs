using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class ProgrammableObjectsScriptable : ScriptableObject
{

    public List<string> accessibleInputCode;
    public List<string> accessibleOutputCode;
    public List<InOutVignette> inputCodes = new List<InOutVignette>();
    public List<InOutVignette> outputCodes = new List<InOutVignette>();
    public List<Arrow> graph = new List<Arrow>();

    public List<InOutVignette> initialOutputActions = new List<InOutVignette>();
    public bool isHackable;
    public bool shouldBeSendToClientEveryFrame;
    public string baseName;
}
