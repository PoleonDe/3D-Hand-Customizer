using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelProperty : MonoBehaviour
{
    private float width = 100f;
    public float Width { get => width; }

    private void Start()
    {
        UpdateStats();
    }
    public void UpdateStats()
    {
        RectTransform rectTrans = this.gameObject.GetComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(width, 0f);
    }

}
