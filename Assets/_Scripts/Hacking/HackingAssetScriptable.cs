using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InOutCode
{
    public string code;
    public string descriptionWithParameter;
    public string descriptionWithoutParameter;
    public bool parameter_int;
    public bool parameter_string;

}

public class HackingAssetScriptable : ScriptableObject
{
    public List<InOutCode> inputCodes = new List<InOutCode>();
    public List<InOutCode> outputCodes = new List<InOutCode>();


    public string DescribeCode(string code, bool with_parameter, bool InNotOut, int parameter_int = 0, string parameter_string = "")
    {
        
        string result = "";
        List<InOutCode> listOfCode;

        if (InNotOut)
        {
            listOfCode = inputCodes;
        }
        else
        {
            listOfCode = outputCodes;
        }

        foreach (InOutCode c in listOfCode)
        {
            
            if (c.code == code)
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

