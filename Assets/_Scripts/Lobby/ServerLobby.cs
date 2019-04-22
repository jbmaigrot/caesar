using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Collections;

using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using UnityEngine.SceneManagement;


public class ServerLobby : MonoBehaviour
{
    public UdpCNetworkDriver m_Driver;
    public NativeList<NetworkConnection> m_Connections;
    private NativeList<NetworkConnection> tmp_Connections;
    public List<bool> lostConnections;

    public int numberOfPlayerSlots = 4;

    private LobbyInterfaceManager lobbyInterfaceManager;

    public LobbyInterfaceManager.LobbyInterface lobbyInterfaceState;
    
    

#if SERVER
    // Start is called before the first frame update
    void Start()
    {
        //lobbyInterfaceManager = FindObjectOfType<LobbyInterfaceManager>();
        //numberOfPlayerSlots = lobbyInterfaceManager.GetNumberOfPlayerSlots();
        //No real need for the interface on server side, and number of player slots should be modified by clients

        Application.targetFrameRate = 58;

        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);

        if (m_Driver.Bind(new IPEndPoint(IPAddress.Any, 9000)) != 0)
            Debug.Log("Failed to bind to port 9000");

        else
            m_Driver.Listen();

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        tmp_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        lobbyInterfaceState = new LobbyInterfaceManager.LobbyInterface();
        lobbyInterfaceState.numberOfPlayerSlots = numberOfPlayerSlots;
        lobbyInterfaceState.playerLobbyCards = new List<PlayerLobbyCardManager.PlayerLobbyCard>();
        for (int i = 0; i < numberOfPlayerSlots; i++)
        {
            var lobbyCard = new PlayerLobbyCardManager.PlayerLobbyCard("", i + 1);
            lobbyInterfaceState.playerLobbyCards.Add(lobbyCard);
        }
    }

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
        m_Connections.Dispose();
        tmp_Connections.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        // Clean up connections
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                //m_Connections.RemoveAtSwapBack(i);
                //--i;
                lostConnections[i] = true;
            }
        }
        for (int i = 0; i < tmp_Connections.Length; i++)
        {
            if (!tmp_Connections[i].IsCreated)
            {
                tmp_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        //Accept new connections into tmp_Connections
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            tmp_Connections.Add(c);
            Debug.Log("Accepted a connection");
        }

        DataStreamReader stream;


        //Getting through tmp_Connections to reconnect lost connections and moving new connections to m_Connections
        for (int i = 0; i < tmp_Connections.Length; i++)
        {
            if (!tmp_Connections[i].IsCreated) continue;

            NetworkEvent.Type cmd;

            cmd = m_Driver.PopEventForConnection(tmp_Connections[i], out stream);
            if (cmd == NetworkEvent.Type.Empty) continue;

            if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtx = default(DataStreamReader.Context);
                uint action = stream.ReadUInt(ref readerCtx);

                switch (action)
                {
                    case Constants.Client_Lobby_ConnectionId:
                        int connectionId = (int)stream.ReadUInt(ref readerCtx);
                        if (connectionId == -1)
                        {
                            if (m_Connections.Length >= numberOfPlayerSlots)
                            {
                                Debug.Log("Sorry, all player slots are allocated in this game.");
                            }
                            else
                            {
                                Debug.Log("Getting a new connection with connection ID : " + m_Connections.Length);
                                SetConnectionId(m_Connections.Length, tmp_Connections[i]);
                                m_Connections.Add(tmp_Connections[i]);
                                lostConnections.Add(false);
                                tmp_Connections.RemoveAtSwapBack(i);

                                lobbyInterfaceState.playerLobbyCards[m_Connections.Length - 1].connected = true;
                                SendLobbyInterfaceState();
                            }
                        }
                        else
                        {
                            if (lostConnections[connectionId] == false)
                            {
                                Debug.Log("Error. Trying to connect with an already attributed connection ID : " + connectionId);
                            }
                            else
                            {
                                Debug.Log("Re-establishing connection with connection ID : " + connectionId);
                                m_Connections[connectionId] = tmp_Connections[i];
                                lostConnections[connectionId] = false;
                                tmp_Connections.RemoveAtSwapBack(i);

                                lobbyInterfaceState.playerLobbyCards[connectionId].connected = true;
                                SendLobbyInterfaceState();
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                //Debug.Log("Client disconnected from server");
                tmp_Connections[i] = default(NetworkConnection);
            }
        }


        //Getting through m_Connections to deal with the real shit
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated) continue;

            NetworkEvent.Type cmd;
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    var readerCtx = default(DataStreamReader.Context);
                    uint action = stream.ReadUInt(ref readerCtx);

                    switch (action)
                    {
                        case Constants.Client_Lobby_PlayerName:
                            char[] buffer;
                            int playerNameLength = (int)stream.ReadUInt(ref readerCtx);
                            buffer = new char[playerNameLength];
                            for (int j = 0; j < playerNameLength; j++)
                            {
                                buffer[j] = (char)stream.ReadByte(ref readerCtx);
                            }
                            string playerName = new string(buffer);

                            lobbyInterfaceState.playerLobbyCards[i].playerName = playerName;

                            SendLobbyInterfaceState();
                            break;

                        case Constants.Client_Lobby_SetTeam:
                            int team = (int)stream.ReadUInt(ref readerCtx);

                            lobbyInterfaceState.playerLobbyCards[i].team = team;

                            SendLobbyInterfaceState();
                            break;

                        case Constants.Client_Lobby_Ready:

                            lobbyInterfaceState.playerLobbyCards[i].isReady = true;

                            //Send state
                            SendLobbyInterfaceState();

                            //Check if everybody is ready
                            var everyBodyReady = true;
                            for (int j = 0; j < lobbyInterfaceState.playerLobbyCards.Count; j++) 
                            {
                                var lobbyCard = lobbyInterfaceState.playerLobbyCards[i];
                                if (lobbyCard.isReady == false && lobbyCard.connected == true)
                                {
                                    everyBodyReady = false;
                                    break;
                                }
                            }
                            if (everyBodyReady == true)
                            {
                                //Launch game
                                SendStartGame();
                                SceneManager.LoadScene(1);
                            }
                            break;

                        case Constants.Client_Lobby_Cancel:

                            lobbyInterfaceState.playerLobbyCards[i].isReady = false;

                            SendLobbyInterfaceState();
                            break;
                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    //Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                    lobbyInterfaceState.playerLobbyCards[i].connected = false;
                    SendLobbyInterfaceState();
                }
            }
        }
    }

    public void SetNumberOfPlayerSlots(int _numberOfPlayerSlots)
    {
        numberOfPlayerSlots = _numberOfPlayerSlots;
    }

    public int GetNumberOfPlayerSlots()
    {
        return numberOfPlayerSlots;
    }

    public void SetConnectionId(int connectionId, NetworkConnection nc)
    {
        using (var writer = new DataStreamWriter(64, Allocator.Temp))
        {
            writer.Write(Constants.Server_Lobby_SetConnectionId);
            writer.Write(connectionId);
            nc.Send(m_Driver, writer);
        }
    }

    public void SendLobbyInterfaceState()
    {

        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated) continue;
            using (var writer = new DataStreamWriter(2048, Allocator.Temp))
            {
                writer.Write(Constants.Server_Lobby_LobbyState);
                writer.Write(lobbyInterfaceState.numberOfPlayerSlots);
                for (int j = 0; j < lobbyInterfaceState.numberOfPlayerSlots; j++)
                {
                    PlayerLobbyCardManager.PlayerLobbyCard tmpCard = lobbyInterfaceState.playerLobbyCards[j];

                    writer.Write(tmpCard.playerNumber);
                    writer.Write(tmpCard.connected ? 1 : 0);

                    byte[] buffer = new byte[tmpCard.playerName.Length];
                    for (int k = 0; k < tmpCard.playerName.Length; k++)
                    {
                        buffer[k] = (byte)tmpCard.playerName.ToCharArray()[k];
                    }
                    writer.Write(tmpCard.playerName.Length);
                    writer.Write(buffer);

                    writer.Write(tmpCard.team);
                    writer.Write(tmpCard.isReady ? 1 : 0);
                }
                m_Driver.Send(m_Connections[i], writer);
            }
        }
    }

    public void SendStartGame()
    {
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated) continue;
            using (var writer = new DataStreamWriter(2048, Allocator.Temp))
            {
                writer.Write(Constants.Server_Lobby_StartGame);
                m_Driver.Send(m_Connections[i], writer);
            }
        }
    }
#endif
}