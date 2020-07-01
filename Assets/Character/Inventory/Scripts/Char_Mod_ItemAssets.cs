using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_Mod_ItemAssets : MonoBehaviour
{
    public static Char_Mod_ItemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        foreach(Char_Mod_Item item in itemList)
        {
            if(item.itemSprite == null)
                item.GetSprite();
        }
    }

    public Transform pfWorldItem;

    public List<Char_Mod_Item> itemList = new List<Char_Mod_Item>();
}
