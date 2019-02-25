using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptWritingPrompt : MonoBehaviour
{
    private string currentString;
    // Start is called before the first frame update
    void Start()
    {
        currentString = "";
    }

    // Update is called once per frame
    void Update()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b') // has backspace/delete been pressed?
            {
                if (currentString.Length != 0)
                {
                    currentString = currentString.Substring(0, currentString.Length - 1);
                }
            }
            else if ((c == '\n') || (c == '\r')) // enter/return
            {
                if (currentString != "")
                {
                    ScriptChatBox.NewChatContent += currentString + "\n";
                    currentString = "";
                }
                               
            }
            else
            {
                currentString += c;
            }
        }
        this.GetComponent<Text>().text  = currentString;
    }
}
