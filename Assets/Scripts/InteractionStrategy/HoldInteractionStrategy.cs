using UnityEngine;

public class HoldInteractionStrategy : IInteractionStrategy
{
    private float holdTime = 0f;
    private bool isHolding = false;

    public void HandleInput(IInteractable target, PlayerInteractionController player, IInputService input)
    {
        if (!(target is IHoldable holdable)) return;

        if (input.IsInteractPressed && !isHolding) // tr??ng h?p b?t ??u nh?n
        {
            isHolding = true;
            holdTime = 0f;
            holdable.OnHoldStart(player);
            return;
        }

        if (input.IsInteractHeld && isHolding) // ?ang gi?
        {
            holdTime += Time.deltaTime;
            holdable.OnHoldProgress(player, holdTime);

            if (holdTime >= holdable.GetHoldDuration())
            // h?t th?i gian gi? gi?i h?n (code g?c là progress >=1f)
            {
                holdable.OnHoldComplete(player);
                target.OnInteract(player);
                Reset();
            }
        }
        if (input.IsInteractReleased && isHolding) // tha nut
        {
            if (holdTime < holdable.GetHoldDuration())
            {
                holdable.OnHoldCanceled(player);
            }
            Reset();
        }
    }


    public void Reset()
    {
        isHolding = false;
        holdTime = 0f;
    }
}