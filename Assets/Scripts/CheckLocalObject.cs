using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CheckLocalObject : NetworkBehaviour
{
    void Start()
    {
        if (isLocalPlayer)
        {
            FindObjectOfType<ThirdPersonCamera>().targetPlayer = transform.GetComponent<Player>();
            FindObjectOfType<ThirdPersonCamera>().UICanvas = transform.Find("Canvas");
        }
    }

    void Update()
    {

    }
}
