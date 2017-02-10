using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZTFast : ZombieType
{
    void Start()
    {
        PossibleDropItem = new List<int>();
        PossibleDropItem.Add(1);
        PossibleDropItem.Add(27);

        zombie = GetComponent<Zombie>();
        //zombie.gobject.HP = 50;
        zombie.agent.speed *= 2;
    }
}
