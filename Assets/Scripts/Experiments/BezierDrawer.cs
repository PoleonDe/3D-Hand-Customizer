using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(BezierDrawer), true)]
public class BezierDrawerEditor : OdinEditor
{
    BezierDrawer bD;

    float wireBoxScale = 0.1f;
    new void OnEnable()
    {
        bD = (BezierDrawer)target;
    }

    void OnSceneGUI()
    {
        if (bD.cubicBezierCurves != null) 
        { 
            float step = 0.1f;
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontStyle = FontStyle.Bold;

            for (int i = 0; i < bD.cubicBezierCurves.Count; i++)
            {
                Handles.color = Color.magenta;
                for (float t = 0f; t < 1f; t += step)
                {
                    Handles.DrawWireCube(bD.cubicBezierCurves[i].GetPoint(t), Vector3.one * 0.01f);
                }

                Handles.color = Color.red;
                Vector3[] cPoints = bD.cubicBezierCurves[i].GetControlVerts();

                for (int j = 0; j < cPoints.Length; j++)
                {
                    Handles.DrawWireCube(cPoints[j], Vector3.one * 0.03f);
                }

                Handles.Label((cPoints[0] + cPoints[1] + cPoints[2] + cPoints[3])/4f, i.ToString());
            }

            if (bD.currentCurve != null)
            {
                int depth = (int)bD.BezierPointsDrawState;
                for (int i = 0; i < depth; i++)
                {
                    Handles.DrawWireCube(bD.currentCurve[i], Vector3.one * 0.03f);
                }
            }
        }
    }
}
#endif


public class BezierDrawer : SerializedMonoBehaviour
{
    const string S = "Settings";
    const string REF = "References";
    const string D = "Debug";

    [TitleGroup(S)] [OdinSerialize] public bool autoUpdate = true;

    [TitleGroup(S)] [OdinSerialize] public bool R { get => r; set { r = value; if (autoUpdate) { RepaintEvent?.Invoke(); } } }
    [TitleGroup(S)] [OdinSerialize] public bool G { get => g; set { g = value; if (autoUpdate) { RepaintEvent?.Invoke(); } } }
    [TitleGroup(S)] [OdinSerialize] public bool B { get => b; set { b = value; if (autoUpdate) { RepaintEvent?.Invoke(); } } }

    private bool r, g, b;
    [TitleGroup(S)] [OdinSerialize] public float DistThreshold { get => distThreshold; set { distThreshold = value; if (autoUpdate) { RepaintEvent?.Invoke(); } } }
    private float distThreshold = 0.1f;

    [TitleGroup(S)] [OdinSerialize] public bool ClampT { get => clampT; set { clampT = value; if (autoUpdate) { RepaintEvent?.Invoke(); } } }

    private bool clampT;

    int textureDimensions = 512;
    [TitleGroup(S)] [OdinSerialize] [ValueDropdown("TextureDimensionsDropdown")] public int TextureDimensions { get => textureDimensions; set { textureDimensions = value; InitializeTexture(); } }
    private int[] TextureDimensionsDropdown = { 128, 256, 512, 1024, 2048 };

    [TitleGroup(S)] public float pointScale = 0.05f;

    [TitleGroup(REF)] public Material material;
    [TitleGroup(REF)] public Texture2D texture;


    [TitleGroup(REF)] public List<CubicBezierCurve> cubicBezierCurves = new List<CubicBezierCurve>();
    [TitleGroup(REF)] public Vector3[] currentCurve = new Vector3[4];

    public enum PaintCurvesOptions {
        ConstantDistance,
        NormalizedDistance,
        RightLeft,
        ExtendTangents
    }
    [TitleGroup(S)] public PaintCurvesOptions paintCurvesOptions;

    public enum BezierDrawState { 
        None = -1,
        P1 = 0,
        P2 = 1,
        P3 = 2,
        P4 = 3,
    }
    private BezierDrawState bezierPointsDrawState = BezierDrawState.None;
    public BezierDrawState BezierPointsDrawState { 
        get => bezierPointsDrawState; 
        set { 
            bezierPointsDrawState = value;
            Debug.Log(value);
        } 
    }

    RaycastHit hit;
    Ray ray;
    Camera cam;

    Action RepaintEvent = null;


