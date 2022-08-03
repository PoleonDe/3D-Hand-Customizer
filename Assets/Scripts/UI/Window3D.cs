using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
public class Window3D : ViewQuad
{
    const string S = "Settings";
    const string REF = "References";
    const string IN = "Internal";

    [TitleGroup(REF)] [ShowInInspector] Navigation navigation;

    new void Start()
    {
        base.Start();
        navigation = GameController.Instance.Camera3D.GetComponent<Navigation>();
        Debug.Log("window3Dstart");
    }
       

    void Update()
    {
        if (UIManager.Instance.isRayHit)
        {
            if (UIManager.Instance.rayHit.collider.gameObject == this.gameObject)
            {
                //Debug.Log(UIManager.Instance.rayHit.textureCoord);
                navigation.PropagateRaycast(UIManager.Instance.rayHit.textureCoord);

                if (Input.GetMouseButton(0))
                {
                    navigation.RotateCamera(UIManager.Instance.MouseDelta);
                }
                if (Input.GetMouseButton(1))
                {
                    navigation.ZoomCamera(UIManager.Instance.MouseDelta);
                }
                if (Input.GetMouseButton(2))
                {
                    navigation.PanCamera(UIManager.Instance.MouseDelta);
                }

                Debug.DrawRay(UIManager.Instance.rayHit.point, Vector3.up * 0.1f);
            }
        }
    }

    public override Vector2[] CalculateAnchors()
    {
        Vector2 A1 = new Vector2((-UIManager.Instance.Canvas.pixelRect.width / 2f) + UIManager.Instance.PanelTool.WidthPixel, UIManager.Instance.Canvas.pixelRect.height / 2f);

        Vector2 A2;
        if (UIManager.Instance.IsWindow2D)
        { // set Anchor bound to View Splitter
            Vector2 viewSplitterScreenPoint = GameController.Instance.CameraUI.WorldToScreenPoint(UIManager.Instance.ViewSplitter.transform.position); //MISSING : + (Vector3.left * UIManager.Instance.ViewSplitter.WidthPixel)
            A2 = new Vector2(viewSplitterScreenPoint.x - (UIManager.Instance.Canvas.pixelRect.width /2f), -UIManager.Instance.Canvas.pixelRect.height / 2f); // offset by half the screen, because anchor is not in bottom left, but in middle
        }
        else
        { // set Anchor bound to PropertyPanel
            A2 = new Vector2((UIManager.Instance.Canvas.pixelRect.width / 2f) - UIManager.Instance.PanelProperty.WidthPixel, -UIManager.Instance.Canvas.pixelRect.height / 2f);
        }
        
        return new Vector2[] {A1,A2};
    }

    public override void UpdateViewQuadDependencies()
    {
        if (quad.IsValid)
        { 
            GameController.Instance.Camera3D.rect = new Rect(0, 0, quad.AspectRatio.x, quad.AspectRatio.y);
        }
    }
}
