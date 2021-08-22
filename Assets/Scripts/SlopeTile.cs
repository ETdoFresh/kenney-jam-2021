using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlopeTile : MonoBehaviour
{
    [SerializeField] private ZCollisionManager zCollisionManager;
    [SerializeField] private RotatableTileDB rotatableTileDB;
    [SerializeField] private FloatValue floorHeight;
    [SerializeField] private GameObject northCollider;
    [SerializeField] private GameObject eastCollider;
    [SerializeField] private GameObject southCollider;
    [SerializeField] private GameObject westCollider;
    [SerializeField] private GameObject currentCollider;
    [SerializeField] private int currentFloor = 0;
    [SerializeField] private bool wasOnSlope;
    [SerializeField] private bool isOnSlope;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Vector3Int tileCellPosition;
    [SerializeField] private Vector3 tileWorldPosition;
    [SerializeField] private Vector3Int tileAboveCellPosition;
    [SerializeField] private Vector3 rampStartPosition;
    [SerializeField] private Vector3 rampEndPosition;
    [SerializeField] private Vector3 closestPoint;
    [SerializeField] private IsoDirection direction;
    [SerializeField] private Tile slopeTile;
    [SerializeField] private bool isNextRenderLevelOnSlope;
    private Vector3 eastRampStartOffset = new Vector3(0.25f, 0.125f, 0);
    private Vector3 eastRampEndOffset = new Vector3(-0.25f, 0.375f, 0);
    private Vector3 southRampStartOffset = new Vector3(-0.25f, 0.125f, 0);
    private Vector3 southRampEndOffset = new Vector3(0.25f, 0.375f, 0);
    private Vector3 northRampStartOffset = new Vector3(0.25f, 0.375f, 0);
    private Vector3 northRampEndOffset = new Vector3(-0.25f, 0.125f, 0);
    private Vector3 westRampStartOffset = new Vector3(-0.25f, 0.375f, 0);
    private Vector3 westRampEndOffset = new Vector3(0.25f, 0.125f, 0);
    private Vector3 currentRampStartOffset = new Vector3(0.25f, 0.125f, 0);
    private Vector3 currentRampEndOffset = new Vector3(-0.25f, 0.375f, 0);
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        tilemap = FindObjectOfType<Tilemap>();
        tileAboveCellPosition = tileCellPosition + Vector3Int.forward;
        tileWorldPosition = tilemap.CellToWorld(tileCellPosition);
    }

    private void OnEnable()
    {
        FindObjectOfType<RotateLeftButton>()?.AddListener(RotateLeft);
        FindObjectOfType<RotateRightButton>()?.AddListener(RotateRight);
    }

    private void Update()
    {
        var playerCharacter = zCollisionManager.PlayerCharacter;
        if (!playerCharacter) return;

        isOnSlope = playerCharacter.TileCellPosition == tileCellPosition;
        isOnSlope |= playerCharacter.tileCellPosition == tileAboveCellPosition;

        if (!wasOnSlope && isOnSlope)
        {
            wasOnSlope = isOnSlope;
            zCollisionManager.DisableCollisionLayers();
            currentCollider.SetActive(true);
            playerCharacter.UpdateRenderSortOrder(currentFloor + (isNextRenderLevelOnSlope ? 1 : 0));
        }

        if (wasOnSlope && !isOnSlope)
        {
            wasOnSlope = isOnSlope;
            if (!FindObjectsOfType<SlopeTile>().Any(x => x.isOnSlope))
            {
                zCollisionManager.UpdateCollisionLayers();
                playerCharacter.UpdateRenderSortOrder(playerCharacter.CurrentFloor);
            }

            playerCharacter.targetZ = playerCharacter.CurrentFloor * floorHeight;
            currentCollider.SetActive(false);
        }

        if (isOnSlope)
            UpdatePlayerCharacterZ(playerCharacter);
    }

    private void UpdatePlayerCharacterZ(PlayerCharacter playerCharacter)
    {
        var playerPosition = playerCharacter.transform.position;
        playerPosition.z = 0;
        var lhs = playerPosition - rampStartPosition;
        var direction = rampEndPosition - rampStartPosition;
        direction = direction.normalized;
        var dotProduct = Vector3.Dot(lhs, direction);
        dotProduct = Mathf.Max(0, dotProduct);
        closestPoint = rampStartPosition + direction * dotProduct;
        var delta = closestPoint - rampStartPosition;
        var range = rampEndPosition - rampStartPosition;
        var t = delta.magnitude / range.magnitude;
        playerCharacter.targetZ = t * floorHeight + currentFloor * floorHeight;
    }

    private void RotateLeft()
    {
        var newPosition = new Vector3Int(-tileCellPosition.y, tileCellPosition.x, tileCellPosition.z);
        SetRotation(rotatableTileDB.GetRotatedLeft(slopeTile));
        SetPosition(newPosition, tilemap);
        DisableAllColliders();
        if (isOnSlope)
        {
            zCollisionManager.DisableCollisionLayers();
            currentCollider.SetActive(true);
        }
    }

    private void RotateRight()
    {
        var newPosition = new Vector3Int(tileCellPosition.y, -tileCellPosition.x, tileCellPosition.z);
        SetRotation(rotatableTileDB.GetRotatedRight(slopeTile));
        SetPosition(newPosition, tilemap);
        DisableAllColliders();
        if (isOnSlope)
        {
            zCollisionManager.DisableCollisionLayers();
            currentCollider.SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {
        //if (!Application.isPlaying) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(rampStartPosition, 0.05f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetVectorWithZToYOffset(rampEndPosition), 0.05f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetVectorWithZToYOffset(closestPoint), 0.02f);
        Gizmos.DrawLine(rampStartPosition, GetVectorWithZToYOffset(rampEndPosition));
    }

    private Vector3 GetVectorWithZToYOffset(Vector3 v)
    {
        return new Vector3(v.x, v.y + v.z * 0.25f, v.z);
    }

    public void SetPosition(Vector3Int tilePosition, Tilemap tilemap1)
    {
        tileCellPosition = tilePosition;
        tileAboveCellPosition = tileCellPosition + Vector3Int.forward;
        tileWorldPosition = tilemap1.CellToWorld(tileCellPosition);
        rampStartPosition = tileWorldPosition + currentRampStartOffset;
        rampStartPosition.z = 0;
        rampEndPosition = tileWorldPosition + currentRampEndOffset;
        rampEndPosition.z = 0;
        rampEndPosition.y += floorHeight * 0.25f;
        transform.position = tileWorldPosition + Vector3.up * 0.25f;
        currentFloor = tilePosition.z;
        GetComponentInChildren<SpriteRenderer>().sortingOrder = currentFloor;
    }

    private void SetNorthEast(Tile tile)
    {
        slopeTile = rotatableTileDB.GetNorthEast(tile);
        var spriteRenderer = _spriteRenderer;
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = slopeTile.sprite;
        currentCollider = northCollider;
        currentRampStartOffset = northRampStartOffset;
        currentRampEndOffset = northRampEndOffset;
        isNextRenderLevelOnSlope = false;
    }

    private void SetNorthWest(Tile tile)
    {
        slopeTile = rotatableTileDB.GetNorthWest(tile);
        var spriteRenderer = _spriteRenderer;
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = slopeTile.sprite;
        currentCollider = westCollider;
        currentRampStartOffset = westRampStartOffset;
        currentRampEndOffset = westRampEndOffset;
        isNextRenderLevelOnSlope = false;
    }

    private void SetSouthEast(Tile tile)
    {
        slopeTile = rotatableTileDB.GetSouthEast(tile);
        var spriteRenderer = _spriteRenderer;
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = slopeTile.sprite;
        currentCollider = eastCollider;
        currentRampStartOffset = eastRampStartOffset;
        currentRampEndOffset = eastRampEndOffset;
        isNextRenderLevelOnSlope = true;
    }

    private void SetSouthWest(Tile tile)
    {
        slopeTile = rotatableTileDB.GetSouthWest(tile);
        var spriteRenderer = _spriteRenderer;
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = slopeTile.sprite;
        currentCollider = southCollider;
        currentRampStartOffset = southRampStartOffset;
        currentRampEndOffset = southRampEndOffset;
        isNextRenderLevelOnSlope = true;
    }

    public void SetRotation(Tile tile)
    {
        direction = rotatableTileDB.GetDirection(tile);
        if (direction == IsoDirection.NorthEast) SetNorthEast(tile);
        if (direction == IsoDirection.NorthWest) SetNorthWest(tile);
        if (direction == IsoDirection.SouthEast) SetSouthEast(tile);
        if (direction == IsoDirection.SouthWest) SetSouthWest(tile);
    }

    private void DisableAllColliders()
    {
        northCollider.SetActive(false);
        eastCollider.SetActive(false);
        southCollider.SetActive(false);
        westCollider.SetActive(false);
    }
}