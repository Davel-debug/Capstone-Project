using System.Transactions;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GroundChecker))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private int maxJumpCount = 2;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeight = 1.0f;
    private float standHeight = 2.0f;
    [SerializeField] private float crouchLerpSpeed = 10f;
    private float currentHeightTarget;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private CapsuleCollider _col;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private Rigidbody _rb;
    private GroundChecker _groundCheck;

    private float _horizontal;
    private float _vertical;
    private int _jumpCount;
    private bool wasGrounded;

    private bool isCrouching;
    private bool isFalling;
    private bool isJumping;

    // Richieste da Update
    private bool jumpRequested = false;
    private bool crouchRequested = false;

    // Lock inverso per sincronizzare Update / FixedUpdate
    private bool lockActive = false;

    

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _groundCheck = GetComponent<GroundChecker>();
        _rb.freezeRotation = true;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else if (cameraTransform == null)
        {
            Debug.LogWarning("Nessuna camera trovata! Assicurati che la tua camera abbia il tag 'MainCamera'.");
        }

        _col = GetComponent<CapsuleCollider>();
        if (_col != null)
        {
            standHeight = _col.height;
            if (debugLogs) Debug.Log($"Altezza iniziale letta dal CapsuleCollider: {standHeight}");
        }
        else
        {
            Debug.LogWarning("CapsuleCollider non trovato, impossibile determinare l’altezza di stand.");
        }

        // Imposta altezza iniziale target
        currentHeightTarget = standHeight;
    }

    void Update()
    {
        if (lockActive) return; // evita doppie letture durante la fisica

        // Leggi input movimento (solo intenzione)
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        // Salto richiesto
        if (Input.GetKeyDown(KeyCode.Space) && _jumpCount < maxJumpCount)
            jumpRequested = true;

        // Crouch richiesto
        if (Input.GetKey(KeyCode.LeftShift))
            crouchRequested = true;
        else
            crouchRequested = false;

        // Determina se il player sta cadendo
        isFalling = !_groundCheck.IsGrounded && _rb.velocity.y < -0.1f;

        // Reset conteggio salti
        if (_groundCheck.IsGrounded && !wasGrounded)
        {
            _jumpCount = 0;
            if (debugLogs) Debug.Log("A terra - reset salti");
        }

        wasGrounded = _groundCheck.IsGrounded;
    }

    void FixedUpdate()
    {
        lockActive = true; // blocca update mentre siamo in fisica

        Move();
        HandleJump();
        HandleCrouch();
        AnimationControl();

        lockActive = false; // riabilita update
    }

    private void HandleJump()
    {
        if (!jumpRequested) return;

        if (!isCrouching)
        {
            isJumping = true;
            Vector3 velocity = _rb.velocity;
            velocity.y = 0;
            _rb.velocity = velocity;

            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpCount++;
            _groundCheck.TriggerJump();

            if (debugLogs) Debug.Log("Salto #" + _jumpCount);
        }

        jumpRequested = false;
    }

    private void HandleCrouch()
    {
        isCrouching = crouchRequested;

        float targetHeight = isCrouching ? crouchHeight : standHeight;
        currentHeightTarget = Mathf.Lerp(currentHeightTarget, targetHeight, Time.fixedDeltaTime * crouchLerpSpeed);

        if (_col != null)
        {
            _col.height = currentHeightTarget;
            _col.center = new Vector3(0, currentHeightTarget / 2f, 0);
        }

        HandleCameraCrouch(currentHeightTarget, standHeight, crouchLerpSpeed);
    }
    private void HandleCameraCrouch(float currentHeight, float standHeight, float lerpSpeed)
    {
        if (cameraTransform == null) return;

        // Calcola la percentuale dell’altezza attuale rispetto a quella in piedi
        float crouchRatio = currentHeight / standHeight;

        // Ottieni la posizione locale corrente della camera
        Vector3 camLocalPos = cameraTransform.localPosition;

        // Altezza target della camera (0.9f per tenerla un po' sotto la testa)
        float targetCamY = standHeight  * crouchRatio;

        // Interpola verso la nuova altezza
        camLocalPos.y = Mathf.Lerp(camLocalPos.y, targetCamY, Time.fixedDeltaTime * lerpSpeed);

        // Applica la posizione interpolata
        cameraTransform.localPosition = camLocalPos;
    }


    private void Move()
    {
        Vector3 inputDir = new Vector3(_horizontal, 0f, _vertical).normalized;

        if (inputDir.sqrMagnitude > 0.01f)
        {
            Quaternion camRot = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
            Vector3 moveDir = camRot * inputDir;

            // Usa walk o run in base al tasto Ctrl
            float currentSpeed = Input.GetKey(KeyCode.LeftControl) ? runSpeed : walkSpeed;
            Vector3 targetVelocity = moveDir * currentSpeed;
            targetVelocity.y = _rb.velocity.y; // mantieni la componente Y

            _rb.velocity = targetVelocity;

            // Rotazione verso direzione movimento
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // se non ti muovi, mantieni la velocità verticale
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        }
    }

    private void AnimationControl()
    {
        if (animator != null)
        {
            float horizontalSpeed = new Vector3(_rb.velocity.x, 0, _rb.velocity.z).magnitude;
            float normalizedSpeed = Mathf.Clamp(horizontalSpeed / runSpeed, 0f, 1f);

            animator.SetFloat("speed", normalizedSpeed);
            animator.SetBool("isGrounded", _groundCheck.IsGrounded);
            animator.SetBool("isFalling", isFalling);
            animator.SetBool("isCrouching", isCrouching);
            animator.SetBool("isJumping", isJumping);

            isJumping = false;
        }
    }
}
