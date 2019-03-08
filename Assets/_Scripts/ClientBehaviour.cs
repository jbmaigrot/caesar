using System.Net;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Collections;

using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class ClientBehaviour : MonoBehaviour
{

    public UdpCNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public bool done;
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
                Debug.Log("Something went wrong during connect");
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd  = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");

                var value = 1;
                using (var writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(value);
                    m_Connection.Send(m_Driver, writer);
                }
            }

            else if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtx = default(DataStreamReader.Context);
                uint value = stream.ReadUInt(ref readerCtx);
                Debug.Log("Got the value  = " + value + " back from the server");
                done = true;
                m_Connection.Disconnect(m_Driver);
                m_Connection = default(NetworkConnection);
            }

            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected for some reason");
                m_Connection = default(NetworkConnection);
            }
        }
    }
}
