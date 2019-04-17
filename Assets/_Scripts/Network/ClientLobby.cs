using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using Unity.Networking.Transport;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using System;
using Unity.Collections;

#if CLIENT
public class ClientLobby : MonoBehaviour
{
    public string ServerIP = "127.0.0.1"; //localhost by default
    public UdpCNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public IPv4UDPSocket socket;

    public int connectionId;
    private bool initialHandshakeDone;
    // Start is called before the first frame update
    void Start()
    {
        connectionId = -1;
        initialHandshakeDone = false;
        //TODO error : A Native Collection has not been disposed, resulting in a memory leak
        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);
        m_Connection = default(NetworkConnection);

        //var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
        var endpoint = new IPEndPoint(IPAddress.Parse(ServerIP), 9000);
        m_Connection = m_Driver.Connect(endpoint);
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
    void FixedUpdate()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            Debug.Log("Something went wrong during connect");
            //var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
            var endpoint = new IPEndPoint(IPAddress.Parse(ServerIP), 9000);
            m_Connection = m_Driver.Connect(endpoint);
            Debug.Log("Connection reestablished");
            initialHandshakeDone = false;
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

                    switch(type)
                    {
                        case Constants.Server_Lobby_SetConnectionId:
                            connectionId = (int)stream.ReadUInt(ref readerCtx);
                            initialHandshakeDone = true;
                            break;
                        case Constants.Server_Lobby_LobbyState:
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

    private void InitialHandshake()
    {
        using (var writer = new DataStreamWriter(64, Allocator.Temp))
        {
            writer.Write(Constants.Client_Lobby_ConnectionId);
            writer.Write(connectionId);
            m_Connection.Send(m_Driver, writer);
        }
    }
}
#endif