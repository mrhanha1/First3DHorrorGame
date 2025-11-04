using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractionStrategy
{
    void HandleInput(IInteractable target, PlayerInteractionController player);
    void Reset();
}