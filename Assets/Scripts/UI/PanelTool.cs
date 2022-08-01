using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
public class PanelTool : SerializedMonoBehaviour
{
    [OdinSerialize] private float width = 50;
    public float Width { get => width; }

    private void Start()
    {
        UpdateStats();
    }

    [Button]
    public void DebugLogRect()
    {
        RectTransform rectTrans = this.gameObject.GetComponent<RectTransform>();
        rectTrans.DebugLogRect();
    }

    [Button]
    public void UpdateStats()
    {
        RectTransform rectTrans = this.gameObject.GetComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(width, 0f);
    }

}
