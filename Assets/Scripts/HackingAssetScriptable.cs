using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct InputCode
{
    public string inputCode;
    public string descriptionWithParameter;
    public string descriptionWithoutParameter;
    public bool parameter_int;
    public bool parameter_string;

}

public struct OutputCode
{
    public string outputCode;
    public string descriptionWithParameter;
    public string descriptionWithoutParameter;
    public bool parameter_int;
    public bool parameter_string;

}

public class HackingAssetScriptable : ScriptableObject
{
    public List<InputCode> inputCodes;
    public List<OutputCode> outputCodes;


    public string DescribeInputCode(string code, bool with_parameter, int parameter_int = 0, string parameter_string = "")
    {
        string result = "";

        foreach (InputCode c in inputCodes)
        {
            if (c.inputCode == code)
            {
                if (with_parameter)
                {
                    result = c.descriptionWithParameter;
                    if (c.parameter_int)
                    {
                        result += parameter_int;
                    }
                    if (c.parameter_string)
                    {
                        result += parameter_string;
                    }
                }
                else
                {
                    result = c.descriptionWithoutParameter;
                }
            }
        }
         return result;
    }

    public string DescribeOutputCode(string code, bool with_parameter, int parameter_int = 0, string parameter_string = "")
    {
        string result = "";

        foreach (OutputCode c in outputCodes)
        {
            if (c.outputCode == code)
            {
                if (with_parameter)
                {
                    result = c.descriptionWithParameter;
                    if (c.parameter_int)
                    {
                        result += parameter_int;
                    }
                    if (c.parameter_string)
                    {
                        result += parameter_string;
                    }
                }
                else
                {
                    result = c.descriptionWithoutParameter;
                }
            }
        }
        return result;
    }
}
