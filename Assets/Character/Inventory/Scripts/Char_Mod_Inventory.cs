using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Char_Mod_Inventory
{

    public event EventHandler OnItemListChanged;

    public List<Char_Mod_Item> itemList;

    public Char_Mod_Inventory()
    {
        this.itemList = new List<Char_Mod_Item>();

        //AddItem(Char_Mod_ItemAssets.Instance.itemList[0]);
        //AddItem(new Char_Mod_Item { itemName = "bar", itemType = Char_Mod_Item.ItemType.Armor, amount = 1, value = 10 });
        //AddItem(new Char_Mod_Item { itemName = "Foobar", itemType = Char_Mod_Item.ItemType.Consumable, amount = 1, value = 10 });
        //AddItem(new Char_Mod_Item { itemName = "Foobar", itemType = Char_Mod_Item.ItemType.Consumable, amount = 1, value = 10 });
        //AddItem(new Char_Mod_Item { itemName = "Foobar", itemType = Char_Mod_Item.ItemType.Consumable, amount = 1, value = 10 });

        //Debug.Log(itemList.Count);
    }

    public void AddItem(Char_Mod_Item item)
    {
        itemList.Add(item);
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public List<Char_Mod_Item> GetItemList()
    {
        return itemList;
    }

}
