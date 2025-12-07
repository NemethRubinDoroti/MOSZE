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
    public CombatUI combatUI;

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

        if (combatUI == null)
        {
            if (UIManager.Instance != null)
            {
                combatUI = UIManager.Instance.combatUI;
            }

            if (combatUI == null)
            {
                combatUI = FindFirstObjectByType<CombatUI>();
            }
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideInGameHUD();
        }

        if (combatUI != null)
        {
            combatUI.ShowCombatUI();
        }
        else
        {
            Debug.LogError("[CombatManager] CombatUI NULL!");
        }
    }

    public void EndCombat()
    {
        currentState = CombatState.CombatEnd;

        // Ellenőrizzük hogy a játékos nyert vagy meghalt
        bool playerWon = CheckWinCondition();
        bool playerAlive = playerCombatant != null && playerCombatant.isAlive;


        if (combatUI == null)
        {
            if (UIManager.Instance != null)
            {
                combatUI = UIManager.Instance.combatUI;
            }

            if (combatUI == null)
            {
                combatUI = FindFirstObjectByType<CombatUI>();
            }
        }

        if (combatUI != null)
        {
            combatUI.HideCombatUI();
        }

        if (playerWon)
        {
            // Ellenőrizzük, hogy a boss volt-e a harcban
            bool bossDefeated = IsBossInCombat() && !IsBossAlive();
            
            if (bossDefeated)
            {
                // Boss legyőzve = Győzelem!
                if (GameManager2D.Instance != null)
                {
                    GameManager2D.Instance.WinGame();
                }
            }
            else
            {
                // Normál ellenség legyőzve, folytatjuk a játékot
                GameManager2D.Instance.scoreSystem?.AddScore(100);
                
                RemoveDeadEnemies();
                
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowInGameHUD();
                }
                
                Time.timeScale = 1f;
            }
        }
        else if (!playerAlive)
        {
            // Vége a dalnak
            if (GameManager2D.Instance != null)
            {
                GameManager2D.Instance.EndGame();
            }
            else
            {
                Debug.LogError("[CombatManager] GameManager2D.Instance NULL!");
            }
        }
        else
        {
            // Ilyen elvileg nem lehet
            Debug.LogWarning("[CombatManager] EndCombat() nem tudott dönteni.");

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowInGameHUD();
            }

            Time.timeScale = 1f;
        }

        // Cleanup
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

    // Egy konkrét halott ellenség törlése azonnal
    private void RemoveDeadEnemy(Combatant enemy)
    {
        if (enemy == null || enemy.isPlayer || enemy.isAlive)
        {
            return;
        }

        if (enemy2DMap != null && enemy2DMap.ContainsKey(enemy))
        {
            Enemy2D enemy2D = enemy2DMap[enemy];
            if (enemy2D != null && enemy2D.gameObject != null)
            {
                Destroy(enemy2D.gameObject);
                enemy2DMap.Remove(enemy);
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

        // Azonnal töröljük a halott ellenségeket
        if (action.type == Action.ActionType.Attack && action.target != null && !action.target.isPlayer)
        {
            // Ha egy ellenséget támadtunk és meghalt, azonnal töröljük
            if (!action.target.isAlive)
            {
                RemoveDeadEnemy(action.target);
            }
        }

        UpdateVisualPositions();

        if (combatUI != null)
        {
            combatUI.UpdateUI();
        }

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

        if (combatUI != null)
        {
            combatUI.ShowPlayerOptions();
        }
    }

    public void StartEnemyTurn()
    {
        currentState = CombatState.EnemyTurn;
        currentTurnIndex = 0;

        if (combatUI != null)
        {
            combatUI.HideAllOptions();
        }

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
            string enemyName = enemy.id;
            if (enemy2DMap != null && enemy2DMap.ContainsKey(enemy) && enemy2DMap[enemy] != null)
            {
                enemyName = enemy2DMap[enemy].gameObject.name;
            }

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
                if (combatUI != null)
                {
                    string actionDescription = "";
                    if (enemyAction.type == Action.ActionType.Attack)
                    {
                        actionDescription = $"{enemyName} támad!";
                    }
                    else if (enemyAction.type == Action.ActionType.Move)
                    {
                        actionDescription = $"{enemyName} átmegy({enemyAction.targetPosition.x}, {enemyAction.targetPosition.y})-re!";
                    }
                    combatUI.AddCombatLog(actionDescription);
                }

                int playerHealthBefore = playerCombatant != null && playerCombatant.stats != null ? playerCombatant.stats.currentHealth : 0;

                ProcessAction(enemyAction);

                if (enemyAction.type == Action.ActionType.Attack && combatUI != null)
                {
                    int playerHealthAfter = playerCombatant != null && playerCombatant.stats != null ? playerCombatant.stats.currentHealth : 0;
                    int damage = playerHealthBefore - playerHealthAfter;

                    if (damage > 0)
                    {
                        combatUI.AddCombatLog($"{enemyName} eltalált! Sebzés: {damage}!");
                    }
                    else
                    {
                        combatUI.AddCombatLog($"{enemyName} támadása nem talált!");
                    }
                }

                // Ha az ellenség meghalt, töröljük (ez akkor történhet, ha a játékos visszatámadott)
                if (!enemy.isAlive)
                {
                    RemoveDeadEnemy(enemy);
                }
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

    private bool IsBossInCombat()
    {
        if (enemy2DMap == null) return false;
        
        foreach (var kvp in enemy2DMap)
        {
            if (kvp.Value != null && kvp.Value.type == Enemy2D.EnemyType.Boss)
            {
                return true;
            }
        }
        return false;
    }
    
    private bool IsBossAlive()
    {
        if (enemy2DMap == null) return false;
        
        foreach (var kvp in enemy2DMap)
        {
            if (kvp.Value != null && kvp.Value.type == Enemy2D.EnemyType.Boss)
            {
                if (kvp.Value.stats != null && kvp.Value.stats.IsAlive())
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void EndTurn()
    {
        if (currentState == CombatState.PlayerTurn)
        {
            StartEnemyTurn();
        }
    }
}

