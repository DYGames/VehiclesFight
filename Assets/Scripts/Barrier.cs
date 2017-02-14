using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Barrier : NetworkBehaviour
{
    public GObject gobject;

    [SyncVar]
    public int ID;
    [SyncVar]
    public bool isBroken;

    public bool isPreview;

    [HideInInspector]
    public MeshRenderer meshrenderer;

    void Start()
    {
        gobject = GetComponent<GObject>();
        meshrenderer = GetComponent<MeshRenderer>();
        isPreview = false;
        isBroken = false;
        ID = -1;
    }

    void Update()
    {
        if (!isPreview && isBroken)
        {
            meshrenderer.material = GameMng.instance.Blur;
        }
        if(ID != -1 && !isBroken)
        {
            isPreview = false;
            meshrenderer.material = GameMng.instance.RepairMaterials[ID];
        }
    }

    //--------------------

    [Command]
    public void CmdHit(int dmg)
    {
        if (gobject.HP <= dmg)
        {
            gobject.CmdsetHP(0);
            isBroken = true;
            return;
        }

        gobject.CmdsetHP(gobject.HP - dmg);
    }

    //---------------------

    public void Repair(int val)
    {
        if (isServer)
            CmdRepair(val);
        else
            ClientRepair(val);
    }

    [Client]
    public void ClientRepair(int val)
    {
        CmdRepair(val);
    }

    [Command]
    public void CmdRepair(int val)
    {
        if (gobject.HP + val > gobject.MAXHP)
        {
            gobject.CmdsetHP(gobject.MAXHP);
            if (isBroken)
            {
                isBroken = false;
            }
            return;
        }

        gobject.CmdsetHP(gobject.HP + val);
    }

    //-----------------------

    [Command]
    public void CmdSetNewBarrier(int id)
    {
        ID = id;
        isBroken = false;
    }
}
