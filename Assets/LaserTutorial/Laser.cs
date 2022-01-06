using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Laser : MonoBehaviour
{

    public float defDistanceRay = 3;
    public LineRenderer lineRenderer;
    public Transform firingPoint;
    Transform m_transform;

    public Grid ground;

    Vector2 s, e, sM, eM, cM, c2M;


    private void Awake()
    {
        m_transform = GetComponent<Transform>();
        s = e = sM = eM = cM = c2M = new Vector2();
    }


    void Shoot(float distance)
    {
        if (distance < 0.1f)
        {
            return;
        }
        Vector3[] temp = Array.Empty<Vector3>();
        lineRenderer.SetPositions(temp);
        if (Physics2D.Raycast(m_transform.position, transform.right))
        {
            RaycastHit2D _hit = Physics2D.Raycast(firingPoint.position, transform.right);
            
            if (_hit.transform.CompareTag("Mirror"))
            {
                List<Vector3> points = new List<Vector3>(); 
                points.Add(firingPoint.position);
                points.Add(_hit.point);
                float tempDist = distance - Vector2.Distance(firingPoint.position, _hit.point);
                //Vector2 ray = (_hit.transform.GetComponent<Mirror>().reflectionPoint.position - _hit.transform.position).normalized;

                Mirror mir = _hit.transform.GetComponent<Mirror>();
                Transform refP = mir.reflectionPoint.transform;

                Vector3 middle = ground.GetCellCenterWorld(ground.WorldToCell(_hit.transform.position));
                Vector2 middle2 = new Vector2(middle.x, middle.y);
                middle2 = new Vector2(middle2.x, middle2.y - (ground.cellSize.y * 0.25f));

                switch (mir.angle)
                {
                    case 0:
                        s = new Vector2(mir.startingPos.x + (ground.cellSize.x * 0.25f), mir.startingPos.y + (ground.cellSize.y * 0.25f));
                        e = new Vector2(mir.startingPos.x - (ground.cellSize.x * 0.25f), mir.startingPos.y - (ground.cellSize.y * 0.25f));

                        sM = new Vector2(middle2.x + (ground.cellSize.x * 0.25f), middle2.y + (ground.cellSize.y * 0.25f));
                        eM = new Vector2(middle2.x - (ground.cellSize.x * 0.25f), middle2.y - (ground.cellSize.y * 0.25f));
                        cM = new Vector2(middle2.x - (ground.cellSize.x * 0.5f), middle2.y);
                        c2M = new Vector2(middle2.x, middle2.y - (ground.cellSize.y * 0.5f));
                        break;

                    case 1:
                        s = new Vector2(mir.startingPos.x + (ground.cellSize.x * 0.25f), mir.startingPos.y - (ground.cellSize.y * 0.25f));
                        e = new Vector2(mir.startingPos.x - (ground.cellSize.x * 0.25f), mir.startingPos.y + (ground.cellSize.y * 0.25f));

                        sM = new Vector2(middle2.x - (ground.cellSize.x * 0.25f), middle2.y + (ground.cellSize.y * 0.25f));
                        eM = new Vector2(middle2.x + (ground.cellSize.x * 0.25f), middle2.y - (ground.cellSize.y * 0.25f));
                        cM = new Vector2(middle2.x - (ground.cellSize.x * 0.5f), middle2.y);
                        c2M = new Vector2(middle2.x, middle2.y + (ground.cellSize.y * 0.5f));
                        break;

                    case 2:
                        s = new Vector2(mir.startingPos.x + (ground.cellSize.x * 0.25f), mir.startingPos.y + (ground.cellSize.y * 0.25f));
                        e = new Vector2(mir.startingPos.x - (ground.cellSize.x * 0.25f), mir.startingPos.y - (ground.cellSize.y * 0.25f));

                        sM = new Vector2(middle2.x + (ground.cellSize.x * 0.25f), middle2.y + (ground.cellSize.y * 0.25f));
                        eM = new Vector2(middle2.x - (ground.cellSize.x * 0.25f), middle2.y - (ground.cellSize.y * 0.25f));
                        cM = new Vector2(middle2.x - (ground.cellSize.x * 0.5f), middle2.y);
                        c2M = new Vector2(middle2.x, middle2.y - (ground.cellSize.y * 0.5f));
                        break;

                    case 3:
                        s = new Vector2(mir.startingPos.x + (ground.cellSize.x * 0.25f), mir.startingPos.y - (ground.cellSize.y * 0.25f));
                        e = new Vector2(mir.startingPos.x - (ground.cellSize.x * 0.25f), mir.startingPos.y + (ground.cellSize.y * 0.25f));

                        sM = new Vector2(middle2.x - (ground.cellSize.x * 0.25f), middle2.y + (ground.cellSize.y * 0.25f));
                        eM = new Vector2(middle2.x + (ground.cellSize.x * 0.25f), middle2.y - (ground.cellSize.y * 0.25f));
                        cM = new Vector2(middle2.x - (ground.cellSize.x * 0.5f), middle2.y);
                        c2M = new Vector2(middle2.x, middle2.y + (ground.cellSize.y * 0.5f));
                        break;
                }

                Vector2 movingLine = (e - s).normalized;

                Vector2 movingLineM = (eM - sM).normalized;

                Vector2 h1 =_hit.point + movingLineM * (ground.cellSize.x * 0.5f);

                List<float> l1 = getABCForLine(cM, c2M);
                List<float> l2 = getABCForLine(_hit.point, h1);

                Vector2 distanceHelp = getIntersection(l1, l2);
                float distance2 = Vector2.Distance(distanceHelp, _hit.point);

                refP.position = e - movingLine * distance2;

                Vector2 temp1 = new Vector2(refP.position.x, refP.position.y);
                Vector2 ray = (temp1 - _hit.point).normalized;

                points.Add( ray * tempDist);
                DrawLine(points);
            }
            else
            {
                Draw2DRay(firingPoint.position, _hit.point);
            }
        }

        else
        {
            Draw2DRay(firingPoint.position, firingPoint.transform.right * distance);
        }
    }


    List<Vector3> Shoot2(float distance, List<Vector3> points)
    {
        if (distance < 0.1f)
        {
            return null;
        }
        Vector3[] temp = Array.Empty<Vector3>();
        lineRenderer.SetPositions(temp);
        if (Physics2D.Raycast(m_transform.position, transform.right))
        {
            RaycastHit2D _hit = Physics2D.Raycast(firingPoint.position, transform.right);

            if (_hit.transform.CompareTag("Mirror"))
            {
                points.Add(firingPoint.position);
                points.Add(_hit.point);
                float tempDist = distance - Vector2.Distance(firingPoint.position, _hit.point);
                //Vector2 ray = (_hit.transform.GetComponent<Mirror>().reflectionPoint.position - _hit.transform.position).normalized;

                Mirror mir = _hit.transform.GetComponent<Mirror>();
                Transform refP = mir.reflectionPoint.transform;

                Vector3 middle = ground.GetCellCenterWorld(ground.WorldToCell(_hit.transform.position));
                Vector2 middle2 = new Vector2(middle.x, middle.y);
                middle2 = new Vector2(middle2.x, middle2.y - (ground.cellSize.y * 0.25f));

                switch (mir.angle)
                {
                    case 0:
                        s = new Vector2(mir.startingPos.x + (ground.cellSize.x * 0.25f), mir.startingPos.y + (ground.cellSize.y * 0.25f));
                        e = new Vector2(mir.startingPos.x - (ground.cellSize.x * 0.25f), mir.startingPos.y - (ground.cellSize.y * 0.25f));

                        sM = new Vector2(middle2.x + (ground.cellSize.x * 0.25f), middle2.y + (ground.cellSize.y * 0.25f));
                        eM = new Vector2(middle2.x - (ground.cellSize.x * 0.25f), middle2.y - (ground.cellSize.y * 0.25f));
                        cM = new Vector2(middle2.x - (ground.cellSize.x * 0.5f), middle2.y);
                        c2M = new Vector2(middle2.x, middle2.y - (ground.cellSize.y * 0.5f));
                        break;

                    case 1:
                        s = new Vector2(mir.startingPos.x + (ground.cellSize.x * 0.25f), mir.startingPos.y - (ground.cellSize.y * 0.25f));
                        e = new Vector2(mir.startingPos.x - (ground.cellSize.x * 0.25f), mir.startingPos.y + (ground.cellSize.y * 0.25f));

                        sM = new Vector2(middle2.x - (ground.cellSize.x * 0.25f), middle2.y + (ground.cellSize.y * 0.25f));
                        eM = new Vector2(middle2.x + (ground.cellSize.x * 0.25f), middle2.y - (ground.cellSize.y * 0.25f));
                        cM = new Vector2(middle2.x - (ground.cellSize.x * 0.5f), middle2.y);
                        c2M = new Vector2(middle2.x, middle2.y + (ground.cellSize.y * 0.5f));
                        break;

                    case 2:
                        s = new Vector2(mir.startingPos.x + (ground.cellSize.x * 0.25f), mir.startingPos.y + (ground.cellSize.y * 0.25f));
                        e = new Vector2(mir.startingPos.x - (ground.cellSize.x * 0.25f), mir.startingPos.y - (ground.cellSize.y * 0.25f));

                        sM = new Vector2(middle2.x + (ground.cellSize.x * 0.25f), middle2.y + (ground.cellSize.y * 0.25f));
                        eM = new Vector2(middle2.x - (ground.cellSize.x * 0.25f), middle2.y - (ground.cellSize.y * 0.25f));
                        cM = new Vector2(middle2.x - (ground.cellSize.x * 0.5f), middle2.y);
                        c2M = new Vector2(middle2.x, middle2.y - (ground.cellSize.y * 0.5f));
                        break;

                    case 3:
                        s = new Vector2(mir.startingPos.x + (ground.cellSize.x * 0.25f), mir.startingPos.y - (ground.cellSize.y * 0.25f));
                        e = new Vector2(mir.startingPos.x - (ground.cellSize.x * 0.25f), mir.startingPos.y + (ground.cellSize.y * 0.25f));

                        sM = new Vector2(middle2.x - (ground.cellSize.x * 0.25f), middle2.y + (ground.cellSize.y * 0.25f));
                        eM = new Vector2(middle2.x + (ground.cellSize.x * 0.25f), middle2.y - (ground.cellSize.y * 0.25f));
                        cM = new Vector2(middle2.x - (ground.cellSize.x * 0.5f), middle2.y);
                        c2M = new Vector2(middle2.x, middle2.y + (ground.cellSize.y * 0.5f));
                        break;
                }

                Vector2 movingLine = (e - s).normalized;

                Vector2 movingLineM = (eM - sM).normalized;

                Vector2 h1 = _hit.point + movingLineM * (ground.cellSize.x * 0.5f);

                List<float> l1 = getABCForLine(cM, c2M);
                List<float> l2 = getABCForLine(_hit.point, h1);

                Vector2 distanceHelp = getIntersection(l1, l2);
                float distance2 = Vector2.Distance(distanceHelp, _hit.point);

                refP.position = e - movingLine * distance2;

                Vector2 temp1 = new Vector2(refP.position.x, refP.position.y);
                Vector2 ray = (temp1 - _hit.point).normalized;

                points.Add(ray * tempDist);

                Vector2 tempp = new Vector2(refP.transform.position.x, refP.transform.position.y);
                Vector2 rayDirection = tempp - _hit.point;
                if (Physics2D.Raycast(_hit.point, rayDirection))
                {
                    RaycastHit2D _hit2 = Physics2D.Raycast(_hit.point, rayDirection);
                    if (_hit.transform.CompareTag("Mirror"))
                    {
                        return Shoot2(tempDist, points);
                    }
                }

                return points;
                
            }
            else
            {
                Draw2DRay(firingPoint.position, _hit.point);
            }
        }

        else
        {
            Draw2DRay(firingPoint.position, firingPoint.transform.right * distance);
        }
        return null;
    }

    // A = y2 - y1; B = x1 - x2; C = Ax1 + By1

    List<float> getABCForLine(Vector2 pA, Vector2 pB)
    {
        List<float> r = new List<float>();
        r.Add(pB.y - pA.y);
        r.Add(pA.x - pB.x);
        r.Add((r[0] * pA.x) + r[1] * pA.y);
        return r;
    }

    Vector2 getIntersection(List<float> a, List<float> b)
    {


        float delta = a[0] * b[1] - b[0] * a[1];
        float x = (b[1] * a[2] - a[1] * b[2]) / delta;
        float y = (a[0] * b[2] - b[0] * a[2]) / delta;

        return new Vector2(x, y);
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
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

    // Update is called once per frame
    void Update()
    {
        DrawLine(Shoot2(defDistanceRay, new List<Vector3>()));
    }
}
