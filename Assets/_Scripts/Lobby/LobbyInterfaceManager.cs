using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInterfaceManager : MonoBehaviour
{

    public List<PlayerLobbyCardManager> playerLobbyCardManagers;

    public InputField numberOfPlayerSlots;

    public GameObject interfacePanel;

    public GameObject playerLobbyCardContainer;
    public GameObject playerLobbyCardPrefab;

    public class LobbyInterface
    {
        public int numberOfPlayerSlots;
        public List<PlayerLobbyCardManager.PlayerLobbyCard> playerLobbyCards;
    };

#if CLIENT
    // Start is called before the first frame update
    void Start()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor


	}

	// Update is called once per frame
	void Update()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor


	}

	public int GetNumberOfPlayerSlots()
	{
		if (!GameState.CLIENT) return -1; // replacement for preprocessor

		return int.Parse(numberOfPlayerSlots.text);
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

        /*
        LobbyInterface lobbyInterfaceState = new LobbyInterfaceManager.LobbyInterface();
        lobbyInterfaceState.numberOfPlayerSlots = 4;
        lobbyInterfaceState.playerLobbyCards = new List<PlayerLobbyCardManager.PlayerLobbyCard>();
        for (int i = 0; i < 4; i++)
        {
            var lobbyCard = new PlayerLobbyCardManager.PlayerLobbyCard("caca", i + 1);
            lobbyInterfaceState.playerLobbyCards.Add(lobbyCard);
        }
        UpdateInterface(lobbyInterfaceState, 0);*/
    }

    public void UpdateInterface(LobbyInterface state, int connectionId)
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		numberOfPlayerSlots.text = state.numberOfPlayerSlots.ToString();

        int cardCount = playerLobbyCardManagers.Count;
        for (int i = cardCount; i > state.numberOfPlayerSlots; i--)
        {
            RemovePlayerCard(playerLobbyCardManagers[i-1]);
        }

        for (int i = 0; i < state.numberOfPlayerSlots; i++)
        {

            if (i >= playerLobbyCardManagers.Count)
            {
                playerLobbyCardManagers.Add(AddPlayerCard(state.playerLobbyCards[i]));
            }

            if (i == connectionId)
            {
                playerLobbyCardManagers[i].UpdateCard(state.playerLobbyCards[i]);
            }
            else
            {
                playerLobbyCardManagers[i].UpdateCard(state.playerLobbyCards[i]);
                playerLobbyCardManagers[i].nonInteractable();
            }
        }
    }

    public PlayerLobbyCardManager AddPlayerCard(PlayerLobbyCardManager.PlayerLobbyCard playerCard)
	{
		if (!GameState.CLIENT) return null; // replacement for preprocessor

		PlayerLobbyCardManager playerCardManager = Instantiate(playerLobbyCardPrefab, playerLobbyCardContainer.transform).GetComponent<PlayerLobbyCardManager>();
        playerCardManager.UpdateCard(playerCard);
        return playerCardManager;
    }

    public void RemovePlayerCard(PlayerLobbyCardManager playerLobbyCardManager)
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		playerLobbyCardManagers.Remove(playerLobbyCardManager);
        Destroy(playerLobbyCardManager.gameObject);        
    }
#endif
}
