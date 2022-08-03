using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class ViewSplitter : ViewQuad
{
    const string S = "Settings";
    const string IN = "Internal";
    const string REF = "References";

    [TitleGroup(S)] public float WidthPixel { get => widthPixel; }
    [TitleGroup(S)] [OdinSerialize] private float widthPixel = 40f;
    [TitleGroup(S)] [OdinSerialize] private float sortingDepth = -0.1f;

    [TitleGroup(IN)] [ShowInInspector] private bool isMoving;

    private void Update()
    {
        if (UIManager.Instance.isRayHit)
        {
            if (UIManager.Instance.rayHit.collider.gameObject == this.gameObject)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isMoving = true;
                }  
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (isMoving)
            { 
                SetPosition(GameController.Instance.CameraUI.ScreenToWorldPoint(Input.mousePosition));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMoving = false;
        }
    }
    public override Vector2[] CalculateAnchors()
    {
        Vector2 A1 = new Vector2(this.transform.position.x - (WidthPixel / 2f), UIManager.Instance.Canvas.pixelRect.height /2f);
        Vector2 A2 = new Vector2(this.transform.position.x + (WidthPixel / 2f), -UIManager.Instance.Canvas.pixelRect.height /2f );
        return new Vector2[2] { A1, A2 };
    }

    public override void UpdateViewQuadDependencies()
    {
        //throw new System.NotImplementedException();
    }

    public void SetPosition(Vector3 _worldPos)
    {
        Debug.Log("moved View Splitter");

        float minX = GameController.Instance.CameraUI.ScreenToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        float maxX = GameController.Instance.CameraUI.ScreenToWorldPoint(new Vector3(UIManager.Instance.Canvas.pixelRect.width,0f,0f)).x;

        if (UIManager.Instance.IsPanelTool) // if PanelTool is Active clamp to Tool Panel
        {
            minX = GameController.Instance.CameraUI.ScreenToWorldPoint(new Vector3(UIManager.Instance.PanelTool.WidthPixel, 0f, 0f)).x;
        }
        if (UIManager.Instance.IsPanelProperty)// if PanelTool is Active clamp to Property Panel
        {
            maxX = GameController.Instance.CameraUI.ScreenToWorldPoint(new Vector3(UIManager.Instance.Canvas.pixelRect.width - UIManager.Instance.PanelProperty.WidthPixel, 0f, 0f)).x;
        }

        this.transform.position = new Vector3(Mathf.Clamp(_worldPos.x,minX,maxX), 0f, sortingDepth);

        UIManager.Instance.NeedsRepaint = true;
    }
}
