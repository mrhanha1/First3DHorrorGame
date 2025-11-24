using System;
using UnityEngine;

public struct RaycastResult
{
    public bool HasHit { get; }
    public IInteractable Interactable { get; }
    public RaycastHit HitInfo { get; }
    public Vector3 RayOrigin { get; }
    public Vector3 RayDirection { get; }
    public float RayDistance { get; }
    public Vector3 Endpoint { get; }
    public RaycastResult(
        bool hasHit,
        IInteractable interactable,
        RaycastHit hitInfo,
        Vector3 rayOrigin,
        Vector3 rayDirection,
        float rayDistance)
    {
        HasHit = hasHit;
        Interactable = interactable;
        HitInfo = hitInfo;
        RayOrigin = rayOrigin;
        RayDirection = rayDirection;
        RayDistance = rayDistance;
        Endpoint = rayOrigin + rayDirection * rayDistance;
    }
    public static RaycastResult Miss(Vector3 rayOrigin, Vector3 rayDirection, float rayDistance)
    {
        return new RaycastResult(
            hasHit: false,
            interactable: null, 
            hitInfo: default, 
            rayOrigin: rayOrigin, 
            rayDirection: rayDirection, 
            rayDistance: rayDistance);
    }
    public static RaycastResult Hit(
        IInteractable interactable, 
        RaycastHit hitInfo, 
        Vector3 rayOrigin, 
        Vector3 rayDirection)
    {
        return new RaycastResult(
            hasHit: true,
            interactable: interactable,
            hitInfo: hitInfo,
            rayOrigin: rayOrigin,
            rayDirection: rayDirection,
            rayDistance: hitInfo.distance
            );
    }
}