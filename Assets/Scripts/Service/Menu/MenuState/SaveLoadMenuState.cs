using UnityEngine;

public class SaveLoadMenuState : MenuState
{
    private SaveMenuUI saveMenuUI;
    private bool isSaveMode;

    public SaveLoadMenuState(MenuManager manager, GameObject panel)
        : base(manager, panel, MenuType.SaveLoad)
    {
        saveMenuUI = panel.GetComponent<SaveMenuUI>();
    }

    public void SetSaveMode(bool saveMode)
    {
        isSaveMode = saveMode;
    }

    public override void Enter()
    {
        base.Enter();
        saveMenuUI?.SetMode(isSaveMode);
    }
}