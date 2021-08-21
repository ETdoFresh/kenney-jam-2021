using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlopeTile : MonoBehaviour
{
    [SerializeField] private int currentFloor = 0;
    [SerializeField] private List<TilemapCollider2D> sameFloorColliders;
    [SerializeField] private List<TilemapCollider2D> nextFloorColliders;
    [SerializeField] private GameObject colliderGameObject;
    [SerializeField] private Vector3Int tilePosition;
    [SerializeField] private Vector3Int enterPosition;
    [SerializeField] private Vector3Int exitPosition;
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private bool isOnSlope;

    private void Awake()
    {
        playerCharacter = FindObjectOfType<PlayerCharacter>();
        tilemap = FindObjectOfType<Tilemap>();
        tilePosition = tilemap.WorldToCell(transform.position);
        enterPosition = tilePosition + new Vector3Int(-1, 0, 0);
        exitPosition = tilePosition + new Vector3Int(2, 1, 0);
    }

    private void Update()
    {
        if (!isOnSlope && playerCharacter.TilePosition == tilePosition)
            EnableSlopeColliders();

        if (isOnSlope && playerCharacter.TilePosition == enterPosition)
            ReturnToCurrentLevel();

        if (isOnSlope && playerCharacter.TilePosition == exitPosition)
            GoToNextLevel();
    }

    private void EnableSlopeColliders()
    {
        isOnSlope = true;
        playerCharacter.SetFloor(currentFloor + 1);
        colliderGameObject.SetActive(true);
        foreach (var sameFloorCollider in sameFloorColliders)
            sameFloorCollider.enabled = false;
        foreach (var nextFloorCollider in nextFloorColliders)
            nextFloorCollider.enabled = false;
    }
    
    private void ReturnToCurrentLevel()
    {
        isOnSlope = false;
        playerCharacter.SetFloor(currentFloor);
        colliderGameObject.SetActive(false);
        foreach (var sameFloorCollider in sameFloorColliders)
            sameFloorCollider.enabled = true;
    }
    
    private void GoToNextLevel()
    {
        isOnSlope = false;
        playerCharacter.SetFloor(currentFloor + 1);
        colliderGameObject.SetActive(false);
        foreach (var nextFloorCollider in nextFloorColliders)
            nextFloorCollider.enabled = true;
    }
}
