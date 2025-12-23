using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType
{
    Instant, // tương tác chỉ chạm 1 lần
    Hold, // tương tác nhấn giữ
    Proximity // tương tác khi vào gần
}

public interface IInteractable
{
    bool CanInteract(PlayerInteractionController player);
    void OnInteract(PlayerInteractionController player);
    string getPromptText();
    Transform getTransform();
}


public interface ILockable
{

    bool IsLocked { get; }
    string RequiredKeyID { get; }
    bool TryUnlock(string keyID);
    void Lock();
}

public interface IHoldable
{
    float GetHoldDuration();
    void OnHoldStart(PlayerInteractionController player);
    void OnHoldProgress(PlayerInteractionController player, float holdTime);
    void OnHoldComplete(PlayerInteractionController player);
    void OnHoldCanceled(PlayerInteractionController player);
}


public interface IProximityTriggerable
{
    float GetProximityRange();
    void OnEnterProximity(PlayerInteractionController player);
    void OnExitProximity(PlayerInteractionController player);
}

public interface ILookable
{
    void OnLookAt(PlayerInteractionController player);
    void OnLookAway(PlayerInteractionController player);
}

public interface IHighlightable
{
    void EnableHighlight();
    void DisableHighlight();
}