using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class TextureDrawing : SerializedMonoBehaviour
{
    public Material material;
    public Texture2D texture;
    public int textureWidth = 512;
    public int textureHeight = 512;

    Camera cam;

    public Vector2 p1UV;
    public Vector2 p2UV;


    void Start()
    {
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
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
        { 
            return;
        }

        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (meshCollider == null || meshCollider.sharedMesh == null)
        {
            return;
        }    

        Mesh mesh = meshCollider.sharedMesh;
        Vector2[] uvs = mesh.uv;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        Debug.DrawLine(p0, p1);
        Debug.DrawLine(p1, p2);
        Debug.DrawLine(p2, p0);

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left Mouse Button Down");
            p1UV = hit.textureCoord;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Left Mouse Button Up");
            p2UV = hit.textureCoord;

            line((int)Mathf.Floor(textureWidth * p1UV.x), (int)Mathf.Floor(textureHeight * p1UV.y), (int)Mathf.Floor(512 * p2UV.x), (int)Mathf.Floor(512 * p2UV.y), Color.white, texture);
            //texture.SetPixel((int)Mathf.Floor(textureWidth * uvCord.x), (int)Mathf.Floor(textureHeight * uvCord.y), Color.black);
            texture.Apply();
        }

        if (Input.GetKeyUp(KeyCode.J))
        {
            TextureDiffusion.GrowTexture(texture);
        }

        if (Input.GetKeyUp(KeyCode.H))
        {
            SaveTextureAsPNG(texture, "C:/Users/Malte/Desktop/HandCustomizer/Assets/Visuals/Texture/savedTexture.png");
        }
    }

    public void line(int x, int y, int x2, int y2, Color color, Texture2D tex)
    {
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            tex.SetPixel(x, y, color);
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
    }

    public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
    {
        byte[] _bytes = _texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(_fullPath, _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
    }
}
