using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private ZCollisionManager zCollisionManager;
    [SerializeField] private FloatValue floorHeight;
    [SerializeField] private float speed = 5;
    private Transform _transform;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Controls _controls;
    public Vector3 movementInput;

    public Tilemap tilemap;
    public Vector3Int tileCellPosition;
    public Vector3 tileWorldPosition;
    [SerializeField] private Vector3 tileGroundWorldPosition;
    [SerializeField] private Vector3 groundWorldPosition;
    [SerializeField] private Vector3 currentHeightWorldPosition;

    private int _previousFloor;

    public Vector3Int TileCellPosition => tileCellPosition;
    public Vector3 TileGroundWorldPosition => tileGroundWorldPosition;
    public Vector3 GroundWorldPosition => groundWorldPosition;
    public int CurrentFloor => Mathf.RoundToInt(_transform.position.z / floorHeight);

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (!tilemap) tilemap = FindObjectOfType<Tilemap>();
    }

    private void OnEnable()
    {
        _controls ??= new Controls();
        _controls?.Enable();
        FindObjectOfType<RotateLeftButton>()?.AddListener(RotateLeft);
        FindObjectOfType<RotateRightButton>()?.AddListener(RotateRight);
        zCollisionManager.RegisterPlayerCharacter(this);
    }

    private void OnDisable()
    {
        _controls?.Disable();
        FindObjectOfType<RotateLeftButton>()?.RemoveListener(RotateLeft);
        FindObjectOfType<RotateRightButton>()?.RemoveListener(RotateRight);
        zCollisionManager.DeregisterPlayerCharacter(this);
    }

    private void FixedUpdate()
    {
        var worldPosition = _transform.position;
        tileCellPosition = tilemap.WorldToCell(worldPosition);
        tileWorldPosition = tilemap.CellToWorld(tileCellPosition) + new Vector3(0, 0.25f, 0);

        var tileGroundCellPosition = tileCellPosition;
        tileGroundCellPosition.z = 0;
        tileGroundWorldPosition = tilemap.CellToWorld(tileGroundCellPosition) + new Vector3(0, 0.25f, 0);

        currentHeightWorldPosition = tileGroundWorldPosition;
        currentHeightWorldPosition.y += worldPosition.z * 0.25f;

        var delta = worldPosition - currentHeightWorldPosition;
        delta.z = 0;
        groundWorldPosition = tileGroundWorldPosition + delta;

        if (_previousFloor != CurrentFloor)
        {
            _previousFloor = CurrentFloor;
            zCollisionManager.UpdateCollisionLayers();
        }

        var position = _transform.position;
        movementInput = _controls.Gameplay.Movement.ReadValue<Vector2>();
        position += speed * Time.deltaTime * movementInput;
        _rigidbody2D.MovePosition(position);
    }

    public void RotateRight()
    {
        var position = _transform.position;
        var delta = position - tileWorldPosition;
        delta.z = 0;
        delta = Quaternion.AngleAxis(90, Vector3.back) * delta;
        delta.y /= 2;
        delta.x *= 2;

        tileCellPosition = new Vector3Int(tileCellPosition.y, -tileCellPosition.x, tileCellPosition.z);
        tileWorldPosition = tilemap.CellToWorld(tileCellPosition) + new Vector3(0, 0.25f, 0);

        _transform.position = tileWorldPosition + delta;
    }

    public void RotateLeft()
    {
        var delta = _transform.position - tileWorldPosition;
        tileCellPosition = new Vector3Int(-tileCellPosition.y, tileCellPosition.x, tileCellPosition.z);
        tileWorldPosition = tilemap.CellToWorld(tileCellPosition) + new Vector3(0, 0.25f, 0);
        delta = Quaternion.AngleAxis(-90, Vector3.back) * delta;
        delta.y /= 2;
        delta.x *= 2;
        _transform.position = tileWorldPosition + delta;
    }

    public void SetRenderFloor(int floor)
    {
        _spriteRenderer.sortingOrder = floor;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(tileGroundWorldPosition, 0.05f);
            Gizmos.DrawLine(tileGroundWorldPosition, tileWorldPosition);

            Gizmos.DrawLine(tileGroundWorldPosition, groundWorldPosition);
            Gizmos.DrawSphere(groundWorldPosition, 0.02f);

            Gizmos.DrawLine(tileGroundWorldPosition, currentHeightWorldPosition);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(currentHeightWorldPosition, 0.05f);
            Gizmos.DrawLine(_transform.position, currentHeightWorldPosition);
        }
    }
}