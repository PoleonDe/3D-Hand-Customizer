using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.Assertions;

public class BezierTexture : SerializedMonoBehaviour
{
    public Material material;
    public Texture2D texture;
    public int textureWidth = 512;
    public int textureHeight = 512;

    public Vector3 pos;

    public CubicBezierCurve cubicBezierCurve;

    [OdinSerialize] public float DistThreshold { get => distThreshold; set { distThreshold = value; Repaint?.Invoke(); } }
    private float distThreshold;

    [OdinSerialize] public Vector3 P1 { get => p1; set { p1 = value; Repaint?.Invoke(); } }
    [OdinSerialize] public Vector3 P2 { get => p2; set { p2 = value; Repaint?.Invoke(); } }
    [OdinSerialize] public Vector3 P3 { get => p3; set { p3 = value; Repaint?.Invoke(); } }
    [OdinSerialize] public Vector3 P4 { get => p4; set { p4 = value; Repaint?.Invoke(); } }

    private Vector3 p1, p2, p3, p4;


    [OdinSerialize] public bool R { get => r; set { r = value; Repaint?.Invoke(); } }
    [OdinSerialize] public bool G { get => g; set { g = value; Repaint?.Invoke(); } }
    [OdinSerialize] public bool B { get => b; set { b = value; Repaint?.Invoke(); } }

    private bool r, g, b;

    [OdinSerialize] public bool ClampT { get => clampT; set { clampT = value; Repaint?.Invoke(); } }

    private bool clampT;

    Action Repaint = null;
    bool initialized = false;


    Camera cam;

    void Start()
    {
        Repaint += CreateAndPaintTexture;

        ///////////////////////////////////////////

        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);

        for (int i = 0; i < textureWidth; i++)
        {
            for (int j = 0; j < textureHeight; j++)
            {
                texture.SetPixel(i, j, Color.black);
            }
        }
        texture.Apply();

        material = this.gameObject.GetComponent<MeshRenderer>().material;
        material.SetTexture("_BaseMap", texture);
        cam = Camera.main;

        CreateAndPaintTexture();

        initialized = true;
    }

    void CreateAndPaintTexture()
    {
        cubicBezierCurve = new CubicBezierCurve(new Vector3[] { p1, p2, p3, p4 });
        PaintTexture(texture, cubicBezierCurve);
    }

    void PaintTexture(Texture2D tex, CubicBezierCurve cubicBezierCurve)
    {
        float x,y,dist,t = 0f;
        Vector3 p, cubP;
        float col;

        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                float w = (float)tex.width;
                float iw = (float)i;
                float h = (float)tex.height;
                float jh = (float)j;

                x = ((iw / w) -0.5f) *2f;
                y = ((jh/ h) - 0.5f) * 2f;

                p = new Vector3(x, y, 0f);

                t = cubicBezierCurve.GetClosestParam(p);
                cubP = cubicBezierCurve.GetPoint(t);
                dist = Vector3.Distance(p, cubP);
                col = dist < distThreshold ? 1f : 0f;

                dist = Mathf.Clamp01(remap(-dist + distThreshold,0f,distThreshold,0f,1f));
                t = clampT ? t * Convert.ToSingle(dist > 0f) : t;

                Color color = new Color(r ? dist : 0f, g ? t : 0f, b ? col : 0f, 1f);
                tex.SetPixel(i, j, color );
                //tex.SetPixel(i, j, new Color(dist/2f, dist / 2f, dist / 2f, 1f));

            }
        }

        tex.Apply();
    }

    private float remap(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    private void OnDrawGizmos()
    {
        if (cubicBezierCurve != null && initialized && this.enabled)
        { 
            Vector3[] cubic = cubicBezierCurve.GetControlVerts();
            float s = 0.1f;
            Vector3 size = new Vector3(s, s, s);

            Gizmos.color = new Color(1, 0, 0, 0.5f);

            Gizmos.DrawCube(cubic[0], size);
            Gizmos.DrawCube(cubic[1], size);
            Gizmos.DrawCube(cubic[2], size);
            Gizmos.DrawCube(cubic[3], size);

            float step = 1f / 25f;
            for (float i = 0; i < 1; i += step)
            {
                Gizmos.DrawCube(cubicBezierCurve.GetPoint(i), size/2);
            }

            Gizmos.color = new Color(0, 0, 1, 0.8f);
            Gizmos.DrawCube(pos, size);

            Gizmos.color = new Color(0, 1, 0, 0.8f);
            Vector3 cubPos = cubicBezierCurve.GetPoint(cubicBezierCurve.GetClosestParam(pos));
            Gizmos.DrawCube(cubPos, size);
        }
    }
}
