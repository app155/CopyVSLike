using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textName.text = data.itemName;
        textDesc = texts[2];
    }

    void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1);

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.dmgs[level] * 100, data.cnts[level]);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.dmgs[level] * 100);
                break;
            default:
                textDesc.text = string.Format(data.itemDesc);
                break;
        }
        
    }

    void LateUpdate()
    {
        
    }

    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0)
                {
                    GameObject newWeapon = new();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }

                else
                {
                    float nextDmg = data.baseDmg;
                    int nextCnt = 0;

                    nextDmg += data.baseDmg * data.dmgs[level];
                    nextCnt += data.cnts[level];

                    weapon.LevelUp(nextDmg, nextCnt);
                }
                level++;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if (level == 0)
                {
                    GameObject newGear = new();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }

                else
                {
                    float nextRate = data.dmgs[level];
                    gear.LevelUp(nextRate);
                }
                level++;
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.hp = GameManager.instance.maxHp;
                break;
        }

        if (level == data.dmgs.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
