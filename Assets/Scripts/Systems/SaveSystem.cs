using System;
using System.Collections.Generic;
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
    public int level = 1;
    public int currentXP = 0;
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
    public MapExportData fullMapData;
}

// Teljes pálya export/import adatstruktúrák
[System.Serializable]
public class MapExportData
{
    public int seed;
    public int width;
    public int height;
    public List<RoomData> rooms;
    public List<CorridorData> corridors;
    public List<EnemySpawnData> enemies;
    public List<HostageSpawnData> hostages;
    public List<ItemSpawnData> items;
}

[System.Serializable]
public class RoomData
{
    public Vector2Int position;
    public int width;
    public int height;
    public int roomType; // Room.RoomType enum értéke
    public List<Vector2Int> doors;
}

[System.Serializable]
public class CorridorData
{
    public Vector2Int start;
    public Vector2Int end;
    public List<Vector2Int> path;
}

[System.Serializable]
public class EnemySpawnData
{
    public Vector2Int position;
    public int enemyType; // Enemy2D.EnemyType enum értéke
}

[System.Serializable]
public class HostageSpawnData
{
    public Vector2Int position;
}

[System.Serializable]
public class ItemSpawnData
{
    public Vector2Int position;
    public int itemType; // Item2D.ItemType enum értéke
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
    public static SaveSystem Instance { get; private set; }
    
    private string savePath;
    private const string SAVE_FOLDER = "Saves";
    
    private void Awake()
    {
        Debug.Log("[SaveSystem] Awake() meghívva");
        
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[SaveSystem] Instance beállítva");
        }
        else
        {
            Debug.LogWarning("[SaveSystem] Instance már létezik, GameObject törlése");
            Destroy(gameObject);
            return;
        }
        
        string persistentPath = Application.persistentDataPath;
        Debug.Log($"[SaveSystem] Application.persistentDataPath: {persistentPath}");
        
        savePath = Path.Combine(persistentPath, SAVE_FOLDER);
        Debug.Log($"[SaveSystem] Save mappa útvonal: {savePath}");
        
        if (!Directory.Exists(savePath))
        {
            Debug.Log("[SaveSystem] Save mappa nem létezik, létrehozás...");
            Directory.CreateDirectory(savePath);
            Debug.Log($"[SaveSystem] Save mappa létrehozva: {Directory.Exists(savePath)}");
        }
        else
        {
            Debug.Log("[SaveSystem] Save mappa már létezik");
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
    
    // Pálya exportálása JSON fájlba
    public void ExportMapToJSON(MapExportData mapData, string fileName = "map_export")
    {
        Debug.Log($"[SaveSystem] ExportMapToJSON hívva: fileName={fileName}");
        
        if (mapData == null)
        {
            Debug.LogError("[SaveSystem] ExportMapToJSON: mapData == NULL! Nem lehet exportálni.");
            return;
        }
        
        try
        {
            string persistentPath = Application.persistentDataPath;
            Debug.Log($"[SaveSystem] Application.persistentDataPath: {persistentPath}");
            
            string mapPath = Path.Combine(persistentPath, "Maps");
            Debug.Log($"[SaveSystem] Map mappa útvonal: {mapPath}");
            
            if (!Directory.Exists(mapPath))
            {
                Debug.Log($"[SaveSystem] Maps mappa nem létezik, létrehozás...");
                Directory.CreateDirectory(mapPath);
                Debug.Log($"[SaveSystem] Maps mappa létrehozva: {Directory.Exists(mapPath)}");
            }
            else
            {
                Debug.Log($"[SaveSystem] Maps mappa már létezik");
            }
            
            string json = JsonUtility.ToJson(mapData, true);
            Debug.Log($"[SaveSystem] JSON generálva, hossz: {json.Length} karakter");
            
            string filePath = Path.Combine(mapPath, $"{fileName}.json");
            Debug.Log($"[SaveSystem] Fájl útvonal: {filePath}");
            
            File.WriteAllText(filePath, json);
            Debug.Log($"[SaveSystem] Fájl írva: {filePath}");
            
            // Ellenőrizzük, hogy a fájl tényleg létrejött-e
            if (File.Exists(filePath))
            {
                Debug.Log($"[SaveSystem] Pálya sikeresen exportálva: {filePath}");
                FileInfo fileInfo = new FileInfo(filePath);
            }
            else
            {
                Debug.LogError($"[SaveSystem] HIBA: A fájl nem jött létre: {filePath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] HIBA a pálya exportálásakor: {e.Message}");
            Debug.LogError($"[SaveSystem] Stack trace: {e.StackTrace}");
        }
    }
    
    // Pálya importálása JSON fájlból
    public MapExportData ImportMapFromJSON(string fileName = "map_export")
    {
        try
        {
            string mapPath = Path.Combine(Application.persistentDataPath, "Maps");
            string filePath = Path.Combine(mapPath, $"{fileName}.json");
            
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"Pálya fájl nem található: {filePath}");
                return null;
            }
            
            string json = File.ReadAllText(filePath);
            MapExportData data = JsonUtility.FromJson<MapExportData>(json);
            
            Debug.Log($"Pálya importálva: {filePath}");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Hiba a pálya importálásakor: {e.Message}");
            return null;
        }
    }
    
    // Pálya fájlok listázása
    public string[] GetMapList()
    {
        try
        {
            string mapPath = Path.Combine(Application.persistentDataPath, "Maps");
            if (!Directory.Exists(mapPath))
            {
                return new string[0];
            }
            
            string[] files = Directory.GetFiles(mapPath, "*.json");
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            return files;
        }
        catch (Exception e)
        {
            Debug.LogError($"Hiba a pálya lista lekérdezésénél: {e.Message}");
            return new string[0];
        }
    }
}
