using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptChatBox : MonoBehaviour
{

    static public string NewChatContent;
    private string ChatContent;
    // Start is called before the first frame update
    void Start()
    {
        NewChatContent = "";
        ChatContent = "\n";
    }

    // Update is called once per frame
    void Update()
    {
        if (NewChatContent != "")
        {
            ChatContent += NewChatContent;
            NewChatContent = "";
        }
        this.GetComponent<Text>().text = ChatContent;
    }
}
