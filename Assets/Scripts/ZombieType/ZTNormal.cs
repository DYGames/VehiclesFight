using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZTNormal : ZombieType
{
    void Start()
    {
        PossibleDropItem = new List<int>();
        PossibleDropItem.Add(1);
        PossibleDropItem.Add(27);

        zombie = GetComponent<Zombie>();
        //zombie.gobject.CmdsetMAXHP(50);
        //zombie.gobject.CmdsetHP(50);
    }
}
