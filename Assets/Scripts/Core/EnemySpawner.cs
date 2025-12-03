using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject enemyPrefab;

    [Header("Enemy Templates")]
    public EnemyTemplate securityBotTemplate;
    public EnemyTemplate patrolBotTemplate;
    public EnemyTemplate heavyBotTemplate;
    public EnemyTemplate bossTemplate;

    private List<Enemy2D> spawnedEnemies = new List<Enemy2D>();

    public Enemy2D SpawnEnemy(Enemy2D.EnemyType type, Vector2Int position)
    {
        GameObject enemyObj;

        // Prefab, fallback új GameObject-et hoz létre
        if (enemyPrefab != null)
        {
            enemyObj = Instantiate(enemyPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        }
        else
        {
            enemyObj = new GameObject($"Enemy_{type}_{position.x}_{position.y}");
            enemyObj.transform.position = new Vector3(position.x, position.y, 0);
        }

        Enemy2D enemy = enemyObj.GetComponent<Enemy2D>();
        if (enemy == null)
        {
            enemy = enemyObj.AddComponent<Enemy2D>();
        }

        enemy.type = type;
        enemy.position = position;

        // Template használata
        EnemyTemplate template = GetTemplateForType(type);
        if (template != null)
        {
            template.ApplyToEnemy(enemy);
            Debug.Log($"[EnemySpawner] Spawnolva az {type} ({template.enemyName}) az ({position.x}, {position.y}) pozíción");
        }
        else
        {
            Debug.LogError($"[EnemySpawner] Nincs template az {type} típushoz! Nem lehet ellenséget spawnolni.");
            Destroy(enemyObj);
            return null;
        }

        spawnedEnemies.Add(enemy);

        return enemy;
    }

    private EnemyTemplate GetTemplateForType(Enemy2D.EnemyType type)
    {
        switch (type)
        {
            case Enemy2D.EnemyType.SecurityBot:
                return securityBotTemplate;
            case Enemy2D.EnemyType.PatrolBot:
                return patrolBotTemplate;
            case Enemy2D.EnemyType.HeavyBot:
                return heavyBotTemplate;
            case Enemy2D.EnemyType.Boss:
                return bossTemplate;
            default:
                return null;
        }
    }

    public void ClearAllEnemies()
    {
        foreach (Enemy2D enemy in spawnedEnemies)
        {
            if (enemy != null && enemy.gameObject != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        spawnedEnemies.Clear();
    }

    public List<Enemy2D> GetSpawnedEnemies()
    {
        return spawnedEnemies;
    }
}

