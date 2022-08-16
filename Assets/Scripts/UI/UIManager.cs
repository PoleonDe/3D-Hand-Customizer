using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class UIManager : SingletonObject<UIManager>
{
    const string S = "Settings";
    const string REF = "References";
    const string IN = "Internal";

    [TitleGroup(S)] public bool NeedsRepaint = false;


    [TitleGroup(IN)] public RaycastHit rayHit;
    [TitleGroup(IN)] public bool isRayHit;
    [TitleGroup(IN)] private Ray ray;
    [TitleGroup(IN)] private Camera cam;
    private Vector3 previousMousePos;
    [TitleGroup(IN)] [ShowInInspector] public Vector3 PreviousMousePos { get => previousMousePos; }
    private Vector3 mouseDelta;
    [TitleGroup(IN)] [ShowInInspector] public Vector3 MouseDelta { get => mouseDelta; }



    [TitleGroup(IN)] public bool IsWindow2D { get => isWindow2D; }
    [TitleGroup(IN)] [OdinSerialize] private bool isWindow2D;
    [TitleGroup(IN)] public bool IsWindow3D { get => isWindow3D; }
    [TitleGroup(IN)] [OdinSerialize] private bool isWindow3D;
    [TitleGroup(IN)] public bool IsPanelProperty { get => isPanelProperty; }
    [TitleGroup(IN)] [OdinSerialize] private bool isPanelProperty;
    [TitleGroup(IN)] public bool IsPanelTool { get => isPanelTool; }
    [TitleGroup(IN)] [OdinSerialize] private bool isPanelTool;

    [TitleGroup(IN)] int lastScreenWidth = 0;
    [TitleGroup(IN)] int lastScreenHeight = 0;


    [TitleGroup(REF)] [ShowInInspector] public PanelProperty PanelProperty {get => panelProperty;}
    [TitleGroup(REF)] PanelProperty panelProperty;
    [TitleGroup(REF)] [ShowInInspector]public PanelTool PanelTool {get => panelTool;}
    [TitleGroup(REF)] PanelTool panelTool;
    [TitleGroup(REF)] [ShowInInspector] public Window2D Window2D {get => window2D;}
    [TitleGroup(REF)] Window2D window2D;
    [TitleGroup(REF)] [ShowInInspector] public Window3D Window3D {get => window3D;}
    [TitleGroup(REF)] Window3D window3D;
    [TitleGroup(REF)] [ShowInInspector] public ViewSplitter ViewSplitter { get => viewSplitter; }
    [TitleGroup(REF)] ViewSplitter viewSplitter;
    [TitleGroup(REF)] [ShowInInspector]public Canvas Canvas {get => canvas;}
    [TitleGroup(REF)] Canvas canvas;

    private void Awake()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        cam = GameController.Instance.CameraUI;

        panelProperty = GameObject.FindObjectOfType<PanelProperty>();
        panelTool = GameObject.FindObjectOfType<PanelTool>();
        window2D = GameObject.FindObjectOfType<Window2D>();
        window3D = GameObject.FindObjectOfType<Window3D>();
        viewSplitter = GameObject.FindObjectOfType<ViewSplitter>();
        canvas = GameObject.FindObjectOfType<Canvas>();

        if (panelProperty == null || panelTool == null || window2D == null || window3D == null || canvas == null || viewSplitter == null)
        {
            Debug.LogError("UI manager couldnt find all required References");
        }
    }

    private void FixedUpdate()
    {
        mouseDelta = Input.mousePosition  - previousMousePos;
    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out rayHit, GameController.Instance.CameraUI.cullingMask))
        {
            isRayHit = true;
        }
        else
        {
            isRayHit = false;
        }

        if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height) // if screenSizeChanged do this:
        {
            NeedsRepaint = true;
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }

        if (NeedsRepaint)
        {
            Repaint();
        }
    }
    private void LateUpdate()
    {
        previousMousePos = Input.mousePosition;
    }

    public void Repaint()
    {
        window3D.UpdateViewQuad();
        window2D.UpdateViewQuad();
        viewSplitter.UpdateViewQuad();
        panelTool.UpdateSize();
        panelProperty.UpdateSize();

        NeedsRepaint = false;
    }
}
