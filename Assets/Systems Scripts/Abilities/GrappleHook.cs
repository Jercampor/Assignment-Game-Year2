using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Fires a rope at whatever the aim origin is pointing at, animates it travelling there,
/// and reports when it attaches. Knows nothing about the player or the game.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class GrappleHook : MonoBehaviour
{
    [SerializeField] private Transform aimOrigin;   // usually the camera
    [SerializeField] private Transform ropeStart;   // where the rope leaves the player
    [SerializeField] private float maxDistance = 40f;
    [SerializeField] private float ropeSpeed = 120f;
    [SerializeField] private float ropeWidth = 0.06f;
    [SerializeField] private LayerMask grappleMask = ~0;

    private LineRenderer line;
    private Coroutine fireRoutine;
    private Vector3 tipPosition;

    public bool IsFiring { get; private set; }
    public bool IsAttached { get; private set; }
    public bool IsActive => IsFiring || IsAttached;
    public Vector3 AnchorPoint { get; private set; }
    
    public Vector3 AnchorNormal { get; private set; }

    public event Action Attached;
    public event Action Released;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = ropeWidth;
        line.endWidth = ropeWidth;
        line.enabled = false;
    }

    /// <summary>Returns true if something was in range and the rope was fired.</summary>
    public bool TryFire()
    {
        if (IsActive) return false;

        if (!Physics.Raycast(aimOrigin.position, aimOrigin.forward, out RaycastHit hit,
                maxDistance, grappleMask, QueryTriggerInteraction.Ignore))
            return false;

        AnchorPoint = hit.point;
        AnchorNormal = hit.normal;
        fireRoutine = StartCoroutine(FireRoutine());
        return true;
    }

    public void Release()
    {
        if (!IsActive) return;

        if (fireRoutine != null) StopCoroutine(fireRoutine);
        fireRoutine = null;
        IsFiring = false;
        IsAttached = false;
        line.enabled = false;

        Released?.Invoke();
    }

    // The rope tip travels to the target over several frames
    private IEnumerator FireRoutine()
    {
        IsFiring = true;
        line.enabled = true;
        tipPosition = ropeStart.position;

        while (Vector3.Distance(tipPosition, AnchorPoint) > 0.1f)
        {
            tipPosition = Vector3.MoveTowards(tipPosition, AnchorPoint, ropeSpeed * Time.deltaTime);
            yield return null;
        }

        tipPosition = AnchorPoint;
        IsFiring = false;
        IsAttached = true;
        fireRoutine = null;
        Attached?.Invoke();
    }

    private void LateUpdate()
    {
        if (!line.enabled) return;

        line.SetPosition(0, ropeStart.position);
        line.SetPosition(1, IsAttached ? AnchorPoint : tipPosition);
    }
}