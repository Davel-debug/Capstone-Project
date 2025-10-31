using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


public class EnemyPerception : MonoBehaviour
{
    [Header("Sight Settings")]
    public float viewDistance = 20f;
    [Range(1, 180)] public float horizontalAngle = 60f;
    [Range(1, 90)] public float verticalAngle = 45f; // nuovo campo visivo verticale
    public float memoryDuration = 3f;
    public LayerMask obstacleMask;
    public Transform eyePoint;

    [Header("Debug Options")]
    public bool showVisionCone = true;
    public Color visionColor = new(1f, 0.8f, 0f, 0.25f);
    public Color seenColor = Color.green;
    public Color memoryColor = Color.blue;

    [Header("Runtime Info (read-only)")]
    public bool playerVisible;
    public Vector3 lastKnownPosition;
    public List<Vector3> recentPositions = new();

    private Transform player;
    private float timeSinceLastSeen = Mathf.Infinity;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (eyePoint == null) eyePoint = transform;
    }

    private void Update()
    {
        bool canSee = CheckFOV();

        if (canSee)
        {
            playerVisible = true;
            lastKnownPosition = player.position;
            timeSinceLastSeen = 0f;

            if (recentPositions.Count == 0 || Vector3.Distance(recentPositions[^1], lastKnownPosition) > 1f)
                recentPositions.Add(lastKnownPosition);
        }
        else
        {
            playerVisible = false;
            timeSinceLastSeen += Time.deltaTime;
        }
    }

    private bool CheckFOV()
    {
        if (player == null) return false;

        Vector3 origin = eyePoint.position;
        Vector3 playerTarget = player.position + Vector3.up * 1.2f;
        Vector3 toPlayer = playerTarget - origin;
        float distance = toPlayer.magnitude;

        if (distance > viewDistance)
            return false;

        Vector3 dirToPlayer = toPlayer.normalized;
        float horizontal = Vector3.Angle(eyePoint.forward, new Vector3(dirToPlayer.x, 0, dirToPlayer.z));
        float vertical = Mathf.Abs(Vector3.SignedAngle(eyePoint.forward, dirToPlayer, eyePoint.right));

        if (horizontal < horizontalAngle / 2f && vertical < verticalAngle / 2f)
        {
            if (!Physics.Raycast(origin, dirToPlayer, distance, obstacleMask))
                return true;
        }

        return false;
    }

    public bool CanSeePlayer() => playerVisible;

    public bool HasRecentSight() => timeSinceLastSeen < memoryDuration;

    public Vector3 GetLastKnownPosition() => lastKnownPosition;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (eyePoint == null) return;

        Vector3 origin = eyePoint.position;
        Handles.color = new Color(visionColor.r, visionColor.g, visionColor.b, 0.2f);

        // Disegna FOV 3D
        Handles.DrawSolidArc(
            origin,
            Vector3.up,
            Quaternion.Euler(0, -horizontalAngle / 2f, 0) * eyePoint.forward,
            horizontalAngle,
            viewDistance
        );
    }
#endif

}
