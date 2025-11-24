using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable, ILookable, IHighlightable
{
    [Header("Interaction")]
    [SerializeField] protected string promptText = "Tương tác";
    [SerializeField] protected float interactionRange = 3f;

    [Header("Feedback")]
    [SerializeField] protected AudioClip interactSound;
    [SerializeField] protected bool useHighlight = true;
    [SerializeField] protected Color highlightColor = Color.yellow;

    protected IAudioService audioService;
    protected IHighlighter highlighter;
    protected bool isBeingLookedAt = false;

    protected virtual void Awake()
    {
        audioService = ServiceLocator.Get<IAudioService>();
        highlighter = ServiceLocator.Get<IHighlighter>();
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public virtual bool CanInteract(PlayerInteractionController player)
    {
        return true;
    }
    public abstract void OnInteract(PlayerInteractionController player);
    public virtual string getPromptText()
    {
        return promptText;
    }
    public virtual Transform getTransform()
    {
        return this.transform;
    }

    public virtual void OnLookAt(PlayerInteractionController player)
    {
        if (isBeingLookedAt) return;
        isBeingLookedAt = true;
        if (useHighlight && CanInteract(player)) EnableHighlight();
    }

    public virtual void OnLookAway(PlayerInteractionController player)
    {
        if (!isBeingLookedAt) return;
        isBeingLookedAt = false;
        if (useHighlight && this != null && gameObject != null)
            DisableHighlight();
    }
    public void EnableHighlight()
    {
        highlighter?.Highlight(gameObject, highlightColor);
    }
    public void DisableHighlight()
    {
        highlighter?.RemoveHighlight(gameObject);
    }
    protected void PlaySound (AudioClip clip)
    {
        if (clip != null)
        {
            audioService?.PlaySoundAtTransform(clip, transform);
        }
    }
}