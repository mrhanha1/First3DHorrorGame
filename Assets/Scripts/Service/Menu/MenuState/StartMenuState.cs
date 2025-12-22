using UnityEngine;

public class StartMenuState : MenuState
{
    public StartMenuState(MenuManager manager, GameObject panelObject)
        : base(manager, panelObject, MenuType.Start)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (ServiceLocator.TryGet<IGameStateService>(out var gameState))
        {
            gameState.LockCursor(false);
        }
    }
}