using UnityEngine;
using System.IO;

public class DataManager
{
    private const string mapsFilePath = "/maps.txt";
    private const string charactersFilePath = "/characters.txt";
    private const string statsFilePath = "/stats.txt";
    private const string settingsFilePath = "/settings.txt";

    public static void SaveData(SaveData data)
    {
        WriteToFile(mapsFilePath, data.mapInfo);
        WriteToFile(charactersFilePath, data.characterInfo);
        WriteToFile(statsFilePath, data.statsInfo);
        WriteToFile(settingsFilePath, data.settingsInfo);
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
                var temp = line.Split(';');

                //Sets the stats for the Map
                GameManager.Instance.SetMapStats(uint.Parse(temp[0]), int.Parse(temp[1]) == 1, uint.Parse(temp[2]));
            }
        }

        if (File.Exists(Application.persistentDataPath + statsFilePath))
        {
            var lines = File.ReadAllText(Application.persistentDataPath + statsFilePath);

            var temp = lines.Split(';');

            GameManager.Instance.SetCurrency(int.Parse(temp[0]));

        }

        if (File.Exists(Application.persistentDataPath + settingsFilePath))
        {
            var lines = File.ReadAllText(Application.persistentDataPath + settingsFilePath);

            var temp = lines.Split(';');

            GameManager.Instance.menuManager.SetAudioSettings(int.Parse(temp[0]) == 1, int.Parse(temp[1]) == 1);

        }

        return true;
    }

    private static void WriteToFile(string filePath, string tempString)
    {
        StreamWriter sw = new StreamWriter(Application.persistentDataPath + filePath, false);

        sw.Write(tempString);

        sw.Close();
    }
}
#region Structs
public struct SaveData
{
    public string mapInfo;
    public string characterInfo;
    public string settingsInfo;
    public string statsInfo;

    //private MapData[] mapData;
    //private CharacterData[] characterData;
    //private SettingsData settingsData;
    //private StatsData statsData;

    public SaveData(MapData[] mapData, CharacterData[] characterData, SettingsData settingsData, StatsData statsData)
    {   
        mapInfo = "";
        foreach (var map in mapData)
            mapInfo += map.saveInfo + System.Environment.NewLine;

        characterInfo = "";
        foreach (var character in characterData)
            characterInfo += character.saveInfo + System.Environment.NewLine;
        settingsInfo = settingsData.saveInfo + System.Environment.NewLine;
        statsInfo = statsData.saveInfo + System.Environment.NewLine;
    }
}
public struct MapData
{
    public string saveInfo;

    public uint ID;
    public bool isUnlocked;
    public uint bestDistance;
    
    public MapData(uint ID, bool isUnlocked, uint bestDistance)
    {
        saveInfo = ID + ";" + (isUnlocked? 1:0) + ";" + bestDistance;
        this.ID = ID;
        this.isUnlocked = isUnlocked;
        this.bestDistance = bestDistance;
    }
}
public struct CharacterData
{
    public string saveInfo;
    //public uint ID;
    //public bool isUnlocked;

    public CharacterData(uint ID, bool isUnlocked)
    {
        saveInfo = ID + ";" + isUnlocked;
    }
}
public struct SettingsData
{
    public string saveInfo;

    public SettingsData(bool isMusicMuted, bool isSFXMuted)
    {
        saveInfo = (isMusicMuted ? 1 : 0) + ";" + (isSFXMuted ? 1 : 0);
    }
}
public struct StatsData
{
    public string saveInfo;

    public StatsData(int coins)
    {
        saveInfo = coins.ToString();
    }
}
#endregion
