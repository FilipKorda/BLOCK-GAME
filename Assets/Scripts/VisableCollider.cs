using UnityEngine;

public class VisableCollider : MonoBehaviour
{
    public bool isMeta;
    public bool isCollider;
    public bool isTrappedPlate;
    public bool isBridgePlate;

    private void OnDrawGizmos()
    {
        Transform tr = transform;

        Vector3[] vertices = new Vector3[]
        {
            tr.TransformPoint(new Vector3(-0.5f, 0.5f, -0.5f)),   // 0
            tr.TransformPoint(new Vector3(0.5f, 0.5f, -0.5f)),    // 1
            tr.TransformPoint(new Vector3(0.5f, -0.5f, -0.5f)),   // 2
            tr.TransformPoint(new Vector3(-0.5f, -0.5f, -0.5f)),  // 3
            tr.TransformPoint(new Vector3(-0.5f, 0.5f, 0.5f)),    // 4
            tr.TransformPoint(new Vector3(0.5f, 0.5f, 0.5f)),     // 5
            tr.TransformPoint(new Vector3(0.5f, -0.5f, 0.5f)),    // 6
            tr.TransformPoint(new Vector3(-0.5f, -0.5f, 0.5f))    // 7
        };

        DrawEdge(vertices[0], vertices[1]);
        DrawEdge(vertices[1], vertices[2]);
        DrawEdge(vertices[2], vertices[3]);
        DrawEdge(vertices[3], vertices[0]);

        DrawEdge(vertices[4], vertices[5]);
        DrawEdge(vertices[5], vertices[6]);
        DrawEdge(vertices[6], vertices[7]);
        DrawEdge(vertices[7], vertices[4]);

        DrawEdge(vertices[0], vertices[4]);
        DrawEdge(vertices[1], vertices[5]);
        DrawEdge(vertices[2], vertices[6]);
        DrawEdge(vertices[3], vertices[7]);
    }
    void DrawEdge(Vector3 startPoint, Vector3 endPoint)
    {
        if (isMeta)
        {
            Debug.DrawLine(startPoint, endPoint, Color.blue, 0f, false);
        }
        else if (isCollider)
        {
            Debug.DrawLine(startPoint, endPoint, Color.gray, 0f, false);
        }
        else if (isTrappedPlate)
        {
            Debug.DrawLine(startPoint, endPoint, Color.red, 0f, false);
        }
        else if(isBridgePlate)
        {
            Debug.DrawLine(startPoint, endPoint, Color.yellow, 0f, false);
        }

    }
}
