using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Collision Settings")]
    public bool verboseLogging = false;

    private Player2D player2D;
    private BoxCollider2D playerCollider;

    private MapGenerator cachedMapGenerator;
    private Tilemap cachedGroundTilemap;
    private Vector2 cachedColliderSize;
    private Vector2 cachedColliderOffset;
    private bool cacheInitialized = false;

#if ENABLE_INPUT_SYSTEM
    private Keyboard keyboard;
#endif

    private void Awake()
    {
        // BoxCollider2D ellenőrzése
        playerCollider = GetComponent<BoxCollider2D>();
        if (playerCollider == null)
        {
            playerCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        // Collider beállítások
        if (playerCollider != null)
        {
            playerCollider.size = new Vector2(0.3f, 0.1f);
            playerCollider.offset = new Vector2(0f, -0.5f);
            playerCollider.isTrigger = true;
        }
    }

    private void Start()
    {
        player2D = GetComponent<Player2D>();
        if (player2D == null)
        {
            Debug.LogError("[PlayerController] Player nincs hozzárendelve!");
        }

        // Van-e collider
        if (playerCollider == null)
        {
            playerCollider = GetComponent<BoxCollider2D>();
            if (playerCollider == null)
            {
                playerCollider = gameObject.AddComponent<BoxCollider2D>();
            }
        }

        // Collider beállítás
        if (playerCollider != null)
        {
            playerCollider.size = new Vector2(0.3f, 0.1f);
            playerCollider.offset = new Vector2(0f, -0.5f);
            playerCollider.isTrigger = true;

            // Cache
            cachedColliderSize = playerCollider.size;
            cachedColliderOffset = playerCollider.offset;
        }

        InitializeCache();

#if ENABLE_INPUT_SYSTEM
        keyboard = Keyboard.current;
        if (keyboard == null)
        {
            Debug.LogWarning("[PlayerController2D] Nem található billentyűzet?");
        }
#endif
    }

    private void InitializeCache()
    {
        if (GameManager2D.Instance != null && GameManager2D.Instance.mapGenerator != null)
        {
            cachedMapGenerator = GameManager2D.Instance.mapGenerator;
            cachedGroundTilemap = cachedMapGenerator.groundTilemap;
            cacheInitialized = true;
        }
        else
        {
            cacheInitialized = false;
        }
    }

    private void Update()
    {
        if (player2D == null) return;

        // Pause ellenőrzés
        if (GameManager2D.Instance != null &&
            GameManager2D.Instance.currentState != GameManager2D.GameState.Playing)
        {
            if (verboseLogging)
            {
                Debug.Log($"[PlayerController2D] Mozgás akadályozva - GameState: {GameManager2D.Instance.currentState}");
            }
            return;
        }

        // Combat ellenőrzés - ne engedjük a mozgást, ha harc van aktív
        if (GameManager2D.Instance != null &&
            GameManager2D.Instance.combatManager != null)
        {
            CombatManager.CombatState combatState = GameManager2D.Instance.combatManager.currentState;
            if (combatState != CombatManager.CombatState.None)
            {
                // Debug log csak ha valóban blokkolva van (ne spam-eljünk)
                if (verboseLogging)
                {
                    Debug.Log($"[PlayerController2D] Mozgás akadályozva - Combat state: {combatState}");
                }
                return;
            }
        }

        if (!cacheInitialized)
        {
            InitializeCache();
        }

        HandleSmoothMovement();

        // Túszok és tárgyak összegyűjtése
        CheckForHostages();
        CheckForItems();
    }

    private void HandleSmoothMovement()
    {
        Vector2 input = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) input.y += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) input.y -= 1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) input.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input.x += 1f;
        }
        else
