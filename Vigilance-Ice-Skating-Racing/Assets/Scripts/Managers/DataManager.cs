using UnityEngine;
using System.IO;

public class DataManager
{
    private const string mapsFilePath = "/maps.txt";
    private const string charactersFilePath = "/characters.txt";
    private const string statsFilePath = "/stats.txt";
    private const string settingsFilePath = "/settings.txt";

    public static void SaveData()
    {
        SaveData data = StatsAndAchievements.GetSaveData();
        WriteToFile(mapsFilePath, data.mapInfo);
        WriteToFile(charactersFilePath, data.characterInfo);
        WriteToFile(statsFilePath, data.statsInfo);
        WriteToFile(settingsFilePath, data.settingsInfo);
    }
    public static void ResetData()
    {
        DeleteFile(mapsFilePath);
        DeleteFile(charactersFilePath);
        DeleteFile(statsFilePath);
        DeleteFile(settingsFilePath);
    }
    
    private static void DeleteFile(string filePath)
    {
        if (File.Exists(Application.persistentDataPath + filePath))
        {
            File.Delete(Application.persistentDataPath + filePath);
        }
    }
    private static void WriteToFile(string filePath, string stringToWrite)
    {
        StreamWriter sw = new StreamWriter(Application.persistentDataPath + filePath, false);

        sw.Write(stringToWrite);

        sw.Close();
    }
    public static bool LoadData()
    {
        //Checks if the map file exits
        if (File.Exists(Application.persistentDataPath + mapsFilePath))
        {
            //Reads the whole file
            var lines = File.ReadAllLines(Application.persistentDataPath + mapsFilePath);

            //Iterates thru all of the lines in the file
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                //If the line is empty, break out of the loop
                if (line == "") break;

                //Splits the line 
                var splitLine = line.Split(';');

                //Sets the stats for the Map
                //GameManager.Instance.SetMapStats(uint.Parse(temp[0]), int.Parse(temp[1]) == 1, uint.Parse(temp[2]));
                StatsAndAchievements.LoadMapData(uint.Parse(splitLine[0]), int.Parse(splitLine[1]) == 1, uint.Parse(splitLine[2]));
            }
        }
        else
        {
            InitializeMap();
        }
        
        //Checks if the map file exits
        if (File.Exists(Application.persistentDataPath + charactersFilePath))
        {
            //Reads the whole file
            var lines = File.ReadAllLines(Application.persistentDataPath + charactersFilePath);

            //Iterates thru all of the lines in the file
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                //If the line is empty, break out of the loop
                if (line == "") break;

                //Splits the line 
                var splitLine = line.Split(';');

                StatsAndAchievements.LoadCharacterData(uint.Parse(splitLine[0]), int.Parse(splitLine[1]) == 1);
            }
        }
        else
        {
            InitializeCharacter();
        }

        if (File.Exists(Application.persistentDataPath + statsFilePath))
        {
            var lines = File.ReadAllText(Application.persistentDataPath + statsFilePath);

            var splitLine = lines.Split(';');

            StatsAndAchievements.LoadStatsData(int.Parse(splitLine[0]));

        }

        if (File.Exists(Application.persistentDataPath + settingsFilePath))
        {
            var lines = File.ReadAllText(Application.persistentDataPath + settingsFilePath);

            var splitLine = lines.Split(';');

            //TODO
            //Set variables on AudioManager
            //GameManager.Instance.menuManager.SetAudioSettings(int.Parse(splitLine[0]) == 1, int.Parse(splitLine[1]) == 1);
        }

        return true;
    }

    private static void InitializeMap()
    {
        StatsAndAchievements.LoadMapData(0, true, 0);
        StatsAndAchievements.LoadMapData(1, false, 0);
        StatsAndAchievements.LoadMapData(2, false, 0);
    }
    private static void InitializeCharacter()
    {
        StatsAndAchievements.LoadCharacterData(0, true);
        StatsAndAchievements.LoadCharacterData(1, false);
        StatsAndAchievements.LoadCharacterData(2, false);
    }
}
#region Structs
public struct SaveData
{
    public string mapInfo;
    public string characterInfo;
    public string settingsInfo;
    public string statsInfo;

    public SaveData(MapData[] mapData, CharacterData[] characterData, SettingsData settingsData, StatsData statsData)
    {   
        mapInfo = "";
        foreach (var map in mapData)
            mapInfo += map.GetSaveInfo() + System.Environment.NewLine;

        characterInfo = "";
        foreach (var character in characterData)
            characterInfo += character.GetSaveInfo() + System.Environment.NewLine;
        settingsInfo = settingsData.GetSaveInfo();
        statsInfo = statsData.GetSaveInfo();
    }
}
public struct MapData
{
    public uint ID;
    public bool isUnlocked;
    public uint bestDistance;
    
    public MapData(uint ID, bool isUnlocked, uint bestDistance)
    {
        this.ID = ID;
        this.isUnlocked = isUnlocked;
        this.bestDistance = bestDistance;
    }
    public MapData(MapData mapData)
    {
        this.ID = mapData.ID;
        this.isUnlocked = mapData.isUnlocked;
        this.bestDistance = mapData.bestDistance;
    }

    public string GetSaveInfo()
    {
        return ID + ";" + (isUnlocked ? 1 : 0) + ";" + bestDistance;
    }
}
public struct CharacterData
{
    public uint ID;
    public bool isUnlocked;

    public CharacterData(uint ID, bool isUnlocked)
    {
        this.ID = ID;
        this.isUnlocked = isUnlocked;
    }
    public CharacterData(CharacterData characterData)
    {
        this.ID = characterData.ID;
        this.isUnlocked = characterData.isUnlocked;
    }

    public string GetSaveInfo()
    {
        return ID + ";" + (isUnlocked ? 1 : 0);
    }
}
public struct SettingsData
{
    public bool isMusicMuted;
    public bool isSFXMuted;

    public SettingsData(bool isMusicMuted, bool isSFXMuted)
    {
        this.isMusicMuted = isMusicMuted;
        this.isSFXMuted = isSFXMuted;
    }
    public string GetSaveInfo()
    {
        return (isMusicMuted ? 1 : 0) + ";" + (isSFXMuted ? 1 : 0);
    }
}
public struct StatsData
{
    public int coins;

    public StatsData(int coins)
    {
        this.coins = coins;
    }
    public string GetSaveInfo()
    {
        return coins.ToString();
    }
}
#endregion