using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
public abstract class ViewQuad : SerializedMonoBehaviour
{
    const string S = "Settings";
    const string REF = "References";
    const string IN = "Internal";

    [TitleGroup(IN)] [ShowInInspector] protected Quad2D quad;

    [TitleGroup(S)] [ShowInInspector] public Vector2 TL = new Vector2(-100f, 100f);
    [TitleGroup(S)] [ShowInInspector] public Vector2 BR = new Vector2(100f, -100f);

    [TitleGroup(REF)] [ShowInInspector] protected MeshFilter meshFilter;
    [TitleGroup(REF)] [ShowInInspector] protected MeshCollider meshCollider;

    protected void Awake()
    {
        meshCollider = this.gameObject.GetComponent<MeshCollider>();
        meshFilter = this.gameObject.GetComponent<MeshFilter>();
        quad = new Quad2D(TL, BR);
    }

    protected void Start()
    {
        UpdateViewQuad();
        Debug.Log("base start");
    }

    public abstract Vector2[] CalculateAnchors();

    public void UpdateViewQuad()
    {
        if (meshFilter == null || meshCollider == null)
        {
            Debug.LogWarning("tried to Update view quad, but MeshFilter or MeshCollider are null");
            Debug.LogWarning("MeshFilter = " + meshFilter + "  MeshCollider = " + meshCollider);
            return;
        }

        Vector2[] anchors = CalculateAnchors();
        TL = anchors[0];
        BR = anchors[1];
        quad.UpdatePositions(TL, BR, true);

        meshCollider.sharedMesh = quad.Mesh2D;
        meshFilter.mesh = quad.Mesh2D;

        Debug.Log("Updated Quad " + this.gameObject.name);

        UpdateViewQuadDependencies();
    }

    public abstract void UpdateViewQuadDependencies();

    [Button]
    public void DebugQuad()
    {
        quad.DebugLogQuad();
    }

}
