using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class RemoteServer
{
    public IPEndPoint endPoint;
    public string serverName;
    public float timestamp;

    public RemoteServer(IPEndPoint ep, string name)
    {
        endPoint = ep;
        serverName = name;
    }
    public override bool Equals(object obj)
    {
        RemoteServer other = (RemoteServer)obj;
        if (other == null) return false;
        return (other.endPoint.Equals(endPoint));
    }
}
