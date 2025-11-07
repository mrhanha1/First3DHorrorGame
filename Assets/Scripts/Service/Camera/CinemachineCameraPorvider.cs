using UnityEngine;



public class CinemachineCameraProvider : ICameraProvider
{

    private Cinemachine.CinemachineBrain brain;

    public CinemachineCameraProvider(Cinemachine.CinemachineBrain brainComponent)
    {
        this.brain = brainComponent ?? Object.FindObjectOfType<Cinemachine.CinemachineBrain>();
    }
    public Camera GetCamera() => brain?.OutputCamera;
    public Transform GetCameraTransform() => GetCamera()?.transform;
    public Vector3 GetCameraPosition() => GetCameraTransform()?.position ?? Vector3.zero;
    public Vector3 GetCameraForward() => GetCameraTransform()?.forward ?? Vector3.forward;
    public Vector3 GetCameraUp() => GetCameraTransform()?.up ?? Vector3.up;
    public Vector3 GetCameraRight() => GetCameraTransform()?.right ?? Vector3.right;
    public bool IsValid() => brain != null && GetCamera() != null;
}