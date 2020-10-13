using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using TMPro;
using System;

public class ControllerGameClient : MonoBehaviour
{
    static public ControllerGameClient singleton;

    TcpClient socket = new TcpClient();

    Buffer buffer = Buffer.Alloc(0);

    public TMP_InputField inputHost;
    public TMP_InputField inputPort;
    public TMP_InputField inputUsername;

    public Transform panelHostDetails;
    public Transform panelUsername;
    public ControllerGameplay panelGameplay;


    void Start()
    {
        // implement singleton design:
        if (singleton)
        {
            // already set...
            Destroy(gameObject); // there's already one out there....
        } else
        {
            singleton = this;
            DontDestroyOnLoad(gameObject); // don't destroy when loading new scenes!

            // show connection screen:
            panelHostDetails.gameObject.SetActive(true);
            panelUsername.gameObject.SetActive(false);
            panelGameplay.gameObject.SetActive(false);

        }

        //Buffer buff = Buffer.Alloc(4);
        //buff.Concat(new byte[] { 1, 2, 3, 4 },-1);
        //buff.Consume(-2);
        //print(buff);

    }

    public void OnButtonConnect()
    {
        string host = inputHost.text;
        UInt16.TryParse(inputPort.text, out ushort port);

        TryToConnect(host, port);
    }

    public void OnButtonUsername()
    {
        string user = inputUsername.text;
        Buffer packet = PacketBuilder.Join(inputUsername.text);
        SendPacketToServer(packet);
    }

    async public void TryToConnect(string host, int port)
    {
        if (socket.Connected) return; // already connected to a server, cancel...
        try
        {
            await socket.ConnectAsync(host, port);

            // switch to "username" screen:
            panelHostDetails.gameObject.SetActive(false);
            panelUsername.gameObject.SetActive(true);
            panelGameplay.gameObject.SetActive(false);

            StartReceivingPackets();

        } catch (Exception e)
        {
            print("FAILED TO CONNECTED...");
            // TODO: display message to player...

            panelHostDetails.gameObject.SetActive(true);
            panelUsername.gameObject.SetActive(false);
            panelGameplay.gameObject.SetActive(false);
        }
    }

    async private void StartReceivingPackets()
    {

        int maxPacketSize = 4096;

        while (socket.Connected)
        {
            byte[] data = new byte[maxPacketSize];

            try
            {
                int bytesRead = await socket.GetStream().ReadAsync(data, 0, maxPacketSize);
                buffer.Concat(data, bytesRead);
                ProcessPackets();
            }
            catch (Exception e) { }
        }
    }

    void ProcessPackets()
    {
        if (buffer.Length < 4) return; // not enough data in buffer

        string packetIdentifier = buffer.ReadString(0, 4);

        switch(packetIdentifier){
            case "JOIN":
                if (buffer.Length < 5) return; // not enough data for a JOIN packet
                byte joinResponse = buffer.ReadUInt8(4);
                
                if (joinResponse == 1 || joinResponse == 2 || joinResponse == 3)
                {
                    // username accepted!
                    // switch to gameplay screen:
                    panelHostDetails.gameObject.SetActive(false);
                    panelUsername.gameObject.SetActive(false);
                    panelGameplay.gameObject.SetActive(true);

                } else if (joinResponse == 9) {

                    // server FULL, send to first screen
                    panelHostDetails.gameObject.SetActive(true);
                    panelUsername.gameObject.SetActive(false);
                    panelGameplay.gameObject.SetActive(false);

                } else {
                    // username denied!

                    // switch to username screen:
                    panelHostDetails.gameObject.SetActive(false);
                    panelUsername.gameObject.SetActive(true);
                    panelGameplay.gameObject.SetActive(false);
                    inputUsername.text = "";

                    // TODO: show error message to user
                }
                
                buffer.Consume(5);

                break;
            case "UPDT":
                if (buffer.Length < 15) return; // not enough data for a UPDT packet

                byte whoseTurn = buffer.ReadUInt8(4);
                byte gameStatus = buffer.ReadUInt8(5);

                byte[] spaces = new byte[9];
                for(int i = 0; i < 9; i++)
                {
                    spaces[i] = buffer.ReadUInt8(6 + i);
                }

                // switch to gameplay:
                panelHostDetails.gameObject.SetActive(false);
                panelUsername.gameObject.SetActive(false);
                panelGameplay.gameObject.SetActive(true);

                panelGameplay.UpdateFromServer(gameStatus, whoseTurn, spaces);

                buffer.Consume(15);

                break;
            case "CHAT":

                byte usernameLength = buffer.ReadByte(4);
                ushort messageLength = buffer.ReadUInt16BE(5);

                int fullPacketLength = 7 + usernameLength + messageLength;

                if (buffer.Length < fullPacketLength) return;

                string username = buffer.ReadString(7, usernameLength);
                string message = buffer.ReadString(7 + usernameLength, messageLength);

                // switch to gameplay:
                panelHostDetails.gameObject.SetActive(false);
                panelUsername.gameObject.SetActive(false);
                panelGameplay.gameObject.SetActive(true);

                // TODO: update chat view

                buffer.Consume(fullPacketLength);

                break;
            default:
                print("unknown packet identifier...");

                buffer.Clear();

                break;
        }


    }

    async public void SendPacketToServer(Buffer packet)
    {
        if (!socket.Connected) return; // not connected to server...

        await socket.GetStream().WriteAsync(packet.bytes, 0, packet.bytes.Length);

    }

    public void SendPlayPacket(int x, int y)
    {
        SendPacketToServer( PacketBuilder.Play(x, y) );
    }

}
