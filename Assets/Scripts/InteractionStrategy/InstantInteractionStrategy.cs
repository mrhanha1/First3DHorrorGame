using UnityEngine;

public class InstantInteractionStrategy : IInteractionStrategy
{
    private readonly KeyCode interactionKey;
    public InstantInteractionStrategy(KeyCode interactionKey)
    {
        this.interactionKey = interactionKey;
    }

    public void HandleInput(IInteractable target, PlayerInteractionController player)
    {
        if (Input.GetKeyDown(interactionKey) && target.CanInteract(player))
        {
            target.OnInteract(player);
        }
    }

    public void Reset() { }
}