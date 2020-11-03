using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class ClientUDP : MonoBehaviour
{

    UdpClient sock = new UdpClient();

    public Transform ball;
    
    void Start()
    {
        // set up receive loop (async):
        ListenForPackets();

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
            case "BALL":
                if (packet.Length < 16) return; // do nothing

                float x = packet.ReadSingleLE(4);
                float y = packet.ReadSingleLE(8);
                float z = packet.ReadSingleLE(12);

                ball.position = new Vector3(x, y, z);

                break;
        }
    }
    /// <summary>
    /// This function sends a packet (current to localhost:320)
    /// </summary>
    /// <param name="packet">The packet to send</param>
    async void SendPacket(Buffer packet)
    {
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
