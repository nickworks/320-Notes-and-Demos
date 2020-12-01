using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkObject
{
    new public static string classID = "BLLT";


    public override int Deserialize(Buffer packet)
    {
        return base.Deserialize(packet);
    }

}
