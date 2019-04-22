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
            connected = _connected;
            if (_playerName == "")
            {
                playerName = Constants.LobbyNames[Random.Range(0, Constants.LobbyNames.Length)];
            } else
            {
                playerName = _playerName;
            }
            team = _team;
            isReady = _isReady;
        }
    };

    public void Start()
    {
        clientLobby = FindObjectOfType<ClientLobby>();

        UpdateCard(new PlayerLobbyCard(""));
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

        playerName.text = playerLobbyCard.playerName;
        playerTeam.value = playerLobbyCard.team;
        
        if (playerLobbyCard.isReady == true)
        {
            ready.interactable = false;
            cancel.interactable = true;

            playerName.interactable = false;
            playerTeam.interactable = false;
        }
    }

    public void SetInteractable(bool interactable)
    {
        playerName.interactable = interactable;
        playerTeam.interactable = interactable;
        ready.interactable = interactable;

        if (interactable)
        {
            GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        } else
        {
            GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        }

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
        playerTeam.interactable = false;
    }

    public void OnClickCancel()
    {
        clientLobby.WriteCancel();
        ready.interactable = true;
        cancel.interactable = false;

        playerName.interactable = true;
        playerTeam.interactable = true;
    }
}
