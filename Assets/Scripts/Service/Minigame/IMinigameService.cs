using Cinemachine;
using System;
using UnityEngine;

public interface IMinigameService
{
    void StartMinigame(IMinigame minigame, Action onComplete = null);
    void ExitMinigame(bool success = false);
    IMinigame CurrentMinigame { get; }
    bool IsMinigameActive { get; }
}
public interface IMinigame
{
    string MinigameName { get; }
    CinemachineVirtualCamera GetVirtualCamera();
    string GetInputActionMap();
    void OnEnter();
    void OnUpdate();
    void OnExit(bool success);
    bool CanExit { get; }
    GameObject GetUI();
}