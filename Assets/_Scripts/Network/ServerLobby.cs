using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Collections;

using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

#if SERVER
public class ServerLobby : MonoBehaviour
{
    private const bool UP = true;
    private const bool DOWN = false;
    public UdpCNetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;
    private NativeList<NetworkConnection> tmp_Connections;
    private bool[] lostConnections;

    private int numberOfPlayerSlots;

    private LobbyInterfaceManager lobbyInterfaceManager;
    // Start is called before the first frame update
    void Start()
    {
        lobbyInterfaceManager = FindObjectOfType<LobbyInterfaceManager>();
        numberOfPlayerSlots = lobbyInterfaceManager.GetNumberOfPlayerSlots();
        lostConnections = new bool[numberOfPlayerSlots];
        for (int i = 0; i < numberOfPlayerSlots; i++)
        {
            lostConnections[i] = false;
        }

        Application.targetFrameRate = 58;

        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);

        if (m_Driver.Bind(new IPEndPoint(IPAddress.Any, 9000)) != 0)
            Debug.Log("Failed to bind to port 9000");

        else
            m_Driver.Listen();

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        tmp_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
        m_Connections.Dispose();
        tmp_Connections.Dispose();
    }

    // Update is called once per frame
    void FixedUpdate()
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
                            } else
                            {
                                Debug.Log("Getting a new connection with connection ID : " + m_Connections.Length);
                                SetConnectionId(m_Connections.Length, tmp_Connections[i]);
                                m_Connections.Add(tmp_Connections[i]);
                                tmp_Connections.RemoveAtSwapBack(i);
                            }
                        } else
                        {
                            if (lostConnections[connectionId] == false)
                            {
                                Debug.Log("Error. Trying to connect with an already attributed connection ID : " + connectionId);
                            } else
                            {
                                Debug.Log("Re-establishing connection with connection ID : " + connectionId);
                                m_Connections[connectionId] = tmp_Connections[i];
                                lostConnections[connectionId] = false;
                                tmp_Connections.RemoveAtSwapBack(i);
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

                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    //Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }
        }
    }

    public void SetNumberOfPlayerSlots(int _numberOfPlayerSlots)
    {
        numberOfPlayerSlots = _numberOfPlayerSlots;
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
}
#endif