using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    private Text costText;
    private Image itemImage;
    private int shopID;
    private ShopItemData itemData;
    
   //Creating a Static method for adding a component to a GameObject with parameters following the Factory Pattern
    public static ShopItem CreateComponent (GameObject target, int shopID, ShopItemData newItemData) {
       ShopItem newComponent = target.AddComponent<ShopItem>();
       newComponent.itemData = newItemData;
       newComponent.shopID = shopID;
       newComponent.costText = target.GetComponentInChildren<Text>();
       newComponent.itemImage = target.GetComponentInChildren<Image>();
       newComponent.Initialize();
       target.GetComponent<Button>().onClick.AddListener(newComponent.OnClicked);
       return newComponent;
    }

    //Causes Physical Pain
    //Hard Coded But will be changed at a later data... deadlines
    //TODO
    //Make StatsAndAchievements.cs Generic 
    //Deep Clean this monstrosity
    private void Initialize()
    {
        if (itemData.isUnlockedFromStart)
        {
            Unlock();
            return;
        }
        
        bool isUnlocked = false;
        int bestDist = -1;
        
        if (shopID == 0)
        {
            MapData data =  StatsAndAchievements.GetMapData((uint)itemData.id);
            isUnlocked = data.isUnlocked;
            bestDist = (int)data.bestDistance;
        }
        else
        {
            CharacterData data = StatsAndAchievements.GetCharacterData((uint) itemData.id);
            isUnlocked = data.isUnlocked;
        }
        costText.text = "$" + itemData.cost;
        itemImage.sprite = isUnlocked? itemData.unLockedTexture2D : itemData.lockedTexture2D;
        //if (bestDist != -1)
    }

    public void Unlock()
    {
        costText.text = "Unlocked";
        itemImage.sprite = itemData.unLockedTexture2D;
    }

    private void OnClicked()
    {
        if (shopID == 0)
        {
            if (StatsAndAchievements.GetMapData((uint)itemData.id).isUnlocked)
            {
                //TODO
                //LoadMap
            }
            else
            {
                if(GameManager.Instance.shopManager.Purchase(shopID, itemData.id))
                    Unlock();
            }
        }
        else
        {
            if (StatsAndAchievements.GetCharacterData((uint)itemData.id).isUnlocked)
            {
                //TODO
                //Select Character
            }
            else
            {
                if(GameManager.Instance.shopManager.Purchase(shopID, itemData.id))
                    Unlock();
            }
        }
        
    }
}


