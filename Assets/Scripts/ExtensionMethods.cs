using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 Vec3(this Vector2 vec2)
    {
        return new Vector3(vec2.x, vec2.y, 0f);
    }

    public static Vector2 Vec2(this Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.y);
    }


    public static void SwitchVectors(ref Vector3 a, ref Vector3 b)
    {
        Vector3 temp = b;
        b = a;
        a = temp;
    }

    public static void DebugLogRect(this RectTransform rectTrans)
    {
        Debug.Log("rect = " + rectTrans.rect);
        Debug.Log("sizeDelta = " + rectTrans.sizeDelta);
        Debug.Log("pivot = " + rectTrans.pivot);
        Debug.Log("anchor Position = " + rectTrans.anchoredPosition);
        Debug.Log("drivenByObject = " + rectTrans.drivenByObject);
        Vector3[] v = new Vector3[4];
        rectTrans.GetLocalCorners(v);
        Debug.Log("GetLocalCorners = " + v[0] + " " + v[1] + " " + v[2] + " " + v[3]);
        rectTrans.GetWorldCorners(v);
        Debug.Log("GetWorldCorners = " + v[0] + " " + v[1] + " " + v[2] + " " + v[3]);
    }

    public static void DebugLogQuad(this Quad2D quad)
    {
        Debug.Log("Width = " + quad.Width);
        Debug.Log("Height = " + quad.Height);
        Debug.Log("AspectRatio = " + quad.AspectRatio);
        Debug.Log("TopLeft = " + quad.TopLeft);
        Debug.Log("BottomRight = " + quad.BottomRight);
    }
}
