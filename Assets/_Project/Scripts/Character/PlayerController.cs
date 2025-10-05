using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GroundChecker))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private int maxJumpCount = 2;

    private bool isCrouching;
    private bool isFalling;
    private bool isJumping;



    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;


    private Rigidbody _rb;
    private GroundChecker _groundCheck;

    private float _horizontal;
    private float _vertical;
    private int _jumpCount;
    private bool wasGrounded;

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
    }

    [SerializeField] private Animator animator;

    void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        // Determina se il player sta cadendo
        isFalling = !_groundCheck.IsGrounded && _rb.velocity.y < -0.1f;

        // Reset conteggio salti
        if (_groundCheck.IsGrounded && !wasGrounded)
        {
            _jumpCount = 0;
            if (debugLogs) Debug.Log("A terra - reset salti");
        }

        wasGrounded = _groundCheck.IsGrounded;

        // Input salto
        if (Input.GetKeyDown(KeyCode.Space) && _jumpCount < maxJumpCount)
        {
            AnimationControl();
            Jump();
            _jumpCount++;
            _groundCheck.TriggerJump();
            
            if (debugLogs) Debug.Log("Salto #" + _jumpCount);
              
        }
        // Solo se è a terra e non sta già facendo slide
        if (_groundCheck.IsGrounded )
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isCrouching = true;
                if (debugLogs) Debug.Log(Input.GetKey(KeyCode.LeftShift));
            }
            else
            {
                if (debugLogs) Debug.Log(Input.GetKey(KeyCode.LeftShift));
                isCrouching = false;
            }
        }
        
        Move();
        AnimationControl();
    }

    private void Jump()
    {
        if (!isCrouching)
        {
            isJumping = true;
            // Annulla la velocità verticale prima del salto
            Vector3 velocity = _rb.velocity;
            velocity.y = 0;
            _rb.velocity = velocity;

            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
    }

    private void AnimationControl()
    {
        // Aggiorna parametri animator
        if (animator != null)
        {
            // velocità orizzontale reale
            float horizontalSpeed = new Vector3(_rb.velocity.x, 0, _rb.velocity.z).magnitude;

            // normalizza in base al runSpeed attuale
            float normalizedSpeed = Mathf.Clamp(horizontalSpeed / runSpeed, 0f, 1f);

            animator.SetFloat("speed", normalizedSpeed);

            animator.SetBool("isGrounded", _groundCheck.IsGrounded);

            animator.SetBool("isFalling", isFalling);

            animator.SetBool("isCrouching", isCrouching);

            animator.SetBool("isJumping", isJumping);
            isJumping= false;


        }
    }
    private void Move()
    {
        Vector3 inputDir = new Vector3(_horizontal, 0f, _vertical).normalized;
        if (inputDir.sqrMagnitude > 0.01f)
        {
            Quaternion camRot = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
            Vector3 moveDir = camRot * inputDir;

            // Usa walk o run in base al tasto Ctrl premuto
            float currentSpeed = Input.GetKey(KeyCode.LeftControl) ? runSpeed : walkSpeed;
            Vector3 targetVelocity = moveDir * currentSpeed;
            targetVelocity.y = _rb.velocity.y; // mantieni Y (gravità/salto)

            _rb.velocity = targetVelocity;

            // Rotazione verso direzione movimento
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }


}
