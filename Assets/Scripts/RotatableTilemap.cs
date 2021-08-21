using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RotatableTilemap : MonoBehaviour
{
    [Serializable]
    public class RotatableTile
    {
        public Vector3Int tilePosition;
        public Tile tile;
        public string tileName;
        public IsoDirection initialDirection;
    }

    private Tilemap _tilemap;
    [SerializeField] private MapDirection mapDirection;
    [SerializeField] private RotatableTileDB rotatableTileDB;
    [SerializeField] private IsoDirection tilemapDirection;
    [SerializeField] private List<RotatableTile> rotatableTiles;

    private void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        PopulateTiles();
    }

    private static IsoDirection GetDirectionFromName(string rotatableTileTileName)
    {
        if (rotatableTileTileName.EndsWith("NE")) return IsoDirection.NorthEast;
        if (rotatableTileTileName.EndsWith("NW")) return IsoDirection.NorthWest;
        if (rotatableTileTileName.EndsWith("SE")) return IsoDirection.SouthEast;
        if (rotatableTileTileName.EndsWith("SW")) return IsoDirection.SouthWest;
        if (rotatableTileTileName.EndsWith("E")) return IsoDirection.NorthEast;
        if (rotatableTileTileName.EndsWith("N")) return IsoDirection.NorthWest;
        if (rotatableTileTileName.EndsWith("S")) return IsoDirection.SouthEast;
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (rotatableTileTileName.EndsWith("W")) return IsoDirection.SouthWest;
        return IsoDirection.NorthEast;
    }

    private void PopulateTiles()
    {
        rotatableTiles.Clear();
        for (var x = _tilemap.cellBounds.xMin; x <= _tilemap.cellBounds.xMax; x++)
        for (var y = _tilemap.cellBounds.yMin; y <= _tilemap.cellBounds.yMax; y++)
        for (var z = _tilemap.cellBounds.zMin; z <= _tilemap.cellBounds.zMax; z++)
        {
            var rotatableTile = new RotatableTile();
            var tilePosition = new Vector3Int(x, y, z);
            rotatableTile.tile = _tilemap.GetTile<Tile>(tilePosition);
            if (!rotatableTile.tile) continue;
            rotatableTile.tilePosition = tilePosition;
            rotatableTile.tileName = rotatableTile.tile.sprite.name;
            rotatableTile.initialDirection = GetDirectionFromName(rotatableTile.tileName);
            rotatableTiles.Add(rotatableTile);
        }
    }

    public void ResetRotation()
    {
        tilemapDirection = mapDirection.direction;
    }

    public void RotateRight()
    {
        var newTilemapDirectionInt = (int) tilemapDirection + 1;
        if (newTilemapDirectionInt == (int) IsoDirection.Size)
            newTilemapDirectionInt = 0;
        tilemapDirection = (IsoDirection) newTilemapDirectionInt;
        
        foreach(var rotatable in rotatableTiles)
            _tilemap.SetTile(rotatable.tilePosition, null);

        foreach (var rotatable in rotatableTiles)
        {
            var newPosition = new Vector3Int(rotatable.tilePosition.y, -rotatable.tilePosition.x, rotatable.tilePosition.z);
            var newTile = rotatableTileDB.GetRotatedRight(rotatable.tile);
            _tilemap.SetTile(newPosition, newTile);
            rotatable.tilePosition = newPosition;
            rotatable.tile = newTile;
        }
    }

    public void RotateLeft()
    {
        var newTilemapDirectionInt = (int) tilemapDirection + 1;
        if (newTilemapDirectionInt == (int) IsoDirection.Size)
            newTilemapDirectionInt = 0;
        tilemapDirection = (IsoDirection) newTilemapDirectionInt;
        
        foreach(var rotatable in rotatableTiles)
            _tilemap.SetTile(rotatable.tilePosition, null);

        foreach (var rotatable in rotatableTiles)
        {
            var newPosition = new Vector3Int(-rotatable.tilePosition.y, rotatable.tilePosition.x, rotatable.tilePosition.z);
            var newTile = rotatableTileDB.GetRotatedLeft(rotatable.tile);
            _tilemap.SetTile(newPosition, newTile);
            rotatable.tilePosition = newPosition;
            rotatable.tile = newTile;
        }
    }
}