using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMeshInstancedDemo : MonoBehaviour
{
    // How many meshes to draw.
    public int population;
    // Range to draw meshes within.
    public float range;

    // Material to use for drawing the meshes.
    public Material material;

    private Matrix4x4[] matrices;
    private MaterialPropertyBlock block;
    
    [SerializeField]
    private Mesh mesh;

    [SerializeField]
    float m_Size;

    private void Setup()
    {
        /*
        if (this.mesh == null) 
        {
            Mesh mesh = CreateQuad();
            this.mesh = mesh;
        }
        */
        matrices = new Matrix4x4[population];
        Vector4[] colors = new Vector4[population];

        block = new MaterialPropertyBlock();

        for (int i = 0; i < population; i++)
        {
            // Build matrix.
            Vector3 position = new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
            Quaternion rotation = Quaternion.Euler(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180));
            Vector3 scale = Vector3.one * m_Size;

            //mat = Matrix4x4.TRS(position, rotation, scale);

            matrices[i] = Matrix4x4.TRS(position, rotation, scale);

            colors[i] = Color.Lerp(Color.red, Color.blue, Random.value);
        }

        // Custom shader needed to read these!!
        block.SetVectorArray("_Colors", colors);
    }

    private Mesh CreateQuad(float width = 1f, float height = 1f)
    {
        // Create a quad mesh.
        var mesh = new Mesh();

        float w = width * .5f;
        float h = height * .5f;
        var vertices = new Vector3[4] {
            new Vector3(-w, -h, 0),
            new Vector3(w, -h, 0),
            new Vector3(-w, h, 0),
            new Vector3(w, h, 0)
        };

        var tris = new int[6] {
            // lower left tri.
            0, 2, 1,
            // lower right tri
            2, 3, 1
        };

        var normals = new Vector3[4] {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
        };

        var uv = new Vector2[4] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.uv = uv;

        return mesh;
    }


    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        // Draw a bunch of meshes each frame.
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, population, block);
    }
}
