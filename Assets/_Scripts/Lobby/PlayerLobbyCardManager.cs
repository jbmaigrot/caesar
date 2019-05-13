using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbyCardManager : MonoBehaviour
{
    public Text playerNumber;
    public Text connected;
    public Text notConnected;
    public InputField playerName;
    public Button randomize;
    public Dropdown playerTeam;
    public Button ready;
    public Button cancel;

    private ClientLobby clientLobby;

    public class PlayerLobbyCard
    {
        public int playerNumber;
        public bool connected;
        public string playerName { get; set; }
        public int team; //0 is the first option in drop down list (i.e. "Team A"), 1 is "Team B"
        public bool isReady;

        public PlayerLobbyCard(string _playerName = "", int _playerNumber = -1, bool _connected = false, int _team = 0, bool _isReady = false)
        {
            playerNumber = _playerNumber;
            connected = _connected;/*
            if (_playerName == "")
            {
                playerName = Constants.LobbyNames[Random.Range(0, Constants.LobbyNames.Length)];
            } else*/
                playerName = _playerName;
            team = _team;
            isReady = _isReady;
        }

        public void RandomizeName()
        {
            playerName = Constants.LobbyNames[Random.Range(0, Constants.LobbyNames.Length)];
        }
    };
#if CLIENT

    public void Start()
    {
        clientLobby = FindObjectOfType<ClientLobby>();
        
    }

    public void UpdateCard(PlayerLobbyCard playerLobbyCard)
    {
        playerNumber.text = "Player " + playerLobbyCard.playerNumber;

        if (playerLobbyCard.connected == true)
        {
            connected.gameObject.SetActive(true);
            notConnected.gameObject.SetActive(false);
        } else
        {
            connected.gameObject.SetActive(false);
            notConnected.gameObject.SetActive(true);
        }

        InputField.OnChangeEvent tmpInputEvt = playerName.onValueChanged;
        playerName.onValueChanged = new InputField.OnChangeEvent();
        playerName.text = playerLobbyCard.playerName;
        playerName.onValueChanged = tmpInputEvt;

        //https://forum.unity.com/threads/change-the-value-of-a-toggle-without-triggering-onvaluechanged.275056/
        //Before adding these lines, an edit to one of the team would change the value for all players
        //due to the fact the OnValueChange function is triggering not only on user input, but also when manually editing the value through code.
        Dropdown.DropdownEvent tmpDropdownEvt = playerTeam.onValueChanged;
        playerTeam.onValueChanged = new Dropdown.DropdownEvent();
        playerTeam.value = playerLobbyCard.team;
        playerTeam.onValueChanged = tmpDropdownEvt;

        if (playerLobbyCard.isReady == true)
        {
            ready.GetComponentInChildren<Text>().text = "Waiting...";
        } else
        {
            ready.GetComponentInChildren<Text>().text = "Ready";
        }

        GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
    }

    public void nonInteractable()
    {
        playerName.interactable = false;
        randomize.interactable = false;
        playerTeam.interactable = false;
        ready.interactable = false;
        cancel.interactable = false;
        
        GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
    }
    
    public void OnEditPlayerName()
    {
        clientLobby.WritePlayerName(playerName.text);
    }

    public void OnEditTeam()
    {
        clientLobby.WriteTeam(playerTeam.value);
    }

    public void OnClickReady()
    {
        clientLobby.WriteReady();
        ready.interactable = false;
        cancel.interactable = true;

        playerName.interactable = false;
        randomize.interactable = false;
        playerTeam.interactable = false;
    }

    public void OnClickCancel()
    {
        clientLobby.WriteCancel();
        ready.interactable = true;
        cancel.interactable = false;

        playerName.interactable = true;
        randomize.interactable = true;
        playerTeam.interactable = true;
    }

    public void OnClickRandomize()
    {
        playerName.text = Constants.LobbyNames[Random.Range(0, Constants.LobbyNames.Length)];
    }

#endif
}
