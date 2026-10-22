using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BankDepositZone : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset markerFont;
    [SerializeField] private Color markerColor = new Color(0.2f, 0.85f, 0.35f);

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Start()
    {
        CreateDeliveryMarker(GetComponent<BoxCollider>());
    }

    private void OnTriggerEnter(Collider other)
    {
        CartCargo cart = other.GetComponentInParent<CartCargo>();
        if (cart == null)
            return;

        int bagsBanked = cart.BankCargo();
        if (bagsBanked > 0)
            Debug.Log($"Banked {bagsBanked} bags. Total banked: {cart.BankedBagCount}.");
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null)
            return;

        Gizmos.color = new Color(0.2f, 0.85f, 0.35f, 0.25f);
        Matrix4x4 previousMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(box.center, box.size);
        Gizmos.color = new Color(0.2f, 0.85f, 0.35f, 1f);
        Gizmos.DrawWireCube(box.center, box.size);
        Gizmos.matrix = previousMatrix;
    }

    private void CreateDeliveryMarker(BoxCollider trigger)
    {
        Vector3 padWorldPosition = new Vector3(transform.position.x, 0.035f, transform.position.z);
        Vector3 padLocalPosition = transform.InverseTransformPoint(padWorldPosition);
        CreateMarkerPiece(
            "Cart Delivery Pad",
            padLocalPosition,
            new Vector3(trigger.size.x, 0.07f, trigger.size.z),
            markerColor
        );

        Vector3 signPosition = new Vector3(0f, 1.7f, trigger.size.z * 0.5f + 0.2f);
        CreateMarkerPiece(
            "Cart Delivery Sign",
            signPosition,
            new Vector3(trigger.size.x * 0.75f, 0.7f, 0.16f),
            new Color(0.08f, 0.38f, 0.16f)
        );

        GameObject labelObject = new GameObject("Cart Delivery Label");
        labelObject.transform.SetParent(transform, false);
        labelObject.transform.localPosition = signPosition + new Vector3(0f, 0f, 0.1f);
        labelObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        TextMeshPro label = labelObject.AddComponent<TextMeshPro>();
        label.font = markerFont;
        label.text = "CART DELIVERY";
        label.fontSize = 3f;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.rectTransform.sizeDelta = new Vector2(trigger.size.x * 0.7f, 0.6f);
    }

    private void CreateMarkerPiece(string objectName, Vector3 localPosition, Vector3 localScale, Color color)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marker.name = objectName;
        marker.transform.SetParent(transform, false);
        marker.transform.localPosition = localPosition;
        marker.transform.localScale = localScale;

        Collider markerCollider = marker.GetComponent<Collider>();
        if (markerCollider != null)
            markerCollider.enabled = false;

        Renderer markerRenderer = marker.GetComponent<Renderer>();
        if (markerRenderer == null)
            return;

        MaterialPropertyBlock colorBlock = new MaterialPropertyBlock();
        markerRenderer.GetPropertyBlock(colorBlock);
        colorBlock.SetColor("_BaseColor", color);
        colorBlock.SetColor("_Color", color);
        markerRenderer.SetPropertyBlock(colorBlock);
    }
}