    private void Start()
    {
        RepaintEvent += Repaint;

        ///
        cam = Camera.main;
        currentCurve = new Vector3[4];
        cubicBezierCurves = new List<CubicBezierCurve>();

        InitializeTexture();

        material = this.gameObject.GetComponent<MeshRenderer>().material;
        material.SetTexture("_BaseMap", texture);
        cam = Camera.main;
    }

    private void InitializeTexture()
    {
        texture = new Texture2D(TextureDimensions, TextureDimensions, TextureFormat.RGBA64, false);

        for (int i = 0; i < TextureDimensions; i++)
        {
            for (int j = 0; j < TextureDimensions; j++)
            {
                texture.SetPixel(i, j, Color.black);
            }
        }
        texture.Apply();
        material = this.gameObject.GetComponent<MeshRenderer>().material;
        material.SetTexture("_BaseMap", texture);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            for (int i = 0; i < cubicBezierCurves.Count; i++)
            {
                Debug.Log("Length of curve " + i + " is " + cubicBezierCurves[i].GetLength());
            }
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            SaveTextureAsPNG(texture, "C:/Users/Malte/Desktop/3DHandCustomizer/Assets/Visuals/Texture/savedTexture.png");
        }

        ray = cam.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0)) //on mouse down
            {
                if (BezierPointsDrawState == BezierDrawState.None)
                {
                    BezierPointsDrawState = BezierDrawState.P1;
                    currentCurve[0] = hit.point; // pos 0
                    AddSphereMesh(hit.point, pointScale);
                }
                else if (BezierPointsDrawState == BezierDrawState.P2)
                {
                    BezierPointsDrawState = BezierDrawState.P3;
                    currentCurve[2] = hit.point; // pos 2
                    AddSphereMesh(hit.point, pointScale);
                }
            }

            if (Input.GetMouseButtonUp(0)) // on mouse up
            {
                if (BezierPointsDrawState == BezierDrawState.P1)
                {
                    BezierPointsDrawState = BezierDrawState.P2;
                    currentCurve[1] = hit.point; // pos 1
                    AddSphereMesh(hit.point, pointScale);
                }
                if (BezierPointsDrawState == BezierDrawState.P3)
                {
                    BezierPointsDrawState = BezierDrawState.P4;
                    currentCurve[3] = hit.point; // pos 3
                    AddSphereMesh(hit.point, pointScale);
                }
            }

            if (autoUpdate)
            { 
                if (bezierPointsDrawState != BezierDrawState.None) //redraw when something is hit and drawing
                {
                    RepaintEvent?.Invoke();
                }
            }
        }


        if (BezierPointsDrawState == BezierDrawState.P4)
        {
            CubicBezierCurve cBC = new CubicBezierCurve(currentCurve);
            cubicBezierCurves.Add(cBC); // add curve
            currentCurve = new Vector3[4]; // reset current curve
            BezierPointsDrawState = BezierDrawState.None; // reset State
        }
    }

    public GameObject AddSphereMesh(Vector3 _position, float _scale)
    {
        GameObject gO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(gO.GetComponent<Collider>());
        gO.transform.parent = this.gameObject.transform;
        gO.transform.position = _position;
        gO.transform.localScale = _scale * Vector3.one;
        return gO;
    }

    [TitleGroup("Actions")][Button]
    public void Repaint()
    {
        switch (paintCurvesOptions)
        {
            case PaintCurvesOptions.ConstantDistance:
                PaintTextureV1(texture, cubicBezierCurves);
                break;
            case PaintCurvesOptions.NormalizedDistance:
                PaintTextureV2(texture, cubicBezierCurves);
                break;
            case PaintCurvesOptions.RightLeft:
                PaintTextureV3(texture, cubicBezierCurves);
                break;
            case PaintCurvesOptions.ExtendTangents:
                PaintTextureV4(texture, cubicBezierCurves);
                break;
        }
    }

    void PaintTextureV1(Texture2D tex, List<CubicBezierCurve> cubicBezierCurves)
    {
        float x, y, dist, t = 0f;
        Vector3 p, cubP;
        float col;

        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                float w = (float)tex.width;
                float iw = (float)i;
                float h = (float)tex.height;
                float jh = (float)j;

                x = ((iw / w) - 0.5f) * 2f;
                y = ((jh / h) - 0.5f) * 2f;

                p = new Vector3(x, y, 0f);

                for (int c = 0; c < cubicBezierCurves.Count; c++)
                {
                    t = cubicBezierCurves[c].GetClosestParam(p);
                    cubP = cubicBezierCurves[c].GetPoint(t);
                    dist = Vector3.Distance(p, cubP);
                    col = dist < distThreshold ? 1f : 0f;

                    dist = Mathf.Clamp01(remap(-dist + distThreshold, 0f, distThreshold, 0f, 1f));
                    t = clampT ? t * Convert.ToSingle(dist > 0f) : t;

                    Color color = new Color(r ? dist : 0f, g ? t : 0f, b ? col : 0f, 1f);
                    tex.SetPixel(i, j, MaxColorRGB(color, tex.GetPixel(i, j),r,g,b) );
                }
            }
        }

        tex.Apply();
    } // constant distance

    void PaintTextureV2(Texture2D tex, List<CubicBezierCurve> cubicBezierCurves)
    {
        float x, y, t = 0f;
        Vector3 p, cubP;
        DistT[] distancesT = new DistT[cubicBezierCurves.Count];

        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                float w = (float)tex.width;
                float iw = (float)i;
                float h = (float)tex.height;
                float jh = (float)j;

                x = ((iw / w) - 0.5f) * 2f;
                y = ((jh / h) - 0.5f) * 2f;

                p = new Vector3(x, y, 0f);

                for (int c = 0; c < cubicBezierCurves.Count; c++)
                {
                    t = cubicBezierCurves[c].GetClosestParam(p);
                    cubP = cubicBezierCurves[c].GetPoint(t);
                    distancesT[c] = new DistT(Vector3.Distance(p, cubP), t);
                }

                if (cubicBezierCurves.Count < 2) //fallback
                {

                }
                else//take percentage calc.
                {
                    distancesT = distancesT.OrderBy(a => a.distance).ToArray(); //.Reverse()
                    float distAVG = (distancesT[0].distance + distancesT[1].distance) / 2f;
                    float distPercent = Mathf.InverseLerp(0f,distAVG, distAVG - distancesT[0].distance);
                    Color color = new Color(r ? distPercent : 0f, g ? distancesT[0].t : 0f, 0f, 1f);
                    tex.SetPixel(i, j, color);
                }

            }
        }
        tex.Apply();
    } // normalized distance
    void PaintTextureV3(Texture2D tex, List<CubicBezierCurve> cubicBezierCurves) //leftRight
    {
        float x, y, t = 0f;
        Vector3 p, cubP, tangent, binormal;
        Plane plane;
        DistTTangent[] distancesTTan = new DistTTangent[cubicBezierCurves.Count];

        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                float w = (float)tex.width;
                float iw = (float)i;
                float h = (float)tex.height;
                float jh = (float)j;

                x = ((iw / w) - 0.5f) * 2f;
                y = ((jh / h) - 0.5f) * 2f;

                p = new Vector3(x, y, 0f);

                for (int c = 0; c < cubicBezierCurves.Count; c++)
                {
                    t = cubicBezierCurves[c].GetClosestParam(p);
                    cubP = cubicBezierCurves[c].GetPoint(t);
                    tangent = cubicBezierCurves[c].GetTangent(t);
                    distancesTTan[c] = new DistTTangent(Vector3.Distance(p, cubP), t, tangent, cubP, cubicBezierCurves[c]);
                }

                distancesTTan = distancesTTan.OrderBy(a => a.distance).ToArray(); //sorted by closest first
                binormal = Vector3.Cross(Vector3.up, distancesTTan[0].tangent);
                plane = new Plane(Vector3.Cross(binormal, distancesTTan[0].tangent), distancesTTan[0].positionOnCurve);

                bool isAlignedToNormal = plane.GetSide(p);


                Color color = new Color(isAlignedToNormal ? 1f : 0f, isAlignedToNormal ? 0f : 1f, 0f, 1f);
                tex.SetPixel(i, j, color);
            }
        }
        tex.Apply();
    }

    void PaintTextureV4(Texture2D tex, List<CubicBezierCurve> cubicBezierCurves) //extendTangents
    {
        float x, y, t, rChannel, gChannel = 0f;
        Vector3 p, cubP, tangent;
        DistTTangent[] distancesTTan = new DistTTangent[cubicBezierCurves.Count];
        float delta = 0.00001f;

        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                float w = (float)tex.width;
                float iw = (float)i;
                float h = (float)tex.height;
                float jh = (float)j;

                x = ((iw / w) - 0.5f) * 2f;
                y = ((jh / h) - 0.5f) * 2f;

                p = new Vector3(x, y, 0f);

                for (int c = 0; c < cubicBezierCurves.Count; c++)
                {
                    t = cubicBezierCurves[c].GetClosestParam(p);
                    cubP = cubicBezierCurves[c].GetPoint(t);
                    tangent = cubicBezierCurves[c].GetTangent(t);
                    distancesTTan[c] = new DistTTangent(Vector3.Distance(p, cubP), t, tangent, cubP, cubicBezierCurves[c]);
                }

                distancesTTan = distancesTTan.OrderBy(a => a.distance).ToArray(); //sorted by closest first

                if (distancesTTan[0].t <= 0f + delta)
                {
                    Vector3 posT0 = distancesTTan[0].cBC.GetPoint(0f);
                    Vector3 pOffset = p - posT0;
                    Vector3 closestPoint = Vector3.Project(pOffset, -distancesTTan[0].tangent) + posT0;
                    float distToP = Vector3.Distance(p, closestPoint);
                    float distAlongTangent = Vector3.Distance(posT0, closestPoint);
                    float curveLength = distancesTTan[0].cBC.GetLength();
                    distAlongTangent = Mathf.InverseLerp(0f, curveLength, curveLength -(distAlongTangent % curveLength));

                    rChannel = distToP;
                    gChannel = distAlongTangent;

                }
                else if (distancesTTan[0].t >= 1f - delta)
                {
                    Vector3 posT1 = distancesTTan[0].cBC.GetPoint(1f);
                    Vector3 pOffset = p - posT1;
                    Vector3 closestPoint = Vector3.Project(pOffset, distancesTTan[0].tangent) + posT1;
                    float distToP = Vector3.Distance(p, closestPoint);
                    float distAlongTangent = Vector3.Distance(posT1, closestPoint);
                    float curveLength = distancesTTan[0].cBC.GetLength();
                    distAlongTangent = Mathf.InverseLerp(0f, curveLength, distAlongTangent % curveLength);

                    rChannel = distToP;
                    gChannel = distAlongTangent;
                }
                else
                {
                    rChannel = distancesTTan[0].distance;
                    gChannel = distancesTTan[0].t;
                }

                rChannel = Mathf.Clamp01(remap(-rChannel + distThreshold, 0f, distThreshold, 0f, 1f));

                Color color = new Color(r ? rChannel : 0f, g ? gChannel : 1f, 0f, 1f);
                tex.SetPixel(i, j, color);
            }
        }
        tex.Apply();
    }

    private float remap(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    private Color MaxColor(Color A, Color B)
    {
        return new Color(Mathf.Max(A.r, B.r), Mathf.Max(A.g, B.g), Mathf.Max(A.b, B.b), Mathf.Max(A.a, B.a));
    }

    private Color MaxColorRGB(Color Base, Color B, bool r, bool g, bool b)
    {
        float maxR = Mathf.Max(Base.r, B.r);
        float maxG = Mathf.Max(Base.g, B.g);
        float maxB = Mathf.Max(Base.b, B.b);
        float maxA = Mathf.Max(Base.a, B.a);
        return new Color(r?maxR:Base.r, g ? maxG : Base.g, b ? maxB : Base.b, maxA);
    }

    public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
    {
        byte[] _bytes = _texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(_fullPath, _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
    }
}

public struct DistT
{
    public float distance;
    public float t;
    public DistT(float _distance, float _t)
    {
        distance = _distance;
        t = _t;
    }
}
public class DistTTangent
{
    public CubicBezierCurve cBC;
    public float distance;
    public float t;
    public Vector3 positionOnCurve;
    public Vector3 tangent;
    public DistTTangent(float _distance, float _t, Vector3 _tangent, Vector3 _positionOnCurve, CubicBezierCurve _cBC)
    {
        distance = _distance;
        t = _t;
        tangent = _tangent;
        positionOnCurve = _positionOnCurve;
        cBC = _cBC;
    }
}

