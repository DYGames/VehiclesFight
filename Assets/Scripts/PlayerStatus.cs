using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStatus : NetworkBehaviour
{
    public GObject gobject;

    [SyncVar]
    public int Money;
    [SyncVar]
    public int CreepScore;

    public int EquipItem;

    bool dieproccesed;

    Rigidbody rigid;

    PlayerInventory inventory;

    void Start()
    {
        inventory = GetComponent<Player>().inventory;
        gobject = GetComponent<GObject>();
        Money = 0;
        CreepScore = 0;
        EquipItem = 0;
        dieproccesed = false;
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (gobject.HP <= 0 && !dieproccesed)
        {
            dieproccesed = true;
            StartCoroutine(DieProcessing());
            if (gameObject.GetComponent<Player>().isLocalPlayer)
            {
                Invoke("AttachOtherPlayer", 3);
            }
        }

    }

    void AttachOtherPlayer()
    {
        var players = FindObjectsOfType<Player>();
        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].Equals(gameObject.GetComponent<Player>()))
            {
                FindObjectOfType<ThirdPersonCamera>().AttachWatchPlayer(players[i].transform);
            }
        }
    }

    IEnumerator DieProcessing()
    {
        yield return null;
        transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(0.7f, 0.0f, 0.0f, 0.7f), 0.075f);
        StartCoroutine(DieProcessing());
    }

    [Command]
    public void CmdHit(GameObject hitter, GameObject p, int dmg)
    {
        GObject player = p.GetComponent<GObject>();
        gobject.CmdsetHP(player.HP - dmg);
        if (gobject.HP <= 0)
        {
            gobject.HP = 0;
        }
    }

    [Command]
    public void CmdZombieHit(Vector3 position, Vector3 forward, GameObject zombieparent, float dmg)
    {
        Zombie zombie = zombieparent.GetComponent<Zombie>();
        GameMng.instance.setStatus(zombie.gobject);
        float mhp = zombie.gobject.HP - dmg;
        zombie.CmdSendHP(mhp, position, forward);
        if (mhp <= 0)
        {
            CreepScore += 1;
            Money += 10;
        }
    }

    public void ZombieHit(Vector3 position, Vector3 forward, GameObject zombieparent, int dmg)
    {
        CmdZombieHit(position, forward, zombieparent, dmg);
    }

    public void setEquipItem(int id)
    {
        EquipItem = id;
        Use(id);
    }

    void Use(int id)
    {
        if (ItemDatabase.instance.Items[inventory.UIInventory.getItemByNum(30 + EquipItem).id].itemType == ItemDatabase.ItemType.Food)
        {
            switch (inventory.UIInventory.getItemByNum(30 + EquipItem).id)
            {
                case 9:
                    gobject.CmdsetHPHeal(10, gobject.MAXHP * 0.1f);
                    break;
                case 10:
                    gobject.CmdsetHPHeal(10, gobject.MAXHP * 0.3f);
                    break;
                case 11:
                    gobject.CmdsetHPHeal(10, gobject.MAXHP * 0.5f);
                    break;
                case 12:
                    gobject.CmdsetHPHeal(10, gobject.MAXHP * 0.8f);
                    break;
                default:
                    return;
            }
            //EFFECT
            //PLAYER BODY AND LOCAL SCREEN
        }
    }

}
