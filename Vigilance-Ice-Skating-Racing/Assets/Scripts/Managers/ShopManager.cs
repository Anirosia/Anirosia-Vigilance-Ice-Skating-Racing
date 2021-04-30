using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public ShopInformation[] shopsInformation;
    public GameObject menuShopItemPrefab;
    
    private void Start()
    {
        InitializeShop();
    }

    public void InitializeShop()
    {
        for (int shopID = 0; shopID < shopsInformation.Length; shopID++)
        {
            LoadData(shopID);
            PopulatePanel(shopID);
        }
    }
    
    public bool Purchase(int shopID, int itemID)
    {
        if (StatsAndAchievements.CanPurchase(shopsInformation[shopID].shopItems[itemID].cost))
        {
            if (shopID == 0)
            {
                StatsAndAchievements.UnlockMap(itemID);
            }
            else
            {
                StatsAndAchievements.UnlockCharacter(itemID);
            }
            
            return true;
        }

        return false;
    }

    //Adds all the items to the shop
    //TODO
    //Add more settings on what is show, IE only reveal if you have 1/2 the cost of the upgrade, or you have another upgrade
    private void PopulatePanel(int id)
    {
        shopsInformation[id].shopGameObjects = new GameObject[shopsInformation[id].shopItems.Length];
        for (int i = 0; i < shopsInformation[id].shopItems.Length; i++)
        {
            shopsInformation[id].shopGameObjects[i] = Instantiate(menuShopItemPrefab,Vector3.zero, Quaternion.identity, shopsInformation[id].containerGameObject.transform);
            ShopItem.CreateComponent(shopsInformation[id].shopGameObjects[i], id, shopsInformation[id].shopItems[i]);
        }
    }

    //Expandable and requires no inspector references for adding new characters
    private void LoadData(int id)
    {
        //Loads Data from Resources Folder
        var loadedData = Resources.LoadAll<ShopItemData>("ShopItems/" + shopsInformation[id].filePath);
        //Sets Shop items array to proper size
        shopsInformation[id].shopItems = new ShopItemData[loadedData.Length];
        
        foreach (var shopData in loadedData)
        {
            shopsInformation[id].shopItems[shopData.id] = shopData;
        }
    }
    
    // private void LoadData(ShopInformation shopInfo)
    // {
    //     var loadedData = Resources.LoadAll<ShopItemData>("ShopItems/" + shopInfo.filePath);
    //     shopInfo.shopItems = new ShopItemData[loadedData.Length];
    //     foreach (var shopData in loadedData)
    //     {
    //         shopInfo.shopItems[shopData.id] = shopData;
    //     }
    // }
}

[Serializable]
public struct ShopInformation
{
    [HideInInspector] public ShopItemData[] shopItems;
    [HideInInspector] public GameObject[] shopGameObjects;
    public GameObject containerGameObject;
    public string filePath;
}
