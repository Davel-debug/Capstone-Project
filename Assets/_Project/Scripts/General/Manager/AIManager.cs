using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    [Header("Player Tracking")]
    [SerializeField] private Transform player;
    [SerializeField] private float noiseRadius = 10f; // errore casuale in metri
    [SerializeField] private float searchNoiseRadius = 5f; // errore casuale in metri per la ricerca specifica
    [SerializeField] private float updateDelay = 0.5f; // ogni quanto aggiorna la posizione

    [Header("Stationary Check")]
    [SerializeField] private float stationaryThreshold = 0.2f; // distanza minima per considerarlo fermo
    [SerializeField] private float stationaryTimeLimit = 5f;   // tempo in secondi prima di dire “è fermo”

    private Vector3 lastKnownPosition;
    private Vector3 lastPlayerPosition;
    private float stationaryTimer = 0f;
    public bool activeTracking = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        lastPlayerPosition = player.position;
        InvokeRepeating(nameof(UpdatePlayerPosition), 0f, updateDelay);
    }

    private void UpdatePlayerPosition()
    {
        if (!activeTracking || player == null) return;
        lastKnownPosition = player.position;
    }

    // Forza l'aggiornamento della posizione: da chiamare quando il player raccoglie un oggetto "rumoroso"
    public void AlertEnemiesToPlayerPosition()
    {
        if (player == null) return;

        lastKnownPosition = player.position;
        Debug.Log($"[AIManager] ALERT — Player rilevato in {lastKnownPosition}");
        var allPerceptions = GameObject.FindObjectsOfType<EnemyPerception>();
        foreach (var p in allPerceptions)
        {
            p.ForceLastSeenPosition(lastKnownPosition);
        }
    }

    // Restituisce posizione con errore casuale (per il Search)
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

    // Posizione esatta (se concessa dallo stato)
    public Vector3 GetExactPlayerPosition()
    {
        return lastKnownPosition;
    }

    // Controlla se il player si è mosso o è fermo da troppo tempo
    public bool PlayerMoved()
    {
        if (player == null) return false;

        float distance = Vector3.Distance(player.position, lastPlayerPosition);

        if (distance < stationaryThreshold)
        {
            stationaryTimer += Time.deltaTime;
            if (stationaryTimer > stationaryTimeLimit)
            {
                stationaryTimer = 0f;
                lastPlayerPosition = player.position;
                return false; // fermo troppo a lungo
            }
        }
        else
        {
            stationaryTimer = 0f;
            lastPlayerPosition = player.position;
        }

        return true; // si è mosso
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Se voglio solo quando ha una posizione valida
        if (lastKnownPosition != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lastKnownPosition, 0.5f);

            // disegna una linea tra il nemico e la posizione
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, lastKnownPosition);
        }
    }
#endif

}
