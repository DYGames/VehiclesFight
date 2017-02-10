using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MiniMap : MonoBehaviour
{
    Transform Player;
    Transform miniPlayer;
    Dictionary<Transform, Transform> OtherPlayers;
    Dictionary<Transform, Transform> Enemys;
    Dictionary<Transform, Transform> Chests;

    public Sprite spritePlayer;
    public Sprite spriteOtherPlayer;
    public Sprite spriteEnemys;
    public Sprite spriteChests;

    public RectTransform MiniMapBack;

    Vector3 MiniMapBackDP;

    void Start()
    {
        StartCoroutine(CheckElement());

        OtherPlayers = new Dictionary<Transform, Transform>();
        Enemys = new Dictionary<Transform, Transform>();
        Chests = new Dictionary<Transform, Transform>();
        MiniMapBackDP = new Vector3(-20, -50);
    }

    void LateUpdate()
    {
        MiniMapBack.parent.localEulerAngles = new Vector3(0, 0, Camera.main.transform.parent.localEulerAngles.y);


        if (Player != null)
        {
            miniPlayer.localPosition = new Vector3(Player.localPosition.x, Player.localPosition.z, 0) + MiniMapBackDP;
            MiniMapBack.localPosition = new Vector3(-miniPlayer.localPosition.x, -miniPlayer.localPosition.y, 0);
        }

    }

    IEnumerator CheckElement()
    {
        yield return new WaitForSeconds(1);

        //--------------------------Player And OtherPlayers
        if (Player == null)
        {
            Player[] p = FindObjectsOfType<Player>();
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i].isLocalPlayer)
                {
                    Player = p[i].transform;
                    miniPlayer = Add(spritePlayer);
                    break;
                }
                else if (!OtherPlayers.ContainsKey(p[i].transform))
                {
                    OtherPlayers.Add(p[i].transform, Add(spriteOtherPlayer));
                }
            }
        }
        CheckAndUpdatePosition(OtherPlayers);

        //-------------------------------Zombies

        CheckAndAdd(Enemys, typeof(Zombie), spriteEnemys);
        CheckAndUpdatePosition(Enemys);

        //------------------------------Chests

        CheckAndAdd(Chests, typeof(OtherInventory), spriteChests);
        CheckAndUpdatePosition(Chests);

        //------------------------------

        StartCoroutine(CheckElement());
    }

    void CheckAndAdd(Dictionary<Transform, Transform> list, System.Type t, Sprite s)
    {
        var oi = FindObjectsOfType(t) as Component[];
        for (int i = 0; i < oi.Length; i++)
        {
            if (!list.ContainsKey(oi[i].transform))
            {

                list.Add(oi[i].transform, Add(s));
            }
        }
    }

    Transform Add(Sprite s)
    {
        GameObject mini = new GameObject();
        mini.transform.parent = MiniMapBack;
        mini.AddComponent<UnityEngine.UI.Image>();
        mini.GetComponent<UnityEngine.UI.Image>().sprite = s;
        mini.GetComponent<RectTransform>().sizeDelta = new Vector2(6, 6);
        return mini.transform;
    }

    void CheckAndUpdatePosition(Dictionary<Transform, Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list.Keys.ToList()[i] == null)
            {
                Destroy(list.Values.ToList()[i].gameObject);
                list.Remove(list.Keys.ToList()[i]);
            }
            else
            {
                Vector3 p = list.Keys.ToList()[i].transform.localPosition;
                list.Values.ToList()[i].transform.localPosition = new Vector3(p.x, p.z, 0) + MiniMapBackDP;
            }
        }

    }

}
