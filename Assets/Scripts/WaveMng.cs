using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class WaveMng : MonoBehaviour
{
    public List<Transform> SpawnPoints;
    public List<BoxCollider> DaySpawnPoints;

    public bool isNight;

    public bool isNextDay;

    public bool isWaitForNextWave;

    public UnityEngine.UI.Text text;

    int Wave;

    Vector3 center;

    public GameObject obj;

    bool clearFlag;

    void Start()
    {
        isNight = false;
        isNextDay = false;
        isWaitForNextWave = true;
        Wave = 1;
        StartCoroutine(SpawnSomeWhere());
        clearFlag = false;
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
        if (GameMng.instance != null)
        {
            if (TimeController.instance.CurrentTime > 18 && !isNight)
                StartNight();
            if (TimeController.instance.CurrentTime > 6 && isNight && isNextDay)
                EndNight();
            if (NetworkServer.active)
            {
                if (isNight && !isNextDay && TimeController.instance.isNextDayForWaveMng)
                {
                    isNextDay = true;
                    TimeController.instance.isNextDayForWaveMng = false;
                }
                clearFlag = true;
                for (int i = 0; i < GameMng.instance.SpawnZombies.Count; i++)
                {
                    if (GameMng.instance.SpawnZombies[i].zombie == null)
                    {
                        GameMng.instance.SpawnZombies.RemoveAt(i);
                    }
                }

                for (int i = 0; i < GameMng.instance.SpawnZombies.Count; i++)
                {
                    Zombie zom = GameMng.instance.SpawnZombies[i].zombie.GetComponent<Zombie>();
                    if (zom != null)
                    {
                        if (!zom.isDie)
                        {
                            clearFlag = false;
                        }
                    }
                }

                if (clearFlag && isWaitForNextWave && isNight)
                {
                    isWaitForNextWave = false;
                    if (NetworkServer.active)
                        GameMng.instance.CmdCheckCurSpawnPoint();
                    if (GameMng.instance.CurSpawnPoint != -1)
                    {
                        GameMng.instance.CmdSetStartWaveFlag(true, Wave);
                        StartCoroutine(StartWave(GameMng.instance.CurSpawnPoint));
                    }
                }
            }

            if (GameMng.instance.StartWaveFlag && GameMng.instance.CurrentWaveNum == Wave)
            {
                StartCoroutine(StartWave(GameMng.instance.CurSpawnPoint));
            }

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
        int wv = Wave;
        Wave++;
        text.gameObject.SetActive(true);
        text.text = "Wave " + wv.ToString() + " Will Start";
        Invoke("ActiveOffText", 2);

        yield return new WaitForSeconds(5);

        text.gameObject.SetActive(true);
        text.text = "Wave " + wv.ToString() + " Start";
        FindObjectOfType<ThirdPersonCamera>().AttachTempTarget(SpawnPoints[n].transform);
        Invoke("Detach", 1);
        isWaitForNextWave = true;
        if (NetworkServer.active)
        {
            for (int i = 0; i < wv * 5; i++)
            {
                GameMng.instance.CmdSpawnMinion(SpawnPoints[n].position, false, gameObject);
                GameMng.ZOB oo = new GameMng.ZOB();
                oo.zombie = obj;
                GameMng.instance.SpawnZombies.Add(oo);
                yield return new WaitForSeconds(0.4f);
            }
        }
        Invoke("ActiveOffText", 2);
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