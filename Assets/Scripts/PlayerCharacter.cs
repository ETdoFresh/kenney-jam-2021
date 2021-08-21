using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    private Transform _transform;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Controls _controls;
    public Vector3 movementInput;

    public int currentFloor;
    public Tilemap tilemap;
    public Vector3Int tilePosition;
    public Vector3 tileCenter;
    public int spriteOrderOffset;

    public Vector3Int TilePosition => tilePosition;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteOrderOffset = _spriteRenderer.sortingOrder;
        if (!tilemap) tilemap = FindObjectOfType<Tilemap>();
    }

    private void OnEnable()
    {
        _controls ??= new Controls();
        _controls?.Enable();
        FindObjectOfType<RotateLeftButton>()?.AddListener(RotateLeft);
        FindObjectOfType<RotateRightButton>()?.AddListener(RotateRight);
    }

    private void OnDisable()
    {
        _controls?.Disable();
        FindObjectOfType<RotateLeftButton>()?.RemoveListener(RotateLeft);
        FindObjectOfType<RotateRightButton>()?.RemoveListener(RotateRight);
    }

    private void Update()
    {
        tilePosition = tilemap.WorldToCell(transform.position);
        tileCenter = tilemap.CellToWorld(tilePosition) + new Vector3(0, 0.25f, 0);
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
        var delta = _transform.position - tileCenter;
        tilePosition = new Vector3Int(tilePosition.y, -tilePosition.x, tilePosition.z);
        tileCenter = tilemap.CellToWorld(tilePosition) + new Vector3(0, 0.25f, 0);
        delta = Quaternion.AngleAxis(90, Vector3.back) * delta;
        delta.y /= 2;
        delta.x *= 2;
        _transform.position = tileCenter + delta;
    }

    public void RotateLeft()
    {
        var delta = _transform.position - tileCenter;
        tilePosition = new Vector3Int(-tilePosition.y, tilePosition.x, tilePosition.z);
        tileCenter = tilemap.CellToWorld(tilePosition) + new Vector3(0, 0.25f, 0);
        delta = Quaternion.AngleAxis(-90, Vector3.back) * delta;
        delta.y /= 2;
        delta.x *= 2;
        _transform.position = tileCenter + delta;
    }

    public void SetFloor(int floor)
    {
        currentFloor = floor;
        _spriteRenderer.sortingOrder = floor * 10 + spriteOrderOffset;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(tileCenter, 0.05f);
            Gizmos.DrawLine(tileCenter, transform.position);
        }
    }
}