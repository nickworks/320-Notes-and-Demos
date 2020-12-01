using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;


public class ObjectRegistry : MonoBehaviour
{

    [Serializable]
    public class RegisteredPrefab
    {
        public string classID = "";
        public NetworkObject prefab;
    }

    //static private Dictionary<string, Type> registeredTypes = new Dictionary<string, Type>();

    static private ObjectRegistry _singleton;

    static private Dictionary<string, NetworkObject> registeredPrefabs = new Dictionary<string, NetworkObject>();

    public RegisteredPrefab[] prefabs;


    private void Start()
    {
        if(_singleton == null)
        {
            _singleton = this;
            DontDestroyOnLoad(this.gameObject);
            RegisterAll();
        } else
        {
            Destroy(this.gameObject);
        }
    }


    public void RegisterAll()
    {
        //RegisterClass<Pawn>();

        foreach(RegisteredPrefab rp in prefabs)
        {
            if(!registeredPrefabs.ContainsKey(rp.classID))
                registeredPrefabs.Add(rp.classID, rp.prefab);
        }

    }

    static public void RegisterClass<T> () where T : NetworkObject
    {
        //string classID = (string) typeof(T).GetField("classID").GetValue(null);
        //registeredTypes.Add(classID, typeof(T));
    }

    static public NetworkObject SpawnFrom(string classID)
    {
        if (registeredPrefabs.ContainsKey(classID))
        {
            //ConstructorInfo cinfo = registeredTypes[classID].GetConstructor(new System.Type[] { });
            //return (NetworkObject) cinfo.Invoke(null);

            return Instantiate(registeredPrefabs[classID]);

        }
        return null;
    }

}
