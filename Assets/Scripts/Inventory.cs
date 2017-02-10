using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public enum ITEMPARENT
    {
        PLAYER,
        OTHER,
    }

    public GameObject UIInventory;

    public GameObject PlayerInventory;
    public GameObject OtherInventory;

    public GameObject Information;
    public Image infoImage;
    public Text infoName;
    public Text infoInfo;

    public List<Item> IInventory;

    public OtherInventory otherinventory = null;

    Vector3[] LocalPositions;
    Rect[] Rects;

    public Item SelectedObject = null;

    bool isOpenInfo;

    public GameObject ItemPrefab;

    public int MaxIdx;
    public TextAsset positions;

    public Transform InventoryPosition;
    public Transform OtherInventoryPosition;

    public Vector2 Ratio;

    public GameObject PlayerModel;

    public Player player;

    void Start()
    {
        player = FindObjectOfType<ThirdPersonCamera>().targetPlayer;
        Ratio = new Vector2(Screen.width / 1920.0f, Screen.height / 1080.0f);
        Rects = new Rect[65];
        LocalPositions = new Vector3[65];
        //   using (System.IO.StreamWriter file =
        //       new System.IO.StreamWriter(@"Assets/Positions.txt"))
        //   {
        //       Vector3 f = IInventory[0].transform.localPosition;
        //       Vector3 q = IInventory[1].transform.localPosition;
        //       Vector3 c = IInventory[2].transform.localPosition;
        //
        //       for (int i = 0; i < 5; i++)
        //       {
        //           for (int j = 0; j < 6; j++)
        //           {
        //               file.WriteLine((f + new Vector3(j * 105, i * -111, 0)).ToString());
        //           }
        //       }
        //
        //       for (int i = 0; i < 5; i++)
        //       {
        //           file.WriteLine((q + new Vector3(i * 106, 0, 0)).ToString());
        //       }
        //
        //
        //       for (int i = 0; i < 5; i++)
        //       {
        //           for (int j = 0; j < 6; j++)
        //           {
        //               file.WriteLine((c + new Vector3(j * 105, i * -111, 0)).ToString());
        //           }
        //       }
        //   }
        string[] lines = positions.text.Split('\n');
        for (int i = 0; i < 65; i++)
        {
            Vector3 inv = getVector3(lines[i]);
            LocalPositions[i] = inv;
            inv = i < 35 ? InventoryPosition.TransformPoint(inv) : OtherInventoryPosition.TransformPoint(inv);
            Rects[i] = new Rect(inv, new Vector2(100 * Ratio.x, 100 * Ratio.y));
        }

        MakeNewItem(30, 2, 1, ITEMPARENT.PLAYER);
        MakeNewItem(31, 5, 1, ITEMPARENT.PLAYER);
        MakeNewItem(32, 9, 2, ITEMPARENT.PLAYER);
        MakeNewItem(0, 3, 1, ITEMPARENT.PLAYER);
        MakeNewItem(1, 13, 2, ITEMPARENT.PLAYER);
        MakeNewItem(2, 25, 1, ITEMPARENT.PLAYER);
        MakeNewItem(3, 26, 1, ITEMPARENT.PLAYER);
        MakeNewItem(4, 27, 1, ITEMPARENT.PLAYER);
        MakeNewItem(5, 1, 1, ITEMPARENT.PLAYER);
        MakeNewItem(26, 17, 3, ITEMPARENT.PLAYER);
        MakeNewItem(27, 18, 1, ITEMPARENT.PLAYER);
        MakeNewItem(28, 20, 1, ITEMPARENT.PLAYER);
        MakeNewItem(29, 24, 1, ITEMPARENT.PLAYER);

        isOpenInfo = true;
    }

    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<ThirdPersonCamera>().targetPlayer;
            return;
        }
        PlayerModel.SetActive(otherinventory == null && UIInventory.activeSelf);
        if (SelectedObject != null)
        {
            SelectedObject.transform.position = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                int num = getNumofMouse();
                Item inum = getItemByNum(num);
                if (inum != null && CheckTypeofItem(num, SelectedObject.id))
                {
                    if (inum.id == SelectedObject.id && inum.num != SelectedObject.num)
                    {
                        SelectedObject.QTY += inum.QTY;
                        DestroyItem(inum, inum.QTY);
                    }
                    else
                    {
                        inum.num = SelectedObject.num;
                        UpdatePosition(inum);
                    }
                    SelectedObject.num = num;
                    UpdatePosition(SelectedObject);
                    SelectedObject = null;
                }
                else if (num != -1 && CheckTypeofItem(num, SelectedObject.id))
                {
                    SelectedObject.num = num;

                    UpdatePosition(SelectedObject);

                    SelectedObject = null;
                }
            }
        }
        else if (Input.GetMouseButtonDown(0) && UIInventory.activeSelf)
        {
            SelectedObject = getItemByNum(getNumofMouse());
            if (SelectedObject != null && SelectedObject.id == -1)
                SelectedObject = null;
        }

        UpdateInfo();
    }

    bool CheckTypeofItem(int num, int id)
    {
        if (!(num >= 30 && num < 35))
            return true;

        if (player.PlayerType != Player.PLAYERTYPE.MARKSMAN && (id == 1 || id == 4))
            return false;

        if (player.PlayerType != Player.PLAYERTYPE.REPAIR && (ItemDatabase.instance.Items[id].itemType == ItemDatabase.ItemType.RepairTool || ItemDatabase.instance.Items[id].itemType == ItemDatabase.ItemType.RepairMaterial))
            return false;

        return true;
    }

    public void UpdatePosition(Item item)
    {
        if (item.num < 35)
            item.transform.SetParent(PlayerInventory.transform);
        else
            item.transform.SetParent(OtherInventory.transform);
        item.transform.localPosition = LocalPositions[item.num];
    }

    //------------------------- info

    void OpenInfo()
    {
        Information.SetActive(true);
    }

    void UpdateInfo()
    {
        if (getNumofMouse() != -1
            && getItemByNum(getNumofMouse()) != null
            && getItemByNum(getNumofMouse()).id != -1
            && SelectedObject == null
            && isOpenInfo
            && UIInventory.activeSelf)
        {
            int n = getItemByNum(getNumofMouse()).id;
            OpenInfo();
            infoImage.sprite = ItemDatabase.instance.Items[n].sprite;
            infoName.text = ItemDatabase.instance.Items[n].name;
            infoInfo.text = ItemDatabase.instance.Items[n].info;
            int i = (Input.mousePosition.y > (Screen.height / 2.0f)) ? -1 : 1;
            Information.transform.position = Input.mousePosition + new Vector3(100 * Ratio.x, 175 * Ratio.y * i, 0);
        }
        else
        {
            CloseInfo();
        }
    }

    void CloseInfo()
    {
        Information.SetActive(false);
    }

    //------------------------------

    public void setOtherInventory(OtherInventory oi)
    {
        otherinventory = oi;
        MaxIdx = otherinventory == null ? 35 : 65;
        if (otherinventory != null)
        {
            for (int i = 35; i < 65; i++)
            {
                int itn = otherinventory.Items[i - 35];
                if (itn != -1)
                {
                    MakeNewItem(i, itn, 1, ITEMPARENT.OTHER);
                }
            }
        }
    }

    public int[] getOtherInventory()
    {
        int[] retn = new int[30];

        for (int i = 0; i < 30; i++)
        {
            Item nitem = getItemByNum(i + 35);
            if (nitem)
                retn[i] = nitem.id;
            else
                retn[i] = -1;
        }

        return retn;
    }

    //-------------------------------util

    public int getEmptySmallNum()
    {
        for (int i = 0; i < 35; i++)
        {
            if (getItemByNum(i) == null)
                return i;
        }

        return -1;
    }

    public int getQTYofId(int id)
    {
        Item item = getItemById(id);
        if (item != null)
        {
            return item.QTY;
        }
        return 0;
    }

    public void MakeNewItem(int num, int id, int qty, ITEMPARENT parent)
    {
        Item idItem = getItemById(id);
        bool flag = false;
        if (idItem != null)
            flag = idItem.transform.parent != (parent == ITEMPARENT.PLAYER ? PlayerInventory.transform : OtherInventory.transform);
        if ((idItem == null || flag) && getItemByNum(num) == null)
        {
            Item ite = Instantiate(ItemPrefab).GetComponent<Item>();
            ite.QTY = qty;
            ite.num = num;
            ite.id = id;
            ite.transform.SetParent(parent == ITEMPARENT.PLAYER ? PlayerInventory.transform : OtherInventory.transform);
            ite.transform.localScale = new Vector3(1, 1, 1);
            UpdatePosition(ite);
            IInventory.Add(ite);
        }
        else if (idItem)
        {
            if (idItem.QTY > 0)
                idItem.QTY++;
        }

    }

    public void DestroyItem(Item ditem, int qty)
    {
        if (ditem == null)
            return;
        ditem.QTY -= qty;

        if (ditem.QTY <= 0)
        {
            Destroy(ditem.gameObject);
            IInventory.Remove(ditem);
        }
    }

    public Item getItemByNum(int num)
    {
        for (int i = 0; i < IInventory.Count; i++)
        {
            if (IInventory[i] != null)
            {
                if (IInventory[i].num == num)
                    return IInventory[i];
            }
        }

        return null;
    }

    public Item getItemById(int id)
    {
        for (int i = 0; i < IInventory.Count; i++)
        {
            if (IInventory[i] != null)
            {
                if (IInventory[i].id == id)
                    return IInventory[i];
            }
        }

        return null;
    }

    int getNumofMouse()
    {
        int n = 0;
        MaxIdx = otherinventory == null ? 35 : 65;
        for (int i = 0; i < MaxIdx; i++)
        {
            if (Rects[i].Contains(Input.mousePosition + new Vector3(50 * Ratio.x, 50 * Ratio.y, 0)))
            {
                return n;
            }
            n++;
        }
        return -1;
    }

    public Vector3 getVector3(string rString)
    {
        var sstring = rString.Split(',', ' ', '(', ')');

        return new Vector3(float.Parse(sstring[1]), float.Parse(sstring[3]), float.Parse(sstring[5]));
    }
}
