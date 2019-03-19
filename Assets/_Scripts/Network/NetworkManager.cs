﻿using System.Collections;
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
    public Client_Character[] characters = new Client_Character[20];
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
    void FixedUpdate()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            if (!done)
            {
                //Debug.Log("Something went wrong during connect");
                var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
                m_Connection = m_Driver.Connect(endpoint);
                //Debug.Log("Connection reestablished");
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
                    //Debug.Log("We are now connected to the server");
                    connected = true;
                }

                else if (cmd == NetworkEvent.Type.Data)
                {
                    var readerCtx = default(DataStreamReader.Context);
                    var type = stream.ReadUInt(ref readerCtx);

                    if (type == Constants.Server_Snapshot)
                    {
                        type = stream.ReadUInt(ref readerCtx);

                        while (type != Constants.Server_SnapshotEnd)
                        {
                            uint j = stream.ReadUInt(ref readerCtx);
                            float x = stream.ReadFloat(ref readerCtx);
                            float z = stream.ReadFloat(ref readerCtx);
                            characters[j].transform.position = new Vector3(x, characters[j].transform.position.y, z);
                            characters[j].speed.x = stream.ReadFloat(ref readerCtx);
                            characters[j].speed.z = stream.ReadFloat(ref readerCtx);

                            type = stream.ReadUInt(ref readerCtx);
                        }
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

    
    public void SetDestination(Vector3 destination)
    {
        using (var writer = new DataStreamWriter(32, Allocator.Temp))
        {
            writer.Write(Constants.Client_SetDestination);
            writer.Write(destination.x);
            writer.Write(destination.y);
            writer.Write(destination.z);

            m_Connection.Send(m_Driver, writer);
        }
    }
    

    public void OnApplicationQuit()
    {
        m_Driver.Dispose();
    }
}
