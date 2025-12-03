using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    public AIBehaviorType behaviorType = AIBehaviorType.Aggressive;
    public int attackRange = 1;
    public int moveRange = 3;

    public enum AIBehaviorType
    {
        Aggressive,    // Mindig támad
        Defensive,     // Sokat védekezik
        Cautious,      // Óvatos
        Random         // Random
    }

    // távolság számítása
    public int CalculateDistance(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
    }

    // Irány
    public Vector2Int GetDirectionTowards(Vector2Int from, Vector2Int to)
    {
        Vector2Int direction = Vector2Int.zero;

        if (to.x > from.x) direction.x = 1;
        else if (to.x < from.x) direction.x = -1;

        if (to.y > from.y) direction.y = 1;
        else if (to.y < from.y) direction.y = -1;

        // Válasszon irányt ha egyik táv se 0
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
}

