using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class ClientUDP : MonoBehaviour
{

    private static ClientUDP _singleton;
    public static ClientUDP singleton
    {
        get { return _singleton; }
        private set { _singleton = value; }
    }

    static UdpClient sockSending;
    static UdpClient sockReceive = new UdpClient(321);

    public List<RemoteServer> availableGameServers = new List<RemoteServer>();

    /// <summary>
    /// Most recent ball update packet
    /// that has been received...
    /// </summary>
    uint ackBallUpdate = 0;

    public Transform ball;
    
    void Start()
    {
        if (singleton != null)
        {
            // already have a clientUDP...
            Destroy(gameObject);

        } else {
            singleton = this;
            DontDestroyOnLoad(gameObject);
            ListenForPackets();
        }
    }

    public void ConnectToServer(string host, ushort port)
    {

        print($"attempt to connect to {host}:{port}");

        // TODO: don't do anything if connected...

        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(host), port);
        sockSending = new UdpClient(ep.AddressFamily);
        sockSending.Connect(ep);

        // set up receive loop (async):
        //ListenForPackets();

        // send a packet to the server (async):
        SendPacket(Buffer.From("JOIN"));
    }

    /// <summary>
    /// This function listens for incoming UDP packets.
    /// </summary>
    async void ListenForPackets()
    {

        while (true)
        {
            UdpReceiveResult res;
            try
            {
                res = await sockReceive.ReceiveAsync();
                ProcessPacket(res);
            } catch {
                print("FAIL");
                break;
            }
        }
    }
    /// <summary>
    /// This function processes a packet and decides what to do next.
    /// </summary>
    /// <param name="packet">The packet to process</param>
    private void ProcessPacket(UdpReceiveResult res)
    {

        Buffer packet = Buffer.From(res.Buffer);

        //print("Received packet?");
        if (packet.Length < 4) return; // do nothing

        string id = packet.ReadString(0, 4);
        print(id);
        switch (id)
        {
            case "REPL":

                ProcessPacketREPL(packet);

                break;
            case "PAWN":
                // extra packet to identify WHICH object (network ID) the player controls

                if (packet.Length < 5) return; // do nothing

                byte networkID = packet.ReadUInt8(4);
                NetworkObject obj = NetworkObject.GetObjectByNetworkID(networkID);
                if (obj)
                {
                    Pawn p = (Pawn) obj;
                    if(p != null) p.canPlayerControl = true;
                }

                break;
            case "HOST":
                if (packet.Length < 7) return; // do nothing

                ushort port = packet.ReadUInt16BE(4);
                int nameLength = packet.ReadUInt8(6);

                if (packet.Length < 7 + nameLength) return; // do nothing...

                string name = packet.ReadString(7, nameLength);

                AddToServerList(new RemoteServer(res.RemoteEndPoint, name));

                break;
        }
    }
    private void AddToServerList(RemoteServer server)
    {
        // check if exists...
        if (!availableGameServers.Contains(server))
        {
            // add
            availableGameServers.Add(server);
        }

        //print("servers: " + availableGameServers.Count);
    }

    private void ProcessPacketREPL(Buffer packet)
    {
        if (packet.Length < 5) return; // do nothing

        int replType = packet.ReadUInt8(4);
        int offset = 5;

        //print($"REPL packet received; type is {replType}");

        if (replType != 1 && replType != 2 && replType != 3) return; // do nothing

        while (offset <= packet.Length){

            int networkID = 0;

            switch (replType) {
                case 1: // create:

                    if (packet.Length < offset + 5) return; // do nothing
                    networkID = packet.ReadUInt8(offset + 4);

                    //print("REPL packet CREATE received....");
                    string classID = packet.ReadString(offset, 4);

                    // check network ID!
                    if (NetworkObject.GetObjectByNetworkID(networkID) != null) return; // ignore if object exists...

                    NetworkObject obj = ObjectRegistry.SpawnFrom(classID);
                    if (obj == null) return; // ERROR: class ID not found!

                    offset += 4; // trim out ClassID off beginning of packet data
                    offset += obj.Deserialize(packet.Slice(offset));

                    NetworkObject.AddObject(obj);

                    break;
                case 2: // update:

                    if (packet.Length < offset + 5) return; // do nothing
                    networkID = packet.ReadUInt8(offset + 4);

                    // lookup the object, using networkID

                    NetworkObject obj2 = NetworkObject.GetObjectByNetworkID(networkID);
                    if (obj2 == null) return;

                    offset += 4; // trim out ClassID off beginning of packet data
                    offset += obj2.Deserialize(packet.Slice(offset));

                    break;
                case 3: // delete:

                    if (packet.Length < offset + 1) return; // do nothing
                    networkID = packet.ReadUInt8(offset);

                    // lookup the object, using networkID
                    NetworkObject obj3 = NetworkObject.GetObjectByNetworkID(networkID);
                    if (obj3 == null) return;

                    NetworkObject.RemoveObject(networkID); // remove obj from list of network objects
                    Destroy(obj3.gameObject); // remove obj from game

                    offset++;

                    break;
            }


            //print($"packet size: {packet.Length} offset: {offset}");
        }
    }

    /// <summary>
    /// This function sends a packet (current to localhost:320)
    /// </summary>
    /// <param name="packet">The packet to send</param>
    async public void SendPacket(Buffer packet)
    {
        if (sockSending == null) return;
        if (!sockSending.Client.Connected) return;
        
        await sockSending.SendAsync(packet.bytes, packet.bytes.Length);
    }
    /// <summary>
    /// When destroying, clean up objects:
    /// </summary>
    private void OnDestroy()
    {
        if(sockSending != null) sockSending.Close();
        if (sockReceive != null)
        {
            print("closing socket");
            sockReceive.Close();
        }
    }
}
