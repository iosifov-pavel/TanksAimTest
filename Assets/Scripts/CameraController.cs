using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float _radius;
    [SerializeField]
    private Transform _tankHead;

    private Camera _camera;
    private TankControl _control;
    private Vector2 _cameraMove;

    private void Awake()
    {
        _camera = Camera.main;
        _control = new TankControl();
        _control.Move.View.performed += UpdateCameraDeltaValue;
        _control.Enable();
        _camera.transform.position = transform.position + -_radius*Vector3.forward;
        _camera.transform.position += Vector3.up * 5;
        _camera.transform.LookAt(transform.position);
    }

    private void UpdateCameraDeltaValue(InputAction.CallbackContext ctx)
    {
        var delta = ctx.ReadValue<Vector2>();
        _cameraMove.x += delta.x;
        _cameraMove.y -= delta.y;
        var newY = Mathf.Clamp(_cameraMove.y, 5, 45);
        _cameraMove = new Vector2(_cameraMove.x, newY);
    }

    private void LateUpdate()
    {
        var newRotation = Quaternion.Euler(_cameraMove.y, _cameraMove.x, 0);
        _camera.transform.position = transform.position + newRotation * (-_radius * Vector3.forward);
        _camera.transform.LookAt(transform.position);
        var headForward = _camera.transform.forward;
        headForward.y = 0;
        _tankHead.transform.forward = headForward.normalized;
    }
}
