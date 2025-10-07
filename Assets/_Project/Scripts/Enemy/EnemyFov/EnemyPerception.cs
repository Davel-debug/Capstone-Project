using UnityEngine;

[ExecuteAlways]
public class EnemyPerception : MonoBehaviour
{
    [Header("Sight Settings")]
    public float viewDistance = 20f;
    [Range(1, 180)] public float viewAngle = 60f;
    public int raysPerSide = 10;
    public LayerMask obstacleMask;
    public Transform eyePoint;

    [Header("Detection Settings")]
    [Tooltip("Tempo minimo in secondi per confermare di aver visto il player")]
    public float detectionTime = 1.5f;
    [Tooltip("Soglia probabilità (0–1) per considerare il player effettivamente visto")]
    public float detectionThreshold = 0.6f;

    [Header("Debug Options")]
    public bool showDebugLogs = false;
    public bool showVisionCone = true;
    public Color visionColor = new Color(1f, 0.8f, 0f, 0.25f);

    [Header("Runtime Info (read-only)")]
    public bool playerVisible;
    public float currentVisibilityValue = 0f;
    private float timeInSight = 0f;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (eyePoint == null) eyePoint = transform;
    }

    private void Update()
    {
        playerVisible = CheckFOV();
    }

    private bool CheckFOV()
    {
        if (player == null) return false;

        bool canSee = false;
        Vector3 origin = eyePoint.position;
        Vector3 playerTarget = player.position + Vector3.up * 1.2f; // Targeting upper body
        Vector3 toPlayer = playerTarget - origin;
        float distance = toPlayer.magnitude;
        Vector3 dirToPlayer = toPlayer.normalized;

        float baseAngle = eyePoint.eulerAngles.y;
        float halfAngle = viewAngle / 2f;

        //Scansiona più raggi nel cono visivo
        for (int i = -raysPerSide; i <= raysPerSide; i++)
        {
            float angleOffset = (halfAngle / raysPerSide) * i;
            Vector3 rayDir = Quaternion.Euler(0, angleOffset, 0) * eyePoint.forward;

            if (Physics.Raycast(origin, rayDir, out RaycastHit hit, viewDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    canSee = true;
                    break;
                }
            }
        }

        //Aggiorna visibilità progressiva
        if (canSee)
        {
            timeInSight += Time.deltaTime;
        }
        else
        {
            timeInSight = Mathf.Max(0f, timeInSight - Time.deltaTime * 2f); // decadimento rapido
        }

        //Calcola probabilità distanza
        float distanceFactor = Mathf.Clamp01(1f - (distance / viewDistance));
        float timeFactor = Mathf.Clamp01(timeInSight / detectionTime);

        currentVisibilityValue = distanceFactor * timeFactor;

        if (showDebugLogs)
        {
            Debug.Log($"[EnemyPerception] dist:{distanceFactor:F2}, tempo:{timeFactor:F2}, tot:{currentVisibilityValue:F2}");
        }

        return currentVisibilityValue >= detectionThreshold;
    }
    public bool CanSeePlayer()
    {
        return playerVisible;
    }

    private void OnDrawGizmos()
    {
        if (!showVisionCone || eyePoint == null) return;

        Gizmos.color = playerVisible ? Color.red : visionColor;
        Vector3 forward = eyePoint.forward * viewDistance;

        int segments = raysPerSide * 2;
        Vector3 prev = eyePoint.position + (Quaternion.Euler(0, -viewAngle / 2f, 0) * forward);

        for (int i = 1; i <= segments; i++)
        {
            float step = -viewAngle / 2f + (viewAngle / segments) * i;
            Vector3 next = eyePoint.position + (Quaternion.Euler(0, step, 0) * forward);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}
