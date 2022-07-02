using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TextureLineToUV : SerializedMonoBehaviour
{
    public Texture2D texture;
    public Material material;
    public bool isInit = false;

    [Range(0,500)]public float distance = 100f;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            Debug.Log("Init");
            material = this.gameObject.GetComponent<MeshRenderer>().material;
            texture = (Texture2D)material.GetTexture("_BaseMap");
            isInit = true;
        }

        if (isInit)
        { 
            if (Input.GetKeyUp(KeyCode.U))
            {
                ResetTexture(texture);
                Vector2 lineposA = new Vector2(0, 0);
                Vector2 lineposB = new Vector2(511, 511);

                for (int i = 0; i < texture.width; i++)
                {
                    for (int j = 0; j < texture.height; j++)
                    {
                        Vector2 p = new Vector2(i, j);
                        float c = SdfLine(p, lineposA, lineposB);
                        c = Mathf.Clamp01(remap(c,0f,distance,1f,0f));
                        Vector2 nP = FindNearestPointOnLine(lineposA, lineposB, p);
                        float nPpercent = PositionPercentOnLine(lineposA, lineposB, nP);
                        texture.SetPixel(i, j, new Color(c, nPpercent, 0,1f));
                    }
                }
                texture.Apply();

            }
        }
    }

    float SdfLine(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ba = b - a;
        Vector2 pa = p - a;
        float h = Mathf.Clamp(Vector2.Dot(pa, ba) / Vector2.Dot(ba, ba), 0, 1);
        return (pa - h * ba).magnitude;
    }
    public Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 end, Vector2 point)
    {
        //Get heading
        Vector2 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector2 lhs = point - origin;
        float dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
    }

    public float PositionPercentOnLine(Vector2 origin, Vector2 end, Vector2 point)
    {
        return Mathf.InverseLerp(origin.x, end.x, point.x);
    }

    private void ResetTexture(Texture2D texture)
    {
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                texture.SetPixel(i, j, Color.black);
            }
        }
        texture.Apply();
    }

    private float remap(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
