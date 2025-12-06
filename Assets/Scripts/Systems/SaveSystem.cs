using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData2D
{
    public PlayerData2D playerData;
    public MapData2D mapData;
    public GameData gameData;
}

[System.Serializable]
public class PlayerData2D
{
    public Vector2Int position;
    public int health;
    public int maxHealth;
    public StatsData stats;
}

[System.Serializable]
public class StatsData
{
    public int attack;
    public int defense;
    public int speed;
    public int accuracy;
}

[System.Serializable]
public class MapData2D
{
    public int seed;
    public int width;
    public int height;
}

[System.Serializable]
public class GameData
{
    public int score;
    public float playTime;
    public string saveTime;
}

public class SaveSystem : MonoBehaviour
{
    private string savePath;
    private const string SAVE_FOLDER = "Saves";
    
    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER);
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
    }
    
    public void SaveGame(GameData2D data, string fileName = "save_001")
    {
        try
        {
            data.gameData.saveTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string json = JsonUtility.ToJson(data, true);
            string filePath = Path.Combine(savePath, $"{fileName}.save");
            
            File.WriteAllText(filePath, json);
            Debug.Log($"Játék mentve: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Hiba a játék mentésekor: {e.Message}");
        }
    }
    
    public GameData2D LoadGame(string fileName = "save_001")
    {
        try
        {
            string filePath = Path.Combine(savePath, $"{fileName}.save");
            
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"A mentési fájl nem található: {filePath}");
                return null;
            }
            
            string json = File.ReadAllText(filePath);
            GameData2D data = JsonUtility.FromJson<GameData2D>(json);
            
            Debug.Log($"Játék betöltve: {filePath}");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Hiba a játék betöltésekor: {e.Message}");
            return null;
        }
    }
    
    public void DeleteSave(string fileName)
    {
        try
        {
            string filePath = Path.Combine(savePath, $"{fileName}.save");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"A mentési fájl törölve: {filePath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Hiba a mentési fájl törlésekor: {e.Message}");
        }
    }
    
    public string[] GetSaveList()
    {
        try
        {
            if (!Directory.Exists(savePath))
            {
                return new string[0];
            }
            
            string[] files = Directory.GetFiles(savePath, "*.save");
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            return files;
        }
        catch (Exception e)
        {
            Debug.LogError($"Hiba a mentési fájlok listázása során: {e.Message}");
            return new string[0];
        }
    }
}

