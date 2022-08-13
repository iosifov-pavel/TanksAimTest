using UnityEngine;

public class MoveController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rbBody;

    private TankControl _input;
    private Vector2 _moveVector;

    private void Awake()
    {
        _input = new TankControl();
        _input.Enable();
    }

    //ƒобавил базовое перемещение чтобы тестировать было проще, но оно тут исключительно и максимально простое. “ак как тестовое его не предполагало, как € пон€л
    private void FixedUpdate()
    {
        _moveVector = _input.Move.Move.ReadValue<Vector2>();
        var force = new Vector3(_moveVector.x, 0, _moveVector.y) * Constants.SpeedScale * Time.fixedDeltaTime;
        _rbBody.AddForce(force, ForceMode.Impulse);
    }
}
