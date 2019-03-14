using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Collections;

using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class Server : MonoBehaviour
{
    public Transform[] players = new Transform[2];
    public UdpCNetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;

    // Start is called before the first frame update
    void Start()
    {
        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);
        if (m_Driver.Bind(new IPEndPoint(IPAddress.Any, 9000)) != 0)
            Debug.Log("Failed to bind to port 9000");
        else
            m_Driver.Listen();

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
        m_Connections.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        //Clean up connections
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        //Accept new connections
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            m_Connections.Add(c);
            Debug.Log("Accepted a connection");
        }

        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
                continue;
            NetworkEvent.Type cmd;
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    var readerCtx = default(DataStreamReader.Context);
                    uint action = stream.ReadUInt(ref readerCtx);

                    using (var writer = new DataStreamWriter(32, Allocator.Temp))
                    {
                        writer.Write(42);
                        switch (action)
                        {
                            case 1:
                                players[i].position = new Vector3(players[i].position.x + Time.deltaTime, players[i].position.y, players[i].position.z);
                                break;

                            case 2:
                                players[i].position = new Vector3(players[i].position.x, players[i].position.y, players[i].position.z + Time.deltaTime);
                                break;

                            case 3:
                                players[i].position = new Vector3(players[i].position.x - Time.deltaTime, players[i].position.y, players[i].position.z);
                                break;

                            case 4:
                                players[i].position = new Vector3(players[i].position.x, players[i].position.y, players[i].position.z - Time.deltaTime);
                                break;

                            default:
                                break;
                        }
                        writer.Write(players[i].position.x);
                        writer.Write(players[i].position.y);
                        writer.Write(players[i].position.z);
                        m_Driver.Send(m_Connections[i], writer);
                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }
        }
    }
}
