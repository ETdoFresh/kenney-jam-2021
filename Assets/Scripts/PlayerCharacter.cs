using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    private Transform _transform;
    private Rigidbody2D _rigidbody2D;
    private Controls _controls;
    public Vector3 movementInput;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _controls ??= new Controls();
        _controls?.Enable();
    }

    private void OnDisable()
    {
        _controls?.Disable();
    }

    private void FixedUpdate()
    {
        var position = _transform.position;
        movementInput = (Vector3) _controls.Gameplay.Movement.ReadValue<Vector2>();
        position += speed * Time.deltaTime * movementInput;
        _rigidbody2D.MovePosition(position);
    }

    public void RotateRight()
    {
        // TODO: Get Center of Floor Tilemap. Rotate around center of floor.
        var _transform = transform;
        var position = _transform.position;
        _transform.position = new Vector3(position.y * 2, -position.x / 2, position.z);
    }

    public void RotateLeft()
    {
        var _transform = transform;
        var position = _transform.position;
        _transform.position = new Vector3(-position.y * 2, position.x / 2, position.z);
    }
}