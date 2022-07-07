using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Adobe.Substance.Runtime;
using Adobe.Substance;
using Sirenix.OdinInspector;
public class SubstanceManipulator : SerializedMonoBehaviour
{
    public SubstanceRuntimeGraph substanceRuntimeGraph;
    public float floatParam = 1f;
    public string paramName = "ENTER PARAMETER NAME HERE";

    void Start()
    {
        substanceRuntimeGraph = this.gameObject.GetComponent<SubstanceRuntimeGraph>();
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.J))
        {
            SetSubstanceFloat();
        }
        if (Input.GetKeyUp(KeyCode.U))
        {
            RandomizeSubstanceParameters();
        }
    }

    void SetSubstanceFloat()
    {
        if (substanceRuntimeGraph != null)
        {
            substanceRuntimeGraph.SetInputFloat(paramName, floatParam);
            substanceRuntimeGraph.RenderAsync();
        }
    }

    [Button]
    public void RandomizeSubstanceParameters()
    {
        substanceRuntimeGraph.SetInputColor("baseColor", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f),0f));
        substanceRuntimeGraph.SetInputFloat("valueRange", Random.Range(0f,1f));
        substanceRuntimeGraph.SetInputFloat("hueVariationIntensity", Random.Range(0f, 1f));
        substanceRuntimeGraph.SetInputInt("hueVariationSeed", Random.Range(0, 1));
        substanceRuntimeGraph.SetInputFloat("spekleVisibility", Random.Range(0f, 1f));
        substanceRuntimeGraph.SetInputColor("spekleColor", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f),0f));
        substanceRuntimeGraph.SetInputFloat("spekleSize", Random.Range(0f, 1f));
        substanceRuntimeGraph.SetInputInt("spekleAmount", Random.Range(0, 256));
        substanceRuntimeGraph.SetInputFloat("spekleUniformAmount", Random.Range(0f, 1f));
        substanceRuntimeGraph.SetInputFloat("spekleUniformPosition", Random.Range(0f, 1f));
        substanceRuntimeGraph.SetInputFloat("spekleUniformSize", Random.Range(0f, 1f));
        substanceRuntimeGraph.SetInputFloat("spekleUniformBrightness", Random.Range(0f, 1f));
        substanceRuntimeGraph.SetInputFloat("spekleSmearingAmount", Random.Range(0f, 1f));
        substanceRuntimeGraph.SetInputFloat("roughness", Random.Range(0f, 1f));
        substanceRuntimeGraph.SetInputFloat("roughnessRange", Random.Range(0f, 1f));
        substanceRuntimeGraph.SetInputInt("microStructureType", Random.Range(1, 12));
        substanceRuntimeGraph.SetInputFloat("microStructureIntensity", Random.Range(0f, 1f));
        substanceRuntimeGraph.SetInputFloat("softness", Random.Range(0f, 0.5f));
        bool pingPong = System.Convert.ToBoolean(Random.Range(-1, 1));
        substanceRuntimeGraph.SetInputBool("usePingPongShift", pingPong);
        if (pingPong)
        { 
            substanceRuntimeGraph.SetInputFloat("pingPongShift", Random.Range(0f, 1f));
        }
        substanceRuntimeGraph.SetInputInt("structureWarpType", Random.Range(1, 12));
        substanceRuntimeGraph.SetInputFloat("structureWarpIntensity", Random.Range(0f, 2.5f));
        substanceRuntimeGraph.SetInputFloat("ambientOcclusionHeight", Random.Range(0f, 3f));
        substanceRuntimeGraph.RenderAsync();
    }
}
