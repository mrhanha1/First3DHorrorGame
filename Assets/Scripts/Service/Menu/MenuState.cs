using UnityEngine;

public enum MenuType
{
    Start,
    Main,
    Settings,
    SaveLoad,
    GameCompleted
}

public abstract class MenuState
{
    protected MenuManager menuManager;
    protected GameObject panel;
    public MenuType MenuType { get; protected set; }

    public MenuState(MenuManager manager, GameObject panelObject, MenuType type)
    {
        menuManager = manager;
        panel = panelObject;
        MenuType = type;
    }

    public virtual void Enter()
    {
        panel?.SetActive(true);
    }

    public virtual void Exit()
    {
        panel?.SetActive(false);
    }

    public virtual void OnBackPressed()
    {
        menuManager.BackToPrevious();
    }
}