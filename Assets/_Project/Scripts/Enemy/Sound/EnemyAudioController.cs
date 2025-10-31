using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudioController : MonoBehaviour
{
    [Header("Footstep Settings")]
    public AudioClip[] footstepSounds;
    public float stepDistance = 2f; // distanza minima tra un passo e l’altro
    public float minSpeedForSteps = 0.2f;
    public float footstepVolume = 0.6f;

    [Header("State Background Sounds")]
    public AudioClip patrolLoop;
    public AudioClip searchLoop;
    public AudioClip chaseLoop;
    public AudioClip attackLoop;
    public float backgroundVolume = 0.4f;

    private NavMeshAgent agent;
    public AudioSource movementSource;
    public AudioSource stateSource;

    private Vector3 lastPosition;
    private float distanceTravelled;

    private string currentState = "Patrol";

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Creiamo due AudioSource separati
        movementSource = gameObject.AddComponent<AudioSource>();
        stateSource = gameObject.AddComponent<AudioSource>();

        movementSource.playOnAwake = false;
        stateSource.playOnAwake = false;
        stateSource.loop = true;

        stateSource.volume = backgroundVolume;
        movementSource.volume = footstepVolume;
    }

    private void Start()
    {
        lastPosition = transform.position;
        PlayStateLoop(patrolLoop);
    }

    private void Update()
    {
        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        float speed = agent.velocity.magnitude;

        if (speed > minSpeedForSteps)
        {
            distanceTravelled += Vector3.Distance(transform.position, lastPosition);

            if (distanceTravelled >= stepDistance)
            {
                PlayFootstep();
                distanceTravelled = 0f;
            }
        }

        lastPosition = transform.position;
    }

    private void PlayFootstep()
    {
        if (footstepSounds.Length == 0) return;

        int index = Random.Range(0, footstepSounds.Length);
        movementSource.PlayOneShot(footstepSounds[index], footstepVolume);
    }

    // ---- GESTIONE STATI ---- //
    public void SetState(string newState)
    {
        if (newState == currentState) return;
        currentState = newState;

        switch (newState)
        {
            case "Patrol":
                PlayStateLoop(patrolLoop);
                break;
            case "Search":
                PlayStateLoop(searchLoop);
                break;
            case "Chase":
                PlayStateLoop(chaseLoop);
                break;
            case "Attack":
                PlayAttack(attackLoop);
                break;
            default:
                stateSource.Stop();
                break;
        }
    }

    private void PlayStateLoop(AudioClip clip)
    {
        if (clip == null) return;

        stateSource.clip = clip;
        stateSource.loop = true;
        stateSource.volume = backgroundVolume;
        stateSource.Play();
    }
    private void PlayAttack(AudioClip clip)
    {
        if (clip == null) return;

        stateSource.clip = clip;
        stateSource.loop = false;
        stateSource.volume = backgroundVolume + 10f;
        stateSource.Play();
    }

}
