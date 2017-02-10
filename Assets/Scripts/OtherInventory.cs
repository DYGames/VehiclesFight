using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OtherInventory : NetworkBehaviour
{
    public bool OpenToDestroy = false;

    [SyncVar]
    public SyncListInt Items;

    public int[] InspectorItems;

    [SyncVar]
    public bool isOpened = false;

    void Awake()
    {
        Items = new SyncListInt();
    }

    void Start()
    {
        if (InspectorItems == null)
            return;

        for (int i = 0; i < Mathf.Min(InspectorItems.Length, 30); i++)
        {
            Items.Add(InspectorItems[i]);
        }
    }

    [Command]
    public void CmdToFill(int n)
    {
        for(int i = 0;i<n;i++)
        {
            Items[i] = Random.Range(1, ItemDatabase.instance.Items.Count - 3);
        }
    }

}