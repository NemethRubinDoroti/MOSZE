using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CombatUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject combatPanel;
    public Button moveButton;
    public Button attackButton;
    public Button itemButton;
    public Button endTurnButton;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI turnIndicatorText;
    public TextMeshProUGUI combatLogText;
    
    [Header("Direction Selection")]
    public GameObject directionSelectionPanel;
    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;
    
    private CombatManager combatManager;
    private bool isSelectingMove = false;
    
#if ENABLE_INPUT_SYSTEM
    private UnityEngine.InputSystem.Keyboard keyboard;
#endif
    
    private void Start()
    {
        if (combatPanel == null)
        {
            Transform panelTransform = transform.Find("CombatPanel");
            if (panelTransform != null)
            {
                combatPanel = panelTransform.gameObject;
            }
            else
            {
                combatPanel = gameObject;
            }
        }
        
        if (combatPanel != null && combatPanel.activeSelf)
        {
            combatPanel.SetActive(false);
        }
        
        if (directionSelectionPanel != null)
        {
            directionSelectionPanel.SetActive(false);
        }
        
        if (moveButton != null)
        {
            moveButton.onClick.AddListener(OnMoveButtonClicked);
        }
        
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(OnAttackButtonClicked);
        }
        
        if (itemButton != null)
        {
            itemButton.onClick.AddListener(OnItemButtonClicked);
        }
        
        if (endTurnButton != null)
        {
            endTurnButton.onClick.AddListener(OnEndTurnButtonClicked);
        }
        
        if (upButton != null)
        {
            upButton.onClick.AddListener(() => OnDirectionSelected(Vector2Int.up));
        }
        
        if (downButton != null)
        {
            downButton.onClick.AddListener(() => OnDirectionSelected(Vector2Int.down));
        }
        
        if (leftButton != null)
        {
            leftButton.onClick.AddListener(() => OnDirectionSelected(Vector2Int.left));
        }
        
        if (rightButton != null)
        {
            rightButton.onClick.AddListener(() => OnDirectionSelected(Vector2Int.right));
        }
        
        combatManager = GameManager2D.Instance.combatManager;
        
#if ENABLE_INPUT_SYSTEM
        keyboard = UnityEngine.InputSystem.Keyboard.current;
