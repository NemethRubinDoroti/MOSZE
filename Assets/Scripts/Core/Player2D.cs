using UnityEngine;

public class Player2D : MonoBehaviour
{
    [Header("Stats Configuration")]
    public PlayerStatsConfig statsConfig;
    
    public Vector2Int position;
    public Stats stats;
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        stats = GetComponent<Stats>();
        if (stats == null)
        {
            stats = gameObject.AddComponent<Stats>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        position = Vector2Int.RoundToInt(transform.position);

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 10;
        }
        
        // Stats config alkalmazása ha elérhető
        if (statsConfig != null)
        {
            statsConfig.ApplyToStats(stats);
        }
    }

    // alapmozgás
    public void Move(Vector2Int direction)
    {
        Vector2Int newPosition = position + direction;

        if (GameManager2D.Instance != null && GameManager2D.Instance.mapGenerator != null)
        {
            if (!GameManager2D.Instance.mapGenerator.IsWalkable(newPosition))
            {
                return;
            }
        }

        position = newPosition;
        transform.position = new Vector3(position.x, position.y, 0);
    }
    
    public Combatant GetCombatant()
    {
        if (stats == null)
        {
            stats = GetComponent<Stats>();
            if (stats == null)
            {
                stats = gameObject.AddComponent<Stats>();
            }
        }
        return new Combatant(position, stats, true, "player");
    }
    
    public void TakeDamage(int damage)
    {
        if (stats != null)
        {
            int oldHealth = stats.currentHealth;
            stats.TakeDamage(damage);
            int newHealth = stats.currentHealth;
            
            Debug.Log($"[Player2D] Sebzés: {damage} → HP: {oldHealth} → {newHealth}");
            
            if (!stats.IsAlive())
            {
                Debug.Log("[Player2D] Játékos HP 0 vagy kevesebb! Meghalás...");
                Die();
            }
        }
        else
        {
            Debug.LogError("[Player2D] TakeDamage hívva, de stats == null!");
        }
    }
    
    private void Die()
    {
        Debug.Log("[Player2D] Die() metódus meghívva");
        
        if (GameManager2D.Instance != null)
        {
            Debug.Log("[Player2D] GameManager2D.Instance megtalálva, EndGame() hívása...");
            GameManager2D.Instance.EndGame();
        }
        else
        {
            Debug.LogError("[Player2D] GameManager2D.Instance == NULL! Nem lehet befejezni a játékot!");
        }
    }
}