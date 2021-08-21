using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CollisionLayer : MonoBehaviour
{
    [SerializeField] private ZCollisionManager zCollisionManager;
    [SerializeField] private int zLayer = -1;
    private Tilemap _tilemap;
    private TilemapRenderer _tilemapRenderer;
    private TilemapCollider2D _tilemapCollider;

    public int ZLayer => zLayer;
    
    private void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        _tilemapRenderer = GetComponent<TilemapRenderer>();
        _tilemapCollider = GetComponent<TilemapCollider2D>();
        if (zLayer == -1) zLayer = _tilemapRenderer.sortingOrder;
    }

    private void OnEnable()
    {
        zCollisionManager.RegisterCollisionLayer(this);
    }

    private void OnDisable()
    {
        zCollisionManager.DeregisterCollisionLayer(this);
    }

    public void ActivateLayer()
    {
        _tilemapCollider.enabled = true;
        _tilemapRenderer.enabled = true;
    }

    public void DeactivateLayer()
    {
        _tilemapCollider.enabled = false;
        _tilemapRenderer.enabled = false;
    }
}