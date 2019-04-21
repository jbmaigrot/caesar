using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInterfaceManager : MonoBehaviour
{

    public List<PlayerLobbyCardManager> playerLobbyCardManagers;

    public InputField numberOfPlayerSlots;
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
}
