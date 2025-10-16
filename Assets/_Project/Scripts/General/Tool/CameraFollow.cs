using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Sensibilità")]
    [Range(0.1f, 10f)] public float sensX = 1f;
    [Range(0.1f, 10f)] public float sensY = 1f;

    [Header("Riferimenti")]
    public Transform orientation;

    [Header("Input System")]
    public InputActionReference lookAction;

    [Header("Limiti di Rotazione")]
    [Tooltip("Limite minimo (verso l'alto) e massimo (verso il basso) della rotazione verticale.")]
    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;

    [Tooltip("Limite minimo (verso sinistra) e massimo (verso destra) della rotazione orizzontale.")]
    public float minHorizontalAngle = -180f;
    public float maxHorizontalAngle = 180f;

    private float xRotation;
    private float yRotation;

    private void OnEnable()
    {
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
    }

    void Update()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
        float mouseX = lookInput.x * sensX;
        float mouseY = lookInput.y * sensY;

        // Aggiorna rotazioni
        yRotation += mouseX;
        xRotation -= mouseY;

        // Applica i limiti
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        yRotation = Mathf.Clamp(yRotation, minHorizontalAngle, maxHorizontalAngle);

        // Applica le rotazioni
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
