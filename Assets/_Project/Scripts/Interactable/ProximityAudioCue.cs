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
    [Tooltip("Lista di clip audio possibili da riprodurre.")]
    public AudioClip[] cueSounds;

    [Tooltip("Volume del suono.")]
    [Range(0f, 1f)] public float volume = 1f;

    [Tooltip("Tempo minimo prima che possa risuonare di nuovo.")]
    public float cooldown = 2f;

    private float lastPlayTime = -999f;
    private Transform player;
    private AudioSource audioSource;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        audioSource = GetComponent<AudioSource>();

        // Setup AudioSource per 3D
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D
        audioSource.volume = volume;
    }

    private void Update()
    {
        if (player == null || cueSounds == null || cueSounds.Length == 0)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= triggerDistance && Time.time - lastPlayTime > cooldown)
        {
            PlayRandomSound();
            lastPlayTime = Time.time;
        }

        // Se vuoi che si fermi quando il player si allontana
        if (!loopWhileNear && audioSource.isPlaying && distance > triggerDistance)
        {
            audioSource.Stop();
        }
    }

    private void PlayRandomSound()
    {
        int index = Random.Range(0, cueSounds.Length);
        AudioClip clip = cueSounds[index];

        if (clip == null) return;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.volume = volume * Random.Range(0.9f, 1.1f);

        audioSource.clip = clip;
        audioSource.Play();

        Debug.Log($"[AudioPing] Oggetto '{gameObject.name}' ha riprodotto: {clip.name}");
    }
}
