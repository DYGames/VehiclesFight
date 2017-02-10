using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZTHeavy : ZombieType
{
    void Start()
    {
        PossibleDropItem = new List<int>();
        PossibleDropItem.Add(1);
        PossibleDropItem.Add(27);

        zombie = GetComponent<Zombie>();
    }

}
