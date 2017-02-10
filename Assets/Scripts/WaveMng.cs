using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class WaveMng : MonoBehaviour
{
    public List<Transform> SpawnPoints;
    public List<BoxCollider> DaySpawnPoints;

    List<GameObject> SpawnZombies;

    public bool isNight;

    public bool isNextDay;

    public bool isWaitForNextWave;

    public UnityEngine.UI.Text text;

    int Wave;

    Vector3 center;

    public GameObject obj;

    void Start()
    {
        isNight = false;
        isNextDay = false;
        isWaitForNextWave = true;
        Wave = 1;
        StartCoroutine(SpawnSomeWhere());
        SpawnZombies = new List<GameObject>();
        if (NetworkServer.active)
            GameMng.instance.CmdSetClearFlag(false);
    }

    IEnumerator SpawnSomeWhere()
    {
        yield return new WaitForSeconds(Random.Range(5, 10));

        if (NetworkServer.active)
        {
            int r = Random.Range(0, DaySpawnPoints.Count);
            GameMng.instance.CmdSpawnMinion(new Vector3(Random.Range(DaySpawnPoints[r].bounds.min.x, DaySpawnPoints[r].bounds.max.x), 3.5f, Random.Range(DaySpawnPoints[r].bounds.min.z, DaySpawnPoints[r].bounds.max.z)), true, gameObject);
            obj.GetComponent<Zombie>().MaxDistance = 15;
        }
        StartCoroutine(SpawnSomeWhere());
    }

    void Update()
    {
        if (TimeController.instance != null)
        {
            if (TimeController.instance.CurrentTime > 18 && !isNight)
                StartNight();
            if (TimeController.instance.CurrentTime > 6 && isNight && isNextDay)
                EndNight();
            if (isNight && !isNextDay && TimeController.instance.isNextDayForWaveMng)
            {
                isNextDay = true;
                TimeController.instance.isNextDayForWaveMng = false;
            }
        }
        if (GameMng.instance != null && NetworkServer.active)
        {
            GameMng.instance.CmdSetClearFlag(true);
            for (int i = 0; i < SpawnZombies.Count; i++)
            {
                if (SpawnZombies[i] == null)
                {
                    SpawnZombies.RemoveAt(i);
                }
            }

            for (int i = 0; i < SpawnZombies.Count; i++)
            {
                Zombie zom = SpawnZombies[i].GetComponent<Zombie>();
                if (zom != null)
                {
                    if (!zom.isDie)
                    {
                        GameMng.instance.CmdSetClearFlag(false);
                    }
                }
            }
        }
        if (GameMng.instance != null && GameMng.instance.ClearFlag == true && isWaitForNextWave && isNight)
        {
            isWaitForNextWave = false;
            GameMng.instance.CmdCheckCurSpawnPoint();
            StartCoroutine(StartWave(GameMng.instance.CurSpawnPoint));
        }

    }

    public void StartNight()
    {
        isNight = true;
        isWaitForNextWave = true;
    }

    public void EndNight()
    {
        text.gameObject.SetActive(true);
        text.text = "Night is Over";
        Invoke("ActiveOffText", 2);

        isNight = false;
        isNextDay = false;
        isWaitForNextWave = true;
        Wave = 1;
    }

    public IEnumerator StartWave(int n)
    {
        text.gameObject.SetActive(true);
        text.text = "Wave " + Wave.ToString() + " Will Start";
        Invoke("ActiveOffText", 2);

        yield return new WaitForSeconds(5);

        text.gameObject.SetActive(true);
        text.text = "Wave " + Wave.ToString() + " Start";
        FindObjectOfType<ThirdPersonCamera>().AttachTempTarget(SpawnPoints[n].transform);
        Invoke("Detach", 1);
        isWaitForNextWave = true;
        if (NetworkServer.active)
        {
            for (int i = 0; i < Wave * 5; i++)
            {
                GameMng.instance.CmdSpawnMinion(SpawnPoints[n].position, false, gameObject);
                SpawnZombies.Add(obj);
                yield return new WaitForSeconds(0.4f);
            }
        }
        Invoke("ActiveOffText", 2);
        Wave++;
    }

    void Detach()
    {
        FindObjectOfType<ThirdPersonCamera>().DetachTempTarget();
    }

    void ActiveOffText()
    {
        text.gameObject.SetActive(false);
    }

    public void ZombieKilled()
    {

    }

}