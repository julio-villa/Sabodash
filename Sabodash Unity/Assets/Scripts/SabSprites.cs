using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabSprites : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite spriteCrown;
    public Sprite spriteBig;
    public Sprite spriteGray;
    public Sprite spriteGrav;
    public Sprite spriteCtrl;
    public Sprite spriteBncy;
    public Sprite spriteStop;
    public Sprite spriteFrwd;

    public int currentSprite;

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update() {
        if (currentSprite == -2) spriteRenderer.sprite = spriteCrown;
        if (currentSprite == -1) spriteRenderer.sprite = null;
        if (currentSprite == 0) spriteRenderer.sprite = spriteBig;
        if (currentSprite == 1) spriteRenderer.sprite = spriteGray;
        if (currentSprite == 2) spriteRenderer.sprite = spriteGrav;
        if (currentSprite == 3) spriteRenderer.sprite = spriteCtrl;
        if (currentSprite == 4) spriteRenderer.sprite = spriteBncy;
        if (currentSprite == 5) spriteRenderer.sprite = spriteStop;
        if (currentSprite == 6) spriteRenderer.sprite = spriteFrwd;
    }
}
