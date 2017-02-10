using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    [Flags]
    public enum PLAYERSTATE
    {
        IDLE = 1,
        WALK = 2,
        RUN = 4,
        ZOOM = 8,
    }

    public enum PLAYERTYPE
    {
        MARKSMAN,
        REPAIR,
        HEALER,
    }

    public float DefinedSpeed;
    float MaxSpeed;
    float fov;
    Rigidbody rigid;
    int AttackDmg;
    public PLAYERSTATE PlayerState;
    [SyncVar]
    public PLAYERTYPE PlayerType;

    PlayerStatus playerstatus;

    public GameObject effect;
    public GameObject BulletMark;

    [HideInInspector]
    public bool isInvOn = false;
    [HideInInspector]
    public bool isOtherInvOn = false;
    [HideInInspector]
    public bool isWorkBenchOn = false;

    WorkBench workbench;
    Barrier barrier;
    GameObject HealTarget;
    public PlayerInventory inventory;

    Animator animator;

    public Transform topbody;

    OtherInventory otherInventory;
    GameObject otherinventoryobject;

    public GameObject QuickSlot;

    public GameObject CampfirePrefab;
    public float MaxMeleeDistance;

    public GameObject RadioPrefab;

    public GameObject FlashLight;

    bool JumpFlag;

    public CapsuleCollider playercollider;

    public bool isGround;
    public bool isLadder;

    public float jumpHeight = 2.0f;

    Vector3 groundVelocity;

    Transform ground;

    float Fwd;

    bool ShootAble;
    bool MeleeAble;

    [SyncVar]
    public bool FlashActive;

    void Start()
    {
        Fwd = 0;
        playercollider = GetComponent<CapsuleCollider>();
        ShootAble = true;
        MeleeAble = true;
        isGround = false;
        isLadder = false;
        JumpFlag = false;
        FlashActive = false;
        transform.position = new Vector3(-9.57f, 3.5f, -0.79f);
        AttackDmg = 20;
        MaxSpeed = 0;
        PlayerState = PLAYERSTATE.IDLE;
        fov = 60;
        playerstatus = GetComponent<PlayerStatus>();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Animator wantedAnim = GetComponentsInChildren<Animator>()[1];
        Destroy(wantedAnim);
        animator.avatar = wantedAnim.avatar;

        if (!isLocalPlayer)
        {
            QuickSlot.SetActive(false);
        }
        else
        {
            CmdSetType(GameData.Instance.PlayerType);
        }
    }

    [Command]
    void CmdSetType(PLAYERTYPE type)
    {
        PlayerType = type;
    }

    void Update()
    {
        FlashLight.SetActive(FlashActive);
        if (!isLocalPlayer)
            return;
        animator.SetBool("Shooting", false);
        Controll();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer || ClearMng.instance.PlayerClear || playerstatus.gobject.HP <= 0)
            return;
        Move();
    }

    void LateUpdate()
    {
        if (!isLocalPlayer)
            return;

        if (PlayerState != PLAYERSTATE.IDLE)
        {
            Quaternion rotation = Quaternion.Euler(0, Camera.main.transform.parent.localEulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.2f);

            if (PlayerState == (PlayerState | PLAYERSTATE.ZOOM))
            {
                topbody.localEulerAngles = new Vector3(topbody.localEulerAngles.x, topbody.localEulerAngles.y, -Camera.main.transform.parent.localEulerAngles.x);
            }
        }
    }

    void Controll()
    {
        GameMng.instance.PressE.SetActive(false);
        RaycastHit hitinfo;
        bool israyhit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitinfo, 8.0f);

        if (PlayerType == PLAYERTYPE.HEALER && Input.GetKeyDown(KeyCode.Mouse0) && israyhit)
        {
            if (hitinfo.transform.gameObject.CompareTag("Player"))
                HealTarget = hitinfo.transform.gameObject;
        }

        if (Input.GetKeyDown(KeyCode.F))
            CmdSetFlash(!FlashActive);


        if (!isOtherInvOn && !isWorkBenchOn)
        {
            if (!isInvOn)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    OpenInventory();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInventory();
            }
        }
        if (!isInvOn)
        {
            if (!isWorkBenchOn)
            {
                if (israyhit)
                {
                    workbench = hitinfo.transform.GetComponent<WorkBench>();
                    if (workbench != null)
                    {
                        GameMng.instance.PressE.SetActive(true);
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            isWorkBenchOn = true;
                            workbench.OpenWorkBech();
                        }
                    }
                }
            }
            else
            {
                if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)) && workbench != null)
                {
                    isWorkBenchOn = false;
                    workbench.CloseWorkBech();
                }
            }
        }

        if (!isOtherInvOn)
        {
            if (israyhit)
            {
                otherInventory = hitinfo.transform.GetComponent<OtherInventory>();
                if (otherInventory != null && otherInventory.enabled)
                {
                    GameMng.instance.PressE.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        isOtherInvOn = true;
                        otherinventoryobject = hitinfo.transform.gameObject;
                        OpenOtherInventory(otherinventoryobject);
                    }
                }
            }
        }
        else
        {
            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)) && otherInventory != null)
            {
                isOtherInvOn = false;
                CloseOtherInventory(otherinventoryobject, inventory.gameObject);
                if (otherInventory.OpenToDestroy)
                    Destroy(otherInventory.gameObject);
            }
        }

        if (isInvOn || isWorkBenchOn || isWorkBenchOn)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0) && PlayerState == (PlayerState | PLAYERSTATE.ZOOM))
            Shoot(Camera.main.transform.position, Camera.main.transform.forward);

        for (KeyCode k = KeyCode.Alpha1; k < KeyCode.Alpha6; k++)
        {
            if (Input.GetKeyDown(k))
            {
                playerstatus.setEquipItem((int)k - 49);
            }
        }

        Item eitem = inventory.UIInventory.getItemByNum(playerstatus.EquipItem + 30);

        if (israyhit && eitem != null)
        {
            barrier = hitinfo.transform.GetComponent<Barrier>();

            Door door = hitinfo.transform.GetComponent<Door>();
            if (door != null && Input.GetMouseButtonDown(1))
            {
                CmdOpenCloseDoor(door.gameObject);
            }

            if (eitem.id == 25 &&
                Input.GetMouseButtonDown(0))
            {
                inventory.UIInventory.DestroyItem(inventory.UIInventory.getItemByNum(30 + playerstatus.EquipItem), 1);
                SpawnCampFire(hitinfo.point + (Vector3.one / 2.0f));
                ClearMng.instance.setCampFirePlant();
            }

            if (barrier != null
                && barrier.isBroken
                && ItemDatabase.instance.Items[eitem.id].itemType == ItemDatabase.ItemType.RepairMaterial
                && barrier.gameObject.CompareTag("Window"))
            {
                barrier.meshrenderer.material = GameMng.instance.RepairMaterials[eitem.id - 13];
                barrier.isPreview = true;
                if (Input.GetMouseButtonDown(0))
                {
                    inventory.UIInventory.DestroyItem(inventory.UIInventory.getItemByNum(30 + playerstatus.EquipItem), 1);
                    CmdSetNewBarrier(barrier.gameObject, eitem.id - 13);
                }
            }

            if (eitem.id == 26 && Input.GetMouseButtonDown(0))
            {
                inventory.UIInventory.DestroyItem(inventory.UIInventory.getItemByNum(30 + playerstatus.EquipItem), 1);
                CmdSetRadio(hitinfo.point + (Vector3.one / 2.0f));
                ClearMng.instance.setRadio();
            }
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetAxis("Mouse ScrollWheel") > 0f) && isGround)
        {
            JumpFlag = true;
        }

        foreach (var item in FindObjectsOfType<Barrier>())
        {
            item.isPreview = false;
        }
    }

    void Move()
    {
        var inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fov, 0.1f);

        isLadder = false;
        var sca = Physics.OverlapSphere(transform.position, 0.65f);
        for (int i = 0; i < sca.Length; i++)
        {
            if (sca[i].transform.gameObject.name.Equals("Ladder"))
                isLadder = true;
        }

        if (!isInvOn && !isOtherInvOn && !isWorkBenchOn)
        {
            FlashLight.transform.localEulerAngles = Vector3.zero;

            if ((Input.GetKeyDown(KeyCode.LeftShift) || !isVectorzero(inputVector)))
                PlayerState = PLAYERSTATE.RUN;
            if (!isVectorzero(inputVector) && (!Input.GetKey(KeyCode.LeftShift)))
                PlayerState = PLAYERSTATE.WALK;
            if (isVectorzero(inputVector))
                PlayerState = PLAYERSTATE.IDLE;
            if (Input.GetKey(KeyCode.Mouse1))
                PlayerState |= PLAYERSTATE.ZOOM;
            else
                PlayerState &= ~PLAYERSTATE.ZOOM;

            if (PlayerState == PLAYERSTATE.IDLE)
            {
                MaxSpeed = 0;
                fov = 60;
            }
            if (PlayerState == (PlayerState | PLAYERSTATE.WALK))
            {
                MaxSpeed = DefinedSpeed;
                if (PlayerState == PLAYERSTATE.WALK)
                {
                    fov = 60;
                }
            }
            if (PlayerState == PLAYERSTATE.RUN)
            {
                MaxSpeed = DefinedSpeed * 2f;
                fov = 55;
            }
            if (PlayerState == (PlayerState | PLAYERSTATE.ZOOM))
            {
                FlashLight.transform.rotation = Camera.main.transform.rotation;
                fov = 45;
                if (PlayerState == (PlayerState | PLAYERSTATE.WALK))
                {
                    MaxSpeed = DefinedSpeed / 1.2f;
                }
                else
                {
                    MaxSpeed = 0;
                }
            }
        }
        else
            inputVector = Vector3.zero;

        if (isLadder)
        {
            rigid.velocity = new Vector3(0, inputVector.z * 5, 0);
            rigid.useGravity = false;
            return;
        }
        rigid.useGravity = true;

        if (isGround)
        {
            var velocityChange = CalculateVelocityChange(inputVector);
            rigid.AddForce(velocityChange, ForceMode.VelocityChange);

            if (JumpFlag)
            {
                JumpFlag = false;
                rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y + CalculateJumpVerticalSpeed(), rigid.velocity.z);
            }

            UpdateAnimator(inputVector);
            isGround = false;
        }
        else
        {
            var velocityChange = CalculateVelocityChange(inputVector) * 0.7f;
            rigid.AddForce(velocityChange, ForceMode.VelocityChange);
            UpdateAnimator(inputVector);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform == ground)
            ground = null;
    }

    void OnCollisionStay(Collision col)
    {
        TrackGrounded(col);
    }

    void OnCollisionEnter(Collision col)
    {
        TrackGrounded(col);
    }

    private Vector3 CalculateVelocityChange(Vector3 inputVector)
    {
        var relativeVelocity = Camera.main.transform.TransformDirection(inputVector);
        if (inputVector.z > 0)
        {
            relativeVelocity.z *= MaxSpeed;
        }
        else
        {
            relativeVelocity.z *= MaxSpeed;
        }
        relativeVelocity.x *= MaxSpeed;

        var currRelativeVelocity = rigid.velocity - groundVelocity;
        var velocityChange = relativeVelocity - currRelativeVelocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -10.0f, 10.0f);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -10.0f, 10.0f);
        velocityChange.y = 0;

        return velocityChange;
    }

    private float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt(2f * jumpHeight * Mathf.Abs(Physics.gravity.y));
    }

    private void TrackGrounded(Collision collision)
    {
        var maxHeight = playercollider.bounds.min.y + playercollider.radius * .9f;
        foreach (var contact in collision.contacts)
        {
            if (contact.point.y < maxHeight)
            {
                if (isKinematic(collision))
                {
                    groundVelocity = collision.rigidbody.velocity;
                    ground = collision.transform;
                }
                else if (isStatic(collision))
                {
                    ground = collision.transform;
                }
                else
                {
                    groundVelocity = Vector3.zero;
                }
                if (!ClearMng.instance.PlayerClear)
                    CheckClear(collision.gameObject);
                isGround = true;
            }

            break;
        }
    }

    private bool isKinematic(Collision collision)
    {
        return isKinematic(collision.transform);
    }

    private bool isKinematic(Transform trs)
    {
        return rigid && rigid.isKinematic;
    }

    private bool isStatic(Collision collision)
    {
        return isStatic(collision.transform);
    }

    private bool isStatic(Transform trs)
    {
        return trs.gameObject.isStatic;
    }

    void CheckClear(GameObject trs)
    {
        if (trs.CompareTag("Rescue"))
        {
            trs.GetComponent<Animator>().Stop();
            GameMng.instance.ClearUI.SetActive(true);
            ClearMng.instance.PlayerClear = true;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + transform.TransformDirection(new Vector3(0, 1, 1)), 0.75f);
        Gizmos.DrawSphere(transform.position, 15);
    }

    public void Shoot(Vector3 campos, Vector3 camfwd)
    {
        RaycastHit hitinfo;
        bool israyhit = Physics.Raycast(campos, camfwd, out hitinfo);
        Item item = inventory.UIInventory.getItemByNum(30 + playerstatus.EquipItem);

        if (item == null)
            return;

        var collist = Physics.OverlapSphere(transform.position + transform.TransformDirection(new Vector3(0, 1, 1)), 0.75f);

        if (item.id == 1 || item.id == 4)
        {
            if (ShootAble)
                animator.SetBool("Shooting", true);
        }
        else if (MeleeAble)
        {
            animator.SetBool("Shooting", true);
        }

        if ((item.id == 1 || item.id == 4) && PlayerType == PLAYERTYPE.MARKSMAN && ShootAble)
        {
            StartCoroutine(HasShoot());
            if (!israyhit || hitinfo.transform == transform)
                return;
            if (hitinfo.transform.gameObject.CompareTag("Zombie"))
            {
                playerstatus.CmdZombieHit(Camera.main.transform.position, Camera.main.transform.forward, hitinfo.transform.GetComponentInParent<Zombie>().gameObject, AttackDmg * 0.25f);
            }
            else
                HitEffectOnWall(hitinfo.point, hitinfo.normal);
        }
        else if ((item.id == 2 || item.id == 3) && MeleeAble)
        {
            StartCoroutine(HasMelee());
            for (int i = 0; i < collist.Length; i++)
            {
                if (collist[i].gameObject.layer == 8)
                {
                    HitEffectOnWall(hitinfo.point, hitinfo.normal);
                }
                if (collist[i].gameObject.CompareTag("Zombie"))
                {
                    playerstatus.ZombieHit(Camera.main.transform.position, Camera.main.transform.forward, collist[i].transform.GetComponentInParent<Zombie>().gameObject, AttackDmg);
                }
            }
        }
        else if (ItemDatabase.instance.Items[item.id].itemType == ItemDatabase.ItemType.RepairTool && MeleeAble)
        {
            StartCoroutine(HasMelee());
            for (int i = 0; i < collist.Length; i++)
            {
                var ba = collist[i].gameObject.GetComponent<Barrier>();
                if (ba != null)
                {
                    HitEffectOnWall(hitinfo.point, hitinfo.normal);
                    ba.Repair(5);
                    GameMng.instance.setStatus(collist[i].gameObject.GetComponent<GObject>());
                }
            }
        }

    }

    void UpdateAnimator(Vector3 move)
    {
        if (!isGround)
        {
            animator.SetFloat("Forward", 0);
            animator.SetFloat("Strafe", 0);
            animator.SetBool("Aiming", false);
            return;
        }

        float fwd = 0;
        if (PlayerState == (PlayerState | PLAYERSTATE.WALK))
            fwd = move.z * 0.5f;
        else if (PlayerState == (PlayerState | PLAYERSTATE.RUN))
            fwd = move.z;
        Fwd = Mathf.Lerp(Fwd, fwd, 0.1f);

        if (animator.GetFloat("Forward") != Fwd)
            animator.SetFloat("Forward", Fwd);
        if (animator.GetFloat("Strafe") != move.x)
            animator.SetFloat("Strafe", move.x);

        animator.SetBool("Aiming", PlayerState == (PlayerState | PLAYERSTATE.ZOOM));

        Item item = inventory.UIInventory.getItemByNum(30 + playerstatus.EquipItem);
        if (item != null)
        {
            if (item.id == 1 || item.id == 4)
                animator.SetInteger("WeaponType", 1);
            if (item.id == 2 || item.id == 3)
                animator.SetInteger("WeaponType", 0);
        }
    }

    IEnumerator HealRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (HealTarget)
        {
            var colls = Physics.OverlapSphere(transform.position, 15);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].transform.gameObject.Equals(HealTarget))
                {
                    GameMng.instance.setStatus(HealTarget.GetComponent<GObject>());
                    CmdSetPlayerHP(HealTarget, 3);
                }
            }
        }

        StartCoroutine(HealRoutine());
    }

    void OpenInventory()
    {
        inventory.Open();
        isInvOn = true;
    }

    void CloseInventory()
    {
        inventory.Close();
        isInvOn = false;
    }

    //----------------------

    void HitEffectOnWall(Vector3 pos, Vector3 normal)
    {
        if (isServer)
        {
            CmdHitEffect(pos, normal);
        }
        else
        {
            ClientHitEffect(pos, normal);
        }
    }

    [Client]
    void ClientHitEffect(Vector3 pos, Vector3 normal)
    {
        CmdHitEffect(pos, normal);
    }

    [Command]
    void CmdHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject o = Instantiate(effect, pos, Quaternion.LookRotation(normal));
        GameObject b = Instantiate(BulletMark, pos + new Vector3(normal.x * 0.01f, normal.y * 0.01f, normal.z * 0.01f), Quaternion.LookRotation(normal));
        NetworkServer.Spawn(o);
        NetworkServer.Spawn(b);
        Destroy(o, 1);
        Destroy(b, 10);
    }

    //-----------------------

    void OpenOtherInventory(GameObject otherinventory)
    {
        OpenInventory();
        if (isServer)
        {
            CmdOpenOtherInventory(otherinventory);
        }
        else
        {
            ClientOpenOtherInventory(otherinventory);
        }
        inventory.OpenOtherInventory(otherinventory.GetComponent<OtherInventory>());
    }

    [Client]
    void ClientOpenOtherInventory(GameObject otherinventory)
    {
        CmdOpenOtherInventory(otherinventory);
    }

    [Command]
    void CmdOpenOtherInventory(GameObject otherinventory)
    {
        otherinventory.GetComponent<OtherInventory>().isOpened = true;
    }

    //-----------------------

    void CloseOtherInventory(GameObject otherinventory, GameObject clientinventory)
    {
        int[] otherinventoryarray = clientinventory.GetComponent<PlayerInventory>().UIInventory.getOtherInventory();
        if (isServer)
        {
            CmdCloseOtherInventory(otherinventory, otherinventoryarray);
        }
        else
        {
            ClientCloseOtherInventory(otherinventory, otherinventoryarray);
        }
        inventory.CloseOtherInventory();
        inventory.DestroyAllOtherInventory();
        CloseInventory();
    }

    [Client]
    void ClientCloseOtherInventory(GameObject otherinventory, int[] clientinventory)
    {
        CmdCloseOtherInventory(otherinventory, clientinventory);
    }

    [Command]
    void CmdCloseOtherInventory(GameObject otherinventory, int[] clientinventory)
    {
        OtherInventory otherinventorycom = otherinventory.GetComponent<OtherInventory>();
        otherinventorycom.isOpened = false;
        for (int i = 0; i < 30; i++)
        {
            otherinventorycom.Items[i] = clientinventory[i];
        }
    }

    //--------------------------

    void SpawnCampFire(Vector3 pos)
    {
        if (isServer)
        {
            CmdSpawnCampFire(pos);
        }
        else
        {
            ClientSpawnCampFire(pos);
        }
    }

    [Client]
    void ClientSpawnCampFire(Vector3 pos)
    {
        CmdSpawnCampFire(pos);
    }

    [Command]
    void CmdSpawnCampFire(Vector3 pos)
    {
        GameObject c = Instantiate(CampfirePrefab, pos, Quaternion.identity);
        NetworkServer.Spawn(c);
    }

    //-------------------------

    [Command]
    void CmdOpenCloseDoor(GameObject door)
    {
        door.GetComponent<Door>().CmdOpenCloseDoor();
    }

    [Command]
    void CmdSetNewBarrier(GameObject b, int i)
    {
        b.GetComponent<Barrier>().CmdSetNewBarrier(i);
    }

    [Command]
    void CmdSetPlayerHP(GameObject p, int hp)
    {
        p.GetComponent<PlayerStatus>().gobject.HP += hp;
    }

    [Command]
    void CmdSetRadio(Vector3 pos)
    {
        GameObject c = Instantiate(RadioPrefab, pos, Quaternion.identity);
        NetworkServer.Spawn(c);
    }

    [Command]
    void CmdSetFlash(bool active)
    {
        FlashActive = active;
    }

    bool isVectorzero(Vector3 v)
    {
        if (v.x == 0 && v.z == 0)
            return true;
        return false;
    }

    IEnumerator HasShoot()
    {
        ShootAble = false;
        yield return new WaitForSeconds(0.4f);
        ShootAble = true;
    }

    IEnumerator HasMelee()
    {
        MeleeAble = false;
        yield return new WaitForSeconds(0.65f);
        MeleeAble = true;
    }

    void PlayFootstepSound()
    {

    }
}
