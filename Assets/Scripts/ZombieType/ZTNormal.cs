using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZTNormal : ZombieBase
{
    void Start()
    {
        zombie = GetComponent<Zombie>();
        PossibleDropItem = new List<int>();
        PossibleDropItem.Add(1);
        PossibleDropItem.Add(27);
    }
}
