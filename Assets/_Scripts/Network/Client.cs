using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
//using Unity.Collections.LowLevel.Unsafe;

using System.Net;
using Unity.Networking.Transport;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

//using Buffers = NetStack.Buffers;
//using Serialization = NetStack.Serialization;

public class Client : MonoBehaviour
{
    public string ServerIP = "127.0.0.1"; //localhost by default
    public UdpCNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public IPv4UDPSocket socket;

    public List<ClientCharacter> characters;
    public GameObject characterPrefab;

    private CameraController cameraController;
    private ClientChatInput chat;
    private ProgrammableObjectsContainer programmableObjectsContainer;
    private HackInterface hackInterface;

    public bool done;
    private int lastSnapshot = 0;

    // Start is called before the first frame update
    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
        chat = FindObjectOfType<ClientChatInput>();
        programmableObjectsContainer = FindObjectOfType<ProgrammableObjectsContainer>();
        hackInterface = FindObjectOfType<HackInterface>();

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
                                int snapshotNumber = (int)stream.ReadUInt(ref readerCtx);
                                if (snapshotNumber <= lastSnapshot)
                                    return; //skip update for this frame
                                else
                                    lastSnapshot = snapshotNumber;

                                int k = (int)stream.ReadUInt(ref readerCtx);
                                if (k < characters.Count)
                                {
                                    cameraController.characterToFollow = characters[k].gameObject;
                                }
                                break;

                            case Constants.Server_MoveCharacter:
                                int j = (int)stream.ReadUInt(ref readerCtx);
                                float x = stream.ReadFloat(ref readerCtx);
                                float z = stream.ReadFloat(ref readerCtx);
                                float angle = stream.ReadFloat(ref readerCtx);
                                float xSpeed = stream.ReadFloat(ref readerCtx);
                                float zSpeed = stream.ReadFloat(ref readerCtx);
                                int isStunned = (int)stream.ReadUInt(ref readerCtx);

                                if (j >= characters.Count)
                                {
                                    GameObject newCharacter = Instantiate(characterPrefab);
                                    newCharacter.GetComponent<ClientCharacter>().number = j;
                                    characters.Add(newCharacter.GetComponent<ClientCharacter>());
                                }
                                if (characters[j] != null)
                                {
                                    if (isStunned==1)
                                    {
                                        foreach (MeshRenderer ryan in characters[j].gameObject.GetComponentsInChildren<MeshRenderer>())
                                        {
                                            ryan.material.color = Color.red;
                                        }
                                    }
                                    else
                                    {
                                        foreach (MeshRenderer ryan in characters[j].gameObject.GetComponentsInChildren<MeshRenderer>())
                                        {
                                            ryan.material.color = Color.white;
                                        }
                                    }
                                    characters[j].transform.SetPositionAndRotation(new Vector3(x, characters[j].transform.position.y, z), Quaternion.Euler(0, angle, 0));
                                    characters[j].speed.x = xSpeed;
                                    characters[j].speed.z = zSpeed;                                                                     
                                }
                                break;

                            case Constants.Server_Message:
                                int length = (int)stream.ReadUInt(ref readerCtx);
                                //Debug.Log(length);
                                byte[] buffer = stream.ReadBytesAsArray(ref readerCtx, length);
                                char[] chars = new char[length];
                                for (int n = 0; n < length; n++)
                                {
                                    chars[n] = (char)buffer[n];
                                }
                                //string message = ;
                                chat.AddMessage(new string(chars));
                                break;

                            case Constants.Server_GetHack:
                                GetHackState(stream, readerCtx);
                                break;

                            default:
                                break;
                        }

                        type = stream.ReadUInt(ref readerCtx);
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

    public void Tacle(int number)
    {
        using (var writer = new DataStreamWriter(32, Allocator.Temp))
        {
            writer.Write(Constants.Client_Tacle);
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
        using (var writer = new DataStreamWriter(32, Allocator.Temp))
        {
            writer.Write(Constants.Client_RequestHack);
            writer.Write(objectId);
            Debug.Log("Client is asking for object with ID " + objectId);
            m_Connection.Send(m_Driver, writer);
        }
    }

    public void GetHackState(DataStreamReader stream, DataStreamReader.Context readerCtx)
    {
        int objectId = (int)stream.ReadUInt(ref readerCtx);
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

            Debug.Log(code);
            Debug.Log(parameter_int);
            Debug.Log(parameter_string);
            Debug.Log(is_fixed);
            
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

            Debug.Log(code);
            Debug.Log(parameter_int);
            Debug.Log(parameter_string);
            Debug.Log(is_fixed);

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
                timeBeforeTransmit.Add(stream.ReadFloat(ref readerCtx));
            }

            Arrow arrow = new Arrow(input, output, transmitTime, timeBeforeTransmit);
            graph.Add(arrow);

            Debug.Log("Arrow from input " + input + " and output " + output);
        }

        GameObject selectedGameObject = programmableObjectsContainer.transform.GetChild(objectId).gameObject;

        hackInterface.SelectedProgrammableObject(selectedGameObject, inputCodes, outputCodes, graph);

    }

    public void OnApplicationQuit()
    {
        m_Driver.Dispose();
    }
}
