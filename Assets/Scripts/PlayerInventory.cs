using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory UIInventory;

    void Start()
    {
        UIInventory = FindObjectOfType<Inventory>();
    }

    void Update()
    {

    }

    public void Open()
    {
        UIInventory.UIInventory.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void OpenOtherInventory(OtherInventory oi)
    {
        UIInventory.OtherInventory.gameObject.SetActive(true);
        UIInventory.setOtherInventory(oi);
    }

    public void Close()
    {
        UIInventory.UIInventory.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void CloseOtherInventory()
    {
        UIInventory.OtherInventory.gameObject.SetActive(false);
        UIInventory.otherinventory = null;
    }

    public void DestroyAllOtherInventory()
    {
        for (int i = 1; i < UIInventory.OtherInventory.transform.childCount; i++)
        {
            GameObject target = UIInventory.OtherInventory.transform.GetChild(i).gameObject;
            Destroy(target);
            UIInventory.IInventory.Remove(target.GetComponent<Item>());
        }
    }
}
