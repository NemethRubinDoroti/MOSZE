using UnityEngine;

public class Enemy2D : MonoBehaviour
{
    public Vector2Int position;
    public Stats stats;
    public EnemyType type;
    public AIBehaviour behavior;
    public SpriteRenderer spriteRenderer;

    public enum EnemyType
    {
        SecurityBot,
        PatrolBot,
        HeavyBot,
        Boss
    }

    private void Start()
    {
        stats = GetComponent<Stats>();
        if (stats == null)
        {
            stats = gameObject.AddComponent<Stats>();
            InitializeDefaultStats();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        behavior = GetComponent<AIBehaviour>();
        if (behavior == null)
        {
            behavior = gameObject.AddComponent<AIBehaviour>();
        }

        position = Vector2Int.RoundToInt(transform.position);
    }

    // Ellenfelek statjai
    private void InitializeDefaultStats()
    {
        switch (type)
        {
            case EnemyType.SecurityBot:
                stats.maxHealth = 50;
                stats.currentHealth = 50;
                stats.attack = 12;
                stats.defense = 5;
                stats.speed = 8;
                stats.accuracy = 70;
                break;

            case EnemyType.PatrolBot:
                stats.maxHealth = 30;
                stats.currentHealth = 30;
                stats.attack = 8;
                stats.defense = 3;
                stats.speed = 10;
                stats.accuracy = 75;
                break;

            case EnemyType.HeavyBot:
                stats.maxHealth = 100;
                stats.currentHealth = 100;
                stats.attack = 20;
                stats.defense = 10;
                stats.speed = 5;
                stats.accuracy = 65;
                break;

            case EnemyType.Boss:
                stats.maxHealth = 200;
                stats.currentHealth = 200;
                stats.attack = 25;
                stats.defense = 15;
                stats.speed = 7;
                stats.accuracy = 80;
                break;
        }
    }

    // mozgás
    public void Move(Vector2Int direction)
    {
        position += direction;
        transform.position = new Vector3(position.x, position.y, 0);
    }

    // támadás
    public void Attack(Player2D target)
    {
        if (target != null && stats != null)
        {
            int damage = Mathf.Max(1, stats.attack - target.stats.defense);
            target.TakeDamage(damage);
        }
    }

    // sebződés
    public void TakeDamage(int damage)
    {
        if (stats != null)
        {
            stats.TakeDamage(damage);
            if (!stats.IsAlive())
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log($"{type} meghalt!");
        Destroy(gameObject);
    }

    public Vector2Int GetPosition()
    {
        return position;
    }
    
    private Combatant combatant;
    
    public Combatant GetCombatant()
    {
        if (combatant == null)
        {
            combatant = new Combatant(position, stats, false, $"enemy_{GetInstanceID()}");
        }
        else
        {
            combatant.position = position;
            combatant.stats = stats;
        }
        return combatant;
    }
}

