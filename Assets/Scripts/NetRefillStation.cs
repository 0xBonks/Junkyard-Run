using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class NetRefillStation : MonoBehaviour
{
    [SerializeField] private NetLauncher netLauncher;
    [SerializeField] private TMP_FontAsset markerFont;
    [SerializeField] private Color stationColor = new Color(0.12f, 0.48f, 0.78f);

    private bool playerInRange;
    private GUIStyle promptStyle;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;

        Rigidbody stationBody = GetComponent<Rigidbody>();
        stationBody.isKinematic = true;
        stationBody.useGravity = false;

        CreateStationMarker();
    }

    private void Update()
    {
        if (!playerInRange || Keyboard.current == null || !Keyboard.current.rKey.wasPressedThisFrame)
            return;

        if (netLauncher != null && netLauncher.RefillNets())
            Debug.Log("Net launcher refilled.");
        else
            Debug.Log("Net launcher is already full.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() == null)
            return;

        playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
            playerInRange = false;
    }

    private void OnGUI()
    {
        if (!playerInRange)
            return;

        if (promptStyle == null)
        {
            promptStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };
            promptStyle.normal.textColor = Color.white;
        }

        GUI.Box(new Rect(Screen.width * 0.5f - 170f, 24f, 340f, 48f), "NET REFILL  |  PRESS R", promptStyle);
    }

    private void CreateStationMarker()
    {
        GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pad.name = "Net Refill Pad";
        pad.transform.SetParent(transform, false);
        pad.transform.localPosition = transform.InverseTransformPoint(
            new Vector3(transform.position.x, 0.04f, transform.position.z)
        );
        pad.transform.localScale = new Vector3(3.2f, 0.08f, 3.2f);
        SetMarkerColor(pad, stationColor);

        GameObject sign = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sign.name = "Net Refill Sign";
        sign.transform.SetParent(transform, false);
        sign.transform.localPosition = new Vector3(0f, 1.7f, 1.5f);
        sign.transform.localScale = new Vector3(2.6f, 0.65f, 0.16f);
        SetMarkerColor(sign, stationColor);

        GameObject labelObject = new GameObject("Net Refill Label");
        labelObject.transform.SetParent(transform, false);
        labelObject.transform.localPosition = new Vector3(0f, 1.7f, 1.4f);
        labelObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        TextMeshPro label = labelObject.AddComponent<TextMeshPro>();
        label.font = markerFont;
        label.text = "NET REFILL";
        label.fontSize = 2.2f;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.rectTransform.sizeDelta = new Vector2(2.5f, 0.5f);
    }

    private void SetMarkerColor(GameObject marker, Color color)
    {
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