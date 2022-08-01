using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
public class Window3D : MonoBehaviour
{
    const string S = "Settings";
    const string REF = "References";
    const string IN = "Internal";

    [TitleGroup(S)] [ShowInInspector] public Vector2 TL;
    [TitleGroup(S)] [ShowInInspector] public Vector2 BR;


    [TitleGroup(IN)] [ShowInInspector] Quad2D quad;

    [TitleGroup(REF)] [ShowInInspector] Navigation3D navigation3D;
    [TitleGroup(REF)] [ShowInInspector] MeshFilter meshFilter;
    [TitleGroup(REF)] [ShowInInspector] MeshCollider meshCollider;

    [TitleGroup(REF)] [ShowInInspector] Canvas canvas;



    private void Start()
    {
        navigation3D = GameController.Instance.Camera3D.GetComponent<Navigation3D>();
        meshCollider = this.gameObject.GetComponent<MeshCollider>();
        meshFilter = this.gameObject.GetComponent<MeshFilter>();
        canvas = GameObject.FindObjectOfType<Canvas>();

        quad = new Quad2D(new Vector2(-canvas.pixelRect.width / 2f, canvas.pixelRect.height / 2f), new Vector2(canvas.pixelRect.width / 2f, -canvas.pixelRect.height / 2f));
        UpdateViewQuad();

    }

    void Update()
    {
        if (UIManager.Instance.isRayHit)
        {
            if (UIManager.Instance.rayHit.collider.gameObject == this.gameObject)
            {
                //Debug.Log(UIManager.Instance.rayHit.textureCoord);
                navigation3D.PropagateRaycast(UIManager.Instance.rayHit.textureCoord);

                if (Input.GetMouseButton(0))
                {
                    navigation3D.RotateCamera(UIManager.Instance.MouseDelta);
                }
                if (Input.GetMouseButton(1))
                {
                    navigation3D.ZoomCamera(UIManager.Instance.MouseDelta);
                }
                if (Input.GetMouseButton(2))
                {
                    navigation3D.PanCamera(UIManager.Instance.MouseDelta);
                }

                Debug.DrawRay(UIManager.Instance.rayHit.point, Vector3.up * 0.1f);
            }
        }
    }

    [Button]
    public void DebugQuad()
    {
        quad.DebugLogQuad();
    }
    public void UpdateViewQuad()
    {
        if (meshFilter == null || meshCollider == null)
        {
            Debug.LogWarning("tried to Update view quad, but MeshFilter or MeshCollider are null");
            Debug.LogWarning("MeshFilter = " + meshFilter + "  MeshCollider = " + meshCollider);
            return;
        }

        //quad.UpdatePositions(new Vector2(-canvas.pixelRect.width / 2f, canvas.pixelRect.height / 2f), new Vector2(canvas.pixelRect.width / 2f, -canvas.pixelRect.height / 2f), true);
        

        quad.UpdatePositions(new Vector2((-canvas.pixelRect.width / 2f) + UIManager.Instance.panelTool.Width, canvas.pixelRect.height / 2f), new Vector2 ((canvas.pixelRect.width / 2f) - UIManager.Instance.panelProperty.Width, -canvas.pixelRect.height / 2f), true);
        GameController.Instance.Camera3D.rect = new Rect(0, 0, quad.AspectRatio.x, quad.AspectRatio.y);

        meshCollider.sharedMesh = quad.Mesh2D;
        meshFilter.mesh = quad.Mesh2D;

        Debug.Log("Updated Quad");
    }
}
