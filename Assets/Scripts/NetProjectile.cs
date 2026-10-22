using UnityEngine;

public class NetProjectile : MonoBehaviour
{
    [SerializeField] private float missLifetime = 5f;
    [SerializeField] private float bagScale = 1.4f;

    private float age;
    private bool caughtScrap;
    public bool IsCaughtBag => caughtScrap;


    private void Update()
    {
        if (caughtScrap)
            return;

        age += Time.deltaTime;
        if (age >= missLifetime)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (caughtScrap)
            return;

        ScrapThief thief = collision.collider.GetComponentInParent<ScrapThief>();
        if (thief != null)
        {
            GameObject recoveredScrap = thief.TakeStolenScrapForNet();
            if (recoveredScrap != null)
                CatchScrap(recoveredScrap);
            return;
        }

        if (collision.collider.CompareTag("Scrap"))
            CatchScrap(collision.collider.gameObject);
    }

    private void CatchScrap(GameObject scrap)
    {
        caughtScrap = true;
        transform.localScale = Vector3.one * bagScale;

        Rigidbody bagBody = GetComponent<Rigidbody>();
        if (bagBody != null)
        {
            bagBody.linearVelocity = Vector3.zero;
            bagBody.angularVelocity = Vector3.zero;
        }

        Rigidbody scrapBody = scrap.GetComponent<Rigidbody>();
        if (scrapBody != null)
        {
            scrapBody.linearVelocity = Vector3.zero;
            scrapBody.angularVelocity = Vector3.zero;
            scrapBody.isKinematic = true;
            scrapBody.detectCollisions = false;
            Destroy(scrapBody);
        }

        foreach (Collider itemCollider in scrap.GetComponentsInChildren<Collider>())
            itemCollider.enabled = false;

        scrap.transform.SetParent(transform, true);
        scrap.transform.position = transform.position;
        scrap.transform.rotation = transform.rotation;
    }
}
