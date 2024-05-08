using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _acceleration = 20f;
    [SerializeField] private float _jumpStrength = 10f;
    [SerializeField] private float _groundedDrag = 2f;
    [SerializeField] private float _airDrag = 0.4f;


    private Vector2 _velocity;

    private Vector2 _rightMovementVector;

    private float _horizontalMovementInput;
    private bool _jumpInput;

    private CharacterController _charController = null;

    private PlayerControls _controls;

    void Start()
    {
        _controls = new PlayerControls();
        _controls.Player.Jump.started += OnJump;
        _controls.Player.Jump.canceled += OnJump;
        _controls.Player.Jump.Enable();
        _controls.Player.Move.started += OnMove;
        _controls.Player.Move.canceled += OnMove;
        _controls.Player.Move.Enable();

        _charController = GetComponent<CharacterController>();
        if (_charController == null)
        {
            Debug.Log("no rigidbody found on player character object!");
        }
    }


    private void Update()
    {

        Vector2 realVel = _charController.velocity;
        ValidateVelocity(realVel);

        DoFloorRaycast();
        ApplyDrag(ref _velocity);
        HandleMovementInput(ref _velocity);
        HandleGravity(ref _velocity);

        _charController.Move(new Vector3(_velocity.x * Time.deltaTime, _velocity.y * Time.deltaTime, 0));
    }

    private void ValidateVelocity(Vector2 realVelocity)
    {
        float tx = realVelocity.x == 0 ? 1 : Mathf.Clamp01((_velocity.x / realVelocity.x) * Time.deltaTime);
        _velocity.x = Mathf.Lerp(_velocity.x, realVelocity.x, tx);

        float ty = realVelocity.y == 0 ? 1 : Mathf.Clamp01((_velocity.y / realVelocity.y) * Time.deltaTime);
        _velocity.y = Mathf.Lerp(_velocity.y, realVelocity.y, ty);
    }

    private void HandleMovementInput(ref Vector2 vel)
    {

        if (_horizontalMovementInput != 0)
        {
            _horizontalMovementInput = Mathf.Clamp(_horizontalMovementInput, -1, 1);

            vel += ((_charController.isGrounded ? _rightMovementVector : Vector2.right) * (_horizontalMovementInput * _acceleration * Time.deltaTime));
            vel.x = Mathf.Clamp(vel.x, -_movementSpeed, _movementSpeed);
        }

        //Debug.Log(_charController.isGrounded);

        if (_jumpInput)
        {
            if (_charController.isGrounded)
            {
                vel.y = _jumpStrength;
            }
            _jumpInput = false;
        }
    }


    private void DoFloorRaycast()
    {
        Ray floorRay = new Ray(new Vector3(_charController.transform.position.x + _charController.center.x, _charController.bounds.min.y, _charController.transform.position.z + _charController.center.z),
                            new Vector3(0, -1, 0));
        RaycastHit hitInfo;
        Physics.Raycast(floorRay, out hitInfo);
        _rightMovementVector = (Vector2)Vector3.Cross(hitInfo.normal, Vector3.forward);
        _rightMovementVector.Normalize();
    }

    private void ApplyDrag(ref Vector2 vel)
    {
        float dragModifier = Mathf.Clamp01(1f - ((_charController.isGrounded ? _groundedDrag : _airDrag) * Time.deltaTime));
        vel.x *= dragModifier;
    }

    void HandleGravity(ref Vector2 newVel)
    {
        newVel.y += Physics.gravity.y * Time.deltaTime;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("jump");
        _jumpInput = true;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("move");
        _horizontalMovementInput = context.ReadValue<float>();
    }
}
