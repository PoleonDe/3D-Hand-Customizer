using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;


public class TextureMorphing : MonoBehaviour
{
    const string S = "Settings";
    const string R = "References";

    [TitleGroup(S)] public Texture2D texA;
    [TitleGroup(S)] public Texture2D texB;
    [TitleGroup(S)] public Texture2D texLerped;
    [TitleGroup(S)] private float percentTex;
    [TitleGroup(S)]
    [ShowInInspector]
    public float PercentTex
    {
        get => percentTex;
        set
        {
            percentTex = value;
            //mF.mesh = LerpTextures(texA, texB, percentTex);
        }
    }

    [TitleGroup(R)] public Material mat;


    private void Start()
    {
        mat = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
    }


    void Update()
    {
        texLerped = LerpTextures(texA, texB, percentTex);
        mat.SetTexture("_MainTex", texLerped);
        mat.SetTexture("_BaseMap", texLerped);
    }
    Texture2D LerpTextures(Texture2D texA, Texture2D texB, float t)
    {
        if ((texA.width != texB.width) || (texA.height != texA.height))
        {
            Debug.LogWarning("texture Formats dont match");
            return null;
        }
        int width = texA.width;
        int height = texA.height;
        //TextureFormat texFormat = texA.format;
        //UnityEngine.Experimental.Rendering.TextureCreationFlags texsFlags = UnityEngine.Experimental.Rendering.TextureCreationFlags.MipChain;

        Texture2D tex = new Texture2D(width, height,TextureFormat.RGBAFloat,false);//, texFormat, texsFlags);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tex.SetPixel(i, j, LerpColor(texA.GetPixel(i, j), texB.GetPixel(i, j), t));
            }
        }

        tex.Apply();

        return tex;
    }

    Color LerpColor(Color colorA, Color colorB, float t)
    {
        return new Color(
            Mathf.LerpUnclamped(colorA.r, colorB.r, t),
            Mathf.LerpUnclamped(colorA.g, colorB.g, t),
            Mathf.LerpUnclamped(colorA.b, colorB.b, t),
            Mathf.LerpUnclamped(colorA.a, colorB.a, t));
    }
}
