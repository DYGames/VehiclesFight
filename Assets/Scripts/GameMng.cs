using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameMng : NetworkBehaviour
{
    public static GameMng instance;

    public GameObject zombieprefab;
    public CanvasScaler canvas;

    [SyncVar]
    public bool isWaitForRescue;
    [SyncVar]
    public float RemainTimeForRescue;
    [SyncVar]
    public int CurSpawnPoint;
    [SyncVar]
    public bool ClearFlag;

    public GameObject RescueBar;
    [HideInInspector]
    public Image RescueProgress;

    public Material[] RepairMaterials;
    public Material Blur;

    public GameObject ClearUI;
    public GameObject PressE;

    public GameObject StatusUI;
    public Image StatusPortrait;
    public Image StatusHPBar;
    public Text StatusHPText;

    float OffCount;

    GObject targetObject;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        OffCount = 0;
        RemainTimeForRescue = 2.0f;
        isWaitForRescue = false;
        RescueProgress = RescueBar.transform.GetChild(0).GetComponent<Image>();
        //Screen.SetResolution(1920, 1080, false);
    }

    void Update()
    {
        if(isWaitForRescue && !RescueBar.activeSelf)
        {
            RescueBar.SetActive(true);
        }


        if (isWaitForRescue)
        {
            RescueProgress.fillAmount = RemainTimeForRescue / 60.0f;
            if(RemainTimeForRescue <= 0)
            {
                ClearMng.instance.Clear();
                isWaitForRescue = false;
            }
        }

        if(OffCount > 0)
        {
            OffCount -= Time.deltaTime;
            if(OffCount <= 0)
            {
                OffCount = 0;
                StatusUI.SetActive(false);
            }
        }

        if (targetObject)
        {
            StatusPortrait.sprite = targetObject.Portrait;
            StatusHPBar.fillAmount = targetObject.HP / targetObject.MAXHP;
            StatusHPText.text = string.Format("{0} / {1}", targetObject.HP, targetObject.MAXHP);
        }
    }

    public void setStatus(GObject gobj)
    {
        targetObject = gobj;
        StatusUI.SetActive(true);
        OffCount = 10;
    }

    [Command]
    public void CmdSpawnMinion(Vector3 position, bool canDay,GameObject caller)
    {
        if (NetworkServer.active)
        {
            if (canDay || (TimeController.instance.CurrentTime > 18 || TimeController.instance.CurrentTime < 6))
            {
                GameObject zombie = Instantiate(zombieprefab, new Vector3(position.x, 4.5f, position.z), Quaternion.identity);
                zombie.GetComponent<Zombie>().zombietype = (Zombie.ZOMBIETYPE)Random.Range(0, 4);
                zombie.GetComponent<Zombie>().zombiebehavior = (Zombie.ZOMBIEBEHAVIOR)Random.Range(0, 5);
                caller.GetComponent<WaveMng>().obj = zombie;
                NetworkServer.Spawn(zombie);
            }
        }
    }

    [Command]
    public void CmdSetClearFlag(bool cf)
    {
        ClearFlag = cf;
    }

    [Command]
    public void CmdCheckCurSpawnPoint()
    {
        if (CurSpawnPoint == -1)
            CurSpawnPoint = Random.Range(0, GameObject.Find("SpawnPoints").transform.childCount);
    }

}
