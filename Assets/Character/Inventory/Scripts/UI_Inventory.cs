using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{

    private Char_Mod_Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;

    private void Awake()
    {
        itemSlotContainer = transform.Find("ItemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("ItemSlotTemplate");
        //Debug.Log(itemSlotTemplate.name);
    }

    public void SetInventory(Char_Mod_Inventory inventory)
    {
        this.inventory = inventory;
        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
        //Debug.Log(inventory.GetItemList().Count);
    }

    private void Inventory_OnItemListChanged(object sender, EventArgs e)
    {
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems()
    {
        if (InventoryIsOpen())
        {
            foreach (Transform child in itemSlotContainer)
            {
                if (child == itemSlotTemplate) continue;
                Destroy(child.gameObject);
            }
            int x = 0;
            int y = 0;
            float itemSlotCellSize = 45f;
            foreach(Char_Mod_Item item in inventory.GetItemList())
            {

                RectTransform itemslotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
                itemslotRectTransform.gameObject.SetActive(true);
                itemslotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
                Image image = itemslotRectTransform.Find("Image").GetComponent<Image>();
                image.sprite = item.itemSprite;

                x++;

                if(x > 4)
                {
                    x = 0;
                    y--;
                }
            }
        }
    }

    public void OpenOrCLoseInventory(bool openOrCLose)
    {
        this.gameObject.SetActive(openOrCLose);
        RefreshInventoryItems();
    }

    public bool InventoryIsOpen()
    {
        return this.gameObject.activeSelf;
    }

}
