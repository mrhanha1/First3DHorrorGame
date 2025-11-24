using UnityEngine;

public class DocumentInteractable : InteractableBase
{
    [Header("Document Settings")]
    [SerializeField] private string documentTitle;
    [SerializeField] private Sprite documentImage;
    [TextArea(5,15)]
    [SerializeField] private string documentText;

    public override void OnInteract(PlayerInteractionController player)
    {
        player.LockInteraction();
        player.LockMovement();

        var uiService = ServiceLocator.Get<IUIService>();
        if (uiService is UIServiceAdapter adapter)
        {
            adapter.ShowDocument(documentTitle, documentImage, documentText, () =>
            {
                player.UnlockMovement();
                player.UnlockInteraction();
            });
        }
    }
}