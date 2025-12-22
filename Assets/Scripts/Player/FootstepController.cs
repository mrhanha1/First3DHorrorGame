using UnityEngine;

namespace StarterAssets
{
    public class FootstepController : MonoBehaviour
    {
        [SerializeField] private AudioClip baseStepSound;
        [SerializeField] private AudioClip sprintStepSound;
        [SerializeField] private float baseStepInterval = 0.5f;
        [SerializeField] private float sprintStepInterval = 0.3f;
        [SerializeField] private float volume = 1f;

        private IAudioService audioService;
        private float stepTimer;
        private bool isPlayingBase;
        private bool isPlayingSprint;

        private void Start()
        {
            audioService = ServiceLocator.Get<IAudioService>();
        }

        private void Update()
        {
            if (isPlayingBase || isPlayingSprint)
            {
                stepTimer -= Time.deltaTime;
                if (stepTimer <= 0f)
                {
                    if (isPlayingBase)
                        PlayBaseStepSound();
                    else if (isPlayingSprint)
                        PlaySprintStepSound();
                }
            }
        }

        public void PlayBaseStep()
        {
            if (!isPlayingBase)
            {
                isPlayingBase = true;
                isPlayingSprint = false;
                stepTimer = 0f;
            }
        }

        public void PlaySprintStep()
        {
            if (!isPlayingSprint)
            {
                isPlayingBase = false;
                isPlayingSprint = true;
                stepTimer = 0f;
            }
        }

        public void StopFootstep()
        {
            isPlayingBase = false;
            isPlayingSprint = false;
        }

        private void PlayBaseStepSound()
        {
            if (baseStepSound != null && audioService != null)
            {
                audioService.PlaySoundAtTransform(baseStepSound, transform, volume, false);
                stepTimer = baseStepInterval;
            }
        }

        private void PlaySprintStepSound()
        {
            if (sprintStepSound != null && audioService != null)
            {
                audioService.PlaySoundAtTransform(sprintStepSound, transform, volume, false);
                stepTimer = sprintStepInterval;
            }
        }
    }
}