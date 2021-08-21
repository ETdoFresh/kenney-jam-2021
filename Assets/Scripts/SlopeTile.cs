using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlopeTile : MonoBehaviour
{
    [SerializeField] private ZCollisionManager zCollisionManager;
    [SerializeField] private FloatValue floorHeight;
    [SerializeField] private GameObject colliderGameObject;
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

    private void Awake()
    {
        tilemap = FindObjectOfType<Tilemap>();
        tileAboveCellPosition = tileCellPosition + Vector3Int.forward;
        tileWorldPosition = tilemap.CellToWorld(tileCellPosition);
        rampStartPosition = tileWorldPosition + new Vector3(0.25f, 0.125f, currentFloor * floorHeight);
        rampEndPosition = tileWorldPosition + new Vector3(-0.25f, 0.375f, (currentFloor + 1) * floorHeight);
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
            playerCharacter.SetRenderFloor(currentFloor + 1);
            zCollisionManager.DisableCollisionLayers();
        }

        if (wasOnSlope && !isOnSlope)
        {
            wasOnSlope = isOnSlope;
            zCollisionManager.UpdateCollisionLayers();
        }

        if (isOnSlope)
            UpdatePlayerCharacterZ(playerCharacter);

        // if (!isOnSlope && playerCharacter.TileCellPosition == tilePosition)
        //     EnableSlopeColliders();
    }

    private void UpdatePlayerCharacterZ(PlayerCharacter playerCharacter)
    {
        var playerPosition = playerCharacter.GroundWorldPosition;
        var lhs = playerPosition - rampStartPosition;
        var direction = rampEndPosition - rampStartPosition;
        direction = direction.normalized;
        var dotProduct = Vector3.Dot(lhs, direction);
        closestPoint = rampStartPosition + direction * dotProduct;
        //playerCharacter.Z = closestPoint.z;
    }


    // private void EnableSlopeColliders()
    // {
    //     isOnSlope = true;
    //     playerCharacter.SetFloor(currentFloor + 1);
    //     colliderGameObject.SetActive(true);
    // }
    //
    // private void ReturnToCurrentLevel()
    // {
    //     isOnSlope = false;
    //     playerCharacter.SetFloor(currentFloor);
    //     colliderGameObject.SetActive(false);
    // }
    //
    // private void GoToNextLevel()
    // {
    //     isOnSlope = false;
    //     playerCharacter.SetFloor(currentFloor + 1);
    //     colliderGameObject.SetActive(false);
    // }

    private void RotateLeft()
    {
        throw new NotImplementedException();
    }

    private void RotateRight()
    {
        // Set Sprite
        // Set New Colliders
        tileCellPosition = new Vector3Int(tileCellPosition.y, -tileCellPosition.x, tileCellPosition.z);
        transform.position = tilemap.CellToWorld(tileCellPosition) + Vector3.up * 0.25f;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(rampStartPosition, 0.05f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rampEndPosition, 0.05f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(closestPoint, 0.02f);
        Gizmos.DrawLine(rampStartPosition, rampEndPosition);
    }
}