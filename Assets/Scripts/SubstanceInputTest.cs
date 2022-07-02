using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Adobe.Substance.Runtime;

public class SubstanceInputTest : SerializedMonoBehaviour
{

    public SubstanceRuntimeGraph substanceRuntimeGraph;
    public Texture2D tex;

    private void Start()
    {
        substanceRuntimeGraph = this.gameObject.GetComponent<SubstanceRuntimeGraph>();
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            UpdateSubstance();
            Debug.Log("pressed B");
        }
    }

    public void UpdateSubstance()
    {
        if (tex != null)
        {
            substanceRuntimeGraph.SetInputTexture("someTexture", tex);
            substanceRuntimeGraph.RenderAsync();
            Debug.Log("Set Texture");
        }
        else
        {
            Debug.Log("Texture is null");
        }
    }
}
