using UnityEngine;

public class CannonShooter : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;   // Drag the prefab here
    public Transform firePoint;           // Empty child at the cannon tip
    public float launchForce = 15f;       // Initial velocity (m/s)

    [Header("Physics (kinematic formula)")]
    [Tooltip("Launch angle in degrees above horizontal. 0 = straight forward.")]
    public float launchAngle = 15f;
    public float gravity = 9.81f;         // Informational only, Unity uses Physics.gravity

    [Header("Optional: aim formula")]
    public bool useBallisticFormula = true;

    void Update()
    {
        // Fire on Space OR Left Mouse Button
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void Fire()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Assign projectilePrefab and firePoint in the Inspector!");
            return;
        }

        // 1. Instantiate the projectile
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // 2. Ensure Rigidbody exists (in case prefab is missing it)
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb == null) rb = proj.AddComponent<Rigidbody>();

        // 3. Compute launch velocity
        Vector3 velocity;

        if (useBallisticFormula)
        {
            // Kinematic formula:
            // X = (1/2) * a * t^2  +  v * t  (position over time)
            // For a projectile launched at angle θ with speed v:
            //   vx = v * cos(θ)
            //   vy = v * sin(θ)
            float rad = launchAngle * Mathf.Deg2Rad;
            Vector3 dir = firePoint.forward;                  // where cannon points
            Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;

            // apply the angled launch on top of the cannon's facing direction
            velocity = flatDir * (launchForce * Mathf.Cos(rad))
                     + Vector3.up * (launchForce * Mathf.Sin(rad));
        }
        else
        {
            // simple version: just shoot forward
            velocity = firePoint.forward * launchForce;
        }

        // 4. Apply initial velocity to the rigidbody (physics does the rest)
        rb.linearVelocity = velocity;   // (Unity 6 / 2023+)
        // For older Unity versions use:
        // rb.velocity = velocity;
    }
}