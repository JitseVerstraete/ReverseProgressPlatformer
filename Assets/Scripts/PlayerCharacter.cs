using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _acceleration = 20f;
    [SerializeField] private float _jumpStrength = 10f;

    private Rigidbody _rigidBody = null;

    private Vector2 _velocity = Vector2.zero;

    void Start()
    {
        _velocity = new Vector2(0, 0);

        _rigidBody = GetComponent<Rigidbody>();
        if (_rigidBody == null)
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

        Debug.Log(_rigidBody.velocity.x);
    }

    void Jump()
    {
        _rigidBody.AddForce(new Vector3(0, _jumpStrength, 0), ForceMode.VelocityChange);
    }

    void Move(float intput)
    {
        if (intput == 0)
        {
            return;
        }


        intput = Mathf.Clamp(intput, -1, 1);


        if (_rigidBody.velocity.x > -_movementSpeed || _rigidBody.velocity.x * intput <= 0)
        {
            _rigidBody.velocity += new Vector3(intput * _acceleration, 0, 0);
            _rigidBody.velocity = new Vector3(Mathf.Clamp(_rigidBody.velocity.x, -_movementSpeed, _movementSpeed), _rigidBody.velocity.y, _rigidBody.velocity.z);
        }

    }
}
