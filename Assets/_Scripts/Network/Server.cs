﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

using Unity.Networking.Transport;
using Unity.Collections;

using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using System;

public class Server : MonoBehaviour
{

    public GameObject prefabPJ;

    public UdpCNetworkDriver m_Driver;
    public List<Transform> players = new List<Transform>();
    public List<Transform> characters = new List<Transform>(); // Players + NPCs
    public Transform BlueBatterie;
    public Transform PositionBlueRelay;
    public Transform RedBatterie;
    public Transform PositionRedRelay;
    private bool OrangeIsBack;
    private bool BlueIsBack;
    private bool redIsVisible = false;
    private bool blueIsVisible = false;

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

    private bool hasSendItsRegard;

    public bool hasSomeoneWin = false;

   
    // Start is called before the first frame update
    void Start()
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		/*if (!GameState.CLIENT)
			Camera.main.transform.parent.gameObject.SetActive(false);*/

		//Application.targetFrameRate = 58;

		tmp_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        programmableObjectsContainer = FindObjectOfType<ProgrammableObjectsContainer>();

        serverLobby = FindObjectOfType<ServerLobby>();
        if (serverLobby == null)
        {
            m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);

            if (m_Driver.Bind(new IPEndPoint(IPAddress.Any, 9000)) != 0)
                Debug.Log("Failed to bind to port 9000");

            else
                m_Driver.Listen();

