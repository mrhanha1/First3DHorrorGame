using UnityEngine;

public class HoldInteractionStrategy : IInteractionStrategy
{
    private readonly KeyCode interactionKey;
    private float holdTime = 0f;
    private bool isHolding = false;

    public HoldInteractionStrategy(KeyCode interactionKey)
    {
        this.interactionKey = interactionKey;
    }
    public void HandleInput(IInteractable target, PlayerInteractionController player)
    {
        if (!(target is IHoldable holdable)) return;

        if (Input.GetKeyDown(interactionKey) && !isHolding) // tr??ng h?p b?t ??u nh?n
        {
            isHolding = true;
            holdTime = 0f;
            holdable.OnHoldStart(player);
        }

        if (Input.GetKey(interactionKey) && isHolding) // ?ang gi?
        {
            holdTime += Time.deltaTime;
            float progress = Mathf.Clamp01(holdTime / holdable.GetHoldDuration());
            holdable.OnHoldProgress(player, holdTime);

            if (holdTime >= holdable.GetHoldDuration())
            // h?t th?i gian gi? gi?i h?n (code g?c là progress >=1f)
            {
                holdable.OnHoldComplete(player);
                target.OnInteract(player);
                Reset();
            }
        }
        if (Input.GetKeyUp(interactionKey) && isHolding) // th? nút
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