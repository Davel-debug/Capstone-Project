using UnityEngine;

public class SceneTracker : MonoBehaviour
{
    public static SceneTracker Instance;
    public string lastLevel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
