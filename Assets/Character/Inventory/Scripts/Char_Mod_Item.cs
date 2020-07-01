using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class Char_Mod_Item 
{
    public string itemName;
    public enum ItemType { Weapon, Armor, Currency, Ammunition, Consumable}
    public ItemType itemType;

    public GameObject itemModel;
    public Sprite itemSprite;

    public int value;
    

    public int amount;
    public bool isStackable;
    public int maxStackSize;


    public Char_Mod_Item(string itemName, ItemType itemType, int value, int amount, int maxStackSize, bool isStackable = false, GameObject itemModel = null)
    {
        this.itemName = itemName;
        this.itemType = itemType;
        this.itemModel = itemModel;
        this.value = value;
        this.amount = amount;
        this.isStackable = isStackable;
        this.maxStackSize = maxStackSize;

    }

    public Sprite GetSprite()
    {
        if(itemSprite == null)
        {
            //Texture2D itemTex = AssetPreview.GetAssetPreview(itemModel);
            //if(itemTex != null)
            //{
            //    Sprite sprite = Sprite.Create(itemTex, new Rect(0, 0, itemTex.width, itemTex.height), new Vector2(0.5f,0.5f));
            //    itemSprite = sprite;
            //}
        }
        return itemSprite;
    }

}
