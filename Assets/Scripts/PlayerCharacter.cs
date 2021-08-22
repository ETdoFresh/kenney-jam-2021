using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private ZCollisionManager zCollisionManager;
    [SerializeField] private FloatValue floorHeight;
    [SerializeField] private Transform model;
    [SerializeField] private float speed = 5;
    [SerializeField] private Vector3 targetForwardDirection = Vector3.forward;
    [SerializeField] private float rotationSpeed = 1;
    [SerializeField] private float animSpeed;
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
    [SerializeField] public float targetZ;
    private Animator _animator;
    private Vector3 _delta;
    private static readonly int Speed = Animator.StringToHash("Speed");
    [SerializeField] private float animAccelerationRate = 5;
    [SerializeField] private float animDecelerationRate = 1;

    public Vector3Int TileCellPosition => tileCellPosition;
    public Vector3 GroundWorldPosition => groundWorldPosition;
    public int CurrentFloor => Mathf.RoundToInt(_spriteRenderer.transform.position.z / floorHeight);

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (!tilemap) tilemap = FindObjectOfType<Tilemap>();
        UpdateRenderSortOrder(CurrentFloor);
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

    private void Update()
    {
        var spritePosition = _spriteRenderer.transform.position;
        spritePosition.z = targetZ;
        spritePosition.z += Random.Range(0, 0.0000001f);
        _spriteRenderer.transform.position = spritePosition;
        var targetRotation = Quaternion.LookRotation(targetForwardDirection);
        model.rotation = Quaternion.RotateTowards(model.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void FixedUpdate()
    {
        var worldPosition = transform.position;
        worldPosition.z = targetZ;
        tileCellPosition = tilemap.WorldToCell(worldPosition);
        tileWorldPosition = tilemap.CellToWorld(tileCellPosition) + new Vector3(0, 0.25f, 0);

        var tileGroundCellPosition = tileCellPosition;
        tileGroundCellPosition.z = 0;
        tileGroundWorldPosition = tilemap.CellToWorld(tileGroundCellPosition) + new Vector3(0, 0.25f, 0);

        currentHeightWorldPosition = tileGroundWorldPosition;
        currentHeightWorldPosition.y += worldPosition.z * 0.25f;

        _delta = worldPosition - currentHeightWorldPosition;
        _delta.z = 0;
        groundWorldPosition = tileGroundWorldPosition + _delta;
        
        var position = transform.position;
        movementInput = _controls.Gameplay.Movement.ReadValue<Vector2>();
        if (movementInput.sqrMagnitude > 0)
            targetForwardDirection = new Vector3(movementInput.x, 0, movementInput.y);
        position += speed * Time.deltaTime * movementInput;
        var newAnimSpeed = movementInput.magnitude * speed;
        animSpeed = animSpeed < newAnimSpeed ?
            Mathf.Lerp(animSpeed, newAnimSpeed, animAccelerationRate * Time.fixedDeltaTime) : 
            Mathf.Lerp(animSpeed, newAnimSpeed, animDecelerationRate * Time.fixedDeltaTime);
        _animator.SetFloat(Speed, animSpeed);
        _rigidbody2D.MovePosition(position);
    }

    public void RotateRight()
    {
        var delta = Quaternion.AngleAxis(90, Vector3.back) * _delta;
        delta.y /= 2;
        delta.x *= 2;

        tileCellPosition = new Vector3Int(tileCellPosition.y, -tileCellPosition.x, tileCellPosition.z);
        tileWorldPosition = tilemap.CellToWorld(tileCellPosition) + new Vector3(0, 0.25f, 0);
        
        var tileGroundCellPosition = tileCellPosition;
        tileGroundCellPosition.z = 0;
        tileGroundWorldPosition = tilemap.CellToWorld(tileGroundCellPosition) + new Vector3(0, 0.25f, 0);
        currentHeightWorldPosition = tileGroundWorldPosition;
        currentHeightWorldPosition.y += targetZ * 0.25f;
        groundWorldPosition = tileGroundWorldPosition + delta;
        //_rigidbody2D.MovePosition(currentHeightWorldPosition + delta);
        transform.position = currentHeightWorldPosition + delta;
        targetForwardDirection = Quaternion.AngleAxis(90, Vector3.up) * targetForwardDirection;
    }

    public void RotateLeft()
    {
        var delta = Quaternion.AngleAxis(-90, Vector3.back) * _delta;
        delta.y /= 2;
        delta.x *= 2;

        tileCellPosition = new Vector3Int(-tileCellPosition.y, tileCellPosition.x, tileCellPosition.z);
        tileWorldPosition = tilemap.CellToWorld(tileCellPosition) + new Vector3(0, 0.25f, 0);
        
        var tileGroundCellPosition = tileCellPosition;
        tileGroundCellPosition.z = 0;
        tileGroundWorldPosition = tilemap.CellToWorld(tileGroundCellPosition) + new Vector3(0, 0.25f, 0);
        currentHeightWorldPosition = tileGroundWorldPosition;
        currentHeightWorldPosition.y += targetZ * 0.25f;
        groundWorldPosition = tileGroundWorldPosition + delta;
        //_rigidbody2D.MovePosition(currentHeightWorldPosition + delta);
        transform.position = currentHeightWorldPosition + delta;
        targetForwardDirection = Quaternion.AngleAxis(-90, Vector3.up) * targetForwardDirection;
    }

    public void UpdateRenderSortOrder(int floor)
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
            Gizmos.DrawLine(transform.position, currentHeightWorldPosition);
        }
    }

    private float MoveTowards(float from, float to, float rate)
    {
        var range = Mathf.Abs(to - from);
        if (range <= rate) return to;
        var sign = Mathf.Sign(to - from);
        return from + sign * rate;
    }
}