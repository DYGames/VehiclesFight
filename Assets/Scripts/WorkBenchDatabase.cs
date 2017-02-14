using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkBenchDatabase : Singleton<WorkBenchDatabase>
{
    [System.Serializable]
    public struct ItemidAndQTY
    {
        public int id;
        public int QTY;
    }

    [System.Serializable]
    public struct WorkBenchSetDic
    {
        public int ReturnItemId;
        public List<ItemidAndQTY> NeedItem;
    }

    public List<WorkBenchSetDic> WorkBenchSetList;

    public int FindIdxById(int id)
    {
        for (int i = 0; i < WorkBenchSetList.Count; i++)
        {
            if (WorkBenchSetList[i].ReturnItemId == id)
                return i;
        }
        return -1;
    }

}