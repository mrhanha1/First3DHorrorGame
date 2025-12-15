using UnityEngine;

public class MainMenuState : MenuState
{

    public MainMenuState(MenuManager manager, GameObject panel)
        : base(manager, panel, MenuType.Main) {   }


    public override void OnBackPressed()
    {
        menuManager.CloseAll();
    }
}