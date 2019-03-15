using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

using System.Net;
using Unity.Networking.Transport;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

using Buffers = NetStack.Buffers;
using Serialization = NetStack.Serialization;

public class NetworkManager : MonoBehaviour
{
    public Transform[] players = new Transform[2];
    public UdpCNetworkDriver m_Driver;
    public NetworkConnection m_Connection;

    public IPv4UDPSocket socket;

    public bool done;

    private bool connected = false;
    private bool player_is_created = false;

    // Start is called before the first frame update
    void Start()
    {
        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);
        m_Connection = default(NetworkConnection);
        
        var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
        m_Connection = m_Driver.Connect(endpoint);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            if (!done)
            {
                Debug.Log("Something went wrong during connect");

                var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
                m_Connection = m_Driver.Connect(endpoint);
                Debug.Log("Connection reestablished");
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
                    connected = true;
                }

                else if (cmd == NetworkEvent.Type.Data)
                {
                    var readerCtx = default(DataStreamReader.Context);
                    var type = stream.ReadUInt(ref readerCtx);
                    var number = stream.ReadUInt(ref readerCtx);

                    if (type == 42)
                    {
                        float x = stream.ReadFloat(ref readerCtx);
                        float y = stream.ReadFloat(ref readerCtx);
                        float z = stream.ReadFloat(ref readerCtx);
                        players[number].position = new Vector3(x, y, z);
                    }

                    /*var readerCtx = default(DataStreamReader.Context);
                    uint value = stream.ReadUInt(ref readerCtx);
                    Debug.Log("Got the value  = " + value + " back from the server");
                    done = true;
                    m_Connection.Disconnect(m_Driver);
                    m_Connection = default(NetworkConnection);*/
                }

                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client got disconnected for some reason");
                    m_Connection = default(NetworkConnection);
                }
            }

            /*if (connected)
            {
                int value = 0;

                if (Input.GetKey(KeyCode.D))
                {
                    value = 11;
                }
                else if (Input.GetKey(KeyCode.Z))
                {
                    value = 12;
                }
                else if (Input.GetKey(KeyCode.Q))
                {
                    value = 13;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    value = 14;
                }

                using (var writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(value);
                    m_Connection.Send(m_Driver, writer);
                }
            }*/
        }
    }

    
    public void SetDestination(Vector3 destination)
    {
        using (var writer = new DataStreamWriter(32, Allocator.Temp))
        {
            writer.Write(1);
            writer.Write(destination.x);
            writer.Write(destination.y);
            writer.Write(destination.z);

            m_Connection.Send(m_Driver, writer);
        }
    }
}
