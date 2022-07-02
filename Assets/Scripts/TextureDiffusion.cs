using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TextureDiffusion : SerializedMonoBehaviour
{
    const string S = "Settings";
    const string R = "References";

    [TitleGroup(S)] public Texture2D baseTexture;
    [TitleGroup(S)] public Texture2D diffuseTexture;


    [TitleGroup(R)] public Material mat;


    private void Start()
    {
        mat = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        diffuseTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.RGBA32, false);
        Graphics.CopyTexture(baseTexture, diffuseTexture);
        mat.SetTexture("_BaseMap", diffuseTexture);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("pressed Space");
            diffuseTexture = GrowTexture(diffuseTexture);
            mat.SetTexture("_BaseMap", diffuseTexture);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("O");
            Graphics.CopyTexture(baseTexture, diffuseTexture);
        }
    }

    public static Texture2D GrowTexture(Texture2D tex)
    {
        int width = tex.width;
        int height = tex.height;

        Texture2D tempTex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int x = 0; x < width; x++) // Init Texture to Black
        {
            for (int y = 0; y < height; y++)
            {
                tempTex.SetPixel(x, y, Color.black);
            }
        }

        for (int x = 0; x < width; x++) // fill TempTex
        {
            for (int y = 0; y < height; y++)
            {
                bool hasAdjacentPixel = false;

                if (x > 0 && hasAdjacentPixel == false) // linke reihe
                {
                    hasAdjacentPixel = tex.GetPixel(x - 1, y).r > 0f;
                }
                if (x < width - 1 && hasAdjacentPixel == false) // rechte reihe
                {
                    hasAdjacentPixel = tex.GetPixel(x + 1, y).r > 0f;
                }
                if (y > 0 && hasAdjacentPixel == false) // obere reihe
                {
                    hasAdjacentPixel = tex.GetPixel(x, y - 1).r > 0f;
                }
                if (y < height - 1 && hasAdjacentPixel == false) // untere reihe
                {
                    hasAdjacentPixel = tex.GetPixel(x, y + 1).r > 0f;
                }

                if (hasAdjacentPixel)
                {
                    tempTex.SetPixel(x, y, new Color(1f,1f,1f,1f));
                }
            }
        }

        for (int x = 0; x < width; x++) // Combine Textures
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, MaxColor(tex.GetPixel(x, y) ,tempTex.GetPixel(x, y)));
            }
        }

        tex.Apply();

        return tex;
    }

    public static Color MaxColor(Color colA, Color colB)
    {
        return new Color(
            Mathf.Max(colA.r, colB.r),
            Mathf.Max(colA.g, colB.g),
            Mathf.Max(colA.b, colB.b),
            Mathf.Max(colA.a, colB.a));
    }
}
