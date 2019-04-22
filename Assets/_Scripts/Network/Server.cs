using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

using Unity.Networking.Transport;
using Unity.Collections;

using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;


public class Server : MonoBehaviour
{
#if SERVER
    public GameObject prefabPJ;
    public UdpCNetworkDriver m_Driver;
    public List<Transform> players = new List<Transform>();
    public List<Transform> characters = new List<Transform>(); // Players + NPCs


    private NativeList<NetworkConnection> m_Connections;
    private NativeList<NetworkConnection> tmp_Connections;
    private List<bool> lostConnections;
    private ServerLobby serverLobby;

    private List<string> messages = new List<string>();
    private List<Vector3> messagesPos = new List<Vector3>();
    private ProgrammableObjectsContainer programmableObjectsContainer;
    private int snapshotCount = 1;
    private int winner = -1; 

    private const float MANUALSTUNRADIUS = 15.0f;
    

    // Start is called before the first frame update
    void Start()
    {

        Application.targetFrameRate = 58;

        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);

        if (m_Driver.Bind(new IPEndPoint(IPAddress.Any, 9000)) != 0)
            Debug.Log("Failed to bind to port 9000");

        else
            m_Driver.Listen();

        serverLobby = FindObjectOfType<ServerLobby>();
        if (serverLobby == null)
        {
            Debug.Log("Didn't find any ServerLobby object in the scene");
            m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
            lostConnections = new List<bool>();
        } 
        else
        {
            m_Connections = serverLobby.m_Connections;
            lostConnections = serverLobby.lostConnections;
        }
        
        tmp_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        programmableObjectsContainer = GameObject.FindObjectOfType<ProgrammableObjectsContainer>();
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
        //Debug.Log(Mathf.Round(1f / Time.deltaTime)); //Framerate

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

        //Accept new connections
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            tmp_Connections.Add(c);
            
