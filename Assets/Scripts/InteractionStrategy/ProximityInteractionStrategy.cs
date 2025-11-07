using UnityEngine;

public class ProximityInteractionStrategy : IInteractionStrategy
{
    private bool isInRange = false;
    public void HandleInput(IInteractable target, PlayerInteractionController player, IInputService input)
    {
        if (!(target is IProximityTriggerable proximity)) return;

        float distance = Vector3.Distance(player.transform.position, target.getTransform().position);
        bool ShouldBeInRange = distance <= proximity.GetProximityRange();

        if (ShouldBeInRange && !isInRange)
        {
            proximity.OnEnterProximity(player);
            isInRange = true;
        }
        else if (!ShouldBeInRange && isInRange)
        {
            proximity.OnExitProximity(player);
            isInRange = false;
        }
    }
    public void Reset() 
    {
        isInRange = false;
    }
}