using UnityEngine;

public class Action
{
    public ActionType type;
    public Vector2Int targetPosition;
    public Combatant target;
    public Combatant actor;
    
    public enum ActionType
    {
        Move,
        Attack,
        UseItem,
        Defend,
        Wait
    }
    
    public Action(ActionType type, Combatant actor)
    {
        this.type = type;
        this.actor = actor;
    }
    
    public Action(ActionType type, Combatant actor, Vector2Int targetPosition)
    {
        this.type = type;
        this.actor = actor;
        this.targetPosition = targetPosition;
    }
    
    public Action(ActionType type, Combatant actor, Combatant target)
    {
        this.type = type;
        this.actor = actor;
        this.target = target;
    }
    
    public void Execute()
    {
        if (actor == null || !actor.isAlive) return;
        
        switch (type)
        {
            case ActionType.Move:
                actor.Move(targetPosition);
                break;
                
            case ActionType.Attack:
                if (target != null)
                {
                    actor.Attack(target);
                }
                break;
                
            case ActionType.Defend:
                // Védekezés??
                break;
                
            case ActionType.Wait:
                break;
        }
    }
}

