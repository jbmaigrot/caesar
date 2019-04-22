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

    public struct LobbyInterface
    {
        public int numberOfPlayerSlots;
        public List<PlayerLobbyCardManager.PlayerLobbyCard> playerLobbyCards;
    };

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetNumberOfPlayerSlots()
    {
        return int.Parse(numberOfPlayerSlots.text);
    }

    public void Hide()
    {
        interfacePanel.SetActive(false);
    }

    public void Show()
    {
        interfacePanel.SetActive(true);
    }

    public void UpdateInterface(LobbyInterface state, int connectionId)
    {
        numberOfPlayerSlots.text = state.numberOfPlayerSlots.ToString();
        for (int i = 0; i < state.numberOfPlayerSlots; i++)
        {

        }
    }
}
