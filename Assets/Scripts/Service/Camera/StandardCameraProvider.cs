using UnityEngine;

public class StandardCameraProvider : ICameraProvider
{
    private Camera camera;
    public StandardCameraProvider( Camera cam)
    {
        this.camera = cam ?? Camera.main;
    }
    public Camera GetCamera() => camera;
    public Transform GetCameraTransform() => camera?.transform;
    public Vector3 GetCameraPosition() => camera?.transform.position ?? Vector3.zero;
    public Vector3 GetCameraForward() => camera?.transform.forward ?? Vector3.forward;
    public Vector3 GetCameraUp() => camera?.transform.up ?? Vector3.up;
    public Vector3 GetCameraRight() => camera?.transform.right ?? Vector3.right;
    public bool IsValid() => camera != null;
}