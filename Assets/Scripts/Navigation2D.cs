using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Navigation2D : SerializedMonoBehaviour
{
    const string S = "Settings";
    const string IN = "Internal";
    const string REF = "References";

    [TitleGroup(S)] public bool canPan = true;
    [TitleGroup(S)] public bool canZoom = true;
    [TitleGroup(S)] public bool invertPan = true;
    [TitleGroup(S)] public bool invertZoom = true;

    [TitleGroup(S)] [SerializeField] public LayerMask raycastMask;

    [TitleGroup(S)] public float panSpeed = 0.5f;
    private float PanSpeed { get => panSpeed / 100f; set => panSpeed = value * 100f; }

    [TitleGroup(S)] public float zoomSpeed = 5f;
    private float ZoomSpeed { get => zoomSpeed / 100f; set => zoomSpeed = value * 100f; }

    Camera cam;

    private void Start()
    {
        cam = this.gameObject.GetComponent<Camera>();
    }

    public void PanCamera(Vector2 _delta)
    {
        if (canPan)
        {
            if (invertPan)
            {
                _delta = _delta * -1f;
            }
            cam.transform.Translate(new Vector3(_delta.x * PanSpeed * cam.orthographicSize, _delta.y * PanSpeed * cam.orthographicSize, 0f), Space.Self);
        }
    }

    public void ZoomCamera(float _delta)
    {
        if (canZoom)
        {
            if (invertZoom)
            {
                _delta = _delta * -1f;
            }
            cam.orthographicSize = (_delta * ZoomSpeed * cam.orthographicSize) + cam.orthographicSize;
        }
    }
}
