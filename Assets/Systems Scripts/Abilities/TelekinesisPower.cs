using UnityEngine;

/// <summary>
/// Raycasts to find a Rigidbody, pulls it to a hold point in front of the camera,
/// and handles throwing it. Knows nothing about the player controller state machine.
/// </summary>
public class TelekinesisPower : MonoBehaviour
{
    [SerializeField] private Transform aimOrigin;           // Usually the camera
    [SerializeField] private Transform holdPoint;           // Where the object floats in front of the player
    [SerializeField] private float maxDistance = 25f;
    [SerializeField] private float pullSpeed = 30f;         // How fast the object flies to the hold point
    [SerializeField] private float throwForce = 40f;
    [SerializeField] private LayerMask pickupMask = ~0;

    private Rigidbody grabbedRigidbody;
    private float originalDrag;
    private bool originalUseGravity;

    public bool IsHolding => grabbedRigidbody != null;
    public Rigidbody GrabbedObject => grabbedRigidbody;

    /// <summary>Returns true if a valid Rigidbody was found and grabbed.</summary>
    public bool TryGrab()
    {
        if (IsHolding) return false;

        if (!Physics.Raycast(aimOrigin.position, aimOrigin.forward, out RaycastHit hit,
                maxDistance, pickupMask, QueryTriggerInteraction.Ignore))
            return false;

        Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
        if (rb == null || rb.isKinematic) return false;

        grabbedRigidbody = rb;
        originalDrag = rb.linearDamping; 
        originalUseGravity = rb.useGravity;

        // Prepare the object for telekinesis floaty physics
        rb.useGravity = false;
        rb.linearDamping = 10f; // Dampen velocity so it doesn't bounce wildly

        return true;
    }

    public void Throw()
    {
        if (!IsHolding) return;

        grabbedRigidbody.linearVelocity = aimOrigin.forward * throwForce;
        Release();
    }

    public void Release()
    {
        if (!IsHolding) return;

        // Restore original rigidbody settings
        grabbedRigidbody.useGravity = originalUseGravity;
        grabbedRigidbody.linearDamping = originalDrag;

        grabbedRigidbody = null;
    }

    private void FixedUpdate()
    {
        if (!IsHolding || grabbedRigidbody == null) return;

        // Smoothly pull/hold the object at the hold point in front of the camera
        Vector3 directionToHold = holdPoint.position - grabbedRigidbody.position;
        float distance = directionToHold.magnitude;

        Vector3 targetVelocity = directionToHold.normalized * Mathf.Min(distance * pullSpeed, pullSpeed);
        grabbedRigidbody.linearVelocity = Vector3.Lerp(grabbedRigidbody.linearVelocity, targetVelocity, 20f * Time.fixedDeltaTime);
    }
}