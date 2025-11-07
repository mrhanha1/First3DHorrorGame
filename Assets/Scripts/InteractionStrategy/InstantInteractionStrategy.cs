using UnityEngine;

public class InstantInteractionStrategy : IInteractionStrategy
{

    public void HandleInput(IInteractable target, PlayerInteractionController player, IInputService input)
    {
        if (input.IsInteractPressed && target.CanInteract(player))
        {
            target.OnInteract(player);
        }
    }

    public void Reset() { }
}