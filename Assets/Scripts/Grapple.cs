using UnityEngine;
using UnityEngine.InputSystem;



public class Grapple : MonoBehaviour
{
    private enum GrappleMode
    {
        None,
        Shooting,
        Attached
    }

    [SerializeField] private GameObject _hook;
    [Header("Grapple settings")]
    [SerializeField] private float _grappleLength;
    [SerializeField] private float _grappleSpeed;
    [SerializeField] private float _reelInSpeed;


    private Vector3 _shootDirection = Vector3.zero;
    private float _currentGrappleDistance = 0;
    private GrappleMode _grappleState = GrappleMode.None;


    private PlayerControls _controls;

    void Start()
    {
        _controls = new PlayerControls();

        _controls.Player.Grapple.started += OnGrapple;
        _controls.Player.Grapple.canceled += OnGrapple;
        _controls.Player.Grapple.Enable();
    }

    void Update()
    {
        switch (_grappleState)
        {
            case GrappleMode.None:
                break;
            case GrappleMode.Shooting:
                _currentGrappleDistance += _grappleSpeed * Time.deltaTime;

                if (_currentGrappleDistance > _grappleLength)
                {
                    ResetGrapple();
                }

                break;
            case GrappleMode.Attached:
                break;
            default:
                break;
        }

    }

    private void OnGrapple(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Debug.Log("grapple started!");
            Vector3 clickPoint = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.value, Mouse.current.position.y.value, 0.5f));
            Vector3 clickDir = (clickPoint - Camera.main.transform.position).normalized;

            int layerMask = LayerMask.GetMask("CharacterClick");
            RaycastHit hitInfo;
            Physics.Raycast(new Ray(Camera.main.transform.position, clickDir), out hitInfo, float.MaxValue, layerMask);

            _shootDirection = (hitInfo.point - transform.position).normalized;
            _grappleState = GrappleMode.Shooting;

        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            ResetGrapple();

        }
    }

    private void ResetGrapple()
    {
        _grappleState = GrappleMode.None;
        _currentGrappleDistance = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, _shootDirection * _currentGrappleDistance);
    }

}
