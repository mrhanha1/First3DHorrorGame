using DG.Tweening;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour, ICameraService
{
    private Camera mainCam;
    private void Awake()
    {
        mainCam = Camera.main;
    }
    public void Shake(float intensity, float duration)
    {
        if (mainCam != null)
        {
            mainCam.transform.DOShakePosition(duration, intensity);
        }
    }
    public void ForceLookAt(Transform target, float duration)
    {
        if (mainCam == null || target == null) return;

        Quaternion targetRotation = Quaternion.LookRotation(target.position
            - mainCam.transform.position);
        mainCam.transform.DORotateQuaternion(targetRotation, duration);
    }
    public void Zoom(float fov, float duration)
    {
        if (mainCam != null)
        {
            mainCam.DOFieldOfView(fov, duration);
        }
    }
    public void ResetZoom(float duration)
    {
        if (mainCam != null)
        {
            mainCam.DOFieldOfView(60f, duration);
        }
    }
}
