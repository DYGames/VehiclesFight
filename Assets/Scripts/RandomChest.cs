using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(OtherInventory))]
public class RandomChest : MonoBehaviour
{
    OtherInventory inventory;

    void Start()
    {
        inventory = GetComponent<OtherInventory>();
        if (NetworkServer.active)
            StartCoroutine(CheckNullAndFill());
    }

    IEnumerator CheckNullAndFill()
    {
        yield return new WaitForSeconds(2);

        bool toFill = true;

        for (int i = 0; i < inventory.Items.Count; i++)
        {
            if (inventory.Items[i] != -1)
                toFill = false;
        }

        if (toFill)
            Invoke("setToFill", 10);
        else
            StartCoroutine(CheckNullAndFill());
    }

    void setToFill()
    {
        inventory.CmdToFill(Random.Range(1, 5));
        StartCoroutine(CheckNullAndFill());
    }

}
