using Cinemachine;
using UnityEngine;

/// <summary>
/// Service quản lý lifecycle của minigame
/// </summary>
public interface IMinigameService
{
    void StartMinigame(IMinigame minigame);
    void ExitMinigame(); // Không cần tham số success
    IMinigame CurrentMinigame { get; }
    bool IsMinigameActive { get; }
}

/// <summary>
/// Interface cho tất cả minigame
/// </summary>
public interface IMinigame
{
    string MinigameName { get; }
    CinemachineVirtualCamera GetVirtualCamera();
    string GetInputActionMap();
    GameObject GetUI();
    bool CanExit { get; }

    void OnEnter();
    void OnUpdate();
    void OnExit(); // Không cần tham số success
}