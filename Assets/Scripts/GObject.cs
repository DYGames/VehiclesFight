using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GObject : NetworkBehaviour
{
    public Sprite Portrait;

    [SyncVar]
    public float MAXHP;
    [SyncVar]
    public float HP;

    [Command]
    public void CmdsetMAXHP(float maxhp)
    {
        MAXHP = maxhp;
    }

    [Command]
    public void CmdsetHP(float hp)
    {
        HP = hp;
    }

    [Command]
    public void CmdsetHPHeal(int time, float amount)
    {
        StartCoroutine(HPHeal(time, amount));
    }

    IEnumerator HPHeal(int time, float amount)
    {
        float secamount = amount / time;
        for (int i = 0; i < time; i++)
        {
            yield return new WaitForSeconds(1);
            HP += secamount;
        }
    }

}
