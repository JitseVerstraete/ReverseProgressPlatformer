using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _acceleration = 20f;
    [SerializeField] private float _jumpStrength = 10f;
    [SerializeField] private float _gravityStrength = 20f;

    private CharacterController _charController = null;

    private Vector2 _velocity = Vector2.zero;

    void Start()
    {
        _velocity = new Vector2(0, 0);

        _charController = GetComponent<CharacterController>();
        if (_charController == null)
        {
            Debug.Log("no rigidbody found on player character object!");
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Move(-1);
        }

        if (Input.GetKey(KeyCode.D))
        {
            Move(1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        HandleGravity(Time.deltaTime);
        _charController.Move(new Vector3(_velocity.x * Time.deltaTime, _velocity.y * Time.deltaTime, 0));
    }

    private void FixedUpdate()
    {

    }

    void Jump()
    {
        _velocity.y += _jumpStrength;
        Debug.Log(_velocity.y);
    }

    void Move(float dirInput)
    {
        if (dirInput == 0)
        {
            return;
        }

        dirInput = Mathf.Clamp(dirInput, -1, 1);

        _velocity.x += dirInput * _acceleration * Time.deltaTime;
        _velocity.x = Mathf.Clamp(_velocity.x, -_movementSpeed, _movementSpeed);
    }

    void HandleGravity(float delta)
    {
        if (_velocity.y < 0 && _charController.isGrounded)
        {
            _velocity.y = 0;
            return;
        }

        _velocity.y -= _gravityStrength * delta;
    }
}
