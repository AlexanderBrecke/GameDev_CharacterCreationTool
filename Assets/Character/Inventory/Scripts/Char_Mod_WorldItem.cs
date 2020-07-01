using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_Mod_WorldItem : MonoBehaviour
{

    public static Char_Mod_WorldItem SpawnWorldItem(Char_Mod_Item item, Vector3 position)
    {
        Transform transform = Instantiate(Char_Mod_ItemAssets.Instance.pfWorldItem, position, Quaternion.identity);

        Char_Mod_WorldItem worldItem = transform.GetComponent<Char_Mod_WorldItem>();
        worldItem.SetItem(item);
        return worldItem;

    }

    private Char_Mod_Item item;

    public void SetItem(Char_Mod_Item item)
    {
        this.item = item;
        Instantiate(item.itemModel, transform);
    }

    public Char_Mod_Item GetItem()
    {
        return item;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }


}
