using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
public class PanelTool : SerializedMonoBehaviour
{
    [OdinSerialize] private float widthPercent = 8;
    public float WidthPercent { get => widthPercent; }
    public float WidthPixel { get => UIManager.Instance.Canvas.pixelRect.width * (widthPercent / 100f); }
    private void Start()
    {
        UpdateSize();
    }

    [Button]
    public void DebugLogRect()
    {
        RectTransform rectTrans = this.gameObject.GetComponent<RectTransform>();
        rectTrans.DebugLogRect();
    }

    [Button]
    public void UpdateSize()
    {
        RectTransform rectTrans = this.gameObject.GetComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(WidthPixel, 0f);
        Debug.Log("Updated PanelTool Size");
    }

}
