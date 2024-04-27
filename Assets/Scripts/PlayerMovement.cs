using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")] public float moveSpeed;
    public Transform orientation;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool _readyToJump = true;

    [Header("Keybinds")] public KeyCode jumpKey = KeyCode.Space;
    
    [Header("Ground Check")] 
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool _grounded;
    
    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveDirection;

    private Rigidbody _rb;

    public Camera FirstPersonCamera;
    // for Camera Control
    private float xRotation;
    private float yRotation;
    public float sensX;
    public float sensY;
    public Transform CameraOrientation;
    
    private void Start()
    {
        if (IsLocalPlayer)
        {
            FirstPersonCamera.GameObject().SetActive(true);
        }
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    private void Update()
    {
        if (!IsOwner) return;
        CameraControl(FirstPersonCamera);
        _grounded = Physics.Raycast(
            transform.position, Vector3.down,
            playerHeight * 0.5f + 0.2f, whatIsGround
            );
        MyInput();
        SpeedControl();
        
        if (_grounded)
            _rb.drag = groundDrag;
        else
            _rb.drag = 0;
    }

    private void CameraControl(Camera camera)
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        Debug.Log($"{mouseX}, {mouseY}");
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
                
        camera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        CameraOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        MovePlayer();
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        /*Debug.Log(Input.GetKey(jumpKey) && readyToJump && grounded);
        Debug.Log("jumpKey: " + Input.GetKey(jumpKey));
        Debug.Log("readyToJump: " + readyToJump );
        Debug.Log("grounded: " + grounded);*/
        if (Input.GetKey(jumpKey) && _readyToJump && _grounded)
        {
            _readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
        
        if (_grounded) 
            _rb.AddForce(_moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!_grounded) 
            _rb.AddForce(_moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        
        _rb.AddForce(transform.up  * jumpForce, ForceMode.Impulse);
        
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }
}
