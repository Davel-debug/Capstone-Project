using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProximityAudioCue : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Raggio di attivazione del suono.")]
    public float triggerDistance = 5f;

    [Tooltip("Ripeti il suono finché il player rimane vicino.")]
    public bool loopWhileNear = false;

    [Header("Audio Settings")]
    [Tooltip("Clip audio da riprodurre.")]
    public AudioClip cueSound;

    [Tooltip("Volume del suono.")]
    [Range(0f, 1f)] public float volume = 1f;

    [Tooltip("Tempo minimo prima che possa risuonare di nuovo.")]
    public float cooldown = 2f;
    private float lastPlayTime = -999f;

    private Transform player;
    private AudioSource audioSource;
    private bool hasPlayed = false;
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        audioSource = GetComponent<AudioSource>();

        // Setup AudioSource
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D
        audioSource.volume = volume;
    }
    private void Update()
    {
        if (player == null || cueSound == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        Debug.Log($"[AudioPing] Oggetto : {gameObject.name}");
        if (distance <= triggerDistance && Time.time - lastPlayTime > cooldown)
        {
            audioSource.clip = cueSound;
            audioSource.Play();
            lastPlayTime = Time.time;

            Debug.Log($"[AudioPing] Oggetto trovato: {gameObject.name}");
        }
    }

}
