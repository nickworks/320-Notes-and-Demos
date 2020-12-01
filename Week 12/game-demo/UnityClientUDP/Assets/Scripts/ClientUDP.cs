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

    UdpClient sock = new UdpClient();

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

            // set up receive loop (async):
            ListenForPackets();

            // send a packet to the server (async):
            SendPacket(Buffer.From("JOIN"));

        }

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
                res = await sock.ReceiveAsync();
                Buffer packet = Buffer.From(res.Buffer);
                ProcessPacket(packet);
            } catch {
                break;
            }
        }
    }
    /// <summary>
    /// This function processes a packet and decides what to do next.
    /// </summary>
    /// <param name="packet">The packet to process</param>
    private void ProcessPacket(Buffer packet)
    {
        if (packet.Length < 4) return; // do nothing

        string id = packet.ReadString(0, 4);
        switch (id)
        {
            case "REPL":

                ProcessPacketREPL(packet);

                break;
        }
    }

    private void ProcessPacketREPL(Buffer packet)
    {
        if (packet.Length < 5) return; // do nothing

        int replType = packet.ReadUInt8(4);
        int offset = 5;

        if (replType != 1 && replType != 2 && replType != 3) return; // do nothing

        while (offset <= packet.Length){
            if (packet.Length < offset + 5) return; // do nothing

            int networkID = packet.ReadUInt8(offset + 4);

            switch (replType) {
                case 1: // create:

                    //print("REPL packet CREATE received....");
                    string classID = packet.ReadString(offset, 4);

                    NetworkObject obj = ObjectRegistry.SpawnFrom(classID);

                    if (obj == null) return; // ERROR: class ID not found!

                    offset += 4; // trim out ClassID off beginning of packet data
                    offset += obj.Deserialize(packet.Slice(offset));

                    NetworkObject.AddObject(obj);

                    break;
                case 2: // update:

                    // lookup the object, using networkID
                    
                    NetworkObject obj2 = NetworkObject.GetObjectByNetworkID(networkID);
                    if (obj2 == null) return;

                    offset += 4; // trim out ClassID off beginning of packet data
                    offset += obj2.Deserialize(packet.Slice(offset));

                    break;
                case 3: // delete:

                    // lookup the object, using networkID
                    NetworkObject obj3 = NetworkObject.GetObjectByNetworkID(networkID);
                    if (obj3 == null) return;

                    NetworkObject.RemoveObject(networkID); // remove obj from list of network objects
                    Destroy(obj3.gameObject); // remove obj from game

                    break;
            }


            //print($"packet size: {packet.Length} offset: {offset}");

            break; // no looping...
        }
    }

    /// <summary>
    /// This function sends a packet (current to localhost:320)
    /// </summary>
    /// <param name="packet">The packet to send</param>
    async public void SendPacket(Buffer packet)
    {
        if (sock == null) return;

        // TODO: remove literals from next line:
        await sock.SendAsync(packet.bytes, packet.bytes.Length, "127.0.0.1", 320);
    }
    /// <summary>
    /// When destroying, clean up objects:
    /// </summary>
    private void OnDestroy()
    {
        sock.Close();
    }
}
