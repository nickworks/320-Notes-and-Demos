using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using TMPro;
using System.Text;

public class ConnectToSever : MonoBehaviour
{

    public string host = "127.0.0.1";
    public ushort port = 320; // 0 - 65535

    public TextMeshProUGUI chatDisplay;
    public TMP_InputField inputDisplay;

    TcpClient socketToServer = new TcpClient();

    void Start()
    {
        DoConnect();
    }

    async void DoConnect()
    {
        try
        {
            await socketToServer.ConnectAsync(host, port);
            AddMessageToChatDisplay("Successfully connected to server...");
        } catch (Exception e)
        {
            AddMessageToChatDisplay($"ERROR: {e.Message}");
            return;
        }

        while (true)
        {
            byte[] data = new byte[socketToServer.Available];
            await socketToServer.GetStream().ReadAsync(data, 0, data.Length);
            if(data.Length > 0) AddMessageToChatDisplay(Encoding.ASCII.GetString(data));
        }


    }
    public void AddMessageToChatDisplay(string txt)
    {
        chatDisplay.text += $"{txt}\n";
    }
    
    public void UserDoneEditingMessage(string txt)
    {
        SendMessageToServer(txt);
        inputDisplay.text = "";
        inputDisplay.Select();
        inputDisplay.ActivateInputField();
    }

    public void SendMessageToServer(string txt)
    {
        if (socketToServer.Connected)
        {
            byte[] data = Encoding.ASCII.GetBytes(txt);
            socketToServer.GetStream().Write(data, 0, data.Length);
        }
    }
}
