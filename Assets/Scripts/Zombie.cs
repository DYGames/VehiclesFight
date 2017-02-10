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

    public enum ZOMBIEBEHAVIOR
    {
        NORMAL,
        RUNANDSTOP,
        JUMP,
        BOMB,
        CLIMB,
    }

    public GObject gobject;

    TextMesh hptext;

    public UnityEngine.AI.NavMeshAgent agent;

    public Transform targetPlayer;

    [SyncVar]
    public ZOMBIETYPE zombietype;
    [SyncVar]
    public ZOMBIEBEHAVIOR zombiebehavior;

    [SyncVar]
    public bool isDie;
    public List<int> tempitems;

    [SyncVar]
    Vector3 hitposition;
    [SyncVar]
    Vector3 hitforward;

    public float MaxDistance;

    Animator animator;

    GameObject agenttarget;

    Vector3 lastAgentVelocity;
    NavMeshPath lastAgentPath;
    
    Rigidbody rigid;

    public RagDoll ragdoll;

    public bool dieprocessed;

    public GameObject ItemBoxPrefab;

    [SyncVar]
    bool HitOnNextFrame;

    void Awake()
    {
        MaxDistance = Mathf.Infinity;
    }

    void Start()
    {
        gobject = GetComponent<GObject>();
        ragdoll = GetComponentInChildren<RagDoll>();
        dieprocessed = false;
        rigid = GetComponent<Rigidbody>();
        agenttarget = null;
        animator = GetComponent<Animator>();
        Animator wta = GetComponentsInChildren<Animator>()[1];
        animator.avatar = wta.avatar;
        Destroy(wta);
        gameObject.AddComponent(GameData.Instance.ZombieTypeTypes[(int)zombietype]);
        gameObject.AddComponent(GameData.Instance.ZombieBehaviorTypes[(int)zombiebehavior]);
        hptext = GetComponentInChildren<TextMesh>();
        isDie = false;
        tempitems = new List<int>();
        StartCoroutine(CheckRoutine());
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + transform.TransformDirection(new Vector3(0, 0, 0.75f)), 0.5f);
    }

    void Update()
    {
        hptext.text = gobject.HP == 0 ? "DIE" : gobject.HP.ToString();

        if (isDie && !dieprocessed)
        {
            dieprocessed = true;
            GetComponent<CapsuleCollider>().enabled = false;
            agent.Stop();
            animator.enabled = false;
            Destroy(agent);
            Invoke("DestroySelf", 10);
        }

        if (HitOnNextFrame)
        {
            HitOnNextFrame = false;
            ragdoll.Hit(hitposition, hitforward, gobject.HP <= 0);
        }


        if (agenttarget == null)
            return;

        var collist = Physics.OverlapSphere(transform.position + transform.TransformDirection(new Vector3(0, 0, 0.75f)), 0.5f);

        for (int i = 0; i < collist.Length; i++)
        {
            if (collist[i].gameObject.Equals(agenttarget) && agent && agent.isActiveAndEnabled)
            {
                animator.SetBool("isAttack", true);
                agent.Stop();
            }
        }

        if (agent && agent.isActiveAndEnabled && Vector3.Distance(agenttarget.transform.position, transform.position) < MaxDistance)
        {
            agent.SetDestination(new Vector3(agenttarget.transform.position.x, transform.position.y, agenttarget.transform.position.z));
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

        if (!isDie && agent && agent.isActiveAndEnabled && isServer)
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
                    agenttarget = tg.gameObject;
                    break;
                }
            }
            StartCoroutine(CheckRoutine());
        }
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
        animator.SetBool("isAttack", false);

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
                player.CmdHit(gameObject, player.gameObject, 10);
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
            gameObject.GetComponent(GameData.Instance.ZombieTypeTypes[(int)zombietype]).SendMessage("GetItems", this, SendMessageOptions.DontRequireReceiver);
            for (int i = 0; i < 30; i++)
            {
                oi.Items.Add(tempitems[i]);
            }
            NetworkServer.Spawn(obj);
        }
    }
}
