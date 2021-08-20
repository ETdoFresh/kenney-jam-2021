using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    private Transform _transform;
    private Rigidbody2D _rigidbody2D;
    private Controls _controls;

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

    private void Update()
    {
        var position = _transform.position;
        position += (Vector3)_controls.Gameplay.Movement.ReadValue<Vector2>() * (speed * Time.deltaTime);
        _rigidbody2D.MovePosition(position);
    }
}
