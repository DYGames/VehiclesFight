using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMng : MonoBehaviour
{
    PlayerStatus Players;
    Player Playerp;
    Inventory Inventory;

    public Image HPBar;
    public Text Money;
    public Text CreepScore;
    public Image[] QuickItems;
    
    void Start()
    {
        Playerp = transform.parent.GetComponent<Player>();
        Players = transform.parent.GetComponent<PlayerStatus>();
        Inventory = transform.parent.GetComponent<PlayerInventory>().UIInventory;
    }

    void SetHPBar(float curhp, float maxhp)
    {
        HPBar.fillAmount = (curhp / maxhp);
    }

    void SetMoney(int money)
    {
        Money.text = money.ToString();
    }

    void setCreepScore(int cs)
    {
        CreepScore.text = cs.ToString();
    }
    
    void Update()
    {
        if (Players != null)
        {
            SetHPBar(Players.gobject.HP, Players.gobject.MAXHP);
            SetMoney(Players.Money);
            setCreepScore(Players.CreepScore);

            for (int i = 0; i < QuickItems.Length; i++)
            {
                Item it = Inventory.getItemByNum(30 + i);
                if (it != null)
                {
                    QuickItems[i].sprite = it.image.sprite;
                    QuickItems[i].color = new Color(System.Convert.ToInt32(i != Players.EquipItem), 1, 1);
                }
                else
                    QuickItems[i].sprite = null;
            }
          //  if (Playerp.PlayerState == (Playerp.PlayerState | Player.PLAYERSTATE.WALK)
          //      || Playerp.PlayerState == (Playerp.PlayerState | Player.PLAYERSTATE.RUN))
          //      anim.Play("Shake");
          //  else
          //      anim.Stop("Shake");
        }
        else
        {
            SetHPBar(0, 100);
            SetMoney(0);
            setCreepScore(0);
        }
    }
}
