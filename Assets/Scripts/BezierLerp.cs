using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class BezierLerp : SerializedMonoBehaviour
{
    public CubicBezierCurve cBC1;
    public CubicBezierCurve cBC2;
    public CubicBezierCurve lerpCurve;
    private float t = 0.5f;
    [PropertyRange(0f,1f), OdinSerialize]public float T { get => t; set { t = value; RepaintLerp(); } }

    private void Start()
    {
        InitCBCs();
    }
    void InitCBCs()
    {
        cBC1 = new CubicBezierCurve(new Vector3[4] {
            new Vector3(0f,0f,0f),
            new Vector3(0f,1f,0f),
            new Vector3(1f,1.5f,0f),
            new Vector3(2f,1f,0f)});

        cBC2 = new CubicBezierCurve(new Vector3[4] {
            new Vector3(-1f,-2f,0f),
            new Vector3(1f,-2f,0f),
            new Vector3(2f,0f,0f),
            new Vector3(0f,-1f,0f)});

        lerpCurve = LerpCubic(cBC1, cBC2, T);
    }
    void RepaintLerp()
    {
        lerpCurve = LerpCubic(cBC1, cBC2, T);
    }
    CubicBezierCurve LerpCubic(CubicBezierCurve A, CubicBezierCurve B, float t)
    {
        Vector3[] posA = A.GetControlVerts();
        Vector3[] posB = B.GetControlVerts();

        Vector3[] newPos = new Vector3[4]{
            VectorLerp(posA[0], posB[0], t),
            VectorLerp(posA[1], posB[1], t),
            VectorLerp(posA[2], posB[2], t),
            VectorLerp(posA[3], posB[3], t)
        };
        return new CubicBezierCurve(newPos);
    }

    Vector3 VectorLerp(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t), Mathf.Lerp(a.z, b.z, t));
    }

    private void OnDrawGizmos()
    {
        if (cBC1 != null && cBC2 != null && lerpCurve != null)
        { 
            float step = 1f / 100f;
            for (float t = 0f; t < 1f; t += step)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(cBC1.GetPoint(t), Vector3.one * 0.01f);
                Gizmos.DrawCube(cBC2.GetPoint(t), Vector3.one * 0.01f);
                Gizmos.color = Color.red;
                Gizmos.DrawCube(lerpCurve.GetPoint(t), Vector3.one * 0.01f);
            }

            Vector3[] cBC1POS = cBC1.GetControlVerts();
            Vector3[] cBC2POS = cBC2.GetControlVerts();
            Vector3[] LerpCurvePOS = lerpCurve.GetControlVerts();
            for (int i = 0; i < 4; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(cBC1POS[i], Vector3.one * 0.04f);
                Gizmos.DrawCube(cBC2POS[i], Vector3.one * 0.04f);
                Gizmos.color = Color.red;
                Gizmos.DrawCube(LerpCurvePOS[i], Vector3.one * 0.04f);
            }
        }
    }
}
