using UnityEngine;
using UnityEngine.Tilemaps;

public class Hostage2D : MonoBehaviour
{
    public Vector2Int position;
    public SpriteRenderer spriteRenderer;
    
    [Header("Visuals")]
    public Sprite hostageSprite;
    public int sortingOrder = 4;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // Ha van sprite, akkor beállítjuk
        if (hostageSprite != null)
        {
            spriteRenderer.sprite = hostageSprite;
        }
        
        spriteRenderer.sortingOrder = sortingOrder;
        
        position = Vector2Int.RoundToInt(transform.position);
        
        // Frissítjük a pozíciót
        UpdateVisualPosition();
    }
    
    private void Update()
    {
        // Frissítjük a pozíciót, ha megváltozott
        Vector2Int currentPos = Vector2Int.RoundToInt(transform.position);
        if (currentPos != position)
        {
            position = currentPos;
            UpdateVisualPosition();
        }
    }
    
    private void UpdateVisualPosition()
    {
        if (GameManager2D.Instance != null && GameManager2D.Instance.mapGenerator != null)
        {
            Tilemap groundTilemap = GameManager2D.Instance.mapGenerator.groundTilemap;
            if (groundTilemap != null)
            {
                Vector3 worldPos = GridUtils.GridToWorldPosition(position, groundTilemap);
                transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(position.x, position.y, transform.position.z);
            }
        }
        else
        {
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }
    }
    
    // Ha a játékos megmenti
    public void OnCollected()
    {
        if (HostageManager.Instance != null)
        {
            HostageManager.Instance.CollectHostage(this);
        }
        
        // Gyűjtés hang lejátszása
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCollect();
        }
        
        // Töröljük a hostage GameObject-et
        Destroy(gameObject);
    }
}

