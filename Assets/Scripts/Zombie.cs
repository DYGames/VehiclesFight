using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class Zombie : NetworkBehaviour
{
    public enum ZOMBIETYPE
    {
        NORMAL,
        FAST,
        SMALL,
        BIG,
        HEAVY,
    }

    public GObject gobject;

    public UnityEngine.AI.NavMeshAgent agent;

    public Transform targetPlayer;

    [SyncVar]
    public ZOMBIETYPE zombietype;
    [SyncVar]
    public bool isDie;
    public List<int> tempitems;

    [SyncVar]
    Vector3 hitposition;
    [SyncVar]
    Vector3 hitforward;

    public float MaxDistance;

    Animator animator;

    public GameObject agenttarget;

    Vector3 lastAgentVelocity;
    NavMeshPath lastAgentPath;

    public RagDoll ragdoll;

    public bool dieprocessed;

    public GameObject ItemBoxPrefab;

    [SyncVar]
    bool HitOnNextFrame;

    float agentspeed;

    AudioSource audiosource;
    public AudioClip[] Clips;

    void Awake()
    {
        MaxDistance = Mathf.Infinity;
    }

    void Start()
    {
        transform.GetChild(0).localPosition = new Vector3(0, -1, 0);
        gobject = GetComponent<GObject>();
        ragdoll = GetComponentInChildren<RagDoll>();
        dieprocessed = false;
        agenttarget = null;
        animator = GetComponent<Animator>();
        animator.avatar = GetComponentsInChildren<Animator>()[1].avatar;
        Destroy(GetComponentsInChildren<Animator>()[1]);
        gameObject.AddComponent(GameData.Instance.ZombieTypeTypes[(int)zombietype]);
        isDie = false;
        tempitems = new List<int>();
        StartCoroutine(CheckRoutine());
        agentspeed = agent.speed;
        audiosource = GetComponent<AudioSource>();
        StartCoroutine(RandomSound());
    }

    IEnumerator RandomSound()
    {
        yield return new WaitForSeconds(Random.Range(3, 6));

        audiosource.clip = Clips[Random.Range(0, Clips.Length)];
        audiosource.Play();

        StartCoroutine(RandomSound());
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + transform.TransformDirection(new Vector3(0, 0, 0.75f)), 0.5f);
    }

    void Update()
    {
        if (isDie && !dieprocessed)
        {
            dieprocessed = true;
            agent.Stop();
            animator.enabled = false;
            Destroy(agent);
            Destroy(gameObject, 10);
        }

        if (HitOnNextFrame)
        {
            HitOnNextFrame = false;
            ragdoll.Hit(hitposition, hitforward, gobject.HP <= 0);
        }


        if (agenttarget == null)
        {
            agent.speed = 0;
            animator.SetBool("isWalk", false);
            return;
        }

        var collist = Physics.OverlapSphere(transform.position + transform.TransformDirection(new Vector3(0, 0, 0.75f)), 0.5f);

        for (int i = 0; i < collist.Length; i++)
        {
            if (collist[i].gameObject.Equals(agenttarget) && agent)
            {
                animator.SetTrigger("Attack");
                agent.Stop();
            }
        }

        if (agent && agent.isActiveAndEnabled)
        {
            agent.speed = agentspeed;
            agent.SetDestination(new Vector3(agenttarget.transform.position.x, transform.position.y, agenttarget.transform.position.z));
            if (agent.velocity.magnitude > 0.05f)
                animator.SetBool("isWalk", true);
            else
                animator.SetBool("isWalk", false);
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    void resume()
    {
        if (agent)
            agent.Resume();
    }

    IEnumerator CheckRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        if (!isDie && agent && isServer)
        {
            string[] str = { "Player", "Barrier", "Tower" };
            float[] rad = { Mathf.Min(Mathf.Infinity, MaxDistance), Mathf.Min(15, MaxDistance), Mathf.Min(Mathf.Infinity, MaxDistance) };
            Transform tg = null;
            for (int i = 0; i < str.Length; i++)
            {
                if (i == 2)
                {
                    var temp = FindNearTarget(transform.position, str[0], rad[0]);
                    if (temp)
                        tg = FindNearTarget(temp.position, str[2], rad[2]);
                }
                else
                    tg = FindNearTarget(transform.position, str[i], rad[i]);
                if (tg != null)
                {
                    if (i == 0 && tg.transform.position.y > transform.position.y + 10)
                        continue;
                    else
                        break;
                }
            }
            agenttarget = tg == null ? null : tg.gameObject;
        }
        StartCoroutine(CheckRoutine());
    }

    Transform FindNearTarget(Vector3 position, string tag, float radius)
    {
        Collider[] target = Physics.OverlapSphere(position, radius);
        var list = new List<Collider>();
        var targetlist = new List<Collider>();
        list.AddRange(target);
        list.ForEach(delegate (Collider a)
        {
            if (a.gameObject.CompareTag(tag))
            {
                targetlist.Add(a);
            }
        });
        targetlist.Sort(delegate (Collider a, Collider b)
        {
            float da = Vector3.Distance(a.transform.position, position);
            float db = Vector3.Distance(b.transform.position, position);
            if (da > db) return 1;
            else if (da < db) return -1;
            else return 0;
        });
        if (targetlist.Count > 0)
            return targetlist[0].transform;
        else
            return null;

    }

    public void Attack()
    {
        //animator.SetBool("isAttack", false);

        if (!NetworkServer.active)
            return;

        var collist = Physics.OverlapSphere(transform.position + transform.TransformDirection(new Vector3(0, 0, 0.75f)), 0.5f);

        for (int i = 0; i < collist.Length; i++)
        {
            Barrier barrier = collist[i].GetComponent<Barrier>();
            PlayerStatus player = collist[i].GetComponent<PlayerStatus>();
            if (barrier)
            {
                barrier.CmdHit(10);
            }
            if (player)
            {
                player.Hit(gameObject, player.gameObject, 10);
            }
        }
    }

    [Command]
    public void CmdSendHP(float hp, Vector3 position, Vector3 forward)
    {
        hitposition = position;
        hitforward = forward;
        HitOnNextFrame = true;
        gobject.CmdsetHP(hp);
        if (hp <= 0)
            gobject.CmdsetHP(0);

        if (hp <= 0 && !isDie)
        {
            isDie = true;
            GameObject obj = Instantiate(ItemBoxPrefab, transform.position + new Vector3(Random.Range(1, 4), 0, Random.Range(1, 4)), Quaternion.identity);
            var oi = obj.GetComponent<OtherInventory>();
            oi.enabled = true;
            oi.OpenToDestroy = true;
            Destroy(obj, 10);
            gameObject.GetComponent(GameData.Instance.ZombieTypeTypes[(int)zombietype]).SendMessage("GetItems", this, SendMessageOptions.DontRequireReceiver);
            for (int i = 0; i < 30; i++)
            {
                oi.Items.Add(tempitems[i]);
            }
            NetworkServer.Spawn(obj);
        }
    }
}
