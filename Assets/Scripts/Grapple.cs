using System;
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
    [SerializeField] private float _grappleLength = 5f;
    [SerializeField] private float _grappleSpeed = 20f;
    [SerializeField] private float _reelInSpeed = 3f;


    private Vector3 _shootDirection = Vector3.zero;
    private float _currentGrappleDistance = 0;
    private GrappleMode _grappleState = GrappleMode.None;

    private Vector3 _attachedPos = new Vector3();


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

                if (_hook != null)
                {
                    _hook.transform.position = transform.position + _shootDirection * _currentGrappleDistance;
                }

                if (_currentGrappleDistance > _grappleLength)
                {
                    ResetGrapple();
                }

                break;
            case GrappleMode.Attached:
                _hook.transform.position = _attachedPos;

                break;
            default:
                break;
        }

    }

    private void OnGrapple(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
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
        _hook.transform.position = transform.position;
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.blue;
        if (_grappleState == GrappleMode.Attached)
        {
            Gizmos.DrawLine(transform.position, _attachedPos);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_grappleState != GrappleMode.Shooting)
        {
            return;
        }

        if (other.attachedRigidbody != null)
        {
            Debug.Log(other.attachedRigidbody.gameObject.name);
            Platform platformComp = other.attachedRigidbody.GetComponent<Platform>();
            if (platformComp != null)
            {
                if (platformComp.Grappleable)
                {
                    AttachGrapple(_hook.transform.position);
                }
                else
                {
                    Debug.Log("wrong platform");
                    ResetGrapple();
                }
            }
        }
    }

    private void AttachGrapple(Vector3 contactPos)
    {
        _grappleState = GrappleMode.Attached;
        _attachedPos = contactPos;
    }
}
