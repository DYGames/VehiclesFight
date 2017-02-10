using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkBench : MonoBehaviour
{
    public GameObject UIWorkBench;

    public GameObject RecipeList;
    public GameObject WorkbenchItemPrefab;

    public GameObject Recipe;
    public GameObject NeedItemList;
    public GameObject WorkbenchNeedItemPrefab;

    public Button WorkButton;

    Inventory playerinvenytory;
    int CurrentSpot;

    bool MouseItemPanel = false;
    bool MouseNeelItemPanel = false;

    void Start()
    {
        CurrentSpot = 0;
        playerinvenytory = FindObjectOfType<Inventory>();

        WorkButton.onClick.AddListener(delegate { ClickWork(); });
        for (int i = 0; i < WorkBenchDatabase.instance.WorkBenchSetList.Count; i++)
        {
            WorkBenchDatabase.WorkBenchSetDic item = WorkBenchDatabase.instance.WorkBenchSetList[i];
            Transform obj = Instantiate(WorkbenchItemPrefab, RecipeList.transform).transform;
            obj.localScale = Vector3.one;
            obj.localPosition = new Vector3(0, 300, 0) - new Vector3(0, i * 150, 0);
            obj.FindChild("Item").GetComponent<Image>().sprite = ItemDatabase.instance.Items[item.ReturnItemId].sprite;
            obj.FindChild("Name").GetComponent<Text>().text = ItemDatabase.instance.Items[item.ReturnItemId].name;
            obj.GetComponent<Button>().onClick.AddListener(delegate { OnClick(item.ReturnItemId); });
        }
    }

    void Update()
    {
        if (!MouseItemPanel && !MouseNeelItemPanel)
            return;

        if (Input.mouseScrollDelta.y != 0)
        {
            Transform target = MouseItemPanel ? RecipeList.transform : NeedItemList.transform;
            float ymax = Mathf.Clamp(target.childCount - 5, 0, Mathf.Infinity) * 150;
            if (target.transform.localPosition.y <= ymax && target.transform.localPosition.y >= 0)
                target.transform.localPosition += new Vector3(0, Input.mouseScrollDelta.y * -10, 0);
            if (target.transform.localPosition.y < 0)
                target.transform.localPosition = Vector3.zero;
            if (target.transform.localPosition.y > ymax)
                target.transform.localPosition = new Vector3(0, ymax, 0);
        }
    }

    public void MouseInItemPanel()
    {
        MouseItemPanel = true;
    }

    public void MouseOutItemPanel()
    {
        MouseItemPanel = false;
    }

    public void MouseInNeedItemPanel()
    {
        MouseNeelItemPanel = true;
    }

    public void MouseOutNeedItemPanel()
    {
        MouseNeelItemPanel = false;
    }

    public void OnClick(int id)
    {
        DestroyAllNeedItem();
        NeedItemList.transform.localPosition = Vector3.zero;

        int idx = WorkBenchDatabase.instance.FindIdxById(id);
        CurrentSpot = idx;
        WorkBenchDatabase.WorkBenchSetDic item = WorkBenchDatabase.instance.WorkBenchSetList[CurrentSpot];

        Recipe.transform.FindChild("Item").GetComponent<Image>().sprite = ItemDatabase.instance.Items[item.ReturnItemId].sprite;
        Recipe.transform.FindChild("Name").GetComponent<Text>().text = ItemDatabase.instance.Items[item.ReturnItemId].name;

        for (int i = 0; i < item.NeedItem.Count; i++)
        {
            Transform obj = Instantiate(WorkbenchNeedItemPrefab, NeedItemList.transform).transform;
            obj.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(0, 300, 0) - new Vector3(0, i * 150, 0);
            obj.FindChild("Item").GetComponent<Image>().sprite = ItemDatabase.instance.Items[item.NeedItem[i].id].sprite;
            obj.FindChild("Name").GetComponent<Text>().text = ItemDatabase.instance.Items[item.NeedItem[i].id].name;
            obj.FindChild("QTY").GetComponent<Text>().text = playerinvenytory.getQTYofId(item.NeedItem[i].id).ToString();
            obj.FindChild("NeedQTY").GetComponent<Text>().text = item.NeedItem[i].QTY.ToString();
        }
        WorkButton.enabled = CheckWorkAble(CurrentSpot);
    }

    public void ClickWork()
    {
        WorkBenchDatabase.WorkBenchSetDic item = WorkBenchDatabase.instance.WorkBenchSetList[CurrentSpot];
        if (CheckWorkAble(CurrentSpot) && playerinvenytory.getEmptySmallNum() != -1)
        {
            for (int i = 0; i < item.NeedItem.Count; i++)
                playerinvenytory.DestroyItem(playerinvenytory.getItemById(item.NeedItem[i].id), item.NeedItem[i].QTY);
            playerinvenytory.MakeNewItem(playerinvenytory.getEmptySmallNum(), item.ReturnItemId, 1, Inventory.ITEMPARENT.PLAYER);
        }
    }

    bool CheckWorkAble(int cspot)
    {
        bool workable = true;
        WorkBenchDatabase.WorkBenchSetDic item = WorkBenchDatabase.instance.WorkBenchSetList[cspot];
        for (int i = 0; i < item.NeedItem.Count; i++)
            if (playerinvenytory.getQTYofId(item.NeedItem[i].id) < item.NeedItem[i].QTY)
                workable = false;
        return workable;
    }

    public void OpenWorkBech()
    {
        RecipeList.transform.localPosition = Vector3.zero;
        NeedItemList.transform.localPosition = Vector3.zero;
        Cursor.lockState = CursorLockMode.None;
        UIWorkBench.SetActive(true);
    }

    public void CloseWorkBech()
    {
        Cursor.lockState = CursorLockMode.Locked;
        DestroyAllNeedItem();
        UIWorkBench.SetActive(false);
    }

    void DestroyAllNeedItem()
    {
        for (int i = 0; i < NeedItemList.transform.childCount; i++)
        {
            Destroy(NeedItemList.transform.GetChild(i).gameObject);
        }
    }

}