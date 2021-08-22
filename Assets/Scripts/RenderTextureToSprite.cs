using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureToSprite : MonoBehaviour
{
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Rect _rect = new Rect(0, 0, 256, 256);
    private Vector2 _pivot = new Vector2(0.5f, 0.125f);

    private void Update()
    {
        RenderTexture.active = renderTexture;
        Texture2D texture2D = new Texture2D(256,256, TextureFormat.RGBA32, false);
        spriteRenderer.sprite = Sprite.Create(texture2D, _rect, _pivot);
    }
}
