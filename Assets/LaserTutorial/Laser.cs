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

    public string name1;
    public string name2;
    public string name3;

    public RaycastHit2D show1;
    public RaycastHit2D show2;
    public RaycastHit2D show3;

    public List<Vector3> points2 = new List<Vector3>();

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


    // Calculate the light beam trajectory through multiple mirrors if found
    List<Vector3> Shoot2(float distance, List<Vector3> points, Vector2 startPos, int level, Mirror mir2 = null)
    {
        // Cancel Shooting the Laser if not enough distance is left
        if (distance < 0.1f)
        {
            return points;
        }

        // Cancel the Shooting if too many recursions occur. Only for debugging shouldnt happen later
        if (level >= 2)
        {
            Debug.Log("=====CANCEL=======");
            return points;
        }

        // Resetting the positions of the lineRenderer to properly redraw them
        Vector3[] temp = Array.Empty<Vector3>();
        lineRenderer.SetPositions(temp);

        // If its the first call from the update function check if anything has been hit. If its not the first call check if a mirror has been hit
        bool condition;
        if (level == 0)
        {
            RaycastHit2D _hitHelp = Physics2D.Raycast(m_transform.position, transform.right);
            condition = _hitHelp.transform.CompareTag("Mirror");
            condition = Physics2D.Raycast(m_transform.position, transform.right);
            Debug.DrawLine(m_transform.position, transform.right * distance, Color.black);
        }
        else
        {
            name3 = mir2.name;
            Vector2 temp1 = new Vector2(mir2.reflectionPoint.position.x, mir2.reflectionPoint.position.y);
            Vector2 ray = (temp1 - startPos).normalized;
            RaycastHit2D _hitHelp = Physics2D.Raycast(startPos, ray);
            condition = _hitHelp.transform.CompareTag("Mirror");
            Debug.DrawLine(startPos, ray * distance, Color.magenta);
        }

        // Depending on the condition set before either 
        if (condition)
        {
            RaycastHit2D _hit;
            if (level == 0)
            {
                // Send Ray from laserFiringPoint
                 _hit = Physics2D.Raycast(startPos, transform.right);
                Debug.DrawLine(startPos, transform.right * distance, Color.yellow);
            }
            else
            {
                // Send Ray from already hit mirror in direction of the mirrors reflectionPoint
                Vector2 posAsV2 = new Vector2(mir2.reflectionPoint.position.x, mir2.reflectionPoint.position.y);
                Vector2 direction2 = (posAsV2 - startPos).normalized;
                 _hit = Physics2D.Raycast(startPos, direction2);
                Debug.DrawLine(startPos, direction2 * distance, Color.cyan);
            }

            show2 = _hit;
            name1 = _hit.transform.name;

            // Check if a Mirror has been hit
            if (_hit.transform.CompareTag("Mirror"))
            {
                // Maybe not correct to add those here ?
                if (level == 0) points.Add(startPos);
                points.Add(_hit.point);

                // reduce distance based on already travelled line
                float tempDist = distance - Vector2.Distance(startPos, _hit.point);

                // Get Mirror Object and refPoint of that Mirror
                Mirror mir;
                Transform refP;
                mir = _hit.transform.GetComponent<Mirror>();
                refP = mir.reflectionPoint.transform;

                repositionRefPoint(mir, _hit, refP);

                // Get refPoint position in Vector 2 and calculate vector from impact point to refPoint
                Vector2 temp1 = new Vector2(refP.position.x, refP.position.y);
                Vector2 ray = (temp1 - _hit.point).normalized;

                // Debugging Lines
                if (level == 0)
                {
                    Debug.DrawLine(_hit.point, ray * tempDist, Color.green);
                }
                else
                {
                    Debug.DrawLine(_hit.point, ray * tempDist, Color.red);
                }
                

                if (Physics2D.Raycast(_hit.point, ray))
                {
                    RaycastHit2D _hit2 = Physics2D.Raycast(_hit.point, ray);

                    show1 = _hit2;
                    name2 = _hit2.transform.name;
                    if (_hit2.transform.CompareTag("Mirror"))
                    {
                        points.Add(_hit2.point);
                        Mirror mirr2 = _hit2.transform.GetComponent<Mirror>();
                        level += 1;
                        //return points;
                        return Shoot2(tempDist, points, _hit2.point, level, mirr2);
                    }
                    else
                    {
                        points.Add(ray * tempDist);
                        return points;
                    }
                   
                }
                else
                {
                    points.Add(ray * tempDist);
                    return points;
                }
  
            }
            else
            {
                //Draw2DRay(startPos, _hit.point);
                Debug.Log("!!ONLY!!");
                points.Add(startPos);
                points.Add(_hit.point);

                // Get refPoint position in Vector 2 and calculate vector from impact point to refPoint
                Vector2 temp1 = new Vector2(mir2.reflectionPoint.position.x, mir2.reflectionPoint.position.y);
                Vector2 ray = (temp1 - _hit.point).normalized;

                points.Add(ray * distance);

                return points;
            }
        }

        else
        {
            Debug.Log("===========ELSE=============");
            if (level == 0)
            {
                Draw2DRay(firingPoint.position, firingPoint.right);
                points.Add(firingPoint.position);
                points.Add(firingPoint.right * distance);
                return points;
            }
            else
            {
                Vector2 temp1 = new Vector2(mir2.reflectionPoint.position.x, mir2.reflectionPoint.position.y);
                Vector2 ray = (temp1 - startPos).normalized;
                Draw2DRay(startPos, ray * distance);
                points.Add(startPos);
                points.Add(ray * distance);
                return points;
            }

            
        }
    }

    // A = y2 - y1; B = x1 - x2; C = Ax1 + By1
    // 1 Shoot Ray from Laser to first mirror                                             |     Add Startingpoint, Add first Mirror hit
    // 2 Position Reflection point accordingly                                            |
    // 3 Shoot Ray from reflection point in the direction of the reflection point         |       
    // 4     If new mirror has been hit shoot from hit to reflection point                |     Add new Mirror
    //       If not return points                                                         |     Add end point
    // 5     If new mirror has been hit go back to step 4                                 |     Add new Mirror 
    //       If not return points                                                         |     Add end point 


    void Shoot3()
    {
        float distance = defDistanceRay;
        RaycastHit2D _hit = Physics2D.Raycast(firingPoint.transform.position, transform.right);
        points2.Add(firingPoint.transform.position);

        if (!_hit)
        {
            points2.Add(firingPoint.position + transform.right * distance);
            return;
        }

        if (_hit.transform.CompareTag("Mirror"))
        {
            while (distance > 0.1f)
            {
                Mirror m1 = _hit.transform.GetComponent<Mirror>();
                Transform p1 = m1.reflectionPoint.transform;
                repositionRefPoint(m1, _hit, p1);
                points2.Add(_hit.point);
                points2.Add(p1.position);

                distance -= Vector2.Distance(firingPoint.transform.position, _hit.point);
                distance -= Vector2.Distance(_hit.point, p1.position);

                Vector2 p1V2 = new Vector2(p1.position.x, p1.position.y);
                Vector2 ray1 = (p1V2 - _hit.point).normalized;

                if (Physics2D.Raycast(p1V2, ray1))
                {
                    _hit = Physics2D.Raycast(p1V2, ray1);
                    if (!_hit.transform.CompareTag("Mirror"))
                    {
                        points2.Add(p1V2 + ray1 * distance);
                        return;
                    }
                }
                else
                {
                    points2.Add(p1V2 + ray1 * distance);
                    return;
                }
            }
        }
        else
        {
            points2.Add(firingPoint.position + transform.right * distance);
        }

    }

    void repositionRefPoint(Mirror mir, RaycastHit2D _hit, Transform refP)
    {
        // Get Middle Point of Cell that has been hit
        Vector3 middle = ground.GetCellCenterWorld(ground.WorldToCell(_hit.transform.position));
        Vector2 middle2 = new Vector2(middle.x, middle.y);
        middle2 = new Vector2(middle2.x, middle2.y - (ground.cellSize.y * 0.25f));

        // Depending on the angle set in the mirror object choose reference points to properly position the refPoint
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

        // Get an imaginary line that the ref point is supposed to move on
        Vector2 movingLine = (e - s).normalized;

        // Get an imaginary line that the laser is impacting on the object 
        Vector2 movingLineM = (eM - sM).normalized;

        // ??? could be a fault
        Vector2 h1 = _hit.point + movingLineM * (ground.cellSize.x * 0.5f);

        // Calculate elements needed for line intersection calculation
        List<float> l1 = getABCForLine(cM, c2M);
        List<float> l2 = getABCForLine(_hit.point, h1);

        // Get point where imaginary lines are intersecting to calculate offset
        Vector2 distanceHelp = getIntersection(l1, l2);

        // Calculate distance between intersecting point and actual hit point 
        float distance2 = Vector2.Distance(distanceHelp, _hit.point);

        // Use the distance to position the refPoint somewhere on the movingline
        refP.position = e - movingLine * distance2;
    }

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
        name1 = "";
        name2 = "";
        name3 = "";
        //DrawLine(Shoot2(defDistanceRay, new List<Vector3>(), firingPoint.position, 0));
        //Shoot(defDistanceRay);
        points2 = new List<Vector3>();
        Shoot3();
        DrawLine(points2);
        
    }
}
