using UnityEngine;

public class AudioService : IAudioService
{
    private readonly Transform audioParent;

    public AudioService()
    {
        this.audioParent = new GameObject("AudioSources").transform;
    }
    public void PlaySound (AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        GameObject audioObj = new GameObject($"Audio_{clip.name}");
        audioObj.transform.position = position;
        audioObj.transform.SetParent(audioParent);

        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 1f; // 3D sound
        source.Play();

        Object.Destroy(audioObj, clip.length+0.1f);
    }
    public void PlaySound2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        GameObject audioObj = new GameObject($"Audio2D_{clip.name}");
        audioObj.transform.SetParent(audioParent);

        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 0f; // 2D sound
        source.Play();

        Object.Destroy(audioObj, clip.length + 0.1f);
    }
    public void PlaySoundAtTransform(AudioClip clip, Transform targetTransform, float volume = 1f)
    {
        PlaySound(clip, targetTransform.position, volume);
    }
    public void StopAllSounds()
    {
        foreach (Transform child in audioParent)
        {
            Object.Destroy(child.gameObject);
        }
    }
}