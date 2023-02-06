using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
public struct BulletState
{
    Vector3 position;
    Vector3 forward;
}
public class MassiveMeshGenerator : MonoBehaviour
{
    [Range(1, 1_000_000)]
    public int instanceCount = 100_000;
    [Range(0.05f, 1f)]
    public float minScale = 0.1f;
    [Range(1f, 100f)]
    public float maxScale = 1f;
    [Range(1f, 100f)]
    public float speed = 10f;
    public Transform Target;

    [Header("Bullet Instance")]
    public Mesh mesh;
    public Material material;
    public int subMeshIndex = 0;
    public Bounds renderBounds = new Bounds(Vector3.zero, Vector3.one * 30f);
    // 메시 데이터 버퍼
    private ComputeBuffer argsBuffer;
    // 위치&스케일 버퍼
    private ComputeBuffer positionBuffer;
    private uint[] argsData = new uint[5];

    
    private void Update()
    {
        if (mesh == null || material == null)
            return;

        InitPositionBuffer();
        DrawInstances();
        //renderBounds = new Bounds((transform.position + Target.position)/2,transform.position - Target.position);
    }

    private void OnEnable()
    {        
        InitArgsBuffer();
    }
    private void OnDestroy()
    {
        if (argsBuffer != null)
            argsBuffer.Release();
        if (positionBuffer != null)
            positionBuffer.Release();
    }
    /// <summary> 메시 데이터 버퍼 생성 </summary>
    private void InitArgsBuffer()
    {
        if (argsBuffer == null)
            argsBuffer = new ComputeBuffer(1, sizeof(uint) * 5, ComputeBufferType.IndirectArguments);
        argsData[0] = (uint)mesh.GetIndexCount(subMeshIndex);
        argsData[1] = (uint)instanceCount;
        argsData[2] = (uint)mesh.GetIndexStart(subMeshIndex);
        argsData[3] = (uint)mesh.GetBaseVertex(subMeshIndex);
        argsData[4] = 0;
        argsBuffer.SetData(argsData);
    }
    /// <summary> 위치, 스케일 데이터 버퍼 생성 </summary>
    private void InitPositionBuffer()
    {
        if (positionBuffer != null)
            positionBuffer.Release();
        //renderBounds = new Bounds(transform.position, Vector3.one * 30f);
        renderBounds = new Bounds((transform.position + Target.position) / 2, transform.position - Target.position);
        Vector4[] positions = new Vector4[instanceCount];

        Vector3 boundsMin = renderBounds.min;
        Vector3 boundsMax = renderBounds.max;
        //얘는 start에서만 실행하도록 고치자.
        // XYZ : 위치, W : 스케일
        for (int i = 0; i < instanceCount; i++)
        {
            ref Vector4 pos = ref positions[i];
            pos.x = Random.Range(boundsMin.x, boundsMax.x);
            pos.y = Random.Range(boundsMin.y, boundsMax.y);
            pos.z = Random.Range(boundsMin.z, boundsMax.z);
            pos.w = Random.Range(minScale, maxScale); // Scale
        }
        Matrix4x4[] matrixBuffer = new Matrix4x4[instanceCount];
        /*
        //https://gist.github.com/Cyanilux/e7afdc5c65094bfd0827467f8e4c3c54
        for (int i = 0; i < instanceCount; i++)
        {
            float height = Random.Range(0.5f, 1.2f);
            Vector3 position = new Vector3(Random.Range(-range, range), height * 0.5f, Random.Range(-range, range));
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            Vector3 scale = new Vector3(1, height, 1);
            matrixBuffer[i] = Matrix4x4.TRS(position, rotation, scale);
        }
        */
        positionBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 4);
        positionBuffer.SetData(positions);
        material.SetFloat("deltaTime", Time.deltaTime);
        material.SetBuffer("positionBuffer", positionBuffer);
    }
    private void DrawInstances()
    {
        Graphics.DrawMeshInstancedIndirect(
            mesh,         // 그려낼 메시
            subMeshIndex, // 서브메시 인덱스
            material,     // 그려낼 마테리얼
            renderBounds, // 렌더링 영역
            argsBuffer    // 메시 데이터 버퍼
        );
        //Graphics.DrawMeshInstanced(mesh, subMeshIndex, material,matrixBuffer);
    }
}