using UnityEngine;

public class CameraTargetFollow : MonoBehaviour
{
    [SerializeField] private float _distance = 10f;
    [SerializeField] private float _verticalAngle = 0f;
    [SerializeField] private float _lerpFactor = 5f;

    Camera _activeCamera;

    void Start()
    {

    }

    void Update()
    {
        _activeCamera = Camera.main;


        Vector3 targetPos = transform.position + (Quaternion.Euler(_verticalAngle, 0f, 0f) * Vector3.back) * _distance;


        _activeCamera.transform.position = Vector3.Lerp(_activeCamera.transform.position, targetPos, _lerpFactor * Time.deltaTime);
        Vector3 camDir = transform.position - _activeCamera.transform.position;
        _activeCamera.transform.rotation = Quaternion.Euler(_verticalAngle, 0f, 0f);

    }
}
