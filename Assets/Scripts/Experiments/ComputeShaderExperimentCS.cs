using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ComputeShaderExperimentCS : SerializedMonoBehaviour
{
    public ComputeShader computeShader;
    public ComputeBuffer computeBuffer;
    public Texture2D texture;

    [Button]
    public void Execute()
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                 512,
                 512,
                 0,
                 RenderTextureFormat.Default,
                 RenderTextureReadWrite.Linear);

        computeShader.SetTexture(0, "rwTex", renderTex);
        computeShader.Dispatch(0, 8, 8, 1);

        texture = new Texture2D(512, 512, TextureFormat.RGBA32, false);
        texture = renderTex.toTexture2D();
        Material mat = this.gameObject.GetComponent<MeshRenderer>().material;
        mat.SetTexture("_BaseMap", texture);

    }
}
public static class ExtensionMethod
{
    public static Texture2D toTexture2D(this RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        var old_rt = RenderTexture.active;
        RenderTexture.active = rTex;

        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();

        RenderTexture.active = old_rt;
        return tex;
    }
}
