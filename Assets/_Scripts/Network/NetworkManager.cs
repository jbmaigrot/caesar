using System;
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
    public string ServerIP = "localhost";
    public UdpCNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public IPv4UDPSocket socket;

    public List<Client_Character> characters;
    public GameObject characterPrefab;

    public bool done;

    private bool connected = false;
    private bool player_is_created = false;

    // Start is called before the first frame update
    void Start()
    {
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
            if (!done)
            {
                //Debug.Log("Something went wrong during connect");
                //var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
                var endpoint = new IPEndPoint(IPAddress.Parse(ServerIP), 9000);
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

                    while (type != Constants.Server_SnapshotEnd)
                    {
                        switch (type)
                        {
                            case Constants.Server_Snapshot:
                                break;

                            case Constants.Server_MoveCharacter:
                                int j = (int)stream.ReadUInt(ref readerCtx);
                                float x = stream.ReadFloat(ref readerCtx);
                                float z = stream.ReadFloat(ref readerCtx);
                                if (j >= characters.Count)
                                {
                                    GameObject newCharacter = Instantiate(characterPrefab);
                                    characters.Add(newCharacter.GetComponent<Client_Character>());
                                    //characters[j] = newCharacter.GetComponent<Client_Character>();
                                }
                                if (characters[j] != null)
                                {
                                    characters[j].transform.position = new Vector3(x, characters[j].transform.position.y, z);
                                    characters[j].speed.x = stream.ReadFloat(ref readerCtx);
                                    characters[j].speed.z = stream.ReadFloat(ref readerCtx);
                                }
                                
                                break;

                            /*case Constants.Server_CreateCharacter:
                                int j1 = (int)stream.ReadUInt(ref readerCtx);
                                float x1 = stream.ReadFloat(ref readerCtx);
                                float z1 = stream.ReadFloat(ref readerCtx);
                                GameObject newCharacter = Instantiate(characterPrefab);
                                characters[j1] = newCharacter.GetComponent<Client_Character>();
                                characters[j1].transform.position = new Vector3(x1, characters[j1].transform.position.y, z1);
                                characters[j1].speed.x = stream.ReadFloat(ref readerCtx);
                                characters[j1].speed.z = stream.ReadFloat(ref readerCtx);
                                break;*/

                            default:
                                break;
                        }

                        type = stream.ReadUInt(ref readerCtx);
                    }
                }

                /*else if (cmd == NetworkEvent.Type.Data)
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
                }*/
                //TODO error : deconnecte sans raison apparente
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
            writer.Write(destination.z);

            m_Connection.Send(m_Driver, writer);
        }
    }
    

    public void OnApplicationQuit()
    {
        m_Driver.Dispose();
    }
}
