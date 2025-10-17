using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    [Header("Player Tracking")]
    [SerializeField] private Transform player;
    [SerializeField] private float noiseRadius = 10f; // errore casuale in metri
    [SerializeField] private float searchNoiseRadius = 5f; // errore casuale in metri per la ricerca specifica
    [SerializeField] private float updateDelay = 0.5f; // ogni quanto aggiorna la posizione
    private Vector3 lastKnownPosition;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        InvokeRepeating(nameof(UpdatePlayerPosition), 0f, updateDelay);
    }

    private void UpdatePlayerPosition()
    {
        if (player == null) return;
        lastKnownPosition = player.position;
    }

    // Restituisce la posizione con un po' di errore casuale (per il Search)
    public Vector3 GetApproximatePlayerPosition()
    {
        Vector2 offset = Random.insideUnitCircle * noiseRadius;
        return lastKnownPosition + new Vector3(offset.x, 0, offset.y);
    }
    public Vector3 GetApproximateSearchPlayerPosition()
    {
        Vector2 offset = Random.insideUnitCircle * searchNoiseRadius;
        return lastKnownPosition + new Vector3(offset.x, 0, offset.y);
    }
    // Restituisce la posizione esatta (solo se concessa dallo stato)
    public Vector3 GetExactPlayerPosition()
    {
        return lastKnownPosition;
    }
}