            Debug.Log("Accepted a temp connection");
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
                    case Constants.Client_ConnectionId:
                        int connectionId = (int)stream.ReadUInt(ref readerCtx);
                        if (connectionId == -1)
                        {
                            if (serverLobby!= null && m_Connections.Length >= serverLobby.GetNumberOfPlayerSlots())
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
                                AddNewPlayer();
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
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client disconnected from server");
                tmp_Connections[i] = default(NetworkConnection);
            }
        }

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
                            int number = (int)stream.ReadUInt(ref readerCtx);
                            if (players[i].GetComponent<ServerCharacter>().canStun && !players[i].GetComponent<ServerCharacter>().isStunned && /*number != i &&*/ Vector3.Distance(players[i].transform.position, characters[number].transform.position) < MANUALSTUNRADIUS)
                            {
                                characters[number].GetComponent<ServerCharacter>().getStun();
                                players[i].GetComponent<ServerCharacter>().doStun();
                            }
                            break;

                        case Constants.Client_StartTaking:
                            int objectId = (int)stream.ReadUInt(ref readerCtx);
                            players[i].GetComponent<ServerCarrier>().StartTaking(programmableObjectsContainer.objectListServer[objectId].GetComponent<ServerCarrier>());
                            break;

                        case Constants.Client_StartGiving:
                            int objectid = (int)stream.ReadUInt(ref readerCtx);
                            players[i].GetComponent<ServerCarrier>().StartGiving(programmableObjectsContainer.objectListServer[objectid].GetComponent<ServerCarrier>());
                            break;

                        case Constants.Client_Open_Door:
                            int numb = (int)stream.ReadUInt(ref readerCtx);
                            if (!players[i].GetComponent<ServerCharacter>().isStunned)
                            {
                                programmableObjectsContainer.objectListServer[numb].OnInput("OnInteract");
                            }
                            break;

                        case Constants.Client_Message:
                            int length = (int)stream.ReadUInt(ref readerCtx);
                            Debug.Log(length);
                            byte[] buffer = stream.ReadBytesAsArray(ref readerCtx, length);
                            char[] chars = new char[length];
                            for (int n = 0; n < length; n++)
                            {
                                chars[n] = (char)buffer[n];
                            }
                            string message = new string(chars);
                            AddMessage(message, players[i].transform.position);
                            break;

                        case Constants.Client_RequestHack:
                            int object_Id = (int)stream.ReadUInt(ref readerCtx);
                            Debug.Log("Server received a request for object with ID " + object_Id);
                            SendHackStatus(object_Id, i);
                            break;

                        case Constants.Client_SetHack:
                            objectId = (int)stream.ReadUInt(ref readerCtx);
                            SetHackStatus(objectId, stream, ref readerCtx);
                            break;

                        default:
                            break;
                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }
        }

        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated) continue;
            // Snapshot (world state)

            for (int j = 0; j < programmableObjectsContainer.objectListServer.Count; j++)
            {
                int charactersIndex = programmableObjectsContainer.objectListServer[j].charactersIndex;
                using (var writer = new DataStreamWriter(16384, Allocator.Temp))
                {
                    //snapshot start
                    writer.Write(Constants.Server_Snapshot);
                    writer.Write(snapshotCount);

                    if (charactersIndex != -1) //we have a character, update on its states and positions
                    {
                        writer.Write(Constants.Server_MoveCharacter);
                        writer.Write(charactersIndex);
                        writer.Write(characters[charactersIndex].position.x);
                        writer.Write(characters[charactersIndex].position.z);
                        writer.Write(characters[charactersIndex].rotation.eulerAngles.y);
                        writer.Write(characters[charactersIndex].GetComponent<NavMeshAgent>().velocity.x);
                        writer.Write(characters[charactersIndex].GetComponent<NavMeshAgent>().velocity.z);
                        writer.Write(characters[charactersIndex].gameObject.GetComponent<ServerCharacter>().isStunned ? 1 : 0);

                        if (characters[charactersIndex].GetComponent<ServerCarrier>())
                        {
                            var carrier = characters[charactersIndex].GetComponent<ServerCarrier>();
                            writer.Write(carrier.charge / carrier.maxCharge); //send charge ratio, rather than raw value (as clients do not know all max charges)
                        }
                        else
                        {
                            writer.Write(0.42f);
                        }
                    }

                    //update objects states
                    writer.Write(Constants.Server_UpdateObject);
                    writer.Write(j);
                    writer.Write(programmableObjectsContainer.objectListServer[j].isLightOn ? 1 : 0);
                    writer.Write(programmableObjectsContainer.objectListServer[j].isDoorOpen ? 1 : 0);

                    if (programmableObjectsContainer.objectListServer[j].GetComponent<ServerCarrier>())
                    {
                        var carrier = programmableObjectsContainer.objectListServer[j].GetComponent<ServerCarrier>();
                        writer.Write(carrier.charge / carrier.maxCharge); //send charge ratio, rather than raw value (as clients do not know all max charges)
                    }
                    else
                    {
                        writer.Write(0f);
                    }

                    //close snapshot
                    writer.Write(Constants.Server_SnapshotEnd);

                    writer.Write(characters.IndexOf(players[i]));//index of the player in the character list
                    m_Driver.Send(m_Connections[i], writer);
                }
            }
        }

        snapshotCount++;
    }

    
    public void Message(string message, Vector3 pos)
    {
        using (var writer = new DataStreamWriter(256, Allocator.Temp))
        {
            writer.Write(Constants.Server_Message);
            writer.Write(message.Length);
            char[] chars = message.ToCharArray();
            byte[] buffer = new byte[message.Length];
            for (int i = 0; i < message.Length; i++)
            {
                buffer[i] = (byte)chars[i];
            }
            writer.Write(buffer);
            writer.Write(pos.x);
            writer.Write(pos.z);
            //send snapshot to all clients
            for (int k = 0; k < m_Connections.Length; k++)
            {
                m_Driver.Send(m_Connections[k], writer);
            }
        }
    }

    public void AddMessage(string message, Vector3 pos)
    {
        Message(message, pos);
        messages.Add(message);
        messagesPos.Add(pos);
        programmableObjectsContainer.ChatInstruction(message);
    }

    public void SendHackStatus(int objectId, int connectionId)
    {
        ProgrammableObjectsData programmableObject = programmableObjectsContainer.objectListServer[objectId];
        using (var writer = new DataStreamWriter(4096, Allocator.Temp))
        {
            writer.Write(Constants.Server_GetHack);

            writer.Write(objectId);

            writer.Write(programmableObject.inputCodes.Count);
            foreach(InOutVignette vignette in programmableObject.inputCodes)
            {
                byte[] buffer = new byte[vignette.code.Length];
                for (int i = 0; i < vignette.code.Length; i++)
                {
                    buffer[i] = (byte)vignette.code.ToCharArray()[i];
                }
                writer.Write(vignette.code.Length);
                writer.Write(buffer);

                writer.Write(vignette.parameter_int);

                buffer = new byte[vignette.parameter_string.Length];
                for (int i = 0; i < vignette.parameter_string.Length; i++)
                {
                    buffer[i] = (byte)vignette.parameter_string.ToCharArray()[i];
                }
                writer.Write(vignette.parameter_string.Length);
                writer.Write(buffer);

                writer.Write(vignette.is_fixed ? 1 : 0);
            }

            writer.Write(programmableObject.outputCodes.Count);
            foreach (InOutVignette vignette in programmableObject.outputCodes)
            {
                byte[] buffer = new byte[vignette.code.Length];
                for (int i = 0; i < vignette.code.Length; i++)
                {
                    buffer[i] = (byte)vignette.code.ToCharArray()[i];
                }
                writer.Write(vignette.code.Length);
                writer.Write(buffer);

                writer.Write(vignette.parameter_int);

                buffer = new byte[vignette.parameter_string.Length];
                for (int i = 0; i < vignette.parameter_string.Length; i++)
                {
                    buffer[i] = (byte)vignette.parameter_string.ToCharArray()[i];
                }
                writer.Write(vignette.parameter_string.Length);
                writer.Write(buffer);

                writer.Write(vignette.is_fixed ? 1 : 0);
            }

            writer.Write(programmableObject.graph.Count);
            foreach(Arrow arrow in programmableObject.graph)
            {
                writer.Write(arrow.input);
                writer.Write(arrow.output);
                writer.Write(arrow.transmitTime);

                writer.Write(arrow.timeBeforeTransmit.Count);
                foreach (float time in arrow.timeBeforeTransmit)
                {
                    writer.Write(time);
                }
            }
            
            m_Driver.Send(m_Connections[connectionId], writer);

            programmableObject.OnInput("OnHack");
        }

    }

    public void SetHackStatus(int objectId, DataStreamReader stream, ref DataStreamReader.Context readerCtx)
    {
        List<InOutVignette> inputCodes = new List<InOutVignette>();
        List<InOutVignette> outputCodes = new List<InOutVignette>();
        List<Arrow> graph = new List<Arrow>();
        bool isAttract = false;

        char[] buffer;
        int inputCodesCount = (int)stream.ReadUInt(ref readerCtx);
        for (int i = 0; i < inputCodesCount; i++)
        {
            int codeLength = (int)stream.ReadUInt(ref readerCtx);
            buffer = new char[codeLength];
            for (int j = 0; j < codeLength; j++)
            {
                buffer[j] = (char)stream.ReadByte(ref readerCtx);
            }
            string code = new string(buffer);

            int parameter_int = (int)stream.ReadUInt(ref readerCtx);

            int parameter_stringLength = (int)stream.ReadUInt(ref readerCtx);
            buffer = new char[parameter_stringLength];
            for (int j = 0; j < parameter_stringLength; j++)
            {
                buffer[j] = (char)stream.ReadByte(ref readerCtx);
            }
            string parameter_string = new string(buffer);

            bool is_fixed = (stream.ReadUInt(ref readerCtx) == 1);

            InOutVignette vignette = new InOutVignette(code, parameter_int, parameter_string, is_fixed);
            inputCodes.Add(vignette);


        }

        int outputCodesCount = (int)stream.ReadUInt(ref readerCtx);
        for (int i = 0; i < outputCodesCount; i++)
        {
            int codeLength = (int)stream.ReadUInt(ref readerCtx);
            buffer = new char[codeLength];
            for (int j = 0; j < codeLength; j++)
            {
                buffer[j] = (char)stream.ReadByte(ref readerCtx);
            }
            string code = new string(buffer);

            int parameter_int = (int)stream.ReadUInt(ref readerCtx);

            if (code == "UseGadget" && parameter_int == InventoryConstants.Attract)
            {
                isAttract = true;
            }

            int parameter_stringLength = (int)stream.ReadUInt(ref readerCtx);
            buffer = new char[parameter_stringLength];
            for (int j = 0; j < parameter_stringLength; j++)
            {
                buffer[j] = (char)stream.ReadByte(ref readerCtx);
            }
            string parameter_string = new string(buffer);

            bool is_fixed = (stream.ReadUInt(ref readerCtx) == 1);

            InOutVignette vignette = new InOutVignette(code, parameter_int, parameter_string, is_fixed);
            outputCodes.Add(vignette);

        }

        int arrowCount = (int)stream.ReadUInt(ref readerCtx);
        for (int i = 0; i < arrowCount; i++)
        {
            int input = (int)stream.ReadUInt(ref readerCtx);
            int output = (int)stream.ReadUInt(ref readerCtx);
            float transmitTime = stream.ReadFloat(ref readerCtx);

            List<float> timeBeforeTransmit = new List<float>();
            int timeBeforeTransmitCount = (int)stream.ReadUInt(ref readerCtx);
            for (int j = 0; j < timeBeforeTransmitCount; j++)
            {
                float timeBeforeTransmitElement = stream.ReadFloat(ref readerCtx);
                timeBeforeTransmit.Add(timeBeforeTransmitElement);
            }

            Arrow arrow = new Arrow(input, output, transmitTime, timeBeforeTransmit);
            graph.Add(arrow);
        }

        ProgrammableObjectsData objectData = programmableObjectsContainer.objectListServer[objectId];

        objectData.inputCodes = inputCodes;
        objectData.outputCodes = outputCodes;
        objectData.graph = graph;
        if (!isAttract)
        {
            objectData.isAttract = false;
        }
    }

    public int AddNewPlayer()
    {
        //On ajoute un nouveau personnage joueur.
        GameObject pj = Instantiate(prefabPJ, programmableObjectsContainer.transform);
        pj.GetComponent<NavMeshAgent>().enabled = false;
        if(players.Count%2 == 0)
        {
            pj.transform.position = FindObjectOfType<PlayerSpawn>().transform.position;
        }
        else
        {
            pj.transform.position = - FindObjectOfType<PlayerSpawn>().transform.position;
        }
        pj.GetComponent<NavMeshAgent>().enabled = true;
        players.Add(pj.transform);
        characters.Add(pj.transform);
        programmableObjectsContainer.objectListServer.Add(pj.GetComponent<ProgrammableObjectsData>());
        pj.GetComponent<ProgrammableObjectsData>().charactersIndex = characters.Count - 1;
        return characters.Count - 1;
    }

    public void SetConnectionId(int connectionId, NetworkConnection nc)
    {
        using (var writer = new DataStreamWriter(64, Allocator.Temp))
        {
            writer.Write(Constants.Server_SetConnectionId);
            writer.Write(connectionId);
            nc.Send(m_Driver, writer);
        }
    }

    public void Win(int team)
    {

        if (winner == -1)
        {
            winner = team;

            for (int i = 0; i < m_Connections.Length; i++)
            { 
                using (var writer = new DataStreamWriter(64, Allocator.Temp))
                {
                    writer.Write(Constants.Server_Win);
                    writer.Write(winner);
                    m_Connections[i].Send(m_Driver, writer);
                }
            }
        }
    }

#endif
}
