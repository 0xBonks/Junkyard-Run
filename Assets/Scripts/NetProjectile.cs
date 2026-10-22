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

        if (caughtScrap || !collision.collider.CompareTag("Scrap"))
            return;

        GameObject scrap = collision.collider.gameObject;

        caughtScrap = true;

        Rigidbody scrapBody = scrap.GetComponent<Rigidbody>();
        if (scrapBody != null)
        {
            scrapBody.isKinematic = true;
            scrapBody.detectCollisions = false;
            Destroy(scrapBody);
        }

        foreach (Collider itemCollider in scrap.GetComponents<Collider>())
            itemCollider.enabled = false;

        transform.localScale = Vector3.one * bagScale;

        scrap.transform.SetParent(transform, true);
        Rigidbody bagBody = GetComponent<Rigidbody>();
        if (bagBody != null)
        {
            bagBody.linearVelocity = Vector3.zero;
            bagBody.angularVelocity = Vector3.zero;
        }
        scrap.transform.position = transform.position;
        scrap.transform.rotation = transform.rotation;
    }

}
