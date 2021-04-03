using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsAndAchievements
{
    //Game Data
    private static List<MapData> mapData = new List<MapData>();
    private static List<CharacterData> characterData = new List<CharacterData>();
    private static StatsData statsData = new StatsData();
    private static SettingsData settingsData;

    public static int Coins { get { return statsData.coins; } set { statsData.coins = value; } }

    #region Data Management
    public static void LoadMapData(uint ID, bool isUnlocked, uint bestDistance)
    {
        mapData.Add(new MapData(ID, isUnlocked, bestDistance));
        GameManager.Instance.menuManager.UpdateMap(ID);
    }

    public static MapData GetMapData(uint ID) => mapData.Count > ID ? mapData[(int)ID] : new MapData(uint.MaxValue, false, 0);
    public static void LoadCharacterData(uint ID, bool isUnlocked)
    {
        characterData.Add(new CharacterData(ID, isUnlocked));
        GameManager.Instance.menuManager.UpdateCharacter(ID);
    }
    public static CharacterData GetCharacterData(uint ID) => characterData.Count > ID ? characterData[(int)ID] : new CharacterData(uint.MaxValue, false);
    public static void LoadSettingsData(bool isMusicMuted, bool isSFXMuted)
    {
        settingsData = new SettingsData(isMusicMuted, isSFXMuted);
    }

    public static SettingsData GetSettingsData() => settingsData;
    public static StatsData GetStatsData() => statsData;
    public static void LoadStatsData(int coins)
    {
        statsData = new StatsData(coins);
    }

    public static SaveData GetSaveData()
    {
        MapData[] tempMapData = new MapData[mapData.Count];

        for (uint i = 0; i < mapData.Count; i++)
        {
            var tempData = GetMapData(i);
            tempMapData[i] = new MapData(tempData);
        }

        CharacterData[] tempCharacterData = new CharacterData[characterData.Count];

        for (uint i = 0; i < characterData.Count; i++)
        {
            var tempData = GetCharacterData(i);
            tempCharacterData[i] = new CharacterData(tempData);
        }

        return new SaveData(
            tempMapData,
            tempCharacterData,
            GetSettingsData(),
            GetStatsData());
    }
    #endregion

    #region Coin Modifying
    public static bool Purchase(int purchasePrice)
    {
        if (Coins >= purchasePrice)
        {
            Coins -= purchasePrice;
            return true;
        }
        return false;
    }
    #endregion

    #region Changing Data
    public static void UnlockMap(int ID)
    {
        if (ID >= mapData.Count) return;
        
        mapData[ID] = new MapData(mapData[ID].ID, true, mapData[ID].bestDistance);
    }

    public static void UnlockCharacter(int ID)
    {
        if (ID >= characterData.Count) return;

        characterData[ID] = new CharacterData(characterData[ID].ID, true);
    }
    #endregion
}
