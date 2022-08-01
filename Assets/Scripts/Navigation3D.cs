using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Navigation3D : SerializedMonoBehaviour
{
    public GameObject cameraOrigin;
    [SerializeField]
    public LayerMask raycastMask;

    public float panSpeed = 0.1f;
    private float PanSpeed { get => panSpeed / 100f; set => panSpeed = value * 100f; }

    public float rotationSpeed = 0.1f;
    private float RotationSpeed { get => rotationSpeed / 100f; set => rotationSpeed = value * 100f; }

    public float zoomSpeed = 0.1f;
    private float ZoomSpeed { get => zoomSpeed / 100f; set => zoomSpeed = value * 100f; }


    private void Start()
    {
        CreateCameraOrigin();
    }

    private void CreateCameraOrigin()
    {
        cameraOrigin = new GameObject("cameraOrigin");
        cameraOrigin.transform.position = this.transform.position;
        cameraOrigin.transform.rotation = this.transform.rotation;
        cameraOrigin.transform.localScale = this.transform.localScale;

        cameraOrigin.transform.Translate(Vector3.forward * 4f, Space.Self);

        this.transform.SetParent(cameraOrigin.transform);
    }
    public void PropagateRaycast(Vector2 textureCoordinates01)
    {
        Ray ray = GameController.Instance.Camera3D.ViewportPointToRay(textureCoordinates01);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastMask))
        {
            Debug.Log("Hit " + hit.collider.gameObject.name + " in Viewport");
            Debug.DrawRay(hit.point, Vector3.up * 0.1f);
        }
    }

    public void PanCamera(Vector2 _delta)
    {
        cameraOrigin.transform.Translate(new Vector3(_delta.x * PanSpeed, _delta.y * PanSpeed * Vector3.Distance(cameraOrigin.transform.position, this.transform.position), 0f), Space.Self);
    }
    public void RotateCamera(Vector2 _delta)
    {
        cameraOrigin.transform.RotateAround(cameraOrigin.transform.position, cameraOrigin.transform.right, -_delta.y * RotationSpeed);
        cameraOrigin.transform.RotateAround(cameraOrigin.transform.position, Vector3.up, _delta.x * RotationSpeed);
    }
    public void ZoomCamera(Vector2 _delta)
    {

    }
}
