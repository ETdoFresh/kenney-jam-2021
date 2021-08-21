using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create ZCollisionManager", fileName = "ZCollisionManager", order = 0)]
public class ZCollisionManager : ScriptableObject
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private List<CollisionLayer> collisionLayers;
    public PlayerCharacter PlayerCharacter => playerCharacter;

    public void RegisterPlayerCharacter(PlayerCharacter newPlayerCharacter)
    {
        playerCharacter = newPlayerCharacter;
    }

    public void DeregisterPlayerCharacter(PlayerCharacter character)
    {
        if (playerCharacter == character) playerCharacter = null;
    }

    public void RegisterCollisionLayer(CollisionLayer collisionLayer)
    {
        collisionLayers.Add(collisionLayer);
    }

    public void DeregisterCollisionLayer(CollisionLayer collisionLayer)
    {
        collisionLayers.Remove(collisionLayer);
    }

    public void UpdateCollisionLayers()
    {
        if (!playerCharacter) return;
        foreach (var collisionLayer in collisionLayers)
            if (playerCharacter.CurrentFloor == collisionLayer.ZLayer)
                collisionLayer.ActivateLayer();
            else
                collisionLayer.DeactivateLayer();
    }

    public void DisableCollisionLayers()
    {
        foreach (var collisionLayer in collisionLayers)
            collisionLayer.DeactivateLayer();
    }
}