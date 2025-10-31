using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip levelMusic;
    public AudioClip victoryMusic;
    public AudioClip gameOverMusic;

    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 0.6f;
    public bool loop = true;

    private AudioSource source;
    private string currentClipName = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            source = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string name = scene.name;

        if (name.Contains("Menu") || name.Contains("HowToPlay"))
            PlayMusic(menuMusic, "Menu");
        else if (name.Contains("Level"))
            PlayMusic(levelMusic, "Level");
        else if (name.Contains("Victory"))
            PlayMusic(victoryMusic, "Victory");
        else if (name.Contains("GameOver"))
            PlayMusic(gameOverMusic, "GameOver");
    }

    private void PlayMusic(AudioClip clip, string clipName)
    {
        if (clip == null) return;
        if (currentClipName == clipName) return; // evita restart inutili

        source.clip = clip;
        source.loop = loop;
        source.volume = volume;
        source.Play();

        currentClipName = clipName;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
