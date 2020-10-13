using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PacketBuilder
{
    public static Buffer Join(string username)
    {
        int packetLength = 5 + username.Length;
        Buffer packet = Buffer.Alloc(packetLength);

        packet.WriteString("JOIN");
        packet.WriteUInt8((byte)username.Length, 4);
        packet.WriteString(username, 5);

        return packet;
    }
    public static Buffer Chat(string message)
    {
        int packetLength = 5 + message.Length;
        Buffer packet = Buffer.Alloc(packetLength);

        packet.WriteString("CHAT");
        packet.WriteUInt8((byte)message.Length, 4);
        packet.WriteString(message, 5);

        return packet;
    }
    public static Buffer Play(int x, int y)
    {
        Buffer packet = Buffer.Alloc(6);

        packet.WriteString("PLAY");
        packet.WriteUInt8((byte)x, 4);
        packet.WriteUInt8((byte)y, 5);

        return packet;
    }
}
