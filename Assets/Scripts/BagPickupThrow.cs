using UnityEngine;
using UnityEngine.InputSystem;

public class BagPickupThrow : MonoBehaviour
{
    [SerializeField] private Transform aimCamera;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private float throwSpeed = 8f;

    private NetProjectile heldBag;
    private Rigidbody heldBody;
    private Collider heldCollider;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (heldBag == null)
                TryPickUpBag();
            else
                ReleaseBag();
        }

        if (Keyboard.current.fKey.wasPressedThisFrame && heldBag != null)
            ThrowBag();
    }

    private void TryPickUpBag()
    {
        if (aimCamera == null || holdPoint == null)
        {
            Debug.LogWarning("Assign Aim Camera and Hold Point on BagPickupThrow.");
            return;
        }

        if (!Physics.Raycast(
                aimCamera.position,
                aimCamera.forward,
                out RaycastHit hit,
                pickupRange))
        {
            return;
        }

        NetProjectile candidate = hit.collider.GetComponentInParent<NetProjectile>();
        if (candidate == null || !candidate.IsCaughtBag)
            return;

        Rigidbody body = candidate.GetComponent<Rigidbody>();
        Collider bagCollider = candidate.GetComponent<Collider>();
        if (body == null || bagCollider == null)
            return;

        heldBag = candidate;
        heldBody = body;
        heldCollider = bagCollider;

        heldBody.linearVelocity = Vector3.zero;
        heldBody.angularVelocity = Vector3.zero;
        heldBody.isKinematic = true;
        heldBody.detectCollisions = false;
        heldCollider.enabled = false;

        heldBag.transform.SetParent(holdPoint, false);
        heldBag.transform.localPosition = Vector3.zero;
        heldBag.transform.localRotation = Quaternion.identity;
    }

    private void ReleaseBag()
    {
        if (heldBag == null)
            return;

        heldBag.transform.SetParent(null, true);

        heldCollider.enabled = true;
        heldBody.detectCollisions = true;
        heldBody.isKinematic = false;
        heldBody.linearVelocity = Vector3.zero;
        heldBody.angularVelocity = Vector3.zero;

        heldBag = null;
        heldBody = null;
        heldCollider = null;
    }

    private void ThrowBag()
    {
        if (aimCamera == null || heldBody == null)
            return;

        Rigidbody body = heldBody;
        ReleaseBag();
        body.AddForce(aimCamera.forward * throwSpeed, ForceMode.VelocityChange);
    }
}
