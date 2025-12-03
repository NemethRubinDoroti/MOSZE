using UnityEngine;
using UnityEngine.Tilemaps;


public static class GridUtils
{

    public static int CalculateDistance(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
    }


    public static float CalculateDistanceFloat(Vector2Int from, Vector2Int to)
    {
        return Vector2.Distance(
            new Vector2(from.x, from.y),
            new Vector2(to.x, to.y)
        );
    }


    public static Vector2Int GetDirectionTowards(Vector2Int from, Vector2Int to)
    {
        Vector2Int direction = Vector2Int.zero;

        if (to.x > from.x) direction.x = 1;
        else if (to.x < from.x) direction.x = -1;

        if (to.y > from.y) direction.y = 1;
        else if (to.y < from.y) direction.y = -1;

        // Előnyben részesítjük az egyik irányt, ha mindkettő nem nulla
        if (direction.x != 0 && direction.y != 0)
        {
            if (Random.Range(0, 2) == 0)
            {
                direction.y = 0;
            }
            else
            {
                direction.x = 0;
            }
        }

        return direction;
    }


    public static Vector3 GridToWorldPosition(Vector2Int gridPosition, Tilemap tilemap = null)
    {
        if (tilemap != null)
        {
            Vector3Int cellPos = new Vector3Int(gridPosition.x, gridPosition.y, 0);
            Vector3 worldPos = tilemap.CellToWorld(cellPos);
            // CellToWorld visszatér a bal alsó sarokkal, hozzáadjuk a cellák méretének felét a középponthoz
            worldPos += new Vector3(tilemap.cellSize.x * 0.5f, tilemap.cellSize.y * 0.5f, 0);
            return worldPos;
        }
        else
        {
            // Ha nincs tilemap, akkor a pozíciót visszatérítjük
            return new Vector3(gridPosition.x, gridPosition.y, 0);
        }
    }


    public static Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        Tilemap tilemap = null;
        if (GameManager2D.Instance != null && GameManager2D.Instance.mapGenerator != null)
        {
            tilemap = GameManager2D.Instance.mapGenerator.groundTilemap;
        }

        return GridToWorldPosition(gridPosition, tilemap);
    }
}


