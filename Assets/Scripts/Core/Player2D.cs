using UnityEngine;

public class Player2D : MonoBehaviour
{
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
    }

    // basic mozgás
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
}

