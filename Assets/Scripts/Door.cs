using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Door : NetworkBehaviour
{
    public Transform DoorAnchor;
    public Quaternion Close;
    public Quaternion Open;

    [SyncVar]
    bool isOpen;

    void Start()
    {
        isOpen = false;
        StartCoroutine(ToOpen());
        StartCoroutine(ToClose());
    }
    
    [Command]
    public void CmdOpenCloseDoor()
    {
        isOpen = !isOpen;
    }

    IEnumerator ToOpen()
    {
        yield return null;
        if (DoorAnchor.localRotation != Open && isOpen)
        {
            DoorAnchor.localRotation = Quaternion.Lerp(DoorAnchor.localRotation, Open, 0.075f);
        }
        StartCoroutine(ToOpen());
    }

    IEnumerator ToClose()
    {
        yield return null;
        if (DoorAnchor.localRotation != Close && !isOpen)
        {
            DoorAnchor.localRotation = Quaternion.Lerp(DoorAnchor.localRotation, Close, 0.075f);
        }
        StartCoroutine(ToClose());
    }
}
