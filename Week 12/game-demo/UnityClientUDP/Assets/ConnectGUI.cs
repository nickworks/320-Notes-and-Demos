using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectGUI : MonoBehaviour
{

    public InputField inputIP;
    public InputField inputPort;

    public ServerRowGUI prefabServerRowGUI;

    List<ServerRowGUI> rows = new List<ServerRowGUI>();

    float timeUntilRefresh = 1;

    public void DirectConnect()
    {
        if (inputIP == null) return;
        if (inputPort == null) return;

        string addr = inputIP.text;
        string port = inputPort.text;

        ushort portNum = 0;
        System.UInt16.TryParse(port, out portNum);

        ClientUDP.singleton.ConnectToServer(addr, portNum);
        
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        timeUntilRefresh -= Time.deltaTime;
        if (timeUntilRefresh <= 0)
        {
            timeUntilRefresh = 2;
            UpdateServerList();
        }
    }
    public void UpdateServerList()
    {
        // remove all old rows
        foreach(ServerRowGUI row in rows)
        {
            if(row != null)
            {
                Destroy(row.gameObject);
            }
        }

        // clear list
        rows.Clear();

        // populate new rows:
        int i = 0;
        foreach(RemoteServer server in ClientUDP.singleton.availableGameServers)
        {
            ServerRowGUI row = Instantiate(prefabServerRowGUI, transform);

            RectTransform rt = (RectTransform)row.transform;
            rt.localScale = Vector3.one;
            rt.sizeDelta = new Vector2(500, 50);
            rt.anchorMax = rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            // each is 70 pixels below the one above it:
            rt.anchoredPosition = new Vector2(0, - i * 70);

            row.Init(server);
            rows.Add(row);
            i++;
        }
    }

}
