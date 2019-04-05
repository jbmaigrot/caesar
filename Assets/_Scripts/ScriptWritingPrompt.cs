using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptWritingPrompt : MonoBehaviour
{
    private string currentString;
    public GameObject HackInterface;
    private bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        currentString = "";
    }

    public void OnMessageSend()
    {

        if (this.GetComponent<InputField>().text[this.GetComponent<InputField>().text.Length - 1] == '\n')
        {
            ScriptChatBox.NewChatContent += this.GetComponent<InputField>().text;
        }
        else
        {
            ScriptChatBox.NewChatContent += this.GetComponent<InputField>().text + "\n";
        }

        this.GetComponent<InputField>().text = "";


    }
    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<InputField>().isFocused && Input.GetKeyDown(KeyCode.Return) && !Input.GetKey(KeyCode.RightShift) && !Input.GetKey(KeyCode.LeftShift))
        {
            OnMessageSend();
        }

        if (this.GetComponent<InputField>().isFocused)
        {
            isActive = true;
        }
        else
        {
            if (isActive)
            {
                isActive = false;
            }
        }
    }
}
