using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObject : MonoBehaviour
{
    public int networkID;
    public static string classID = "NWOB";

    private static Dictionary<int, NetworkObject> currentObjects = new Dictionary<int, NetworkObject>();

    static public void AddObject(NetworkObject obj)
    {
        if(!currentObjects.ContainsKey(obj.networkID))
            currentObjects.Add(obj.networkID, obj);
    }
    static public NetworkObject GetObjectByNetworkID(int networkID)
    {
        if (!currentObjects.ContainsKey(networkID)) return null;
        return currentObjects[networkID];
    }
    static public void RemoveObject(NetworkObject obj)
    {
        RemoveObject(obj.networkID);
    }
    static public void RemoveObject(int networkID)
    {
        if(currentObjects.ContainsKey(networkID))
            currentObjects.Remove(networkID);
    }


    public virtual void Serialize()
    {
        
    }
    public virtual int Deserialize(Buffer packet)
    {
        networkID = packet.ReadUInt8(0);

        float px = packet.ReadSingleBE(1);
        float py = packet.ReadSingleBE(5);
        float pz = packet.ReadSingleBE(9);

        float rx = packet.ReadSingleBE(13);
        float ry = packet.ReadSingleBE(17);
        float rz = packet.ReadSingleBE(21);

        float sx = packet.ReadSingleBE(25);
        float sy = packet.ReadSingleBE(29);
        float sz = packet.ReadSingleBE(33);

        transform.position = new Vector3(px, py, pz);
        transform.rotation = Quaternion.Euler(rx, ry, rz);
        transform.localScale = new Vector3(sx, sy, sz);

        return 37;
    }
}
