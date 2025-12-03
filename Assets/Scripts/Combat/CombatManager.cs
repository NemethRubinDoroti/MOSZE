using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CombatManager : MonoBehaviour
{
    public enum CombatState
    {
        None,
        PlayerTurn,
        EnemyTurn,
        CombatEnd
    }
    
    public CombatState currentState = CombatState.None;
    public List<Combatant> combatants;
    public int currentTurnIndex;
    
    private Combatant playerCombatant;
    private List<Combatant> enemyCombatants;
    private Dictionary<Combatant, Enemy2D> enemy2DMap;
    
    // Harc kezdete
    public void StartCombat(Combatant player, List<Combatant> enemies, List<Enemy2D> enemy2DList = null)
    {
        combatants = new List<Combatant>();
        enemyCombatants = new List<Combatant>();
        enemy2DMap = new Dictionary<Combatant, Enemy2D>();
        
        playerCombatant = player;
        combatants.Add(player);
        
        for (int i = 0; i < enemies.Count; i++)
        {
            Combatant enemy = enemies[i];
            combatants.Add(enemy);
            enemyCombatants.Add(enemy);
            
            if (enemy2DList != null && i < enemy2DList.Count)
            {
                enemy2DMap[enemy] = enemy2DList[i];
            }
        }
        
        currentState = CombatState.PlayerTurn;
        currentTurnIndex = 0;
        
        Debug.Log($"[CombatManager] Harc kezdődött {enemies.Count} ellenséggel");
    }
    
    public void EndCombat()
    {
        currentState = CombatState.CombatEnd;
        
        bool playerWon = CheckWinCondition();
        bool playerAlive = playerCombatant != null && playerCombatant.isAlive;
        
        // Ellenőrizzük, hogy a játékos nyert-e vagy meghalt-e
        if (playerWon)
        {
            // Halott ellenségeket töröljük a játékból
            RemoveDeadEnemies();
            
            Debug.Log("[CombatManager] Játékos nyert!");
        }
        else if (!playerAlive)
        {
            Debug.Log("[CombatManager] Játékos meghalt!");
            if (GameManager2D.Instance != null)
            {
                GameManager2D.Instance.EndGame();
            }
        }
        
        // A harc állapotát töröljük, hogy RemoveDeadEnemies() hozzáférhessen enemy2DMap-hez
        combatants.Clear();
        enemyCombatants.Clear();
        enemy2DMap?.Clear();
        currentState = CombatState.None;
    }
    
    // Halott ellenségeket töröljük a játékból
    private void RemoveDeadEnemies()
    {
        if (enemy2DMap == null)
        {
            return;
        }
        
        foreach (var kvp in enemy2DMap)
        {
            if (kvp.Value != null && kvp.Value.stats != null && !kvp.Value.stats.IsAlive())
            {
                Destroy(kvp.Value.gameObject);
            }
        }
    }
    
    // Cselekvések végrehajtása
    public void ProcessAction(Action action)
    {
        if (currentState == CombatState.None || currentState == CombatState.CombatEnd)
        {
            return;
        }
        
        if (action == null || action.actor == null || !action.actor.isAlive)
        {
            return;
        }
        
        action.Execute();
        
        // Pozíciók frissítése (ha kell)
        UpdateVisualPositions();
        
        // Ellenőrizzük, hogy a harc véget ért-e
        if (CheckWinCondition() || !playerCombatant.isAlive)
        {
            EndCombat();
            return;
        }
        
        // Ugrás a következő körre (csak a játékos körében)
        if (currentState == CombatState.PlayerTurn)
        {
            // Kis késleltetés az ellenség kör előtt
            StartCoroutine(DelayedEnemyTurn());
        }
    }
    
    private IEnumerator DelayedEnemyTurn()
    {
        // ez tuti így?
        yield return new WaitForSecondsRealtime(0.5f);
        StartEnemyTurn();
    }
    
    private void UpdateVisualPositions()
    {
        // Tilemap-et kapunk a cellák világkoordinátáihoz
        Tilemap tilemap = null;
        if (GameManager2D.Instance != null && GameManager2D.Instance.mapGenerator != null)
        {
            tilemap = GameManager2D.Instance.mapGenerator.groundTilemap;
        }
        
        // Játékos pozícióját frissítjük
        if (playerCombatant != null && GameManager2D.Instance != null && GameManager2D.Instance.player != null)
        {
            GameManager2D.Instance.player.position = playerCombatant.position;
            GameManager2D.Instance.player.transform.position = GridUtils.GridToWorldPosition(playerCombatant.position, tilemap);
        }
        
        // Ellenségek pozícióját frissítjük
        if (enemy2DMap != null)
        {
            foreach (var kvp in enemy2DMap)
            {
                if (kvp.Key != null && kvp.Value != null && kvp.Value.gameObject != null)
                {
                    kvp.Value.position = kvp.Key.position;
                    kvp.Value.transform.position = GridUtils.GridToWorldPosition(kvp.Key.position, tilemap);
                }
            }
        }
    }
    
    public void StartPlayerTurn()
    {
        currentState = CombatState.PlayerTurn;
        currentTurnIndex = 0;
    }
    
    public void StartEnemyTurn()
    {
        currentState = CombatState.EnemyTurn;
        currentTurnIndex = 0;
        
        // Minden ellenség körének végrehajtása
        ProcessEnemyTurns();
    }
    
    private void ProcessEnemyTurns()
    {
        StartCoroutine(ProcessEnemyTurnsCoroutine());
    }
    
    // Ellenségek körének végrehajtása
    private IEnumerator ProcessEnemyTurnsCoroutine()
    {
        foreach (Combatant enemy in enemyCombatants)
        {
            if (!enemy.isAlive) continue;
            
            // Ellenség viselkedését kapjuk meg az Enemy2D-ből, ha van
            Action enemyAction = null;
            if (enemy2DMap != null && enemy2DMap.ContainsKey(enemy))
            {
                Enemy2D enemy2D = enemy2DMap[enemy];
                if (enemy2D != null && enemy2D.behavior != null)
                {
                    enemyAction = enemy2D.behavior.GetNextAction(enemy, playerCombatant);
                }
            }
            
            // Ha nincs konkrét viselkedés, akkor a default
            if (enemyAction == null)
            {
                // Default AI: próbáljon támadni, ha közel van, különben közelebb megy
                int distance = enemy.CalculateDistance(playerCombatant);
                if (distance <= 1)
                {
                    enemyAction = new Action(Action.ActionType.Attack, enemy, playerCombatant);
                }
                else
                {
                    // Mozgás a játékos felé
                    Vector2Int direction = GridUtils.GetDirectionTowards(enemy.position, playerCombatant.position);
                    Vector2Int newPos = enemy.position + direction;
                    enemyAction = new Action(Action.ActionType.Move, enemy, newPos);
                }
            }
            
            if (enemyAction != null)
            {
                ProcessAction(enemyAction);
                
                // Kis késleltetés az ellenség cselekvései között a láthatóság érdekében
                yield return new WaitForSecondsRealtime(0.5f);
            }
            
            // Ellenőrizzük, hogy a játékos meghalt-e
            if (!playerCombatant.isAlive)
            {
                EndCombat();
                yield break;
            }
        }
        
        // Minden ellenség cselekvését végrehajtottuk, visszatérünk a játékos köréhez
        StartPlayerTurn();
    }
    
    
    public bool CheckWinCondition()
    {
        // A játékos nyert, ha minden ellenség meghalt
        foreach (Combatant enemy in enemyCombatants)
        {
            if (enemy.isAlive)
            {
                return false;
            }
        }
        return playerCombatant != null && playerCombatant.isAlive;
    }
    
    public void EndTurn()
    {
        if (currentState == CombatState.PlayerTurn)
        {
            StartEnemyTurn();
        }
    }
}

