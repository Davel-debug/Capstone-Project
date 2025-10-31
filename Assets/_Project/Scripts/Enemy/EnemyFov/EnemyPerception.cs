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

    [Header("Debug Options")]
    public bool showDebugLogs = false;
    public bool showVisionCone = true;
    public Color visionColor = new Color(1f, 0.8f, 0f, 0.25f);
    public Color borderColor = Color.red;
    public Color hitColor = Color.green;

    [Header("Runtime Info (read-only)")]
    public bool playerVisible;

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
        Vector3 playerTarget = player.position + Vector3.up * 1.2f;
        Vector3 toPlayer = playerTarget - origin;
        float distance = toPlayer.magnitude;
        Vector3 dirToPlayer = toPlayer.normalized;

        // Verifica angolo base
        float angleToPlayer = Vector3.Angle(eyePoint.forward, dirToPlayer);
        if (angleToPlayer < viewAngle / 2f && distance <= viewDistance)
        {
            // Controlla se c'è un ostacolo in mezzo
            if (!Physics.Raycast(origin, dirToPlayer, distance, obstacleMask))
                canSee = true;
        }

        if (showDebugLogs)
        {
            Debug.Log($"[EnemyPerception] playerInFOV={canSee}, angle={angleToPlayer:F1}, dist={distance:F1}");
        }

        return canSee;
    }

    public bool CanSeePlayer() => playerVisible;

    private void OnDrawGizmos()
    {
        if (!showVisionCone || eyePoint == null) return;

        Vector3 origin = eyePoint.position;
        Gizmos.color = playerVisible ? hitColor : visionColor;

        // Disegna cerchio raggio di visione
        Gizmos.DrawWireSphere(origin, viewDistance);

        // Calcola bordi FOV
        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2, false);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = borderColor;
        Gizmos.DrawLine(origin, origin + leftBoundary * viewDistance);
        Gizmos.DrawLine(origin, origin + rightBoundary * viewDistance);

        // Se il player è visibile, disegna una linea verde verso di lui
        if (playerVisible && player != null)
        {
            Gizmos.color = hitColor;
            Gizmos.DrawLine(origin, player.position + Vector3.up * 1.2f);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += eyePoint.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
