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

    public Action GetNextAction(Combatant enemy, Combatant player)
    {
        if (enemy == null || player == null || !enemy.isAlive || !player.isAlive)
        {
            return null;
        }

        int distance = GridUtils.CalculateDistance(enemy.position, player.position);

        switch (behaviorType)
        {
            case AIBehaviorType.Aggressive:
                return GetAggressiveAction(enemy, player, distance);

            case AIBehaviorType.Defensive:
                return GetDefensiveAction(enemy, player, distance);

            case AIBehaviorType.Cautious:
                return GetCautiousAction(enemy, player, distance);

            case AIBehaviorType.Random:
                return GetRandomAction(enemy, player, distance);

            default:
                return GetAggressiveAction(enemy, player, distance);
        }
    }

    private Action GetAggressiveAction(Combatant enemy, Combatant player, int distance)
    {
        // Támadás, ha közel van
        if (distance <= attackRange)
        {
            return new Action(Action.ActionType.Attack, enemy, player);
        }

        // Mozgás a játékos felé, ha nincs túl messze
        if (distance <= moveRange * 2)
        {
            Vector2Int direction = GridUtils.GetDirectionTowards(enemy.position, player.position);
            Vector2Int newPos = enemy.position + direction;
            return new Action(Action.ActionType.Move, enemy, newPos);
        }

        // Várakozás, ha túl messze
        return new Action(Action.ActionType.Wait, enemy);
    }

    private Action GetDefensiveAction(Combatant enemy, Combatant player, int distance)
    {
        // Támadás, ha nagyon közel
        if (distance <= 1)
        {
            return new Action(Action.ActionType.Attack, enemy, player);
        }

        // Védekezés, ha közel van, de nincs támadási távolságban
        if (distance <= attackRange + 1)
        {
            return new Action(Action.ActionType.Defend, enemy);
        }

        // Mozgás közelebb, ha távol van
        if (distance <= moveRange * 2)
        {
            Vector2Int direction = GridUtils.GetDirectionTowards(enemy.position, player.position);
            Vector2Int newPos = enemy.position + direction;
            return new Action(Action.ActionType.Move, enemy, newPos);
        }

        return new Action(Action.ActionType.Wait, enemy);
    }

    private Action GetCautiousAction(Combatant enemy, Combatant player, int distance)
    {
        // Támadás, ha van elég életerő
        float healthPercent = (float)enemy.stats.currentHealth / enemy.stats.maxHealth;

        if (distance <= attackRange && healthPercent > 0.5f)
        {
            return new Action(Action.ActionType.Attack, enemy, player);
        }

        // Mozgás közelebb, ha van elég életerő
        if (distance <= moveRange && healthPercent > 0.3f)
        {
            Vector2Int direction = GridUtils.GetDirectionTowards(enemy.position, player.position);
            Vector2Int newPos = enemy.position + direction;
            return new Action(Action.ActionType.Move, enemy, newPos);
        }

        // Védekezés,ha kevés életerő
        return new Action(Action.ActionType.Defend, enemy);
    }

    private Action GetRandomAction(Combatant enemy, Combatant player, int distance)
    {
        int rand = Random.Range(0, 4);

        switch (rand)
        {
            case 0:
                if (distance <= attackRange)
                {
                    return new Action(Action.ActionType.Attack, enemy, player);
                }
                break;

            case 1:
                if (distance > 1)
                {
                    Vector2Int direction = GridUtils.GetDirectionTowards(enemy.position, player.position);
                    Vector2Int newPos = enemy.position + direction;
                    return new Action(Action.ActionType.Move, enemy, newPos);
                }
                break;

            case 2:
                return new Action(Action.ActionType.Defend, enemy);

            default:
                return new Action(Action.ActionType.Wait, enemy);
        }

        return new Action(Action.ActionType.Wait, enemy);
    }

}

