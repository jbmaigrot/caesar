using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using Unity.Networking.Transport;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using System;
using Unity.Collections;
using System.Net.Sockets;

#if CLIENT
public class ClientLobby : MonoBehaviour
{
    public string ServerIP = "127.0.0.1"; //localhost by default
    public IPAddress iPAddress;
    public UdpCNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public IPv4UDPSocket socket;

    public int connectionId;
    private bool initialHandshakeDone;

    public bool establishingConnection;

    private LobbyInterfaceManager lobbyInterfaceManager;
    private ConnectingMessageManager connectingMessageManager;
    private PopupMessageManager popupMessageManager;
    private IPConnectionInterfaceManager iPConnectionInterfaceManager;

    // Start is called before the first frame update
    void Start()
    {
        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);

        lobbyInterfaceManager = FindObjectOfType<LobbyInterfaceManager>();
        connectingMessageManager = FindObjectOfType<ConnectingMessageManager>();
        popupMessageManager = FindObjectOfType<PopupMessageManager>();
        iPConnectionInterfaceManager = FindObjectOfType<IPConnectionInterfaceManager>();

        establishingConnection = false;
    }

    public void OnDestroy()
    {
        try
        {
            m_Driver.Dispose();
        }
        catch (InvalidOperationException e)
        {
            Debug.Log(e.Message);
        }

    }

    // Update is called once per frame
    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();
        
        if (establishingConnection == true)
        {
            
            if (!m_Connection.IsCreated)
            {
                Debug.Log("Something went wrong during connect");

                if (initialHandshakeDone == true)
                {
                    var endpoint = new IPEndPoint(iPAddress, 9000);
                    connectingMessageManager.Show();
                    m_Connection = m_Driver.Connect(endpoint);
                    Debug.Log("Trying to reestablished connection");
                    initialHandshakeDone = false;
                } else
                {
                    CancelConnection();
                    popupMessageManager.Show("Could not establish a connection.");
                }
                return;
            }
            else
            {
                DataStreamReader stream;
                NetworkEvent.Type cmd;

                while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
                {
                    if (cmd == NetworkEvent.Type.Connect)
                    {
                        Debug.Log("We are now connected to the server");
                        if (initialHandshakeDone == false)
                        {
                            InitialHandshake();
                        }
                    }

                    else if (cmd == NetworkEvent.Type.Data)
                    {
                        var readerCtx = default(DataStreamReader.Context);
                        var type = stream.ReadUInt(ref readerCtx);

                        switch (type)
                        {
                            case Constants.Server_Lobby_SetConnectionId:
                                connectionId = (int)stream.ReadUInt(ref readerCtx);
                                initialHandshakeDone = true;
                                connectingMessageManager.Hide();
                                iPConnectionInterfaceManager.Hide();
                                break;
                            case Constants.Server_Lobby_LobbyState:
                                lobbyInterfaceManager.Show();
                                lobbyInterfaceManager.UpdateInterface(ReadLobbyState(stream, ref readerCtx), connectionId);
                                break;
                            case Constants.Server_Lobby_StartGame:
                                break;
                            default:
                                break;
                        }
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        Debug.Log("Client got disconnected for some reason");
                        m_Connection = default(NetworkConnection);
                    }
                }
            }
        }
        
    }

    private void InitialHandshake()
    {
        using (var writer = new DataStreamWriter(64, Allocator.Temp))
        {
            writer.Write(Constants.Client_Lobby_ConnectionId);
            writer.Write(connectionId);
            m_Connection.Send(m_Driver, writer);
        }
    }

    public void EstablishConnection(IPAddress ip)
    {
        connectionId = -1;
        m_Connection = default(NetworkConnection);

        var endpoint = new IPEndPoint(ip, 9000);
        iPAddress = ip;
        connectingMessageManager.Show();
        try
        {
            m_Connection = m_Driver.Connect(endpoint);
        }
        catch(SocketException e)
        {
            CancelConnection();
            popupMessageManager.Show("Could not establish a connection.");
            throw;
        }
        
        initialHandshakeDone = false;
        establishingConnection = true;
        //TODO error : A Native Collection has not been disposed, resulting in a memory leak
        
    }

    public void CancelConnection()
    {
        m_Driver.Dispose();
        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);
        //m_Driver.Disconnect(m_Connection);
        m_Connection = default(NetworkConnection);
        establishingConnection = false;
        connectingMessageManager.Hide();
    }

    public LobbyInterfaceManager.LobbyInterface ReadLobbyState(DataStreamReader stream, ref DataStreamReader.Context readerCtx)
    {
        LobbyInterfaceManager.LobbyInterface tmpState = new LobbyInterfaceManager.LobbyInterface();
        tmpState.playerLobbyCards = new List<PlayerLobbyCardManager.PlayerLobbyCard>();

        int numberOfPlayerSlots = (int)stream.ReadUInt(ref readerCtx);
        tmpState.numberOfPlayerSlots = numberOfPlayerSlots;

        char[] buffer;
        for (int i = 0; i < numberOfPlayerSlots; i++)
        {
            int playerNumber = (int)stream.ReadUInt(ref readerCtx);
            bool connected = stream.ReadUInt(ref readerCtx) == 1;

            int playerNameLength = (int)stream.ReadUInt(ref readerCtx);
            buffer = new char[playerNameLength];
            for (int j = 0; j < playerNameLength; j++)
            {
                buffer[j] = (char)stream.ReadByte(ref readerCtx);
            }
            string playerName = new string(buffer);

            int team = (int)stream.ReadUInt(ref readerCtx);
            bool isReady = stream.ReadUInt(ref readerCtx) == 1;

            PlayerLobbyCardManager.PlayerLobbyCard tmpCard = new PlayerLobbyCardManager.PlayerLobbyCard(playerName, playerNumber, connected, team, isReady);
            tmpState.playerLobbyCards.Add(tmpCard);
        }

        return tmpState;
    }


    public void WritePlayerName(string playerName)
    {
        using (var writer = new DataStreamWriter(64, Allocator.Temp))
        {
            writer.Write(Constants.Client_Lobby_PlayerName);

            byte[] buffer;
            buffer = new byte[playerName.Length];
            for (int i = 0; i < playerName.Length; i++)
            {
                buffer[i] = (byte)playerName.ToCharArray()[i];
            }
            writer.Write(playerName.Length);
            writer.Write(buffer);
            m_Connection.Send(m_Driver, writer);
        }
    }

    public void WriteTeam(int team)
    {
        using (var writer = new DataStreamWriter(64, Allocator.Temp))
        {
            writer.Write(Constants.Client_Lobby_SetTeam);
            writer.Write(team);
            m_Connection.Send(m_Driver, writer);
        }
    }

    public void WriteReady()
    {
        using (var writer = new DataStreamWriter(64, Allocator.Temp))
        {
            writer.Write(Constants.Client_Lobby_Ready);
            m_Connection.Send(m_Driver, writer);
        }
    }

    public void WriteCancel()
    {
        using (var writer = new DataStreamWriter(64, Allocator.Temp))
        {
            writer.Write(Constants.Client_Lobby_Cancel);
            m_Connection.Send(m_Driver, writer);
        }
    }
}
#endif