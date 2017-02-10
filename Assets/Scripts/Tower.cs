using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    bool isPlayerUp;
    public float towerup;
    Vector3 towerdwn;
    void Start()
    {
        isPlayerUp = false;
    }

    void Update()
    {

    }

    public void Select(Transform player)
    {
        if (!isPlayerUp)
        {
            towerdwn = player.position;
            player.position = new Vector3(transform.position.x, towerup, transform.position.z);
            isPlayerUp = true;
        }
        else
        {
            player.position = towerdwn;
            isPlayerUp = false;
        }
    }
}
