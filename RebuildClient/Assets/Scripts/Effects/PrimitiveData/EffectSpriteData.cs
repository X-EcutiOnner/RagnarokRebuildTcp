﻿using Assets.Scripts.Sprites;
using UnityEngine;
using UnityEngine.U2D;

namespace Assets.Scripts.Effects.PrimitiveData
{
    public class EffectSpriteData
    {
        public Texture2D Texture;
        public SpriteAtlas Atlas;
        public BillboardStyle Style;
        public string[] SpriteList;
        public int TextureCount;
        public int FrameRate;
        public bool AnimateTexture;
        public Vector3 BaseRotation;
        
        public float Width;
        public float Height;

        public float Alpha = 255;
        public float MaxAlpha = 255;
        public float AlphaSpeed = 0;
        public float FadeOutLength = 0;
        public Color Color = UnityEngine.Color.white;
    }
}