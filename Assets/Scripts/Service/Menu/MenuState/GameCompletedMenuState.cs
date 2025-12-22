using UnityEngine;
using DG.Tweening;

public class GameCompletedMenuState : MenuState
{
    private CanvasGroup canvasGroup;

    public GameCompletedMenuState(MenuManager manager, GameObject panel)
        : base(manager, panel, MenuType.GameCompleted)
    {
        canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }
    }

    public override void Enter()
    {
        panel.SetActive(true);
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1f, 0.5f).SetUpdate(true);
    }

    public override void Exit()
    {
        canvasGroup.DOKill();
        panel.SetActive(false);
    }
}