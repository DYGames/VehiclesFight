using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int QTY
    {
        get
        {
            return qty;
        }
        set
        {
            qty = value;
            setQTYText();
        }
    }
    public int num;
    public int id
    {
        get
        {
            return Id;
        }
        set
        {
            Id = value;
            UpdateImage();
        }
    }
    int Id;
    int qty;
    public bool isUseAble;
    public UnityEngine.UI.Image image;
    public Sprite uiimage;
    public UnityEngine.UI.Text QTYText;

    public Item()
    {
        isUseAble = false;
    }

    void setQTYText()
    {
        QTYText.text = qty.ToString();
    }


    public void UpdateImage()
    {
        if (Id == -1)
            image.sprite = null;
        else
            image.sprite = ItemDatabase.instance.Items[Id].sprite;
    }
}
