using System.Collections.Generic;
using UnityEngine;

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
    private System.Collections.Generic.Dictionary<Combatant, Enemy2D> enemy2DMap;
    private Coroutine enemyTurnsCoroutine; // Referencia a coroutine-hoz, hogy meg tudjuk állítani
    private bool bossDefeatedThisCombat = false; // Flag, hogy boss legyőzve lett-e ebben a harcban
    
    public void StartCombat(Combatant player, List<Combatant> enemies, List<Enemy2D> enemy2DList = null)
    {
        combatants = new List<Combatant>();
        enemyCombatants = new List<Combatant>();
        enemy2DMap = new System.Collections.Generic.Dictionary<Combatant, Enemy2D>();
        bossDefeatedThisCombat = false; // Reset flag új harc kezdetén
        
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
        
        // Harc kezdés hang
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCombatStart();
        }
    }

    public void EndCombat()
    {
        Debug.Log("[CombatManager] EndCombat() meghívva");
        currentState = CombatState.CombatEnd;
        
        // Megállítjuk az összes futó coroutine-t (pl. ProcessEnemyTurnsCoroutine)
        StopAllCoroutines();
        enemyTurnsCoroutine = null; // Töröljük a referenciát
        
        bool playerWon = CheckWinCondition();
        bool playerAlive = playerCombatant != null && playerCombatant.isAlive;
        
        Debug.Log($"[CombatManager] EndCombat állapot: playerWon={playerWon}, playerAlive={playerAlive}");
        
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
        
        // UIManager-en keresztül is elrejtjük
        if (UIManager.Instance != null && UIManager.Instance.combatUI != null)
        {
            UIManager.Instance.HideCombatUI();
        }
        
        // Harc vége hang
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCombatEnd();
        }

        if (playerWon)
        {
            // Ellenőrizzük, hogy boss legyőzve lett-e (flag alapján, mert a boss már törölve lehet)
            // Vagy még mindig a harcban van és él-e
            bool bossDefeated = bossDefeatedThisCombat;
            
            // Ha a flag nincs beállítva, még ellenőrizzük, hogy van-e boss a harcban
            if (!bossDefeated)
            {
                bool bossInCombat = IsBossInCombat();
                bool bossAlive = IsBossAlive();
                bossDefeated = bossInCombat && !bossAlive;
            }
            
            Debug.Log($"[CombatManager] Boss ellenőrzés: bossDefeatedThisCombat={bossDefeatedThisCombat}, bossDefeated={bossDefeated}");
            
            if (bossDefeated)
            {
                Debug.Log("[CombatManager] Boss legyőzve! Győzelem!");
                // Boss legyőzve = Győzelem!
                if (GameManager2D.Instance != null)
                {
                    GameManager2D.Instance.WinGame();
                }
                else
                {
                    Debug.LogError("[CombatManager] GameManager2D.Instance == NULL! Nem lehet győzelmet kijelenteni!");
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
            Debug.Log("[CombatManager] Játékos halott a harcban! Játék vége...");
            if (GameManager2D.Instance != null)
            {
                Debug.Log("[CombatManager] GameManager2D.Instance megtalálva, EndGame() hívása...");
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

        
        // ELLENŐRIZZÜK, hogy boss volt-e, MIELŐTT törölnénk!
        if (enemy2DMap != null && enemy2DMap.ContainsKey(enemy))
        {
            Enemy2D enemy2D = enemy2DMap[enemy];
            if (enemy2D != null)
            {
                // Ha boss volt, eltároljuk ezt az információt
                if (enemy2D.type == Enemy2D.EnemyType.Boss)
                {
                    bossDefeatedThisCombat = true; // Eltároljuk, hogy boss legyőzve lett
                    Debug.Log("[CombatManager] Boss legyőzve! Flag beállítva.");
                }
                
                if (enemy2D.gameObject != null)
                {
                    // XP adás ellenség legyőzésekor
                    GiveXPForEnemy(enemy2D);
                    
                    Destroy(enemy2D.gameObject);
                }
                enemy2DMap.Remove(enemy);
            }
        }
        
        combatants.Remove(enemy);
        enemyCombatants.Remove(enemy);
    }
    
    private void GiveXPForEnemy(Enemy2D enemy)
    {
        if (enemy == null || ExperienceSystem.Instance == null)
        {
            return;
        }
        
        // XP lekérése az EnemyTemplate-ből
        int xpReward = GetXPFromEnemy(enemy);
        
        if (xpReward > 0)
        {
            ExperienceSystem.Instance.AddXP(xpReward);
            
            // UI frissítése (ha van combat UI)
            if (combatUI != null)
            {
                combatUI.AddCombatLog($"XP kapott: +{xpReward} XP");
            }
        }
    }
    
    private int GetXPFromEnemy(Enemy2D enemy)
    {
        if (enemy == null || GameManager2D.Instance == null)
        {
            return 0;
        }
        
        EnemySpawner spawner = GameManager2D.Instance.mapGenerator?.enemySpawner;
        if (spawner == null)
        {
            return 0;
        }
        
        // Template lekérése típus alapján
        EnemyTemplate template = null;
        switch (enemy.type)
        {
            case Enemy2D.EnemyType.SecurityBot:
                template = spawner.securityBotTemplate;
                break;
            case Enemy2D.EnemyType.PatrolBot:
                template = spawner.patrolBotTemplate;
                break;
            case Enemy2D.EnemyType.HeavyBot:
                template = spawner.heavyBotTemplate;
                break;
            case Enemy2D.EnemyType.Boss:
                template = spawner.bossTemplate;
                break;
        }
        
        return template != null ? template.experienceReward : 0;
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
        
        // Támadás hang lejátszása
        if (action.type == Action.ActionType.Attack)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAttack();
            }
        }

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
            // Ha a játékos meghalt, azonnal elrejtjük a CombatUI-t ÉS megállítjuk a coroutine-t
            if (!playerCombatant.isAlive)
            {
                // Azonnal megállítjuk az ellenség kör coroutine-ját
                if (enemyTurnsCoroutine != null)
                {
                    StopCoroutine(enemyTurnsCoroutine);
                    enemyTurnsCoroutine = null;
                }
                
                // Azonnal elrejtjük a CombatUI-t, még mielőtt az EndCombat() meghívódik
                if (combatUI != null)
                {
                    combatUI.HideCombatUI();
                }
                if (UIManager.Instance != null && UIManager.Instance.combatUI != null)
                {
                    UIManager.Instance.HideCombatUI();
                }
            }
            
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
    
    private System.Collections.IEnumerator DelayedEnemyTurn()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        StartEnemyTurn();
    }

    private void UpdateVisualPositions()
    {
        UnityEngine.Tilemaps.Tilemap tilemap = null;
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
        // Ha van már futó coroutine, megállítjuk
        if (enemyTurnsCoroutine != null)
        {
            StopCoroutine(enemyTurnsCoroutine);
        }
        enemyTurnsCoroutine = StartCoroutine(ProcessEnemyTurnsCoroutine());
    }
    
    private System.Collections.IEnumerator ProcessEnemyTurnsCoroutine()
    {
        // Másolatot készítünk a listáról, hogy ne legyen "Collection was modified" hiba
        List<Combatant> enemiesToProcess = new List<Combatant>(enemyCombatants);
        
        foreach (Combatant enemy in enemiesToProcess)
        {
            // Ellenőrizzük MINDEN iteráció elején, hogy a harc még aktív-e (ha véget ért, kilépünk)
            if (currentState == CombatState.CombatEnd || currentState == CombatState.None)
            {
                Debug.Log("[CombatManager] Harc véget ért, megszakítjuk az ellenség körét");
                enemyTurnsCoroutine = null;
                yield break;
            }
            
            // Ellenőrizzük, hogy a játékos még él-e (MINDEN iteráció elején)
            if (playerCombatant == null || !playerCombatant.isAlive)
            {
                Debug.Log("[CombatManager] Játékos halott, megszakítjuk az ellenség körét");
                // Elrejtjük a CombatUI-t
                if (combatUI != null)
                {
                    combatUI.HideCombatUI();
                }
                if (UIManager.Instance != null && UIManager.Instance.combatUI != null)
                {
                    UIManager.Instance.HideCombatUI();
                }
                enemyTurnsCoroutine = null;
                yield break;
            }
            
            // Ellenőrizzük, hogy az ellenség még a listában van-e (lehet, hogy törölték)
            if (!enemyCombatants.Contains(enemy))
            {
                continue;
            }
            
            if (!enemy.isAlive) continue;
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
                
                // Ellenőrizzük újra, hogy a harc még aktív-e (ProcessAction meghívhatja az EndCombat-et)
                if (currentState == CombatState.CombatEnd || currentState == CombatState.None)
                {
                    Debug.Log("[CombatManager] Harc véget ért ProcessAction után, megszakítjuk az ellenség körét");
                    yield break;
                }
                
                // Ha a játékos meghalt, azonnal kilépünk és elrejtjük a UI-t
                // Ezt a log üzenetek ELŐTT ellenőrizzük, hogy ne próbáljuk használni a combatUI-t
                if (playerCombatant == null || !playerCombatant.isAlive)
                {
                    Debug.Log("[CombatManager] Játékos meghalt, azonnal elrejtjük a CombatUI-t és megszakítjuk az ellenség körét");
                    if (combatUI != null)
                    {
                        combatUI.HideCombatUI();
                    }
                    if (UIManager.Instance != null && UIManager.Instance.combatUI != null)
                    {
                        UIManager.Instance.HideCombatUI();
                    }
                    enemyTurnsCoroutine = null;
                    yield break;
                }
                
                if (enemyAction.type == Action.ActionType.Attack && combatUI != null && playerCombatant != null && playerCombatant.isAlive)
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
                Debug.Log("[CombatManager] Játékos halott az ellenség kör után! EndCombat() hívása...");
                EndCombat();
                yield break;
            }
        }

        // Minden ellenség cselekvését végrehajtottuk, visszatérünk a játékos köréhez
        enemyTurnsCoroutine = null; // Töröljük a referenciát
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
        if (enemy2DMap == null)
        {
            Debug.Log("[CombatManager] IsBossInCombat: enemy2DMap == null");
            return false;
        }
        
        foreach (var kvp in enemy2DMap)
        {
            if (kvp.Value != null && kvp.Value.type == Enemy2D.EnemyType.Boss)
            {
                Debug.Log("[CombatManager] IsBossInCombat: Boss találva a harcban");
                return true;
            }
        }
        
        Debug.Log("[CombatManager] IsBossInCombat: Nincs boss a harcban");
        return false;
    }
    
    private bool IsBossAlive()
    {
        if (enemy2DMap == null)
        {
            Debug.Log("[CombatManager] IsBossAlive: enemy2DMap == null");
            return false;
        }
        
        foreach (var kvp in enemy2DMap)
        {
            if (kvp.Value != null && kvp.Value.type == Enemy2D.EnemyType.Boss)
            {
                bool alive = kvp.Value.stats != null && kvp.Value.stats.IsAlive();
                Debug.Log($"[CombatManager] IsBossAlive: Boss találva, él-e: {alive}");
                return alive;
            }
        }
        
        Debug.Log("[CombatManager] IsBossAlive: Nincs boss a harcban");
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

