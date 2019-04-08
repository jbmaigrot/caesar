using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if CLIENT
public class ClientChatInput : MonoBehaviour
{
    public Text chatBox;

    private InputField inputField;
    private Client client;
    
    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponentInChildren<InputField>();
        client = FindObjectOfType<Client>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            client.Message(inputField.text);
            inputField.text = "";
        }
    }

    //
    public void AddMessage(string message) 
    {
        chatBox.text += "\n" + message;
    }
}
#endif

