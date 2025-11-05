using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProximityAudioCue : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Raggio di attivazione del suono.")]
    public float triggerDistance = 5f;

    [Tooltip("Se attivo, il volume aumenta o diminuisce in base alla distanza.")]
    public bool useDistanceFade = true;

    [Header("Audio Settings")]
    [Tooltip("Lista di clip audio possibili da riprodurre.")]
    public AudioClip[] cueSounds;

    [Tooltip("Volume massimo del suono.")]
    [Range(0f, 1f)] public float maxVolume = 1f;

    [Tooltip("Velocità di fade in/out.")]
    public float fadeSpeed = 2f;

    [Tooltip("Tempo minimo prima che possa risuonare di nuovo.")]
    public float cooldown = 2f;

    private float lastPlayTime = -999f;
    private Transform player;
    private AudioSource audioSource;

    private float targetVolume = 0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        audioSource = GetComponent<AudioSource>();

        // Setup AudioSource per 3D
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D
        audioSource.loop = true;
        audioSource.volume = 0f; // parte silenzioso

        // Avvia un suono casuale in loop (per modificare il one-shot è sotto )
        if (cueSounds != null && cueSounds.Length > 0)
        {
            audioSource.clip = cueSounds[Random.Range(0, cueSounds.Length)];
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (useDistanceFade)
        {
            if (distance < triggerDistance)
            {
                float t = 1f - (distance / triggerDistance);
                targetVolume = Mathf.Lerp(0f, maxVolume, t);
            }
            else
            {
                targetVolume = 0f;
            }

            // Applica fade fluido
            audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
        }
        else
        {
            // Per comportamento classico disattiva
            if (distance <= triggerDistance && Time.time - lastPlayTime > cooldown)
            {
                PlayRandomSound();
                lastPlayTime = Time.time;
            }
            else if (audioSource.isPlaying && distance > triggerDistance)
            {
                audioSource.Stop();
            }
        }
    }

    private void PlayRandomSound()
    {
        if (cueSounds == null || cueSounds.Length == 0) return;

        int index = Random.Range(0, cueSounds.Length);
        AudioClip clip = cueSounds[index];
        if (clip == null) return;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.volume = maxVolume * Random.Range(0.9f, 1.1f);
        audioSource.spatialBlend = 1f;
        audioSource.loop = false;

        audioSource.PlayOneShot(clip);
        Debug.Log($"[AudioPing] Oggetto '{gameObject.name}' ha riprodotto: {clip.name}");
    }
}
