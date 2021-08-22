using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RotatableObject : MonoBehaviour
{
    [SerializeField] private RotatableTileDB rotatableTileDB;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Vector3Int tileCellPosition;
    [SerializeField] private Vector3 tileWorldPosition;
    [SerializeField] private IsoDirection direction;
    [SerializeField] private Tile slopeTile;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        tilemap = FindObjectOfType<Tilemap>();
        tileCellPosition = tilemap.WorldToCell(transform.position);
        tileWorldPosition = tilemap.CellToWorld(tileCellPosition);
    }

    private void OnEnable()
    {
        FindObjectOfType<PlayerCharacter>()?.rotateLeft.AddListener(RotateLeft);
        FindObjectOfType<PlayerCharacter>()?.rotateRight.AddListener(RotateRight);
    }

    private void OnDisable()
    {
        FindObjectOfType<PlayerCharacter>()?.rotateLeft.RemoveListener(RotateLeft);
        FindObjectOfType<PlayerCharacter>()?.rotateRight.RemoveListener(RotateRight);
    }

    private void RotateLeft()
    {
        var newPosition = new Vector3Int(-tileCellPosition.y, tileCellPosition.x, tileCellPosition.z);
        SetRotation(rotatableTileDB.GetRotatedLeft(slopeTile));
        SetPosition(newPosition, tilemap);
    }

    private void RotateRight()
    {
        var newPosition = new Vector3Int(tileCellPosition.y, -tileCellPosition.x, tileCellPosition.z);
        SetRotation(rotatableTileDB.GetRotatedRight(slopeTile));
        SetPosition(newPosition, tilemap);
    }

    public void SetPosition(Vector3Int tilePosition, Tilemap tilemap1)
    {
        tileCellPosition = tilePosition;
        tileWorldPosition = tilemap1.CellToWorld(tileCellPosition);
        transform.position = tileWorldPosition + Vector3.up * 0.25f;
    }

    private void SetNorthEast(Tile tile)
    {
        slopeTile = rotatableTileDB.GetNorthEast(tile);
        _spriteRenderer.sprite = slopeTile.sprite;
    }

    private void SetNorthWest(Tile tile)
    {
        slopeTile = rotatableTileDB.GetNorthWest(tile);
        _spriteRenderer.sprite = slopeTile.sprite;
    }

    private void SetSouthEast(Tile tile)
    {
        slopeTile = rotatableTileDB.GetSouthEast(tile);
        _spriteRenderer.sprite = slopeTile.sprite;
    }

    private void SetSouthWest(Tile tile)
    {
        slopeTile = rotatableTileDB.GetSouthWest(tile);
        _spriteRenderer.sprite = slopeTile.sprite;
    }

    public void SetRotation(Tile tile)
    {
        direction = rotatableTileDB.GetDirection(tile);
        if (direction == IsoDirection.NorthEast) SetNorthEast(tile);
        if (direction == IsoDirection.NorthWest) SetNorthWest(tile);
        if (direction == IsoDirection.SouthEast) SetSouthEast(tile);
        if (direction == IsoDirection.SouthWest) SetSouthWest(tile);
    }
}