#endif
        {
            // Fallback to legacy input
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) input.y += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) input.y -= 1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) input.x -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) input.x += 1f;
        }

        if (input != Vector2.zero)
        {
            input.Normalize();
            Vector3 movement = new Vector3(input.x, input.y, 0) * moveSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + movement;

            // Debug: logoljuk, hogy van input
            if (verboseLogging)
            {
                Debug.Log($"[PlayerController2D] Input: {input}, cél: {newPosition}");
            }

            // Collision check - csak grid check
            if (CanMoveTo(newPosition))
            {
                transform.position = newPosition;
                if (player2D != null)
                {
                    player2D.position = Vector2Int.RoundToInt(newPosition);
                }
            }
        }
    }

    private void CheckForHostages()
    {
        if (player2D == null || GameManager2D.Instance == null || GameManager2D.Instance.mapGenerator == null)
        {
            return;
        }

        HostageSpawner hostageSpawner = GameManager2D.Instance.mapGenerator.hostageSpawner;
        if (hostageSpawner == null)
        {
            return;
        }

        Vector2Int playerPos = player2D.position;
        List<Hostage2D> hostages = hostageSpawner.GetSpawnedHostages();

        foreach (Hostage2D hostage in hostages)
        {
            if (hostage != null && hostage.position == playerPos)
            {
                hostage.OnCollected();
                break; // Max 1 túsz / frame!
            }
        }
    }

    private void CheckForItems()
    {
        if (player2D == null || GameManager2D.Instance == null || GameManager2D.Instance.mapGenerator == null)
        {
            return;
        }

        ItemSpawner itemSpawner = GameManager2D.Instance.mapGenerator.itemSpawner;
        if (itemSpawner == null)
        {
            return;
        }

        Vector2Int playerPos = player2D.position;
        List<Item2D> items = itemSpawner.GetSpawnedItems();

        foreach (Item2D item in items)
        {
            if (item != null && item.position == playerPos)
            {
                item.OnCollected();
                break; // Max 1 tárgy / frame!
            }
        }
    }

    private bool CanMoveTo(Vector3 targetPosition)
    {
        // Korai return ha nincs cache
        if (!cacheInitialized || cachedMapGenerator == null)
        {
            return true; // Ha nincs map generator, engedjük a mozgást
        }

        // Használjuk a cache-elt értékeket
        Vector2 colliderSize = cachedColliderSize;
        Vector2 colliderOffset = cachedColliderOffset;

        // A collision box középpontja a targetPosition + offset
        float centerX = targetPosition.x + colliderOffset.x;
        float centerY = targetPosition.y + colliderOffset.y;
        float halfWidth = colliderSize.x * 0.5f;
        float halfHeight = colliderSize.y * 0.5f;

        // Ellenőrizzük a collision box 4 sarkát
        // Bal alsó
        Vector3Int cellPos = cachedGroundTilemap != null
            ? cachedGroundTilemap.WorldToCell(new Vector3(centerX - halfWidth, centerY - halfHeight, 0))
            : new Vector3Int(Mathf.FloorToInt(centerX - halfWidth), Mathf.FloorToInt(centerY - halfHeight), 0);
        if (!cachedMapGenerator.IsWalkable(new Vector2Int(cellPos.x, cellPos.y)))
            return false;

        // Jobb alsó
        cellPos = cachedGroundTilemap != null
            ? cachedGroundTilemap.WorldToCell(new Vector3(centerX + halfWidth, centerY - halfHeight, 0))
            : new Vector3Int(Mathf.FloorToInt(centerX + halfWidth), Mathf.FloorToInt(centerY - halfHeight), 0);
        if (!cachedMapGenerator.IsWalkable(new Vector2Int(cellPos.x, cellPos.y)))
            return false;

        // Bal felső
        cellPos = cachedGroundTilemap != null
            ? cachedGroundTilemap.WorldToCell(new Vector3(centerX - halfWidth, centerY + halfHeight, 0))
            : new Vector3Int(Mathf.FloorToInt(centerX - halfWidth), Mathf.FloorToInt(centerY + halfHeight), 0);
        if (!cachedMapGenerator.IsWalkable(new Vector2Int(cellPos.x, cellPos.y)))
            return false;

        // Jobb felső
        cellPos = cachedGroundTilemap != null
            ? cachedGroundTilemap.WorldToCell(new Vector3(centerX + halfWidth, centerY + halfHeight, 0))
            : new Vector3Int(Mathf.FloorToInt(centerX + halfWidth), Mathf.FloorToInt(centerY + halfHeight), 0);
        if (!cachedMapGenerator.IsWalkable(new Vector2Int(cellPos.x, cellPos.y)))
            return false;

        return true; // Minden sarok walkable
    }
}
