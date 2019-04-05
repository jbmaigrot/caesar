using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientChatInput : MonoBehaviour
{
    public NetworkManager networkManager;
    public Text chatBox;
    private InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponentInChildren<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputField.isFocused && Input.GetKeyDown(KeyCode.Return))
        {
            networkManager.Message(inputField.text);
            inputField.text = "";
        }
    }

    //
    public void AddMessage(string message) 
    {
        chatBox.text += "\n" + message;
    }
}

