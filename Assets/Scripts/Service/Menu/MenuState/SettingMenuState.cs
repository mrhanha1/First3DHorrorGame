using UnityEngine;

public class SettingsMenuState : MenuState
{
    private SettingsUI settingsUI;

    public SettingsMenuState(MenuManager manager, GameObject panel)
        : base(manager, panel, MenuType.Settings)
    {
        settingsUI = panel.GetComponent<SettingsUI>();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}