using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float gravityForce;
    [SerializeField] private float sensitivity;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject pauseMenu;

    //ground check
    [SerializeField] private GameObject groundCheck;
    [SerializeField] float playerHeight;
    [SerializeField] private float groundDrag;
    [SerializeField] LayerMask groundLayermask;
    bool grounded;
    float groundCheckRadius;

    Rigidbody rb = new Rigidbody();
    PlayerInput playerInput;
    Vector3 moveDir = new Vector3();
    Vector3 lookDir = new Vector3();
    Vector3 oldLookDir = new Vector3();
    Vector3 spawnPos = new Vector3(0, 100, 0);
    bool stopInput;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        groundCheckRadius = groundCheck.GetComponent<SphereCollider>().radius;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }
    private void Update()
    {
        SpeedControl();
        //grounded = Physics.Raycast(transform.position, Vector3.down, (playerHeight * .5f) + .2f, groundLayermask);
        Collider[] collider = Physics.OverlapSphere(groundCheck.gameObject.transform.position, groundCheckRadius, groundLayermask);
        if (collider.Length >= 1) {
            grounded = true;
        }
        else {
            grounded = false;
        }
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

    }

    public void SetSensitivity(float newSens)
    {
        sensitivity = newSens/100; //sens values in the menus are whole numbers because it looks nicer
    }

    public float GetSensitivity()
    {
        return sensitivity * 100;
    }
    private void FixedUpdate()
    {
        if (grounded)
        {
            rb.AddForce(((transform.right * moveDir.x) + (transform.forward * moveDir.z)) * 8000 * Time.deltaTime);
        }
        else
        {
            rb.AddForce(((transform.right * moveDir.x) + (transform.forward * moveDir.z)) * airMultiplier * 8000 * Time.deltaTime);
            rb.AddForce(-transform.up * gravityForce * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Explosion")
        {
            Debug.Log("fart");

            gameObject.transform.position = spawnPos;
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        if (!stopInput)
        {
            moveDir = context.ReadValue<Vector3>().normalized;
        }
    }

    public void StopInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            stopInput = !stopInput;
        }

    }

    public void Pause(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            stopInput = !stopInput;
            Cursor.visible = !Cursor.visible;
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && grounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

    }

    public void Look(InputAction.CallbackContext context)
    {
        if (context.performed && !stopInput)
        {
            lookDir = context.ReadValue<Vector2>();
            oldLookDir = oldLookDir + new Vector3(lookDir.x, lookDir.y, lookDir.z);
            gameObject.transform.rotation = Quaternion.Euler(0, oldLookDir.x * sensitivity, oldLookDir.z * sensitivity); //this rotates the characters x and z posistions 

            if (!(oldLookDir.y * -1 * sensitivity > 90 || oldLookDir.y * -1 * sensitivity < -90))
            {
                head.transform.rotation = Quaternion.Euler(oldLookDir.y * -1 * sensitivity, oldLookDir.x * sensitivity, oldLookDir.z * sensitivity); //this rotates the cameras y, this is so the Y look direction has no affect on move direction 
            }

        }
    }

    public override void OnNetworkSpawn() //this was to fix a weird ass bug where clients couldnt move 
    {
        base.OnNetworkSpawn();
        // Make sure this belongs to us
        if (!IsOwner) { return; }

        playerInput.enabled = true;
    }

}
