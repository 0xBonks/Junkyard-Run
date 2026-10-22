using TMPro;
using UnityEngine;

public class CartCargo : MonoBehaviour
{
    [SerializeField, Range(1, 3)] private int capacity = 3;
    [SerializeField] private TMP_Text cargoCountText;

    private int cargoCount;

    private void Start()
    {
        UpdateCounter();
    }

    private void OnTriggerEnter(Collider other)
    {
        NetProjectile bag = other.GetComponentInParent<NetProjectile>();

        if (bag == null || !bag.IsCaughtBag)
            return;

        if (cargoCount >= capacity)
        {
            Debug.Log("Cart is full.");
            return;
        }

        Rigidbody body = bag.GetComponent<Rigidbody>();
        Collider bagCollider = bag.GetComponent<Collider>();

        if (body == null || bagCollider == null)
            return;

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.isKinematic = true;
        body.detectCollisions = false;
        bagCollider.enabled = false;

        bag.transform.SetParent(transform, true);
        bag.transform.localPosition = GetSlotPosition(cargoCount);
        bag.transform.localRotation = Quaternion.identity;

        cargoCount++;
        UpdateCounter();
    }

    private Vector3 GetSlotPosition(int index)
    {
        switch (index)
        {
            case 0: return new Vector3(-0.7f, 1f, 0f);
            case 1: return new Vector3(0.7f, 1f, 0f);
            default: return new Vector3(0f, 2.4f, 0f);
        }
    }

    private void UpdateCounter()
    {
        if (cargoCountText != null)
            cargoCountText.text = $"CARGO {cargoCount}/{capacity}";
    }
}
