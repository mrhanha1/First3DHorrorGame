using UnityEngine;

public class RaycastDetector
{
    private readonly ICameraProvider cameraProvider;
    private readonly LayerMask interactableLayer;
    private readonly float maxDistance;

    private readonly RaycastHit[] raycastBuffer = new RaycastHit[8];

    private bool enableDebug;

    public RaycastDetector (
        ICameraProvider cameraProvider,
        LayerMask interactableLayer,
        float maxDistance,
        bool enableDebug = false
        )
    {
        this.cameraProvider = cameraProvider;
        this.interactableLayer = interactableLayer;
        this.maxDistance = maxDistance;
        this.enableDebug = enableDebug;
    }

    public RaycastResult DetectInteractable()
    {
        if (!ValidateCameraProvider())
        {
            return CreateMissResult();
        }
        Vector3 rayOrigin = cameraProvider.GetCameraPosition();
        Vector3 rayDirection = cameraProvider.GetCameraForward();
        Ray ray = new Ray(rayOrigin, rayDirection);

        int hitCount = Physics.RaycastNonAlloc(
            ray,
            raycastBuffer,
            maxDistance,
            interactableLayer,
            QueryTriggerInteraction.Ignore);

        if (hitCount > 0)
        {
            RaycastHit closestHit = getClosestHit(hitCount);

            if (enableDebug)
            {
                Debug.Log($"[RaycastDetector]:HIT {closestHit.collider.name}at distance {closestHit.distance:F2}");
                Debug.DrawRay(rayOrigin, rayDirection * maxDistance, Color.red, 0.1f);
            }
            IInteractable interactable = FindInteractable(closestHit.collider);

            if (interactable != null)
            {
                if (enableDebug)
                {
                    Debug.Log($"[RaycastDetector]:Found interactable on {closestHit.collider.name}");
                }
                return RaycastResult.Hit(
                    interactable,
                    closestHit,
                    rayOrigin,
                    rayDirection);
            }
        }
        if (enableDebug)
        {
            Debug.Log("[RaycastDetector]:MISS: no interactable detected");
        }
        return CreateMissResult();
    }

    private IInteractable FindInteractable(Collider collider)
    {
        var interactable = collider.GetComponent<IInteractable>(); // object raycast bat duoc la interactable
        if (interactable != null)
        {
            return interactable;
        }
        else interactable = collider.GetComponentInParent<IInteractable>(); // neu khong phai thi tim tren parent
        return interactable;
    }
    private RaycastHit getClosestHit(int hitCount)
    {
        RaycastHit closestHit = raycastBuffer[0];
        for (int i = 1; i < hitCount; i++)
        {
            if (raycastBuffer[i].distance < closestHit.distance)
            {
                closestHit = raycastBuffer[i];
            }
        }
        return closestHit;
    }
    private bool ValidateCameraProvider()
    {
        if (cameraProvider == null || !cameraProvider.IsValid())
        {
            if (enableDebug)
            {
                Debug.LogWarning("[RaycastDetector]: Invalid camera provider");
            }
            return false;
        }
        return true;
    }
    private RaycastResult CreateMissResult()
    {
        Vector3 rayOrigin = cameraProvider?.GetCameraPosition() ?? Vector3.zero;
        Vector3 rayDirection = cameraProvider?.GetCameraForward() ?? Vector3.forward;
        return RaycastResult.Miss(rayOrigin, rayDirection, maxDistance);
    }
    public void SetDebugMode(bool enabled)
    {
        enableDebug = enabled;
    }
    public float MaxDistance => maxDistance;
}