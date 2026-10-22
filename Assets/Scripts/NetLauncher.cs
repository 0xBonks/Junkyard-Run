using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetLauncher : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform aimCamera;
    [SerializeField] private TMP_Text netCountText;
    [SerializeField] private float launchSpeed = 18f;
    [SerializeField] private int startingNets = 5;
    [SerializeField] private int maxNets = 10;

    private int netsRemaining;

    private void Awake()
    {
        netsRemaining = Mathf.Clamp(startingNets, 0, maxNets);
        UpdateNetCount();
    }

    private void Update()
    {
        if (Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame ||
            Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        if (netsRemaining <= 0)
        {
            Debug.Log("No nets remaining.");
            return;
        }

        if (projectilePrefab == null || muzzle == null || aimCamera == null)
        {
            Debug.LogWarning("Assign the projectile, Muzzle, and Aim Camera on NetLauncher.");
            return;
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            muzzle.position,
            Quaternion.identity
        );

        if (!projectile.TryGetComponent<Rigidbody>(out Rigidbody body))
        {
            Debug.LogError("NetProjectile prefab needs a Rigidbody.");
            Destroy(projectile);
            return;
        }

        body.AddForce(aimCamera.forward * launchSpeed, ForceMode.VelocityChange);
        netsRemaining--;
        UpdateNetCount();
    }

    public bool RefillNets()
    {
        if (netsRemaining >= maxNets)
            return false;

        netsRemaining = maxNets;
        UpdateNetCount();
        return true;
    }

    private void UpdateNetCount()
    {
        if (netCountText != null)
            netCountText.text = $"NETS {netsRemaining}/{maxNets}";
    }
}
