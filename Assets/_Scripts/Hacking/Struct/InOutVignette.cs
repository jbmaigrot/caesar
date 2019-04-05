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

    public InOutVignette(string _code, int _parameter_int, string _parameter_string, bool _is_fixed)
    {
        code = _code;
        parameter_int = _parameter_int;
        parameter_string = _parameter_string;
        is_fixed = _is_fixed;
    }
}
