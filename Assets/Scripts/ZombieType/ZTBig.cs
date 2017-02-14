using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZTBig : ZombieBase
{

	void Start()
    {
        zombie = GetComponent<Zombie>();
        PossibleDropItem = new List<int>();
        PossibleDropItem.Add(1);
        PossibleDropItem.Add(27);

        transform.localScale = Vector3.one * 2;
        zombie.agent.speed *= 0.5f;
    }
}