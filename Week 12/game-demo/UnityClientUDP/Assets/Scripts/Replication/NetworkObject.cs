using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObject
{
    public int networkID;
    public static string classID = "NWOB";

    public virtual void Serialize()
    {
        // TODO: turn object into byte array
    }
    public virtual void Deserialize()
    {
        // TODO: turn object into byte array
    }
}
