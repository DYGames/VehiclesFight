﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZTBig : ZombieType
{

	void Start()
    {
        PossibleDropItem = new List<int>();
        PossibleDropItem.Add(1);
        PossibleDropItem.Add(27);

        zombie = GetComponent<Zombie>();
        //zombie.gobject.HP = 100;
        transform.localScale = Vector3.one * 2;
        zombie.agent.speed *= 0.5f;
    }
}