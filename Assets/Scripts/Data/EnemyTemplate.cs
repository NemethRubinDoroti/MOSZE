using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Template", menuName = "Game/Enemy Template")]
public class EnemyTemplate : ScriptableObject
{
    [Header("Enemy Identity")]
    public Enemy2D.EnemyType enemyType;
    public string enemyName;
    [TextArea(2, 4)]
    public string description;

    [Header("Base Stats")]
    public int maxHealth = 50;
    public int attack = 10;
    public int defense = 5;
    public int speed = 8;
    [Range(0, 100)]
    public int accuracy = 70;

    [Header("AI Behavior")]
    public AIBehaviour.AIBehaviorType behaviorType = AIBehaviour.AIBehaviorType.Aggressive;
    public int attackRange = 1;
    public int moveRange = 3;

    [Header("Visual")]
    public Sprite sprite;
    public Color spriteColor = Color.white;

    [Header("Combat Rewards")]
    public int experienceReward = 10;
    public int scoreReward = 50;

    // Template alkalmazása
    public void ApplyToEnemy(Enemy2D enemy)
    {
        if (enemy == null) return;

        enemy.type = enemyType;

        if (enemy.stats == null)
        {
            enemy.stats = enemy.gameObject.GetComponent<Stats>();
            if (enemy.stats == null)
            {
                enemy.stats = enemy.gameObject.AddComponent<Stats>();
            }
        }

        enemy.stats.maxHealth = maxHealth;
        enemy.stats.currentHealth = maxHealth;
        enemy.stats.attack = attack;
        enemy.stats.defense = defense;
        enemy.stats.speed = speed;
        enemy.stats.accuracy = accuracy;

        if (enemy.behavior == null)
        {
            enemy.behavior = enemy.gameObject.GetComponent<AIBehaviour>();
            if (enemy.behavior == null)
            {
                enemy.behavior = enemy.gameObject.AddComponent<AIBehaviour>();
            }
        }

        enemy.behavior.behaviorType = behaviorType;
        enemy.behavior.attackRange = attackRange;
        enemy.behavior.moveRange = moveRange;

        if (enemy.spriteRenderer == null)
        {
            enemy.spriteRenderer = enemy.gameObject.GetComponent<SpriteRenderer>();
            if (enemy.spriteRenderer == null)
            {
                enemy.spriteRenderer = enemy.gameObject.AddComponent<SpriteRenderer>();
            }
        }

        if (sprite != null)
        {
            enemy.spriteRenderer.sprite = sprite;
        }
        enemy.spriteRenderer.color = spriteColor;
        enemy.spriteRenderer.sortingOrder = 5;
    }


    public Stats CreateStats()
    {
        Stats stats = new Stats();
        stats.maxHealth = maxHealth;
        stats.currentHealth = maxHealth;
        stats.attack = attack;
        stats.defense = defense;
        stats.speed = speed;
        stats.accuracy = accuracy;
        return stats;
    }
}

