using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Create RotatableTileDB", fileName = "RotatableTileDB", order = 0)]
public class RotatableTileDB : ScriptableObject
{
    [SerializeField] private Tile[] tiles = new Tile[0];

    public IsoDirection GetDirection(Tile tile)
    {
        var tileName = tile.name;
        if (tileName.EndsWith("NE")) return IsoDirection.NorthEast;
        if (tileName.EndsWith("NW")) return IsoDirection.NorthWest;
        if (tileName.EndsWith("SE")) return IsoDirection.SouthEast;
        if (tileName.EndsWith("SW")) return IsoDirection.SouthWest;
        if (tileName.EndsWith("E")) return IsoDirection.SouthEast;
        if (tileName.EndsWith("N")) return IsoDirection.NorthEast;
        if (tileName.EndsWith("S")) return IsoDirection.SouthWest;
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (tileName.EndsWith("W")) return IsoDirection.NorthWest;
        return IsoDirection.NorthEast;
    }
    
    public Tile GetRotatedRight(Tile tile)
    {
        if (tiles.Contains(tile))
        {
            if (tile.name.EndsWith("NE")) return GetTileWithNewSuffix(tile, "SE");
            if (tile.name.EndsWith("SE")) return GetTileWithNewSuffix(tile, "SW");
            if (tile.name.EndsWith("SW")) return GetTileWithNewSuffix(tile, "NW");
            if (tile.name.EndsWith("NW")) return GetTileWithNewSuffix(tile, "NE");
            if (tile.name.EndsWith("E")) return GetTileWithNewSuffix(tile, "S");
            if (tile.name.EndsWith("S")) return GetTileWithNewSuffix(tile, "W");
            if (tile.name.EndsWith("W")) return GetTileWithNewSuffix(tile, "N");
            if (tile.name.EndsWith("N")) return GetTileWithNewSuffix(tile, "E");
        }
        return tile;
    }

    public Tile GetRotatedLeft(Tile tile)
    {
        if (tiles.Contains(tile))
        {
            if (tile.name.EndsWith("NE")) return GetTileWithNewSuffix(tile, "NW");
            if (tile.name.EndsWith("SE")) return GetTileWithNewSuffix(tile, "NE");
            if (tile.name.EndsWith("SW")) return GetTileWithNewSuffix(tile, "SE");
            if (tile.name.EndsWith("NW")) return GetTileWithNewSuffix(tile, "SW");
            if (tile.name.EndsWith("E")) return GetTileWithNewSuffix(tile, "N");
            if (tile.name.EndsWith("S")) return GetTileWithNewSuffix(tile, "E");
            if (tile.name.EndsWith("W")) return GetTileWithNewSuffix(tile, "S");
            if (tile.name.EndsWith("N")) return GetTileWithNewSuffix(tile, "W");
        }
        return tile;
    }

    public Tile GetSouthEast(Tile tile) => GetEast(tile);
    public Tile GetEast(Tile tile)
    {
        if (tiles.Contains(tile))
        {
            if (tile.name.EndsWith("NE")) return GetTileWithNewSuffix(tile, "SE");
            if (tile.name.EndsWith("SE")) return GetTileWithNewSuffix(tile, "SE");
            if (tile.name.EndsWith("SW")) return GetTileWithNewSuffix(tile, "SE");
            if (tile.name.EndsWith("NW")) return GetTileWithNewSuffix(tile, "SE");
            if (tile.name.EndsWith("E")) return GetTileWithNewSuffix(tile, "E");
            if (tile.name.EndsWith("S")) return GetTileWithNewSuffix(tile, "E");
            if (tile.name.EndsWith("W")) return GetTileWithNewSuffix(tile, "E");
            if (tile.name.EndsWith("N")) return GetTileWithNewSuffix(tile, "E");
        }
        return tile;
    }
    
    public Tile GetNorthEast(Tile tile) => GetNorth(tile);
    public Tile GetNorth(Tile tile)
    {
        if (tiles.Contains(tile))
        {
            if (tile.name.EndsWith("NE")) return GetTileWithNewSuffix(tile, "NE");
            if (tile.name.EndsWith("SE")) return GetTileWithNewSuffix(tile, "NE");
            if (tile.name.EndsWith("SW")) return GetTileWithNewSuffix(tile, "NE");
            if (tile.name.EndsWith("NW")) return GetTileWithNewSuffix(tile, "NE");
            if (tile.name.EndsWith("E")) return GetTileWithNewSuffix(tile, "N");
            if (tile.name.EndsWith("S")) return GetTileWithNewSuffix(tile, "N");
            if (tile.name.EndsWith("W")) return GetTileWithNewSuffix(tile, "N");
            if (tile.name.EndsWith("N")) return GetTileWithNewSuffix(tile, "N");
        }
        return tile;
    }
    
    public Tile GetSouthWest(Tile tile) => GetSouth(tile);
    public Tile GetSouth(Tile tile)
    {
        if (tiles.Contains(tile))
        {
            if (tile.name.EndsWith("NE")) return GetTileWithNewSuffix(tile, "SW");
            if (tile.name.EndsWith("SE")) return GetTileWithNewSuffix(tile, "SW");
            if (tile.name.EndsWith("SW")) return GetTileWithNewSuffix(tile, "SW");
            if (tile.name.EndsWith("NW")) return GetTileWithNewSuffix(tile, "SW");
            if (tile.name.EndsWith("E")) return GetTileWithNewSuffix(tile, "S");
            if (tile.name.EndsWith("S")) return GetTileWithNewSuffix(tile, "S");
            if (tile.name.EndsWith("W")) return GetTileWithNewSuffix(tile, "S");
            if (tile.name.EndsWith("N")) return GetTileWithNewSuffix(tile, "S");
        }
        return tile;
    }
    
    public Tile GetNorthWest(Tile tile) => GetWest(tile);
    public Tile GetWest(Tile tile)
    {
        if (tiles.Contains(tile))
        {
            if (tile.name.EndsWith("NE")) return GetTileWithNewSuffix(tile, "NW");
            if (tile.name.EndsWith("SE")) return GetTileWithNewSuffix(tile, "NW");
            if (tile.name.EndsWith("SW")) return GetTileWithNewSuffix(tile, "NW");
            if (tile.name.EndsWith("NW")) return GetTileWithNewSuffix(tile, "NW");
            if (tile.name.EndsWith("E")) return GetTileWithNewSuffix(tile, "W");
            if (tile.name.EndsWith("S")) return GetTileWithNewSuffix(tile, "W");
            if (tile.name.EndsWith("W")) return GetTileWithNewSuffix(tile, "W");
            if (tile.name.EndsWith("N")) return GetTileWithNewSuffix(tile, "W");
        }
        return tile;
    }

    private Tile GetTileWithNewSuffix(Tile tile, string suffix)
    {
        var tileName = tile.name;
        tileName = tileName.Substring(0, tileName.Length - suffix.Length) + suffix;
        foreach (var newTile in tiles)
        {
            if (newTile.name.Equals(tileName, StringComparison.InvariantCultureIgnoreCase))
                return newTile;
        }
        return tile;
    }
}