#endif
    }
    
    private void Update()
    {
        // Billentyűzet input kezelése
        if (isSelectingMove && combatManager.currentState == CombatManager.CombatState.PlayerTurn)
        {
            Vector2Int? selectedDirection = null;
            
#if ENABLE_INPUT_SYSTEM
            if (keyboard != null)
            {
                if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
                {
                    selectedDirection = Vector2Int.up;
                }
                else if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
                {
                    selectedDirection = Vector2Int.down;
                }
                else if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
                {
                    selectedDirection = Vector2Int.left;
                }
                else if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
                {
                    selectedDirection = Vector2Int.right;
                }
            }
            else
#endif
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    selectedDirection = Vector2Int.up;
                }
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    selectedDirection = Vector2Int.down;
                }
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    selectedDirection = Vector2Int.left;
                }
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    selectedDirection = Vector2Int.right;
                }
            }
            
            if (selectedDirection.HasValue)
            {
                OnDirectionSelected(selectedDirection.Value);
            }
        }
    }
    
    public void ShowCombatUI()
    {
        if (combatPanel == null)
        {
            combatPanel = gameObject;
        }
        
        Transform current = combatPanel.transform;
        while (current != null)
        {
            if (!current.gameObject.activeSelf)
            {
                current.gameObject.SetActive(true);
            }
            current = current.parent;
        }
        
        combatPanel.SetActive(true);
        combatPanel.transform.SetAsLastSibling();
        
        Canvas canvas = combatPanel.GetComponentInParent<Canvas>();
        if (canvas != null && !canvas.enabled)
        {
            canvas.enabled = true;
        }
        
        Canvas.ForceUpdateCanvases();
        UpdateUI();
        ShowPlayerOptions();
    }
    
    public void HideCombatUI()
    {
        // Minden gombot először letiltunk, hogy ne kaphassanakinputot
        if (moveButton != null)
        {
            moveButton.interactable = false;
            moveButton.gameObject.SetActive(false);
        }
        if (attackButton != null)
        {
            attackButton.interactable = false;
            attackButton.gameObject.SetActive(false);
        }
        if (itemButton != null)
        {
            itemButton.interactable = false;
            itemButton.gameObject.SetActive(false);
        }
        if (endTurnButton != null)
        {
            endTurnButton.interactable = false;
            endTurnButton.gameObject.SetActive(false);
        }
        
        // Irányválasztás elrejtése
        HideDirectionSelection();
        
        if (combatPanel != null)
        {
            combatPanel.SetActive(false);
        }
    }
    
    public void ShowPlayerOptions()
    {
        if (isSelectingMove)
        {
            HideDirectionSelection();
        }
        
        // Gombok megjelenítése és engedélyezése
        if (moveButton != null)
        {
            moveButton.gameObject.SetActive(true);
            moveButton.interactable = true;
        }
        if (attackButton != null)
        {
            attackButton.gameObject.SetActive(true);
            attackButton.interactable = HasValidTargets();
        }
        if (itemButton != null)
        {
            itemButton.gameObject.SetActive(true);
            itemButton.interactable = HasUsableItems();
        }
        if (endTurnButton != null)
        {
            endTurnButton.gameObject.SetActive(true);
            endTurnButton.interactable = true;
        }
        
        if (turnIndicatorText != null)
        {
            turnIndicatorText.text = "Játékos Köre";
        }
        
        UpdateUI();
    }
    
    public void HideAllOptions()
    {
        if (moveButton != null) moveButton.interactable = false;
        if (attackButton != null) attackButton.interactable = false;
        if (itemButton != null) itemButton.interactable = false;
        if (endTurnButton != null) endTurnButton.interactable = false;
        
        HideDirectionSelection();
        
        if (turnIndicatorText != null)
        {
            turnIndicatorText.text = "Ellenség Köre";
        }
    }
    
    public void UpdateUI()
    {
        if (playerHealthText != null && GameManager2D.Instance.player.stats != null)
        {
            playerHealthText.text = $"HP: {GameManager2D.Instance.player.stats.currentHealth}/{GameManager2D.Instance.player.stats.maxHealth}";
        }
        
        if (enemyHealthText != null && GameManager2D.Instance.combatManager.combatants != null)
        {
            int aliveEnemies = 0;
            foreach (var combatant in GameManager2D.Instance.combatManager.combatants)
            {
                if (!combatant.isPlayer && combatant.isAlive)
                {
                    aliveEnemies++;
                }
            }
            enemyHealthText.text = $"Ellenségek: {aliveEnemies}";
        }
        
        UpdateItemButtons();
    }
    
    private bool HasValidTargets()
    {
        // Van-e ellenség a támadási távolságban
        if (GameManager2D.Instance.combatManager.combatants == null)
        {
            return false;
        }
        
        Vector2Int playerPos = GameManager2D.Instance.player.position;
        int attackRange = 1;
        
        foreach (var combatant in GameManager2D.Instance.combatManager.combatants)
        {
            if (!combatant.isPlayer && combatant.isAlive)
            {
                int distance = GridUtils.CalculateDistance(combatant.position, playerPos);
                if (distance <= attackRange)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    private bool HasUsableItems()
    {
        // Ellenőrizzük, hogy a játékosnak van-e Heal tárgy
        return ItemManager.Instance.HasItem(Item2D.ItemType.Heal);
    }
    
    // bővíthető fgv. ha akarunk tárgy kiválasztást is
    public void UpdateItemButtons()
    {
        if (itemButton != null)
        {
            itemButton.interactable = HasUsableItems();
        }
    }
    
    private void OnMoveButtonClicked()
    {
        if (combatManager.currentState != CombatManager.CombatState.PlayerTurn ||
            combatManager.currentState == CombatManager.CombatState.None ||
            combatManager.currentState == CombatManager.CombatState.CombatEnd)
        {
            return;
        }
        
        // Irányválasztás UI megjelenítése
        ShowDirectionSelection();
    }
    
    private void ShowDirectionSelection()
    {
        isSelectingMove = true;
        
        if (moveButton != null) moveButton.interactable = false;
        if (attackButton != null) attackButton.interactable = false;
        if (itemButton != null) itemButton.interactable = false;
        if (endTurnButton != null) endTurnButton.interactable = false;
        
        if (directionSelectionPanel != null)
        {
            directionSelectionPanel.SetActive(true);
        }
        
        // Iránygombok megjelenítése és engedélyezése
        Button[] directionButtons = { upButton, downButton, leftButton, rightButton };
        foreach (Button btn in directionButtons)
        {
            if (btn != null)
            {
                if (btn.transform.parent != null && !btn.transform.parent.gameObject.activeSelf)
                {
                    btn.transform.parent.gameObject.SetActive(true);
                }
                btn.gameObject.SetActive(true);
                btn.interactable = true;
            }
        }
        
        AddCombatLog("Irányválasztás");
    }
    
    private void OnDirectionSelected(Vector2Int direction)
    {
        if (combatManager.currentState != CombatManager.CombatState.PlayerTurn ||
            !isSelectingMove)
        {
            return;
        }
        
        Player2D player = GameManager2D.Instance.player;
        
        Vector2Int playerPos = player.position;
        Vector2Int targetPos = playerPos + direction;
        
        // Ellenőrizzük, hogy a cél pozíció érvényes-e
        if (IsValidMovePosition(targetPos))
        {
            Combatant playerCombatant = combatManager.combatants[0]; // Játékos az első
            
            Action moveAction = new Action(Action.ActionType.Move, playerCombatant, targetPos);
            combatManager.ProcessAction(moveAction);
            
            AddCombatLog($"Játékos mozgott {GetDirectionName(direction)}");
        }
        else
        {
            AddCombatLog("Nem lehet oda mozogni!");
        }
        
        // Irányválasztás elrejtése
        HideDirectionSelection();
    }
    
    private void HideDirectionSelection()
    {
        isSelectingMove = false;
        
        if (directionSelectionPanel != null)
        {
            directionSelectionPanel.SetActive(false);
        }
        
        // Iránygombok elrejtése
        if (upButton != null)
        {
            upButton.gameObject.SetActive(false);
            upButton.interactable = false;
        }
        if (downButton != null)
        {
            downButton.gameObject.SetActive(false);
            downButton.interactable = false;
        }
        if (leftButton != null)
        {
            leftButton.gameObject.SetActive(false);
            leftButton.interactable = false;
        }
        if (rightButton != null)
        {
            rightButton.gameObject.SetActive(false);
            rightButton.interactable = false;
        }
        
        // Fő gombok ismét megjelenítése, ha még játékos kör
        if (combatManager.currentState == CombatManager.CombatState.PlayerTurn)
        {
            ShowPlayerOptions();
        }
    }
    
    private bool IsValidMovePosition(Vector2Int position)
    {
        // Ellenőrizzük, hogy a pozíció járható-e
        if (!GameManager2D.Instance.mapGenerator.IsWalkable(position))
        {
            return false;
        }
        
        // Ellenőrizzük, hogy foglalt-e a pozíció
        if (combatManager.combatants != null)
        {
            foreach (Combatant combatant in combatManager.combatants)
            {
                if (combatant != null && combatant.isAlive && combatant.position == position)
                {
                    return false; // Már vannak ott
                }
            }
        }
        
        return true;
    }
    
    private string GetDirectionName(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return "up";
        if (direction == Vector2Int.down) return "down";
        if (direction == Vector2Int.left) return "left";
        if (direction == Vector2Int.right) return "right";
        return "unknown";
    }
    
    private void OnAttackButtonClicked()
    {
        if (combatManager.currentState != CombatManager.CombatState.PlayerTurn ||
            combatManager.currentState == CombatManager.CombatState.None ||
            combatManager.currentState == CombatManager.CombatState.CombatEnd)
        {
            return;
        }
        
        // Célpont kiválasztás
        ShowTargetSelection();
    }
    
    private void OnItemButtonClicked()
    {
        if (combatManager.currentState != CombatManager.CombatState.PlayerTurn ||
            combatManager.currentState == CombatManager.CombatState.None ||
            combatManager.currentState == CombatManager.CombatState.CombatEnd)
        {
            return;
        }
        
        if (ItemManager.Instance == null)
        {
            AddCombatLog("Nincs tárgy!");
            return;
        }
        
        // Tárgy kiválasztás UI megjelenítése
        ShowItemSelection();
    }
    
    // bővíthető fgv. ha akarunk tárgy kiválasztást is
    private void ShowItemSelection()
    {
        if (ItemManager.Instance == null)
        {
            return;
        }
        
        if (ItemManager.Instance.HasItem(Item2D.ItemType.Heal))
        {
            ItemManager.Instance.UseHeal();
        }
        else
        {
            AddCombatLog("Nincs használható tárgy!");
        }
    }
    
    private void OnEndTurnButtonClicked()
    {
        if (combatManager.currentState != CombatManager.CombatState.None &&
            combatManager.currentState != CombatManager.CombatState.CombatEnd)
        {
            AddCombatLog("Játékos befezte a kört");
            combatManager.EndTurn();
        }
    }
    
    // Bővíthető fgv. ha akarunk célpont kiválasztást is
    private void ShowTargetSelection()
    {
        if (combatManager.combatants == null)
        {
            return;
        }
        
        Player2D player = GameManager2D.Instance.player;
        
        Vector2Int playerPos = player.position;
        int attackRange = 1;
        
        // Érvényes célok keresése
        List<Combatant> validTargets = new List<Combatant>();
        foreach (var combatant in combatManager.combatants)
        {
            if (!combatant.isPlayer && combatant.isAlive)
            {
                int distance = GridUtils.CalculateDistance(combatant.position, playerPos);
                if (distance <= attackRange)
                {
                    validTargets.Add(combatant);
                }
            }
        }
        
        if (validTargets.Count == 0)
        {
            AddCombatLog("Nincs ellenség a támadási távolságban!");
            return;
        }
        
        // Ha csak egy cél van, akkor közvetlenül támadjunk rá
        if (validTargets.Count == 1)
        {
            AttackTarget(validTargets[0]);
        }
        else
        {
            AttackTarget(validTargets[0]);
        }
    }
    
    private void AttackTarget(Combatant target)
    {
        if (combatManager.combatants == null || combatManager.combatants.Count == 0)
        {
            return;
        }
        
        // Harcrendszer aktív-e
        if (combatManager.currentState == CombatManager.CombatState.None || 
            combatManager.currentState == CombatManager.CombatState.CombatEnd)
        {
            return;
        }
        
        Combatant playerCombatant = combatManager.combatants[0]; // Játékos az első
        
        // Sebzés kiszámítása a támadás előtt
        int damage = 0;
        if (Random.Range(0, 100) < playerCombatant.stats.accuracy)
        {
            damage = Mathf.Max(1, playerCombatant.stats.attack - target.stats.defense);
        }
        
        // Támadás végrehajtása
        Action attackAction = new Action(Action.ActionType.Attack, playerCombatant, target);
        combatManager.ProcessAction(attackAction);
        
        // Combat log
        if (damage > 0)
        {
            AddCombatLog($"Játékos ütött: {damage}!");
        }
        else
        {
            AddCombatLog("Játékos támadása mellément!");
        }
        
        UpdateUI();
    }
    
    public void AddCombatLog(string message)
    {
        if (combatLogText != null)
        {
            string currentText = combatLogText.text;
            combatLogText.text = message + "\n" + currentText;
            
            // Combat log limit: maximum 4 sor
            string[] lines = combatLogText.text.Split('\n');
            // Szűrjük ki az üres sorokat és csak a nem üres sorokat számoljuk
            System.Collections.Generic.List<string> nonEmptyLines = new System.Collections.Generic.List<string>();
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    nonEmptyLines.Add(line);
                }
            }
            
            // Ha több mint 4 sor van, csak az első 4-et tartjuk meg (legújabbak)
            if (nonEmptyLines.Count > 4)
            {
                combatLogText.text = string.Join("\n", nonEmptyLines.GetRange(0, 4));
            }
            else
            {
                combatLogText.text = string.Join("\n", nonEmptyLines);
            }
        }
    }
}
