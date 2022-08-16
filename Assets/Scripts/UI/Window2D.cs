using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class Window2D : ViewQuad
{
    const string S = "Settings";
    const string REF = "References";
    const string IN = "Internal";

    [TitleGroup(S)] public bool canNavigate = true;
    [TitleGroup(REF)] [ShowInInspector] Navigation2D navigation;

    new void Start()
    {
        base.Start();
        navigation = GameController.Instance.Camera2D.GetComponent<Navigation2D>();
        Debug.Log("window2Dstart");
    }

    void Update()
    {
        if (canNavigate)
        {
            if (UIManager.Instance.isRayHit)
            {
                if (UIManager.Instance.rayHit.collider.gameObject == this.gameObject)
                {
                    if (Input.GetMouseButton(1) || Input.mouseScrollDelta != Vector2.zero)
                    {
                        navigation.ZoomCamera(Input.mouseScrollDelta.y);
                        Debug.Log("zoom");
                    }
                    if (Input.GetMouseButton(0) || Input.GetMouseButton(2))
                    {
                        navigation.PanCamera(UIManager.Instance.MouseDelta);
                        Debug.Log("pan");
                    }

                    Debug.DrawRay(UIManager.Instance.rayHit.point, Vector3.up * 0.1f);
                }
            }
        }
    }

    public override Vector2[] CalculateAnchors()
    {
        Vector2 A2 = new Vector2((UIManager.Instance.Canvas.pixelRect.width / 2f) - UIManager.Instance.PanelProperty.WidthPixel, UIManager.Instance.Canvas.pixelRect.height / 2f);

        Vector2 A1;
        if (UIManager.Instance.IsWindow2D)
        { // set Anchor bound to View Splitter
            Vector2 viewSplitterScreenPoint = GameController.Instance.CameraUI.WorldToScreenPoint(UIManager.Instance.ViewSplitter.transform.position); //MISSING : + (Vector3.left * UIManager.Instance.ViewSplitter.WidthPixel)
            A1 = new Vector2(viewSplitterScreenPoint.x - (UIManager.Instance.Canvas.pixelRect.width / 2f), -UIManager.Instance.Canvas.pixelRect.height / 2f); // offset by half the screen, because anchor is not in bottom left, but in middle
        }
        else
        { // set Anchor bound to PropertyPanel
            A1 = new Vector2((UIManager.Instance.Canvas.pixelRect.width / 2f) - UIManager.Instance.PanelProperty.WidthPixel, -UIManager.Instance.Canvas.pixelRect.height / 2f);
        }

        return new Vector2[] { A1, A2 };
    }

    public override void UpdateViewQuadDependencies()
    {
        if (quad.IsValid)
        {
            GameController.Instance.Camera2D.rect = new Rect(0, 0, quad.AspectRatio.x, quad.AspectRatio.y);
        }
    }
}
