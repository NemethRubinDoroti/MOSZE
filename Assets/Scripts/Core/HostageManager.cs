using UnityEngine;

public class HostageManager : MonoBehaviour
{
    public static HostageManager Instance { get; private set; }

    private int totalHostages = 0;
    private int collectedHostages = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Túszok számlálásához
    public void InitializeHostageCount(int count)
    {
        totalHostages = count;
        collectedHostages = 0;
    }

    public void CollectHostage(Hostage2D hostage)
    {
        if (hostage == null) return;
        
        collectedHostages++;
        
        if (UIManager.Instance != null && UIManager.Instance.inGameHUD != null)
        {
            UIManager.Instance.inGameHUD.UpdateHostageCount(collectedHostages, totalHostages);
        }
        
        Debug.Log($"[HostageManager] Túsz megmentve! ({collectedHostages}/{totalHostages})");
        
        // Ha minden túsz meg van mentve, spawnoljuk a boss-t
        if (AreAllHostagesCollected())
        {
            if (GameManager2D.Instance != null && GameManager2D.Instance.mapGenerator != null)
            {
                GameManager2D.Instance.mapGenerator.SpawnBossIfAllHostagesRescued();
            }
        }
    }

    public int GetTotalHostages()
    {
        return totalHostages;
    }

    public int GetCollectedHostages()
    {
        return collectedHostages;
    }

    public bool AreAllHostagesCollected()
    {
        return collectedHostages >= totalHostages && totalHostages > 0;
    }
}

