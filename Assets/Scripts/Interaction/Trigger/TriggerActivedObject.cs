using System.Collections;
using UnityEngine;
public class TriggerActivedObject : MonoBehaviour
{
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private ParticleSystem vfx;
    [SerializeField] private bool isLoop = false;
    [SerializeField] private float autoDisableAfter = 5f;

    //private void Start()
    //{
    //    PlaySpawnSound();
    //    PlayVFX();

    //    if (autoDestroyAfter > 0)
    //        Destroy(gameObject, autoDestroyAfter);
    //}

    private void PlaySpawnSound()
    {
        if (spawnSound == null) return;
        var audio = ServiceLocator.Get<IAudioService>();
        audio?.PlaySoundAtTransform(spawnSound, transform, 1f, isLoop);
    }

    private void PlayVFX()
    {
        if (vfx) vfx.Play();
    }
    private void OnEnable()
    {
        PlaySpawnSound();
        PlayVFX();
        if (autoDisableAfter > 0)
            StartCoroutine(DisableAfter(autoDisableAfter));
    }
    private IEnumerator DisableAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }       
}