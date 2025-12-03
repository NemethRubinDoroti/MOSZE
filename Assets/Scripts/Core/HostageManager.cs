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

    // Ha a játékos megmenti
    public void CollectHostage(Hostage2D hostage)
    {
        if (hostage == null) return;

        collectedHostages++;

        Debug.Log($"[HostageManager] Túsz megmentve! ({collectedHostages}/{totalHostages})");
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

