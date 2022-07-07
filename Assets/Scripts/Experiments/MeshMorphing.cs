using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
public class MeshMorphing : SerializedMonoBehaviour
{
    const string S = "Settings";
    const string R = "References";

    [TitleGroup(S)] public Mesh meshA;
    [TitleGroup(S)] public Mesh meshB;
    [TitleGroup(S)] private float percentMesh;
    [TitleGroup(S)] [ShowInInspector] public float PercentMesh { 
        get => percentMesh;
        set { 
            percentMesh = value;
            mF.mesh = MorphMeshes(meshA, meshB, percentMesh);
        }
    }

    [TitleGroup(R)] public MeshFilter mF;


    void Start()
    {
        mF = this.gameObject.GetComponent<MeshFilter>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            percentMesh -= 0.1f;
            percentMesh = Mathf.Clamp01(percentMesh);
            Debug.Log(percentMesh);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            percentMesh += 0.1f;
            percentMesh = Mathf.Clamp01(percentMesh);
            Debug.Log(percentMesh);
        }
    }


    Mesh MorphMeshes(Mesh meshA, Mesh meshB, float t)
    {
        if (meshA.vertexCount != meshB.vertexCount)
        {
            Debug.LogWarning("Meshes dont have the same VertexCount");
            return null;
        }

        int vCount = meshA.vertexCount;

        Mesh mesh = new Mesh();

        Vector3[] meshApos = meshA.vertices;
        Vector3[] meshAnorm = meshA.normals;
        Vector2[] meshAuv = meshA.uv;

        Vector3[] meshBpos = meshB.vertices;
        Vector3[] meshBnorm = meshB.normals;
        Vector2[] meshBuv = meshB.uv;


        Vector3[] pos = new Vector3[vCount];
        Vector3[] norm = new Vector3[vCount];
        Vector2[] uv = new Vector2[vCount];

        for (int i = 0; i < vCount; i++)
        {
            pos[i] = LerpVector3(meshApos[i], meshBpos[i], t);
            norm[i] = LerpVector3(meshAnorm[i], meshBnorm[i], t);
            uv[i] = LerpVector2(meshAuv[i], meshBuv[i], t);
        }

        mesh.SetVertices(pos.ToList());
        mesh.SetNormals(norm.ToList());
        mesh.SetUVs(0, uv.ToList());
        mesh.triangles = meshA.triangles;

        return mesh;
    }

    Vector3 LerpVector3(Vector3 a, Vector3 b, float t)
    {
        Vector3 vec = new Vector3(
            Mathf.LerpUnclamped(a.x, b.x, t),
            Mathf.LerpUnclamped(a.y, b.y, t),
            Mathf.LerpUnclamped(a.z, b.z, t)
            );

        return vec;
    }

    Vector2 LerpVector2(Vector2 a, Vector2 b, float t)
    {
        Vector2 vec = new Vector2(
            Mathf.LerpUnclamped(a.x, b.x, t),
            Mathf.LerpUnclamped(a.y, b.y, t)
            );

        return vec;
    }


}
