using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBeam : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float distance;
    public Transform firingPoint;
    public List<Vector3> points2 = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        points2 = new List<Vector3>();
        points2.Add(firingPoint.transform.position);
        points2.Add(firingPoint.position + transform.right * distance);
        DrawLine(points2);
    }

    void DrawLine(List<Vector3> points)
    {
        if (points == null)
        {
            return;
        }
        lineRenderer.positionCount = points.Count;
        Vector3[] temp = points.ToArray();
        lineRenderer.SetPositions(temp);
    }
}
