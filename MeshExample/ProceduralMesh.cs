using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour
{
    [SerializeField] private int xValue, yValue;

    private MeshFilter meshFilter;
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[,] vertices2dArray;
    private Vector3[,] vertices2dArray_Origin;

    private float rate = -1f;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        DOTween.To(() => rate, x => rate = x, 6, 2).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        GenerateMesh();
    }

    private void Update()
    {
        //动画
        for (int x = 0; x <= xValue; x++)
        {
            for (int y = yValue; y > -1; y--)
            {
                Vector3 pos = vertices2dArray_Origin[x, y]
                    + new Vector3(Mathf.Cos((270 + (yValue - y) * rate) * Mathf.Deg2Rad), 0);
                vertices2dArray[x, y] = pos;
            }
        }

        for (int index = 0, y = 0; y <= yValue; y++)
        {
            for (int x = 0; x <= xValue; x++, index++)
            {
                vertices[index] = vertices2dArray[x, y];
            }
        }

        mesh.vertices = vertices;
    }

    private void OnDrawGizmosSelected()
    {
        if (vertices2dArray == null) return;

        Gizmos.color = Color.red;
        for (int y = 0; y <= yValue; y++)
        {
            for (int x = 0; x <= xValue; x++)
            {
                Gizmos.DrawSphere(transform.position + vertices2dArray[x, y], 0.1f);
            }
        }
    }

    private void GenerateMesh()
    {
        //顶点 + UV
        vertices = new Vector3[(xValue + 1) * (yValue + 1)];
        vertices2dArray = new Vector3[xValue + 1, yValue + 1];
        vertices2dArray_Origin = new Vector3[xValue + 1, yValue + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0, y = 0; y <= yValue; y++)
        {
            for (int x = 0; x <= xValue; x++, i++)
            {
                vertices[i] = new Vector2(x, y);
                vertices2dArray[x, y] = new Vector2(x, y);
                vertices2dArray_Origin[x, y] = new Vector2(x, y);
                uv[i] = new Vector2((float)x / xValue, (float)y / yValue);
            }
        }
        //三角形(ti的意思是TriangleIndex，vi的意思是VerticesIndex)
        int[] triangles = new int[xValue * yValue * 6];
        for (int ti = 0, vi = 0, y = 0; y < yValue; y++, vi++)
        {
            for (int x = 0; x < xValue; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xValue + 1;
                triangles[ti + 5] = vi + xValue + 2;
            }
        }

        mesh = new Mesh() { vertices = vertices, triangles = triangles, uv = uv, name = "Flag" };
        meshFilter.mesh = mesh;
    }
}