            m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
            lostConnections = new List<bool>();
        }
        else
        {
            m_Driver = serverLobby.m_Driver;
            m_Connections = serverLobby.m_Connections;
            lostConnections = serverLobby.lostConnections;
            for (int i = 0; i < m_Connections.Length; i++)
            {
                AddNewPlayer(serverLobby.lobbyInterfaceState.playerLobbyCards[i].team, serverLobby.lobbyInterfaceState.playerLobbyCards[i].playerName);
            }
            serverLobby.stopUpdate = true;
        }

        ServerBattery[] Batteries;
        Batteries = FindObjectsOfType<ServerBattery>();

        if (Batteries[0].team == 0)
        {
            RedBatterie = Batteries[0].transform;
            BlueBatterie = Batteries[1].transform;
        }
        else
        {
            RedBatterie = Batteries[1].transform;
            BlueBatterie = Batteries[0].transform;
        }
        PositionBlueRelay = BlueBatterie;
        PositionRedRelay = RedBatterie;
        hasSendItsRegard = false;
        
    }

    public void OnApplicationQuit()
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		Debug.Log("Call to OnApplicationQuit() in server");

        try
        {
            m_Driver.Dispose();
            m_Connections.Dispose();
            tmp_Connections.Dispose();
        }
        catch (InvalidOperationException e)
        {
            Debug.Log(e.Message);
        }
    }

    // Update is called once per frame
    void Update()
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		if (!hasSendItsRegard)
        {
            AddMessage("WELCOME TO TONIGHT CEREMONY. GOOD LUCK TO BOTH TEAM AND REMEMBER TO HAVE FUN. @everyone", Vector3.zero, null);
            hasSendItsRegard = true;
            NewAnnoncement(0);
        }
        //Debug.Log(Mathf.Round(1f / Time.deltaTime)); //Framerate

        m_Driver.ScheduleUpdate().Complete();

        // Clean up connections
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
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
                                AddNewPlayer(-1);
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
                                SetConnectionId(connectionId, tmp_Connections[i]);
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

            ServerCharacter selectedPlayer = players[i].gameObject.GetComponent<ServerCharacter>();

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
                            float dest_y = stream.ReadFloat(ref readerCtx);
                            float dest_z = stream.ReadFloat(ref readerCtx);
                            selectedPlayer.normalDestination = new Vector3(dest_x, dest_y, dest_z);
                            break;

                        case Constants.Client_Tacle:
                            int number = (int)stream.ReadUInt(ref readerCtx);
                            if (selectedPlayer.canStun && !selectedPlayer.isStunned && /*number != i &&*/ Vector3.Distance(selectedPlayer.transform.position, characters[number].transform.position) < MANUALSTUNRADIUS)
                            {
                                characters[number].GetComponent<ServerCharacter>().getStun();
                                selectedPlayer.doStun();

                                using (var writer = new DataStreamWriter(16384, Allocator.Temp))
                                {
                                    writer.Write(Constants.Server_ConfirmStun);
                                    m_Driver.Send(m_Connections[i], writer);
                                }
                            }
                            break;

                        case Constants.Client_StartTaking:
                            int objectId = (int)stream.ReadUInt(ref readerCtx);
                            selectedPlayer.carrier.StartTaking(programmableObjectsContainer.objectListServer[objectId].GetComponent<ServerCarrier>());
                            break;

                        case Constants.Client_StartGiving:
                            int objectid = (int)stream.ReadUInt(ref readerCtx);
                            selectedPlayer.carrier.StartGiving(programmableObjectsContainer.objectListServer[objectid].GetComponent<ServerCarrier>());
                            break;

                        case Constants.Client_Open_Door:
                            int numb = (int)stream.ReadUInt(ref readerCtx);
                            if (!selectedPlayer.isStunned)
                            {
                                programmableObjectsContainer.objectListServer[numb].OnInput("OnInteract");
                            }
                            break;

                        case Constants.Client_Message:
                            int length = (int)stream.ReadUInt(ref readerCtx);
                            //Debug.Log(length);
                            byte[] buffer = stream.ReadBytesAsArray(ref readerCtx, length);
                            char[] chars = new char[length];
                            for (int n = 0; n < length; n++)
                            {
                                chars[n] = (char)buffer[n];
                            }
                            string message = new string(chars);
                            AddMessage(message, players[i].transform.position,players[i].GetComponent<ProgrammableObjectsData>(),i);
                            break;

                        case Constants.Client_Ping:
                            Vector2 mapPos = new Vector2(stream.ReadFloat(ref readerCtx), stream.ReadFloat(ref readerCtx));
                            Ping(mapPos);
                            break;

                        case Constants.Client_RequestHack:
                            int object_Id = (int)stream.ReadUInt(ref readerCtx);
                            Debug.Log("Server received a request for object with ID " + object_Id);
                            SendHackStatus(object_Id, i);
                            break;

                        case Constants.Client_SetHack:
                            objectId = (int)stream.ReadUInt(ref readerCtx);
                            SetHackStatus(objectId, stream, ref readerCtx, i);
                            break;

                        case Constants.Client_GiveBackHackToken:
                            objectId = (int)stream.ReadUInt(ref readerCtx);
                            if (programmableObjectsContainer.objectListServer[objectId].isBeingHackedServer == i)
                            {
                                programmableObjectsContainer.objectListServer[objectId].isBeingHackedServer = -1;
                            }
                            break;
                        case Constants.Client_HackInterfaceIsOpen:
                            objectId = (int)stream.ReadUInt(ref readerCtx);
                            programmableObjectsContainer.objectListServer[objectId].OnInput("OnHack");
                            break;
                        case Constants.Client_ThiefHasBeenStunned:
                            int team = (int)stream.ReadUInt(ref readerCtx);
                            bool alreadyMoved = false;
                            if (team == 0)
                            {
                                foreach( InOutVignette ryan in RedBatterie.GetComponent<ProgrammableObjectsData>().outputCodes)
                                {
                                    if(ryan.code == "UseGadget" && ryan.parameter_int == InventoryConstants.OrangeRelay)
                                    {
                                        alreadyMoved = true;
                                    }
                                }
                                if (!alreadyMoved)
                                {
                                    AddMessage("THE ORANGE RELAY IS BACK IN ITS SERVER. @everyone", Vector3.zero,null);
                                    NewAnnoncement(3);
                                    InOutVignette reynolds = new InOutVignette();
                                    reynolds.code = "UseGadget";
                                    reynolds.is_fixed = true;
                                    reynolds.parameter_int = InventoryConstants.OrangeRelay;
                                    RedBatterie.GetComponent<ProgrammableObjectsData>().outputCodes.Add(reynolds);
                                    PositionRedRelay = RedBatterie;
                                }                                
                                OrangeIsBack = true;
                            }
                            else if (team == 1)
                            {
                                foreach (InOutVignette ryan in BlueBatterie.GetComponent<ProgrammableObjectsData>().outputCodes)
                                {
                                    if (ryan.code == "UseGadget" && ryan.parameter_int == InventoryConstants.BlueRelay)
                                    {
                                        alreadyMoved = true;
                                    }
                                }
                                if (!alreadyMoved)
                                {
                                    AddMessage("THE BLUE RELAY IS BACK IN ITS SERVER. @everyone", Vector3.zero,null);
                                    NewAnnoncement(7);
                                    InOutVignette reynolds = new InOutVignette();
                                    reynolds.code = "UseGadget";
                                    reynolds.is_fixed = true;
                                    reynolds.parameter_int = InventoryConstants.BlueRelay;
                                    BlueBatterie.GetComponent<ProgrammableObjectsData>().outputCodes.Add(reynolds);
                                    PositionBlueRelay=BlueBatterie;
                                }                               
                                BlueIsBack = true;
                            }
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
                if (programmableObjectsContainer.objectListServer[j].shouldBeSendToClientEveryFrame|| programmableObjectsContainer.objectListServer[j].shouldBeSendToClientJustOnceMore/*|| programmableObjectsContainer.objectListServer[j].transform==PositionBlueRelay || programmableObjectsContainer.objectListServer[j].transform == PositionRedRelay*/)
                {
                    programmableObjectsContainer.objectListServer[j].shouldBeSendToClientJustOnceMore = false;
                    int charactersIndex = programmableObjectsContainer.objectListServer[j].charactersIndex;
                    using (var writer = new DataStreamWriter(16384, Allocator.Temp))
                    {
                        //snapshot start
                        writer.Write(Constants.Server_Snapshot);
                        writer.Write(snapshotCount);

                        if (charactersIndex != -1) //we have a character, update on its states and positions
                        {
                            ServerCharacter selectedCharacter = characters[charactersIndex].gameObject.GetComponent<ServerCharacter>();

                            writer.Write(Constants.Server_MoveCharacter);
                            writer.Write(charactersIndex);
                            writer.Write(characters[charactersIndex].position.x);
                            writer.Write(characters[charactersIndex].position.y);
                            writer.Write(characters[charactersIndex].position.z);
                            writer.Write(characters[charactersIndex].rotation.eulerAngles.y);

                            writer.Write(selectedCharacter.navMeshAgent.velocity.x);
                            writer.Write(selectedCharacter.navMeshAgent.velocity.z);
                            writer.Write(selectedCharacter.isStunned ? selectedCharacter.timeBeforeEndOfStun : 0f);

                            if (selectedCharacter.carrier)
                            {
                                writer.Write(selectedCharacter.carrier.charge / selectedCharacter.carrier.maxCharge); //send charge ratio, rather than raw value (as clients do not know all max charges)
                            }
                            else
                            {
                                writer.Write(0f);
                            }

                            //Send information about teammates
                            int curTeam = players[i].GetComponent<ServerCharacter>().team;

                            if (selectedCharacter.team == curTeam)
                            //means this is an ally of the player associated with the current connection
                            {
                                writer.Write(Constants.Server_TeammateInfo);

                                char[] playerNameAsChars = selectedCharacter.playerName.ToCharArray();
                                byte[] buffer = new byte[playerNameAsChars.Length];

                                for (int k = 0; k < playerNameAsChars.Length; k++)
                                {
                                    buffer[k] = (byte)playerNameAsChars[k];
                                }
                                writer.Write(playerNameAsChars.Length);
                                writer.Write(buffer);
                            }
                        }

                        //update objects states
                        writer.Write(Constants.Server_UpdateObject);
                        writer.Write(j);
                        writer.Write(programmableObjectsContainer.objectListServer[j].uniqueNumber);
                        writer.Write(programmableObjectsContainer.objectListServer[j].isLightOn ? 1 : 0);
                        writer.Write(programmableObjectsContainer.objectListServer[j].isDoorOpen ? 1 : 0);

                        if (programmableObjectsContainer.objectListServer[j].GetComponent<ServerCarrier>())
                        {
                            // source state
                            ServerSource source = programmableObjectsContainer.objectListServer[j].GetComponent<ServerSource>();
                            ServerBattery battery = programmableObjectsContainer.objectListServer[j].GetComponent<ServerBattery>();
                            if (source)
                            {
                                if (!source.isActiveAndEnabled)
                                {
                                    writer.Write(0);
                                }
                                else if (source.takenFrom)
                                {
                                    writer.Write(3);
                                }
                                else if (source.carrier.charge > 0)
                                {
                                    writer.Write(2);
                                }
                                else
                                {
                                    writer.Write(1);
                                }
                            }
                            else if (battery)
                            {
                                if (battery.receiving)
                                {
                                    writer.Write(1);
                                }
                                else
                                {
                                    writer.Write(0);
                                }
                            }
                            else
                            {
                                writer.Write(0);
                            }
                            var carrier = programmableObjectsContainer.objectListServer[j].GetComponent<ServerCarrier>();
                            writer.Write(carrier.charge / carrier.maxCharge); //send charge ratio, rather than raw value (as clients do not know all max charges)

                        }
                        else
                        {
                            writer.Write(0);
                            writer.Write(0f);

                        }
                        if (programmableObjectsContainer.objectListServer[j].sendingToRedServer)
                        {
                            if (programmableObjectsContainer.objectListServer[j].sendingToBlueServer)
                            {
                                writer.Write(3);
                            }
                            else
                            {
                                writer.Write(2);
                            }
                        }
                        else
                        {
                            if (programmableObjectsContainer.objectListServer[j].sendingToBlueServer)
                            {
                                writer.Write(1);
                            }
                            else
                            {
                                writer.Write(0);
                            }
                        }

                        //close snapshot
                        writer.Write(Constants.Server_SnapshotEnd);

                        //writer.Write(characters.IndexOf(players[i]));
                        writer.Write(players[i].GetComponent<ProgrammableObjectsData>().charactersIndex); //index of the player in the character list
                        writer.Write(players[i].GetComponent<ServerCarrier>().givingTo == null ? 0 : 1);
                        writer.Write(players[i].GetComponent<ServerCarrier>().takingFrom == null ? 0 : 1);
                        if (OrangeIsBack)
                        {
                            writer.Write(1);
                        }
                        else
                        {
                            writer.Write(0);
                        }

                        if (BlueIsBack)
                        {
                            writer.Write(1);
                        }
                        else
                        {
                            writer.Write(0);
                        }
                        m_Driver.Send(m_Connections[i], writer);
                    }
                }
                
            }
        }

        UpdateRelays();

        OrangeIsBack = false;
        BlueIsBack = false;
        snapshotCount++;
    }

    
    public void Message(string message, Vector3 pos, bool isPrivateMessage, List<ProgrammableObjectsData> receivers, int sendByPlayer, bool isPriorityMessage)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

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
            writer.Write(pos.y);
            writer.Write(pos.z);
            if (isPrivateMessage)
            {
                writer.Write(1);
            }
            else
            {
                writer.Write(0);
            }
            if (isPriorityMessage)
            {
                writer.Write(1);
            }
            else
            {
                writer.Write(0);
            }

            //send snapshot to all clients
            for (int k = 0; k < m_Connections.Length; k++)
            {
                if(!isPrivateMessage || receivers.Contains(players[k].GetComponent<ProgrammableObjectsData>()) || k==sendByPlayer || isPriorityMessage)
                {
                    m_Driver.Send(m_Connections[k], writer);
                }
            }

            
        }
    }

    public void AddMessage(string message, Vector3 pos, ProgrammableObjectsData sender)
    {
        AddMessage(message, pos, sender, -1);
    }

    public void AddMessage(string message, Vector3 pos, ProgrammableObjectsData sender, int sendByPlayer)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

        bool isPrivateMessage = false;
        bool isEveryoneMessage = false;
        List<ProgrammableObjectsData> receivers = new List<ProgrammableObjectsData>();
        bool isArobaseOn = false;
        string messageCopy ="\"";
        messages.Add(message);
        messagesPos.Add(pos);

        if (message != "")
        {
            string justOneWord = "";
            foreach (char c in message)
            {
                if ((c == '\n') || (c == '\r') || (c == ' '))
                {
                    if (isArobaseOn && justOneWord != "")
                    {
                        isPrivateMessage = true;
                        if (justOneWord == "me")
                        {
                            receivers.Add(sender);
                        }else if(programmableObjectsContainer.objectNameServer.ContainsKey(justOneWord))
                        {
                            receivers.Add(programmableObjectsContainer.objectNameServer[justOneWord]);
                        }
                        if(justOneWord == "everyone")
                        {
                            isEveryoneMessage = true;
                        }
                          
                    }
                    if (!isArobaseOn)
                    {
                        messageCopy += c;
                    }
                    isArobaseOn = false;
                    justOneWord = "";
                }
                else if (c == '@' && justOneWord == "" && !isArobaseOn)
                {
                    isArobaseOn = true;
                }
                else if(isArobaseOn)
                {
                    justOneWord += c;
                }
                else
                {
                    messageCopy += c;
                }
            }
            messageCopy += '\"';
            if (isArobaseOn && justOneWord != "")
            {
                isPrivateMessage = true;
                if (justOneWord == "me")
                {
                    receivers.Add(sender);
                }
                else if (programmableObjectsContainer.objectNameServer.ContainsKey(justOneWord))
                {
                    receivers.Add(programmableObjectsContainer.objectNameServer[justOneWord]);
                }
                if (justOneWord == "everyone")
                {
                    isEveryoneMessage = true;
                }
            }
            justOneWord = "";

            Message(message, pos, isPrivateMessage, receivers, sendByPlayer, isEveryoneMessage);

            foreach (char c in message)
            {
                if ((c == '\n') || (c == '\r') || (c == ' '))
                {
                    if (justOneWord != "")
                    {
                        if (isPrivateMessage)
                        {
                            foreach(ProgrammableObjectsData r in receivers)
                            {
                                r.ChatInstruction(justOneWord,messageCopy);
                            }
                        }
                        else
                        {
                            programmableObjectsContainer.ChatInstruction(justOneWord,messageCopy);
                        }
                    }

                    justOneWord = "";
                }
                else
                {
                    justOneWord += c;
                }
            }
            if (justOneWord != "")
            {
                if (isPrivateMessage)
                {
                    foreach (ProgrammableObjectsData r in receivers)
                    {
                        r.ChatInstruction(justOneWord,messageCopy);
                    }
                }
                else
                {
                    programmableObjectsContainer.ChatInstruction(justOneWord,messageCopy);
                }
            }
        }
       
    }

    public void Ping(Vector2 mapPos)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		using (var writer = new DataStreamWriter(32, Allocator.Temp))
        {
            writer.Write(Constants.Server_Ping);
            writer.Write(mapPos.x);
            writer.Write(mapPos.y);
            for (int k = 0; k < m_Connections.Length; k++)
            {
                m_Driver.Send(m_Connections[k], writer);
            }
        }
    }

    public void UpdateRelays()
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		using (var writer = new DataStreamWriter(256, Allocator.Temp))
        {
            writer.Write(Constants.Server_UpdateRelays);

            writer.Write(PositionRedRelay.position.x);
            writer.Write(PositionRedRelay.position.y);
            writer.Write(PositionRedRelay.position.z);
            writer.Write(redIsVisible ? 1 : 0);

            writer.Write(PositionBlueRelay.position.x);
            writer.Write(PositionBlueRelay.position.y);
            writer.Write(PositionBlueRelay.position.z);
            writer.Write(blueIsVisible ? 1 : 0);

            //send update to all clients
            for (int k = 0; k < m_Connections.Length; k++)
            {
                m_Driver.Send(m_Connections[k], writer);
            }
        }
    }

    public void SendHackStatus(int objectId, int connectionId)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		ProgrammableObjectsData programmableObject = programmableObjectsContainer.objectListServer[objectId];
        using (var writer = new DataStreamWriter(4096, Allocator.Temp))
        {
            writer.Write(Constants.Server_GetHack);

            writer.Write(objectId);
            if(programmableObject.isBeingHackedServer == -1)
            {
                programmableObject.isBeingHackedServer = connectionId;
                //Debug.Log(programmableObject.isBeingHackedServer);
                writer.Write(1);
                writer.Write(programmableObject.inputCodes.Count);
                foreach (InOutVignette vignette in programmableObject.inputCodes)
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
                foreach (Arrow arrow in programmableObject.graph)
                {
                    writer.Write(arrow.input);
                    writer.Write(arrow.output);
                    // The following lines are commented because it caused problems with infinite loops and is not used anymore
                    /*
                    writer.Write(arrow.transmitTime);

                    writer.Write(arrow.timeBeforeTransmit.Count);
                    foreach (float time in arrow.timeBeforeTransmit)
                    {
                        writer.Write(time);
                    }*/
                }
                m_Driver.Send(m_Connections[connectionId], writer);

                
            }
            else
            {
                writer.Write(0);
                m_Driver.Send(m_Connections[connectionId], writer);
                using (var writer2 = new DataStreamWriter(32, Allocator.Temp))
                {
                    writer2.Write(Constants.Server_WarnInterfaceHacking);
                    m_Driver.Send(m_Connections[programmableObject.isBeingHackedServer], writer2);
                }
            }
        }
    }

    public void SetHackStatus(int objectId, DataStreamReader stream, ref DataStreamReader.Context readerCtx, int indexPlayer)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		List<InOutVignette> inputCodes = new List<InOutVignette>();
        List<InOutVignette> outputCodes = new List<InOutVignette>();
        List<Arrow> graph = new List<Arrow>();
        bool isAttract = false;
        bool sendingToBlue = false;
        bool sendingToRed = false;

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

            if (code == "UseGadget")
            {
                if(parameter_int == InventoryConstants.Attract)
                {
                    isAttract = true;
                }
                if(parameter_int == InventoryConstants.BlueRelay)
                {
                    sendingToBlue = true;
                }
                if(parameter_int == InventoryConstants.OrangeRelay)
                {
                    sendingToRed = true;
                }
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
            // The following lines are commented because it caused problems with infinite loops and is not used anymore
            /*float transmitTime = stream.ReadFloat(ref readerCtx);

            List<float> timeBeforeTransmit = new List<float>();
            int timeBeforeTransmitCount = (int)stream.ReadUInt(ref readerCtx);
            for (int j = 0; j < timeBeforeTransmitCount; j++)
            {
                float timeBeforeTransmitElement = stream.ReadFloat(ref readerCtx);
                timeBeforeTransmit.Add(timeBeforeTransmitElement);
            }*/

            Arrow arrow = new Arrow(input, output);
            graph.Add(arrow);
        }

        ProgrammableObjectsData objectData = programmableObjectsContainer.objectListServer[objectId];

        int RelayHasMoved = (int)stream.ReadUInt(ref readerCtx);

        if(RelayHasMoved%3 == 1)
        {
            PositionRedRelay = players[indexPlayer].transform;
            redIsVisible = false;
            objectData.shouldBeSendToClientJustOnceMore=true;
        }
        else if (RelayHasMoved % 3 == 2)
        {
            PositionRedRelay = objectData.transform;
            redIsVisible = true;
            objectData.shouldBeSendToClientJustOnceMore = true;
        }

        if (RelayHasMoved / 3 == 1)
        {
            PositionBlueRelay = players[indexPlayer].transform;
            blueIsVisible = false;
            objectData.shouldBeSendToClientJustOnceMore = true;
        }
        else if (RelayHasMoved / 3 == 2)
        {
            PositionBlueRelay = objectData.transform;
            blueIsVisible = true;
            objectData.shouldBeSendToClientJustOnceMore = true;
        }

        

        objectData.inputCodes = inputCodes;
        objectData.outputCodes = outputCodes;
        objectData.UpdateArrowGraph(graph);

        if (!isAttract)
        {
            objectData.isAttract = false;
        }
        objectData.sendingToRedServer = sendingToRed;
        objectData.sendingToBlueServer = sendingToBlue;
        if(sendingToRed && objectData.transform == objectData.BlueBatterie)
        {
            objectData.sendingToRedServer = false;
            objectData.GetComponent<ServerBattery>().RelayWin();
        }

        if (sendingToBlue && objectData.transform == objectData.RedBatterie)
        {
            objectData.sendingToBlueServer = false;
            objectData.GetComponent<ServerBattery>().RelayWin();
        }
    }

    public int AddNewPlayer(int team)
	{
		if (!GameState.SERVER) return -1; // replacement for preprocessor

		return AddNewPlayer(team, "defaultName");
    }

    public int AddNewPlayer(int team, string playerName)
	{
		if (!GameState.SERVER) return -1; // replacement for preprocessor

		//On ajoute un nouveau personnage joueur.
		GameObject pj = Instantiate(prefabPJ, programmableObjectsContainer.transform);
        pj.GetComponent<NavMeshAgent>().enabled = false;
        if (team == -1)
        {
            if (players.Count % 2 == 0)
            {
                pj.transform.position = FindObjectOfType<PlayerSpawn>().transform.position;
                pj.GetComponent<ServerCharacter>().team = 0;
                pj.GetComponent<NavMeshAgent>().areaMask = 13;
            }
            else
            {
                pj.transform.position = -FindObjectOfType<PlayerSpawn>().transform.position;
                pj.GetComponent<ServerCharacter>().team = 1;
                pj.GetComponent<NavMeshAgent>().areaMask = 21;
            }
        }
        else
        {
            if (team == 0)
            {
                pj.transform.position = FindObjectOfType<PlayerSpawn>().transform.position;
                pj.GetComponent<ServerCharacter>().team = 0;
                pj.GetComponent<NavMeshAgent>().areaMask = 13;
            }
            else if (team == 1)
            {
                pj.transform.position = -FindObjectOfType<PlayerSpawn>().transform.position;
                pj.GetComponent<ServerCharacter>().team = 1;
                pj.GetComponent<NavMeshAgent>().areaMask = 21;
            }
        }

        pj.GetComponent<ServerCharacter>().playerName = playerName;
        
        pj.GetComponent<NavMeshAgent>().enabled = true;
        players.Add(pj.transform);
        characters.Add(pj.transform);
        programmableObjectsContainer.objectListServer.Add(pj.GetComponent<ProgrammableObjectsData>());
        pj.GetComponent<ProgrammableObjectsData>().charactersIndex = characters.Count - 1;
        return characters.Count - 1;
    }

    public void SetConnectionId(int connectionId, NetworkConnection nc)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		using (var writer = new DataStreamWriter(64, Allocator.Temp))
        {
            writer.Write(Constants.Server_SetConnectionId);
            writer.Write(connectionId);
            nc.Send(m_Driver, writer);
        }
    }

    public void Win(int team)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		hasSomeoneWin = true;
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

    public void SendPath(Vector3[] pathAs3dPositions, NetworkConnection nc)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		using (var writer = new DataStreamWriter(1024, Allocator.Temp))
        {
            writer.Write(Constants.Server_SendPath);
            writer.Write(pathAs3dPositions.Length);
            for (int i = 0; i < pathAs3dPositions.Length; i++)
            {
                writer.Write(pathAs3dPositions[i].x);
                writer.Write(pathAs3dPositions[i].y);
                writer.Write(pathAs3dPositions[i].z);
            }
            nc.Send(m_Driver, writer);
        }
    }

    public NetworkConnection GetNetworkConnectionFromPlayerTransform(Transform tf)
	{
        if (!GameState.SERVER)
        {
            Debug.Log("GetNetworkConnectionFromPlayerTransform is called client side. It should be called server side only. Returning default connection value.");
            return default(NetworkConnection);
        }
        else
        {
            int index = players.IndexOf(tf);
            if (index != -1)
            {
                return m_Connections[index];
            }
            else
            {
                Debug.Log("Could not find requested player transform in the players list. Returning default connection.");
                return default(NetworkConnection);
            }
        }
    }

    public void NewAnnoncement (int number)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		using (var writer = new DataStreamWriter(1024, Allocator.Temp))
        {
            writer.Write(Constants.Server_SendAnnoncement);
            writer.Write(number);
            for(int k = 0; k < m_Connections.Length; k++)
            {
                m_Driver.Send(m_Connections[k], writer);
            }
        }
    }

    public void SendRegards(string name, int team)
    {
        if (!GameState.SERVER) return;
        string message = "";
        message = string.Concat("@", name, " HAS JOIN THE TEAM. ");
        //AddMessage(message, Vector3.zero, null); This should work, it does not work client side, probably a question of frame delay.

        if(BlueBatterie.GetComponent<ServerBattery>().team == team)
        {
            BlueBatterie.GetComponent<ProgrammableObjectsData>().BatteryNewPlayerInTeam(name);
        }
        if (RedBatterie.GetComponent<ServerBattery>().team == team)
        {
            RedBatterie.GetComponent<ProgrammableObjectsData>().BatteryNewPlayerInTeam(name);
        }
    }
}
