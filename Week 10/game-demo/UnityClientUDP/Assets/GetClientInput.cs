using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetClientInput : MonoBehaviour
{

    
    void Update()
    {

        Buffer b = PacketBuilder.CurrentInput();
        if (b != null) ClientUDP.singleton.SendPacket(b);

    }
}
