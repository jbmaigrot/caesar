using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InOutVignette
{
    public string code;
    public int parameter_int;
    public string parameter_string;
    public bool is_fixed;
    public InOutVignette()
    {
        code = "Default";
        parameter_int = 0;
        parameter_string = "";
    }
}
