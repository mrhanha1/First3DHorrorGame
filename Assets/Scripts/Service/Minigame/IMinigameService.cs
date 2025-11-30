using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Service quản lý lifecycle của minigame
/// </summary>
public interface IMinigameService
{
    void StartMinigame(IMinigame minigame);
    void ExitMinigame();
    void PauseMinigame();
    void ResumeMinigame();
    IMinigame CurrentMinigame { get; }
    bool IsMinigameActive { get; }
    Dictionary<string, bool> GetAllMinigameStates();
    void SetMinigameStates(Dictionary<string, bool> states);
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
    void OnExit();
    void OnPause();
    void OnResume();
}