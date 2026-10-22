using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class ScrapThief : MonoBehaviour
{
    [SerializeField] private float approachSpeed = 2.5f;
    [SerializeField] private float escapeSpeed = 4f;
    [SerializeField] private float stealDistance = 1.2f;
    [SerializeField] private float escapeDistance = 0.8f;
    [SerializeField] private float stunDuration = 2.5f;
    [SerializeField] private float arenaHalfExtent = 19.5f;
    [SerializeField] private float escapeDropInset = 2f;
    [SerializeField] private Vector3 escapePoint = new Vector3(9f, 0.8f, 9f);
    [SerializeField] private Vector3 carryOffset = new Vector3(0f, 0.8f, -0.45f);

    private Rigidbody body;
    private CapsuleCollider bodyCollider;
    private Renderer bodyRenderer;
    private GameObject carryingIndicator;
    private GameObject targetScrap;
    private GameObject stolenScrap;
    private Vector3 spawnPosition;
    private float nextTargetSearchTime;
    private float stunnedUntil;
    private bool isStunned;
    private bool returningToStart;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        bodyCollider = GetComponent<CapsuleCollider>();
        bodyRenderer = GetComponentInChildren<Renderer>();
        spawnPosition = transform.position;
        CreateCarryingIndicator();

        if (bodyRenderer != null)
        {
            MaterialPropertyBlock colorBlock = new MaterialPropertyBlock();
            bodyRenderer.GetPropertyBlock(colorBlock);
            Color thiefColor = new Color(0.72f, 0.18f, 0.08f);
            colorBlock.SetColor("_BaseColor", thiefColor);
            colorBlock.SetColor("_Color", thiefColor);
            bodyRenderer.SetPropertyBlock(colorBlock);
        }
    }

    private void FixedUpdate()
    {
        if (isStunned)
        {
            if (Time.time < stunnedUntil)
                return;

            FinishStun();
        }

        KeepInsideArena();

        if (returningToStart)
        {
            MoveToward(spawnPosition, escapeSpeed);
            if (HorizontalDistanceSquared(body.position, spawnPosition) <= escapeDistance * escapeDistance)
            {
                returningToStart = false;
                body.linearVelocity = new Vector3(0f, body.linearVelocity.y, 0f);
                nextTargetSearchTime = Time.time;
            }

            return;
        }

        if (stolenScrap != null)
        {
            MoveToward(escapePoint, escapeSpeed);
            if (HorizontalDistanceSquared(body.position, escapePoint) <= escapeDistance * escapeDistance)
                EscapeWithScrap();

            return;
        }

        if (!IsAvailableScrap(targetScrap))
            targetScrap = null;

        if (targetScrap == null && Time.time >= nextTargetSearchTime)
        {
            targetScrap = FindClosestAvailableScrap();
            nextTargetSearchTime = Time.time + 1f;
        }

        if (targetScrap == null)
            return;

        Vector3 targetPosition = targetScrap.transform.position;
        if (HorizontalDistanceSquared(body.position, targetPosition) <= stealDistance * stealDistance)
        {
            StealTargetScrap();
            return;
        }

        MoveToward(targetPosition, approachSpeed);
    }

    public GameObject TakeStolenScrapForNet()
    {
        if (stolenScrap == null)
            return null;

        GameObject recoveredScrap = stolenScrap;
        stolenScrap = null;
        StunAtSpawn();
        Debug.Log("Net caught the thief. Stolen scrap recovered; the thief is stunned briefly.");
        return recoveredScrap;
    }

    private GameObject FindClosestAvailableScrap()
    {
        GameObject closestScrap = null;
        float closestDistance = float.PositiveInfinity;

        foreach (GameObject scrap in GameObject.FindGameObjectsWithTag("Scrap"))
        {
            if (!IsAvailableScrap(scrap))
                continue;

            float distance = HorizontalDistanceSquared(body.position, scrap.transform.position);
            if (distance >= closestDistance)
                continue;

            closestScrap = scrap;
            closestDistance = distance;
        }

        return closestScrap;
    }

    private bool IsAvailableScrap(GameObject scrap)
    {
        return scrap != null &&
            scrap.CompareTag("Scrap") &&
            scrap.GetComponentInParent<NetProjectile>() == null &&
            scrap.GetComponentInParent<ScrapThief>() == null &&
            HasEnabledCollider(scrap);
    }

    private bool HasEnabledCollider(GameObject scrap)
    {
        foreach (Collider itemCollider in scrap.GetComponentsInChildren<Collider>())
        {
            if (itemCollider.enabled)
                return true;
        }

        return false;
    }

    private void StealTargetScrap()
    {
        if (targetScrap == null)
            return;

        stolenScrap = targetScrap;
        targetScrap = null;

        Rigidbody scrapBody = stolenScrap.GetComponent<Rigidbody>();
        if (scrapBody != null)
        {
            scrapBody.linearVelocity = Vector3.zero;
            scrapBody.angularVelocity = Vector3.zero;
            scrapBody.isKinematic = true;
            scrapBody.detectCollisions = false;
        }

        foreach (Collider itemCollider in stolenScrap.GetComponentsInChildren<Collider>())
            itemCollider.enabled = false;

        stolenScrap.transform.SetParent(transform, false);
        stolenScrap.transform.localPosition = carryOffset;
        carryingIndicator.SetActive(true);
        Debug.LogWarning("A thief stole scrap! Net it before it escapes.");
    }

    private void EscapeWithScrap()
    {
        DropStolenScrapAtEscapePoint();
        stolenScrap = null;
        returningToStart = true;
        body.linearVelocity = Vector3.zero;
        Debug.LogWarning("The thief dropped the scrap by the far fence. Retrieve it while the thief returns to the yard.");
    }

    private void DropStolenScrapAtEscapePoint()
    {
        if (stolenScrap == null)
            return;

        GameObject droppedScrap = stolenScrap;
        droppedScrap.transform.SetParent(null, true);
        float safeExtent = Mathf.Max(1f, arenaHalfExtent - escapeDropInset);
        Vector3 dropPosition = new Vector3(
            Mathf.Clamp(escapePoint.x, -safeExtent, safeExtent),
            0.5f,
            Mathf.Clamp(escapePoint.z, -safeExtent, safeExtent)
        );
        droppedScrap.transform.position = dropPosition;
        droppedScrap.transform.rotation = Quaternion.identity;

        Rigidbody scrapBody = droppedScrap.GetComponent<Rigidbody>();
        if (scrapBody != null)
        {
            scrapBody.position = dropPosition;
            scrapBody.rotation = Quaternion.identity;
            scrapBody.isKinematic = false;
            scrapBody.detectCollisions = true;
            scrapBody.linearVelocity = Vector3.zero;
            scrapBody.angularVelocity = Vector3.zero;
        }

        foreach (Collider itemCollider in droppedScrap.GetComponentsInChildren<Collider>())
            itemCollider.enabled = true;
    }

    private void StunAtSpawn()
    {
        targetScrap = null;
        returningToStart = false;
        isStunned = true;
        stunnedUntil = Time.time + stunDuration;

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.isKinematic = true;
        body.detectCollisions = false;
        bodyCollider.enabled = false;
        carryingIndicator.SetActive(false);
        if (bodyRenderer != null)
            bodyRenderer.enabled = true;

        transform.position = spawnPosition;
    }

    private void FinishStun()
    {
        isStunned = false;
        body.isKinematic = false;
        body.detectCollisions = true;
        bodyCollider.enabled = true;
        if (bodyRenderer != null)
            bodyRenderer.enabled = true;

        body.position = spawnPosition;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        carryingIndicator.SetActive(false);
        nextTargetSearchTime = Time.time;
    }

    private void KeepInsideArena()
    {
        Vector3 position = body.position;
        float safeExtent = Mathf.Max(1f, arenaHalfExtent - bodyCollider.radius - 0.25f);
        bool outsideBounds = Mathf.Abs(position.x) > safeExtent || Mathf.Abs(position.z) > safeExtent;
        bool fellBelowGround = position.y < -1f;

        if (!outsideBounds && !fellBelowGround)
            return;

        if (stolenScrap != null)
        {
            DropStolenScrapAtEscapePoint();
            stolenScrap = null;
            carryingIndicator.SetActive(false);
        }

        position.x = Mathf.Clamp(position.x, -safeExtent, safeExtent);
        position.z = Mathf.Clamp(position.z, -safeExtent, safeExtent);
        position.y = spawnPosition.y;
        body.position = position;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        returningToStart = true;
        Debug.LogWarning("The thief was kept inside the yard and recovered from the edge.");
    }

    private void CreateCarryingIndicator()
    {
        carryingIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        carryingIndicator.name = "Carrying Alert";
        carryingIndicator.transform.SetParent(transform, false);
        carryingIndicator.transform.localPosition = new Vector3(0f, 1.45f, 0f);
        carryingIndicator.transform.localScale = Vector3.one * 0.35f;

        Collider indicatorCollider = carryingIndicator.GetComponent<Collider>();
        if (indicatorCollider != null)
            indicatorCollider.enabled = false;

        Renderer indicatorRenderer = carryingIndicator.GetComponent<Renderer>();
        if (indicatorRenderer != null)
        {
            MaterialPropertyBlock colorBlock = new MaterialPropertyBlock();
            indicatorRenderer.GetPropertyBlock(colorBlock);
            Color alertColor = new Color(1f, 0.08f, 0.02f);
            colorBlock.SetColor("_BaseColor", alertColor);
            colorBlock.SetColor("_Color", alertColor);
            indicatorRenderer.SetPropertyBlock(colorBlock);
        }

        carryingIndicator.SetActive(false);
    }

    private void MoveToward(Vector3 destination, float speed)
    {
        Vector3 direction = destination - body.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            body.linearVelocity = new Vector3(0f, body.linearVelocity.y, 0f);
            return;
        }

        direction.Normalize();
        body.linearVelocity = new Vector3(
            direction.x * speed,
            body.linearVelocity.y,
            direction.z * speed
        );
        body.MoveRotation(Quaternion.LookRotation(direction, Vector3.up));
    }

    private static float HorizontalDistanceSquared(Vector3 first, Vector3 second)
    {
        first.y = 0f;
        second.y = 0f;
        return (first - second).sqrMagnitude;
    }
}