using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;
//using Unity.Collections.LowLevel.Unsafe;

using System.Net;
using Unity.Networking.Transport;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

//using Buffers = NetStack.Buffers;
//using Serialization = NetStack.Serialization;

public class Client : MonoBehaviour
{
    public GameObject characterPrefab;

#if CLIENT
    public string ServerIP = "127.0.0.1"; //localhost by default
    public IPAddress iPAddress;
    public UdpCNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    //public IPv4UDPSocket socket;

    public List<ClientCharacter> characters;
    public List<ClientCharacter> allyCharacters;
    

    private CameraController cameraController;
    private ClientChat chat;
    public ProgrammableObjectsContainer programmableObjectsContainer;
    private HackInterface hackInterface;
    private bool knowOrientationOfCam = false;
    
    private int lastSnapshot = 0;

    public ClientLobby clientLobby;
    public int connectionId;
    private bool initialHandshakeDone;
    public int team;// 0 or 1 ; -1 in case we didn't use the lobby -> automatically assigned based on connectionID

    public int playerIndex;
    public int[] inventory = new int[3];

    private bool isNapperoned = false;

    private Minimap minimap;

    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
        chat = FindObjectOfType<ClientChat>();
        programmableObjectsContainer = FindObjectOfType<ProgrammableObjectsContainer>();
        hackInterface = FindObjectOfType<HackInterface>();
        inventory[0] = InventoryConstants.Attract;
        inventory[1] = InventoryConstants.Stunbox;
        inventory[2] = InventoryConstants.Powerpump;
        minimap = FindObjectOfType<Minimap>();
    }

    void Awake() { 
        //TODO error : A Native Collection has not been disposed, resulting in a memory leak

        clientLobby = FindObjectOfType<ClientLobby>();
        if (clientLobby == null)
        {
            iPAddress = IPAddress.Parse(ServerIP);
            var endpoint = new IPEndPoint(iPAddress, 9000);
            m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);
            //m_Connection = default(NetworkConnection);
            m_Connection = m_Driver.Connect(endpoint);
            connectionId = -1;
            initialHandshakeDone = false;
        }
        else
        {
            m_Driver = clientLobby.m_Driver;
            m_Connection = clientLobby.m_Connection;
            connectionId = clientLobby.connectionId;
            iPAddress = clientLobby.iPAddress;
            initialHandshakeDone = true;
            team = clientLobby.team;
            clientLobby.stopUpdate = true;
        }
    }

    public void OnApplicationQuit()
    {
        Debug.Log("Call to OnApplicationQuit() in client");
        
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
    void LateUpdate()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            Debug.Log("Something went wrong during connect");
            //var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
            var endpoint = new IPEndPoint(iPAddress, 9000);
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

                    switch (type)
                    {
                        case Constants.Server_Snapshot:
                            int snapshotNumber = (int)stream.ReadUInt(ref readerCtx);

                            if (snapshotNumber < lastSnapshot)
                            {
                                Debug.Log("we received snapshot " + snapshotNumber + " but lastSnapshot is " + lastSnapshot);
                                return; //skip update for this frame
                            }
                            else
                            {
                                lastSnapshot = snapshotNumber;

                                type = stream.ReadUInt(ref readerCtx);
                                if (type == Constants.Server_MoveCharacter)
                                {
                                    int j = (int)stream.ReadUInt(ref readerCtx);
                                    float x = stream.ReadFloat(ref readerCtx);
                                    float y = stream.ReadFloat(ref readerCtx);
                                    float z = stream.ReadFloat(ref readerCtx);
                                    float angle = stream.ReadFloat(ref readerCtx);
                                    float xSpeed = stream.ReadFloat(ref readerCtx);
                                    float zSpeed = stream.ReadFloat(ref readerCtx);
                                    int isStunned = (int)stream.ReadUInt(ref readerCtx);

                                    if (j >= characters.Count)
                                    {
                                        for (int k = characters.Count; k <= j; k++)
                                        {
                                            GameObject newCharacter = Instantiate(characterPrefab, programmableObjectsContainer.transform);
                                            newCharacter.GetComponent<ClientCharacter>().number = k;
                                            characters.Add(newCharacter.GetComponent<ClientCharacter>());
                                            newCharacter.GetComponent<ProgrammableObjectsData>().objectIndexClient = programmableObjectsContainer.objectListClient.Count;
                                            programmableObjectsContainer.objectListClient.Add(newCharacter.GetComponent<ProgrammableObjectsData>());
                                        }
                                    }
                                    if (characters[j] != null)
                                    {
                                        if (isStunned == 1)
                                        {
                                            characters[j].GetTacled(true);
                                            if (j == playerIndex)
                                            {
                                                hackInterface.CloseByStun();
                                                if (team == 0)
                                                {
                                                    if (inventory[0] == InventoryConstants.BlueRelay || inventory[1] == InventoryConstants.BlueRelay || inventory[2] == InventoryConstants.BlueRelay)
                                                    {
                                                        ThiefHasBeenIntercepted();
                                                    }
                                                }
                                                else
                                                {
                                                    if (inventory[0] == InventoryConstants.OrangeRelay || inventory[1] == InventoryConstants.OrangeRelay || inventory[2] == InventoryConstants.OrangeRelay)
                                                    {
                                                        ThiefHasBeenIntercepted();
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            characters[j].GetTacled(false);
                                        }
                                        characters[j].transform.position = new Vector3(x, y, z);
                                        characters[j].mesh.localRotation = Quaternion.Euler(0, angle, 0); //we preserve the orientation of the parent object to allow for UI element to face the camera
                                        characters[j].speed.x = xSpeed;
                                        characters[j].speed.z = zSpeed;
                                    }
                                    // Charge
                                    float charge = stream.ReadFloat(ref readerCtx);
                                    if (characters[j].GetComponent<ServerCarrier>())
                                    {
                                        var carrier = characters[j].GetComponent<ServerCarrier>();
                                        carrier.clientCharge = charge;
                                    }

                                    type = stream.ReadUInt(ref readerCtx);

                                    //Identify character as teammate
                                    if (type == Constants.Server_TeammateInfo)
                                    {
                                        ClientCharacter allyCharacter = characters[j].GetComponent<ClientCharacter>();
                                        int playerNameLength = (int)stream.ReadUInt(ref readerCtx);
                                        char[] playerNameBuffer = new char[playerNameLength];
                                        for (int k = 0; k < playerNameLength; k++)
                                        {
                                            playerNameBuffer[k] = (char)stream.ReadByte(ref readerCtx);
                                        }

                                        allyCharacter.isAlly = true;
                                        allyCharacter.playerName = new string(playerNameBuffer);

                                        if (allyCharacter.isKnownAsAlly != true)
                                        {
                                            allyCharacters.Add(characters[j].GetComponent<ClientCharacter>());

                                            characters[j].GetComponentInChildren<Canvas>().enabled = true;
                                            characters[j].GetComponentInChildren<AllyNameDisplay>().enabled = true;
                                            characters[j].GetComponentInChildren<AllyNameDisplay>().allyNameText.text = new string(playerNameBuffer);

                                            allyCharacter.isKnownAsAlly = true;
                                        }

                                        type = stream.ReadUInt(ref readerCtx); //Should be Constants.Server_UpdateObject (need to be removed from the stream)
                                    }
                                }

                                int l = (int)stream.ReadUInt(ref readerCtx);
                                ProgrammableObjectsData progObject = programmableObjectsContainer.objectListClient[l];
                                //Light
                                if ((int)stream.ReadUInt(ref readerCtx) == 0)
                                {
                                    if (progObject.GetComponentInChildren<Light>() != null)
                                        progObject.GetComponentInChildren<Light>().enabled = false;
                                }
                                else
                                {
                                    if (progObject.GetComponentInChildren<Light>() != null)
                                        progObject.GetComponentInChildren<Light>().enabled = true;
                                }
                                //Door
                                if ((int)stream.ReadUInt(ref readerCtx) == 0)
                                {
                                    if (progObject.GetComponentInChildren<DoorScript>() != null)
                                        progObject.GetComponentInChildren<DoorScript>().OnClose();
                                }
                                else
                                {
                                    if (progObject.GetComponentInChildren<DoorScript>() != null)
                                        progObject.GetComponentInChildren<DoorScript>().OnOpen();
                                }
                                int sourceIsActive = (int)stream.ReadUInt(ref readerCtx);
                                if (progObject.GetComponent<ServerSource>())
                                {
#if !SERVER
                                    if (sourceIsActive==1)
                                    {
                                        progObject.gameObject.SetActive(true);
                                        Debug.Log("Active");
                                    }
                                    else
                                    {
                                        progObject.gameObject.SetActive(false);
                                        Debug.Log("NotActive");
                                    }
#endif
                                }
                                    // Charge
                                    float chargeRatio = stream.ReadFloat(ref readerCtx);
                                if (progObject.GetComponent<ServerCarrier>())
                                {
                                    var carrier = progObject.GetComponent<ServerCarrier>();
                                    carrier.clientCharge =  chargeRatio;
                                }

                                //presence of Relay

                                uint relayCode = stream.ReadUInt(ref readerCtx);
                                if (relayCode % 2 == 1)
                                {
                                    progObject.SetSendingToBatterie(false, true);
                                }
                                else
                                {
                                    progObject.SetSendingToBatterie(false, false);
                                }
                                relayCode /= 2;
                                if (relayCode == 1)
                                {
                                    progObject.SetSendingToBatterie(true, true);
                                }
                                else
                                {
                                    progObject.SetSendingToBatterie(true, false);
                                }

                                // End
                                type = stream.ReadUInt(ref readerCtx); //Should be Constants.Server_SnapshotEnd (need to be removed from the stream)

                                playerIndex = (int)stream.ReadUInt(ref readerCtx);
                                if (playerIndex < characters.Count)
                                {
                                    if (!isNapperoned)
                                    {
                                        SpriteRenderer napperon = characters[playerIndex].transform.Find("napperon").GetComponent<SpriteRenderer>();
                                        Renderer range = characters[playerIndex].transform.Find("range").GetComponent<Renderer>();

                                        lineRenderer = characters[playerIndex].transform.Find("lineRenderer").GetComponent<LineRenderer>();
                                        lineRenderer.enabled = true;

                                        if (team == 0)
                                        {
                                            napperon.enabled = true;
                                            napperon.color = new Color(0.961f, 0.51f, 0.365f, 1f);

                                            range.enabled = true;

                                            minimap.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, -45);
                                            minimap.player = characters[playerIndex].transform;
                                            minimap.transform.Find("Map player").GetComponent<Image>().color = new Color(0.961f, 0.51f, 0.365f, 1f);
                                        }
                                        else if (team == 1)
                                        {
                                            napperon.enabled = true;
                                            napperon.color = new Color(0.361f, 0.784f, 0.949f, 1f);

                                            range.enabled = true;

                                            minimap.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, 135);
                                            minimap.player = characters[playerIndex].transform;
                                            minimap.transform.Find("Map player").GetComponent<Image>().color = new Color(0.361f, 0.784f, 0.949f, 1f);
                                        }
                                                
                                        isNapperoned = true;
                                    }
                                    
                                    
                                    cameraController.characterToFollow = characters[playerIndex].gameObject;
                                    if (team == -1)
                                    {
                                        if (!knowOrientationOfCam && (playerIndex - FindObjectOfType<ServerGameCreator>().NbPnj) % 2 == 0)//Une manière dirty dirty de récupérer l'équipe dans laquelle on se trouve. A changer
                                        {
                                            cameraController.RotateCamera180();
                                            knowOrientationOfCam = true;
                                        }
                                    }
                                    else
                                    {
                                        if (!knowOrientationOfCam && team == 0)
                                        {
                                            cameraController.RotateCamera180();
                                            knowOrientationOfCam = true;
                                        }
                                    }
                                }
                                if((int)stream.ReadUInt(ref readerCtx)==1)//OrangeIsBack so we should delete the orange Relay from the inventory.
                                {
                                    for(int ryan = 0;ryan < inventory.Length; ryan++)
                                    {
                                        if (inventory[ryan] == InventoryConstants.OrangeRelay) inventory[ryan] = InventoryConstants.Empty;
                                    }
                                }
                                if ((int)stream.ReadUInt(ref readerCtx) == 1)//BlueIsBack so we should delete the blue Relay from the inventory.
                                {
                                    for (int ryan = 0; ryan < inventory.Length; ryan++)
                                    {
                                        if (inventory[ryan] == InventoryConstants.BlueRelay) inventory[ryan] = InventoryConstants.Empty;
                                    }
                                }
                            }
                            break;

                            

                        case Constants.Server_Message:
                            int length = (int)stream.ReadUInt(ref readerCtx);
                            byte[] buffer = stream.ReadBytesAsArray(ref readerCtx, length);
                            char[] chars = new char[length];
                            for (int n = 0; n < length; n++)
                            {
                                chars[n] = (char)buffer[n];
                            }
                            float xPos = stream.ReadFloat(ref readerCtx);
                            float yPos = stream.ReadFloat(ref readerCtx);
                            float zPos = stream.ReadFloat(ref readerCtx);
                            chat.AddMessage(new string(chars), new Vector3(xPos, yPos, zPos));
                            break;

                        case Constants.Server_GetHack:
                            GetHackState(stream, ref readerCtx);
                            break;
                            

                        case Constants.Server_SetConnectionId:
                            connectionId = (int)stream.ReadUInt(ref readerCtx);
                            initialHandshakeDone = true;
                            break;

                        case Constants.Server_Win:
                            int winner = (int)stream.ReadUInt(ref readerCtx);
                            // TO DO
                            Debug.Log("Team " + winner + " wins!");
                            break;

                        case Constants.Server_SendPath:
                            int pathLength = (int)stream.ReadUInt(ref readerCtx);
                            Vector3[] pathAs3dPositions = new Vector3[pathLength];
                            for (int i = 0; i < pathLength; i++)
                            {
                                float x = stream.ReadFloat(ref readerCtx);
                                float y = stream.ReadFloat(ref readerCtx);
                                float z = stream.ReadFloat(ref readerCtx);
                                pathAs3dPositions[i] = new Vector3(x, y, z);
                            }

                            DrawPath(pathAs3dPositions);
                            break;

                        default:
                            break;
                    }
                }

                //TODO error : deconnecte sans raison apparente
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client got disconnected for some reason");
                    m_Connection = default(NetworkConnection);
                }
            }
        }

        UpdateDrawPath();
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

    public void Tacle(int number)
    {
        using (var writer = new DataStreamWriter(32, Allocator.Temp))
        {
            writer.Write(Constants.Client_Tacle);
            writer.Write(number);

            m_Connection.Send(m_Driver, writer);
        }
    }

    public void StartTaking(int objectId)
    {
        using (var writer = new DataStreamWriter(32, Allocator.Temp))
        {
            writer.Write(Constants.Client_StartTaking);
            writer.Write(objectId);

            m_Connection.Send(m_Driver, writer);
        }
    }

    public void StartGiving(int objectId)
    {
        using (var writer = new DataStreamWriter(32, Allocator.Temp))
        {
            writer.Write(Constants.Client_StartGiving);
            writer.Write(objectId);

            m_Connection.Send(m_Driver, writer);
        }
    }

    public void DoorInteract(int number)
    {
        using (var writer = new DataStreamWriter(32, Allocator.Temp))
        {
            writer.Write(Constants.Client_Open_Door);
            writer.Write(number);

            m_Connection.Send(m_Driver, writer);
        }
    }

    public void Message(string message)
    {
        using (var writer = new DataStreamWriter(256, Allocator.Temp))
        {
            writer.Write(Constants.Client_Message);
            writer.Write(message.Length);
            char[] chars = message.ToCharArray();
            byte[] buffer = new byte[message.Length];
            for (int i = 0; i < message.Length; i++)
            {
                buffer[i] = (byte)chars[i];
            }
            writer.Write(buffer);
            // message.ToCharArray()
            m_Connection.Send(m_Driver, writer);
        }
    }

    public void RequestHackState(int objectId)
    {
        if(!characters[playerIndex].isTacle)
        using (var writer = new DataStreamWriter(32, Allocator.Temp))
        {
            writer.Write(Constants.Client_RequestHack);
            writer.Write(objectId);
            Debug.Log("Client is asking for object with ID " + objectId);
            m_Connection.Send(m_Driver, writer);
        }
    }

    public void GetHackState(DataStreamReader stream, ref DataStreamReader.Context readerCtx)
    {
        int objectId = (int)stream.ReadUInt(ref readerCtx);

        if(stream.ReadUInt(ref readerCtx) == 1)
        {
            List<InOutVignette> inputCodes = new List<InOutVignette>();
            List<InOutVignette> outputCodes = new List<InOutVignette>();
            List<Arrow> graph = new List<Arrow>();

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

            GameObject selectedGameObject = programmableObjectsContainer.objectListClient[objectId].gameObject;

            hackInterface.SelectedProgrammableObject(selectedGameObject, inputCodes, outputCodes, graph);
        }
        else
        {
            //TODO HackRequestDenied
        }
        

    }

    public void SetHackState(int objectId, List<InOutVignette> inputCodes, List<InOutVignette> outputCodes, List<Arrow> graph)
    {
        using (var writer = new DataStreamWriter(4096, Allocator.Temp))
        {
            writer.Write(Constants.Client_SetHack);

            writer.Write(objectId);

            writer.Write(inputCodes.Count);
            foreach (InOutVignette vignette in inputCodes)
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

            writer.Write(outputCodes.Count);
            foreach (InOutVignette vignette in outputCodes)
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

            writer.Write(graph.Count);
            foreach (Arrow arrow in graph)
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
            
            m_Connection.Send(m_Driver, writer);
        }
        
    }

    public void GiveBackHackToken(int objectId)
    {
        using (var writer = new DataStreamWriter(4096, Allocator.Temp))
        {
            writer.Write(Constants.Client_GiveBackHackToken);

            writer.Write(objectId);
            m_Connection.Send(m_Driver, writer);
        }
    }

    private void InitialHandshake()
    {
        using (var writer = new DataStreamWriter(64, Allocator.Temp))
        {
            writer.Write(Constants.Client_ConnectionId);
            writer.Write(connectionId);
            m_Connection.Send(m_Driver, writer);
        }
    }

    private void ThiefHasBeenIntercepted()
    {
        using (var writer = new DataStreamWriter(64, Allocator.Temp))
        {
            writer.Write(Constants.Client_ThiefHasBeenStunned);
            if (team == 1) writer.Write(0);
            else writer.Write(1);
            m_Connection.Send(m_Driver, writer);
        }
    }

    private void DrawPath(Vector3[] pathAs3dPositions)
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = pathAs3dPositions.Length;
            lineRenderer.SetPositions(pathAs3dPositions);
        }
    }

    private void UpdateDrawPath()
    {
        if (lineRenderer != null)
        {
            Vector3 playerPos = lineRenderer.transform.parent.position;
            Vector3 startLine = new Vector3(playerPos.x, playerPos.y + 0.25f, playerPos.z);
            lineRenderer.SetPosition(0, startLine);

            if (lineRenderer.positionCount > 2)
            {
                if (Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1)) < 2.5f)
                {
                    for (int i = 0; i < lineRenderer.positionCount - 1; i++)
                    {
                        lineRenderer.SetPosition(i, lineRenderer.GetPosition(i + 1));
                    }
                    lineRenderer.positionCount -= 1;
                }
            }
        }
    }

    public void OpenedHackInterface(int objectId)
    {
        using (var writer = new DataStreamWriter(4096, Allocator.Temp))
        {
            writer.Write(Constants.Client_HackInterfaceIsOpen);

            writer.Write(objectId);
            m_Connection.Send(m_Driver, writer);
        }
    }
#endif
                                }
