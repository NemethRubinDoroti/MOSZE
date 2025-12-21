using System.Collections.Generic;
using UnityEngine;

public class HostageSpawner : MonoBehaviour
{
    [Header("Hostage Sprite")]
    public Sprite hostageSprite;
    
    private List<Hostage2D> spawnedHostages = new List<Hostage2D>();
    
    public Hostage2D SpawnHostage(Vector2Int position)
    {
        // Dinamikusan létrehozunk egy új GameObject-et
        GameObject hostageObj = new GameObject($"Hostage_{position.x}_{position.y}");
        hostageObj.transform.position = new Vector3(position.x, position.y, 0);
        
        Hostage2D hostage = hostageObj.GetComponent<Hostage2D>();
        if (hostage == null)
        {
            hostage = hostageObj.AddComponent<Hostage2D>();
        }
        
        // Beállítjuk a pozíciót
        hostage.position = position;
        
        // Beállítjuk a sprite-ot, ha van
        if (hostageSprite != null)
        {
            hostage.hostageSprite = hostageSprite;
            if (hostage.spriteRenderer != null)
            {
                hostage.spriteRenderer.sprite = hostageSprite;
            }
        }
        
        spawnedHostages.Add(hostage);
        
        return hostage;
    }
    
    public void ClearAllHostages()
    {
        foreach (Hostage2D hostage in spawnedHostages)
        {
            if (hostage != null && hostage.gameObject != null)
            {
                Destroy(hostage.gameObject);
            }
        }
        spawnedHostages.Clear();
    }
    
    public List<Hostage2D> GetSpawnedHostages()
    {
        // Töröljük a null referenciákat
        spawnedHostages.RemoveAll(h => h == null);
        return spawnedHostages;
    }
    
    public int GetHostageCount()
    {
        return GetSpawnedHostages().Count;
    }
}

