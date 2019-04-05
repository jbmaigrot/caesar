﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

using Unity.Networking.Transport;
using Unity.Collections;

using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class Server : MonoBehaviour
{
    public UdpCNetworkDriver m_Driver;
    public List<Transform> players;
    public List<Transform> characters; // Players + NPCs

    private List<Transform> newCharacters;
    private NativeList<NetworkConnection> m_Connections;

    private ProgrammableObjectsContainer programmableObjectsContainer;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 58;

        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);

        if (m_Driver.Bind(new IPEndPoint(IPAddress.Any, 9000)) != 0)
            Debug.Log("Failed to bind to port 9000");

        else
            m_Driver.Listen();

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        programmableObjectsContainer = GameObject.FindObjectOfType<ProgrammableObjectsContainer>();
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
        m_Connections.Dispose();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(Mathf.Round(1f / Time.deltaTime)); //Framerate

        m_Driver.ScheduleUpdate().Complete();

        // Clean up connections
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
            //Debug.Log("Accepted a connection");
        }

        DataStreamReader stream;
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
                        case Constants.Client_SetDestination:
                            float dest_x = stream.ReadFloat(ref readerCtx);
                            float dest_z = stream.ReadFloat(ref readerCtx);
                            players[i].gameObject.GetComponent<NavMeshAgent>().SetDestination(new Vector3(dest_x, 0, dest_z));
                            break;

                        case Constants.Client_Tacle:
                            int number = (int) stream.ReadUInt(ref readerCtx);
                            if (players[i].GetComponent<ServerCharacter>().canStun && !players[i].GetComponent<ServerCharacter>().isStunned && number != i /*&& Vector3.Distance(players[i].transform.position, characters[name].transform.position) < 30 */)
                            {
                                players[i].GetComponent<ServerCharacter>().doStun();
                                characters[number].GetComponent<ServerCharacter>().getStun();
                            }                                
                            break;

                        case Constants.Client_Message:
                            //TO DO
                            break;

                        case Constants.Client_GetHack:
                            int objectId = (int)stream.ReadUInt(ref readerCtx);
                            SendHackStatus(objectId, i);
                            //TO DO
                            break;

                        case Constants.Client_SetHack:
                            //TO DO
                            break;

                        default:
                            break;
                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    //Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }

            // Send snapshot (world state)
            using (var writer = new DataStreamWriter(1024, Allocator.Temp))
            {
                //snapshot start
                writer.Write(Constants.Server_Snapshot);
                writer.Write(0); // (Temp 0) character to follow 

                var n = characters.Count;

                //update characters positions
                for (int j = 0; j < n; j++)
                {
                    writer.Write(Constants.Server_MoveCharacter);
                    writer.Write(j);
                    writer.Write(characters[j].position.x);
                    writer.Write(characters[j].position.z);
                    writer.Write(characters[j].rotation.eulerAngles.y);
                    writer.Write(characters[j].GetComponent<NavMeshAgent>().velocity.x);
                    writer.Write(characters[j].GetComponent<NavMeshAgent>().velocity.z);
                    writer.Write(characters[j].gameObject.GetComponent<ServerCharacter>().isStunned?1:0);
                }

                //close snapshot
                writer.Write(Constants.Server_SnapshotEnd);


                //send snapshot to all clients
                for (int k = 0; k < m_Connections.Length; k++)
                {
                    m_Driver.Send(m_Connections[k], writer);
                }
            }
        }
    }

    public void SendHackStatus(int objectId, int connectionId)
    {
        ProgrammableObjectsData programmableObject = programmableObjectsContainer.transform.GetChild(objectId).GetComponentInChildren<ProgrammableObjectsData>();

        using (var writer = new DataStreamWriter(4096, Allocator.Temp))
        {
            writer.Write(Constants.Server_GetHack);
            foreach(InOutVignette vignette in programmableObject.inputCodes)
            {
                for (int i = 0; i < vignette.code.Length; i++)
                {
                    writer.Write(vignette.code.ToCharArray()[i]);
                }
                writer.Write('\0');
                writer.Write(vignette.parameter_int);
                for (int i = 0; i < vignette.parameter_string.Length; i++)
                {
                    writer.Write(vignette.parameter_string.ToCharArray()[i]);
                }
                writer.Write('\0');
                writer.Write(vignette.is_fixed ? 1 : 0);
            }

            writer.Write('\0');

            foreach(InOutVignette vignette in programmableObject.outputCodes)
            {
                for(int i = 0; i < vignette.code.Length; i++)
                {
                    writer.Write(vignette.code.ToCharArray()[i]);
                }
                writer.Write('\0');
                writer.Write(vignette.parameter_int);
                for (int i = 0; i < vignette.parameter_string.Length; i++)
                {
                    writer.Write(vignette.parameter_string.ToCharArray()[i]);
                }
                writer.Write('\0');
                writer.Write(vignette.is_fixed ? 1 : 0);
            }

            writer.Write('\0');

            foreach(Arrow arrow in programmableObject.graph)
            {
                writer.Write(arrow.input);
                writer.Write(arrow.output);
                writer.Write(arrow.transmitTime);
                foreach (float time in arrow.timeBeforeTransmit)
                {
                    writer.Write(time);
                    writer.Write('\0');
                }
            }

            writer.Write(Constants.Server_SnapshotEnd);
            m_Driver.Send(m_Connections[connectionId], writer);
        }

    }
}
