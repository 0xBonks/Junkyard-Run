using UnityEngine;

public class SafetyFence : MonoBehaviour
{
    [SerializeField, Min(5f)] private float halfExtent = 19.5f;
    [SerializeField, Min(1f)] private float fenceHeight = 2.6f;
    [SerializeField, Min(0.1f)] private float fenceThickness = 0.45f;
    [SerializeField, Min(0.1f)] private float postSpacing = 4f;
    [SerializeField, Min(2)] private int railCount = 3;
    [SerializeField] private Color fenceColor = new Color(0.2f, 0.25f, 0.28f);

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        float fullLength = halfExtent * 2f;
        float sidePosition = halfExtent - fenceThickness * 0.5f;
        float postSize = fenceThickness * 1.25f;

        AddCollisionPanel("North Collision", new Vector3(0f, fenceHeight * 0.5f, sidePosition),
            new Vector3(fullLength + fenceThickness, fenceHeight, fenceThickness));
        AddCollisionPanel("South Collision", new Vector3(0f, fenceHeight * 0.5f, -sidePosition),
            new Vector3(fullLength + fenceThickness, fenceHeight, fenceThickness));
        AddCollisionPanel("East Collision", new Vector3(sidePosition, fenceHeight * 0.5f, 0f),
            new Vector3(fenceThickness, fenceHeight, fullLength + fenceThickness));
        AddCollisionPanel("West Collision", new Vector3(-sidePosition, fenceHeight * 0.5f, 0f),
            new Vector3(fenceThickness, fenceHeight, fullLength + fenceThickness));

        AddFenceEdge("North", true, sidePosition, fullLength, postSize);
        AddFenceEdge("South", true, -sidePosition, fullLength, postSize);
        AddFenceEdge("East", false, sidePosition, fullLength, postSize);
        AddFenceEdge("West", false, -sidePosition, fullLength, postSize);
    }

    private void AddCollisionPanel(string objectName, Vector3 localPosition, Vector3 size)
    {
        GameObject panel = new GameObject(objectName);
        panel.transform.SetParent(transform, false);
        panel.transform.localPosition = localPosition;
        panel.transform.localScale = size;
        panel.AddComponent<BoxCollider>();
    }

    private void AddFenceEdge(string edgeName, bool runsAlongX, float sidePosition, float fullLength, float postSize)
    {
        float halfLength = fullLength * 0.5f;
        int postCount = Mathf.CeilToInt(fullLength / postSpacing);

        for (int index = 0; index <= postCount; index++)
        {
            float alongEdge = Mathf.Lerp(-halfLength, halfLength, (float)index / postCount);
            Vector3 postPosition = runsAlongX
                ? new Vector3(alongEdge, fenceHeight * 0.5f, sidePosition)
                : new Vector3(sidePosition, fenceHeight * 0.5f, alongEdge);
            Vector3 postScale = new Vector3(postSize, fenceHeight, postSize);
            CreateFencePiece($"{edgeName} Post {index}", postPosition, postScale);
        }

        float railSpacing = fenceHeight / (railCount + 1f);
        Vector3 railScale = runsAlongX
            ? new Vector3(fullLength, fenceThickness * 0.65f, fenceThickness * 0.65f)
            : new Vector3(fenceThickness * 0.65f, fenceThickness * 0.65f, fullLength);

        for (int index = 1; index <= railCount; index++)
        {
            Vector3 railPosition = runsAlongX
                ? new Vector3(0f, railSpacing * index, sidePosition)
                : new Vector3(sidePosition, railSpacing * index, 0f);
            CreateFencePiece($"{edgeName} Rail {index}", railPosition, railScale);
        }
    }

    private void CreateFencePiece(string objectName, Vector3 localPosition, Vector3 localScale)
    {
        GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
        piece.name = objectName;
        piece.transform.SetParent(transform, false);
        piece.transform.localPosition = localPosition;
        piece.transform.localScale = localScale;

        Collider pieceCollider = piece.GetComponent<Collider>();
        if (pieceCollider != null)
            pieceCollider.enabled = false;

        Renderer pieceRenderer = piece.GetComponent<Renderer>();
        if (pieceRenderer == null)
            return;

        MaterialPropertyBlock colorBlock = new MaterialPropertyBlock();
        pieceRenderer.GetPropertyBlock(colorBlock);
        colorBlock.SetColor(BaseColorId, fenceColor);
        colorBlock.SetColor(ColorId, fenceColor);
        pieceRenderer.SetPropertyBlock(colorBlock);
    }
}