using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad2D
{
    Mesh mesh2D;
    public Mesh Mesh2D { get => mesh2D; }
    public float Width { get => Mathf.Abs(bottomRight.x - topLeft.x); }
    public float Height { get => Mathf.Abs(bottomRight.y - topLeft.y); }
    public Vector2 AspectRatio { get => new Vector2(1f, Height / Width); }

    Vector3 topLeft; // Top Left
    public Vector3 TopLeft { get => topLeft; }
    Vector3 bottomRight; // Bottom Right
    public Vector3 BottomRight { get => bottomRight; }


public Quad2D(Vector2 _topLeft, Vector2 _bottomRight)
    {
        //assign the class variables
        topLeft = _topLeft.Vec3();
        bottomRight = _bottomRight.Vec3();

        if (_topLeft.x == _bottomRight.x || _topLeft.y == _bottomRight.y)
        {
            Debug.LogWarning("One Dimension of Quad is 0");
        }

        //create Mesh
        mesh2D = new Mesh();
        mesh2D.name = "QuadMesh";

        Vector3[] vertices = CalculatePositions();
        int[] triangles = new int[6] {
            0,1,2,
            0,2,3};
        Vector3[] normals = new Vector3[4] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
        Vector2[] uvs = CalculateUVs(true);

        mesh2D.vertices = vertices;
        mesh2D.triangles = triangles;
        mesh2D.normals = normals;
        mesh2D.uv = uvs;
    }

    private Vector3[] CalculatePositions()
    {
        //calculate missing edge points
        Vector3 topRight = new Vector3(bottomRight.x, topLeft.y, 0f);
        Vector3 bottomLeft = new Vector3(topLeft.x, bottomRight.y, 0f);

        //check if bottomRight is further left / up then topLeft
        if (bottomRight.x < topLeft.x) //flip x´s
        {
            ExtensionMethods.SwitchVectors(ref topRight, ref topLeft);
            ExtensionMethods.SwitchVectors(ref bottomRight, ref bottomLeft);
        }
        if (bottomRight.y > topLeft.y) //flip y´s
        {
            ExtensionMethods.SwitchVectors(ref topRight, ref bottomRight);
            ExtensionMethods.SwitchVectors(ref topLeft, ref bottomLeft);
        }

        return new Vector3[4] { topLeft, topRight, bottomRight, bottomLeft };
    }

    private Vector2[] CalculateUVs(bool UseAspectRatio = false)
    {
        if (UseAspectRatio)
        {
            float H, W;
            if (Width >= Height) // Horizontal or Square 
            {
                H = Height / Width;
                W = 1f;
            }
            else // Vertical
            {
                H = 1f;
                W = Width / Height;
            }
            return new Vector2[4] { new Vector2(0f, H), new Vector2(W, H), new Vector2(W, 0f), new Vector2(0f, 0f) };
        }
        else
        {
            return new Vector2[4] { new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f) };
        }
    }

    public void UpdatePositions(Vector2 _topLeft, Vector2 _bottomRight, bool UseAspectRatio)
    {
        if (mesh2D == null)
        {
            Debug.LogWarning("Quad wasnt Initialized");
            return;
        }

        //assign the class variables
        topLeft = _topLeft.Vec3();
        bottomRight = _bottomRight.Vec3();

        mesh2D.vertices = CalculatePositions();
        mesh2D.uv = CalculateUVs(UseAspectRatio);
    }
}
