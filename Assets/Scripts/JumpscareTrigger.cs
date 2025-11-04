using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class JumpscareTrigger : MonoBehaviour
{
    [SerializeField] private GameObject jumpscarePrefab;
    [SerializeField] private AudioClip jumpscareSound;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool isTriggerOnce = true;
    [SerializeField] private float jumpscareDuration = 3f;

    private ICameraService cameraService;
    private IAudioService audioService;
    private bool hasTriggered = false;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        cameraService = ServiceLocator.Get<ICameraService>();
        audioService = ServiceLocator.Get<IAudioService>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && isTriggerOnce) return;
        if (!other.CompareTag("Player")) return;

        TriggerJumpscare(other.GetComponent<PlayerInteractionController>());
        if (isTriggerOnce)
        {
            hasTriggered = true;
        }
    }
    private void TriggerJumpscare(PlayerInteractionController player)
    {
        player?.LockInteraction();
        player?.LockMovement();
        if (jumpscarePrefab != null && spawnPoint != null)
        {
            Instantiate(jumpscarePrefab, spawnPoint.position, spawnPoint.rotation);
            Destroy(jumpscarePrefab, jumpscareDuration);
        }
        audioService?.PlaySound(jumpscareSound,transform.position);
        cameraService?.Shake(0.5f, jumpscareDuration);
        
        DOVirtual.DelayedCall(jumpscareDuration, () =>
        {
            player?.UnlockInteraction();
            player?.UnlockMovement();
        });
    }
}