using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class UIManager : SingletonObject<UIManager>
{
    const string S = "Settings";
    const string REF = "References";
    const string IN = "Internal";


    [TitleGroup(IN)] public RaycastHit rayHit;
    [TitleGroup(IN)] public bool isRayHit;
    [TitleGroup(IN)] private Ray ray;
    [TitleGroup(IN)] private Camera cam;
    private Vector3 previousMousePos;
    [TitleGroup(IN)] [ShowInInspector] public Vector3 PreviousMousePos { get => previousMousePos; }
    private Vector3 mouseDelta;
    [TitleGroup(IN)] [ShowInInspector] public Vector3 MouseDelta { get => mouseDelta; }



    [TitleGroup(IN)] [ShowInInspector] private bool isWindow2D;
    [TitleGroup(IN)] [ShowInInspector] private bool isWindow3D;
    [TitleGroup(IN)] [ShowInInspector] private bool isPanelProperty;
    [TitleGroup(IN)] [ShowInInspector] private bool isPanelTool;

    [TitleGroup(IN)] int lastScreenWidth = 0;
    [TitleGroup(IN)] int lastScreenHeight = 0;


    [TitleGroup(REF)] public PanelProperty panelProperty;
    [TitleGroup(REF)] public PanelTool panelTool;
    [TitleGroup(REF)] public Window2D window2D;
    [TitleGroup(REF)] public Window3D window3D;

    private void Start()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        cam = GameController.Instance.CameraUI;

        panelProperty = GameObject.FindObjectOfType<PanelProperty>();
        panelTool = GameObject.FindObjectOfType<PanelTool>();
        window2D = GameObject.FindObjectOfType<Window2D>();
        window3D = GameObject.FindObjectOfType<Window3D>();

        if (panelProperty == null || panelTool == null || window2D == null || window3D == null)
        { 
            
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
            window3D.UpdateViewQuad();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }
    private void LateUpdate()
    {
        previousMousePos = Input.mousePosition;
    }
}
