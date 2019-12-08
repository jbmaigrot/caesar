using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class IPConnectionInterfaceManager : MonoBehaviour
{

    public InputField ipInputField;
    private ClientLobby clientLobby;
    private PopupMessageManager popupMessageManager;
    public GameObject interfacePanel;

    bool allowEnter;

#if CLIENT
    // Start is called before the first frame update
    void Start()
    {
		if(!GameState.CLIENT) return; // replacement for preprocessor

        clientLobby = FindObjectOfType<ClientLobby>();
        popupMessageManager = FindObjectOfType<PopupMessageManager>();
    }

    // Update is called once per frame
    void Update()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor
		
		// Check objects -----------------------------
		if (!clientLobby)
			clientLobby = FindObjectOfType<ClientLobby>();

		if (!popupMessageManager)
			popupMessageManager = FindObjectOfType<PopupMessageManager>();

		if (!clientLobby || !popupMessageManager) return;
		// -------------------------------------------

		if (allowEnter && (ipInputField.text.Length > 0) && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
        {
            ConnectButton();
            allowEnter = false;
        }
        else
            allowEnter = ipInputField.isFocused;
    }

    public void ConnectButton()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		if (!clientLobby || !popupMessageManager) return;

		IPAddress ip = default(IPAddress);
        try
        {
            ip = IPAddress.Parse(ipInputField.text);
            clientLobby.EstablishConnection(ip);
        }
        catch(FormatException e)
        {
            Debug.Log("IPAddress parsing : Exception caught!!!");
            Debug.Log("Source : " + e.Source);
            Debug.Log("Message : " + e.Message);
            popupMessageManager.Show("Couldn't parse IP address. It should look like 127.0.0.1.");
            return;
        }

        catch(SocketException e)
        {
            Debug.Log("SocketException : could not reach/connect to this IP");
            Debug.Log("Source : " + e.Source);
            Debug.Log("Message : " + e.Message);
            popupMessageManager.Show("Could not reach specified IP.");
            
            //return;
        }

    }

    public void Hide()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		interfacePanel.SetActive(false);
    }

    public void Show()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		interfacePanel.SetActive(true);
    }
#endif
}
