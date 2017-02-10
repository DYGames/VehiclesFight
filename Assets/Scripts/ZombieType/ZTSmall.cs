using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZTSmall : ZombieType
{
    void Start()
    {
        PossibleDropItem = new List<int>();
        PossibleDropItem.Add(1);
        PossibleDropItem.Add(27);

        zombie = GetComponent<Zombie>();
        //zombie.gobject.HP = 50;
        //transform.localScale = Vector3.one * 0.5f;
    }

}
