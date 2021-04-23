using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ShopItemData")]
public class ShopItemData : ScriptableObject
{
    [Header("Assignables")]
    public Sprite lockedTexture2D; 
    public Sprite unLockedTexture2D;
    
    [Header("Settings")]
    public bool isUnlockedFromStart;
    public bool isShowBasedOnPrice;
    public float priceDividerToShowInShop;
    
    [Header("Stats")]
    public int id;
    public int cost;
}
