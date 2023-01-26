using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWithMouse : MonoBehaviour
{
    public Camera m_Camera;
    public Shader m_DrawingShader;

    RenderTexture m_SplatMap;
    Material m_SnowMaterial, m_DrawingMaterial;
    // Start is called before the first frame update
    void Start()
    {
        m_DrawingMaterial = new Material(m_DrawingShader);
        m_DrawingMaterial.SetVector("_Color", Color.red);

        m_SnowMaterial = GetComponent<MeshRenderer>().material;
        m_SplatMap = new RenderTexture(1024, 1024, 0,RenderTextureFormat.ARGBFloat);
        m_SnowMaterial.SetTexture("_Splat", m_SplatMap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
