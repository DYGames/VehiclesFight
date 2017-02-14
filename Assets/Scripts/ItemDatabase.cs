using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : Singleton<ItemDatabase>
{
    public enum ItemType
    {
        None,
        RepairTool,
        Weapon,
        Food,
        RepairMaterial,
        Material,
        MaterialTool,
        ClearItem,
    }

    [Serializable]
    public struct ItemDic
    {
        public int id;
        public Sprite sprite;
        public string name;
        public string info;
        public ItemType itemType;
    }

    public ItemDic[] tempItemDic;
    public Dictionary<int, ItemDic> Items;

    void Awake()
    {
        Items = new Dictionary<int, ItemDic>();
        foreach (var item in tempItemDic)
        {
            Items.Add(item.id, item);
        }
    }
}
