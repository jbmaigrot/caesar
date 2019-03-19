using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class ProgrammableObjectsScriptable : ScriptableObject
{

    public List<string> accessibleInputCode;
    public List<string> accessibleOutputCode;
    public List<InputHack> inputCodes = new List<InputHack>();
    public List<OutputHack> outputCodes = new List<OutputHack>();
    public List<Arrow> graph = new List<Arrow>();
    
}
