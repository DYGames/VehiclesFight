using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClearMng : NetworkBehaviour
{
    public static ClearMng instance;
    public GameObject RescueObject;

    public bool PlayerClear;

    void Start()
    {
        instance = this;
        PlayerClear = false;
    }

    void Update()
    {
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.F7))
        {
            SetWaitForRescue();
        }

        if (isLocalPlayer && isServer && GameMng.instance.isWaitForRescue)
        {
            CmdSetRemainTime();
        }

        if (isLocalPlayer && isServer)
        {
            CmdUpdateCurrentTime();
        }
    }

    public void setCampFirePlant()
    {
        StartCoroutine(WaitForRescueFind());
    }

    public void setRadio()
    {
        SetWaitForRescue();
    }

    IEnumerator WaitForRescueFind()
    {
        yield return new WaitForSeconds(Random.Range(5, 10) * 10);
        SetWaitForRescue();
    }

    public void SetWaitForRescue()
    {
        if (isServer)
            CmdSetWaitForRescue();
        else
            ClientSetWaitForRescue();
    }

    public void Clear()
    {
        FindObjectOfType<ThirdPersonCamera>().AttachTempTarget(Instantiate(RescueObject).transform);
        Invoke("DetachRescue", 2);
        GameMng.instance.RescueBar.SetActive(false);
    }

    void DetachRescue()
    {
        FindObjectOfType<ThirdPersonCamera>().DetachTempTarget();
    }

    [Client]
    public void ClientSetWaitForRescue()
    {
        CmdSetWaitForRescue();
    }

    [Command]
    public void CmdSetWaitForRescue()
    {
        GameMng.instance.isWaitForRescue = true;
    }

    [Command]
    public void CmdSetRemainTime()
    {
        GameMng.instance.RemainTimeForRescue -= Time.deltaTime;
    }

    [Command]
    public void CmdUpdateCurrentTime()
    {
        float tmp = TimeController.instance.CurrentTime;
        TimeController.instance.CurrentTime = Mathf.Repeat(TimeController.instance.CurrentTime + (Time.deltaTime * 0.1f), 24);
        if (tmp > 23 && TimeController.instance.CurrentTime < 1)
            TimeController.instance.isNextDayForWaveMng = true;
    }
}
