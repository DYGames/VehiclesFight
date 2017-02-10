using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieType : MonoBehaviour
{
    public Zombie zombie;
    public List<int> PossibleDropItem;

    void GetItems(Zombie z)
    {
        for (int i = 0; i < 30; i++)
        {
            z.tempitems.Add(-1);
        }

        for (int i = 0; i < Random.Range(1, 7); i++)
        {
            z.tempitems[i] = Random.Range(0,100) < 5 ? PossibleDropItem[Random.Range(0, PossibleDropItem.Count - 1)] : PossibleDropItem.Count;
        }
    }
}
