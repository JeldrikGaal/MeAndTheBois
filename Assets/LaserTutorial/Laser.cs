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

    public GameManager gM;

    private void Awake()
    {
        m_transform = GetComponent<Transform>();
        s = e = sM = eM = cM = c2M = new Vector2();
    }
    // A = y2 - y1; B = x1 - x2; C = Ax1 + By1
    // 1 Shoot Ray from Laser to first mirror                                             |     Add Startingpoint, Add first Mirror hit
    // 2 Position Reflection point accordingly                                            |
    // 3 Shoot Ray from reflection point in the direction of the reflection point         |       
    // 4     If new mirror has been hit shoot from hit to reflection point                |     Add new Mirror
    //       If not return points                                                         |     Add end point
    // 5     If new mirror has been hit go back to step 4                                 |     Add new Mirror 
    //       If not return points                                                         |     Add end point 
    // ==============================================================================================================


    void Shoot3()
    {
        float distance = defDistanceRay;
        RaycastHit2D _hit = Physics2D.Raycast(firingPoint.transform.position, transform.right);
        Debug.DrawRay(firingPoint.transform.position, transform.right, Color.red);
        points2.Add(firingPoint.transform.position);

        if (!_hit)
        {
            points2.Add(firingPoint.position + transform.right * distance);
            return;
        }

        // Send Message that object has been hit
        if (_hit.transform.gameObject)
        {
            if (_hit.transform.CompareTag("CombiTile"))
            {
                _hit.transform.gameObject.GetComponent<CombiTile>().HitBySolar(_hit.point, transform.right);
            }
        }

        if (_hit.transform.CompareTag("Mirror"))
        {
            while (distance > 0.1f)
            {
                RaycastHit2D _hitSave = _hit;
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
                    if (!_hit.transform.CompareTag("Mirror"))
                    {
                        points2.Add(p1V2 + ray1 * distance);
                        return;
                    }

                    _hit = Physics2D.Raycast(p1V2, ray1);

                    if (_hit.transform.gameObject)
                    {
                        if (_hit.transform.CompareTag("CombiTile"))
                        {
                            _hit.transform.gameObject.GetComponent<CombiTile>().HitBySolar(_hit.point, ray1);
                        }
                    }

                    // If other objects has been hit draw line to that object
                    if (!_hit.transform.CompareTag("Mirror"))
                    {
                        p1V2 = new Vector2(p1.position.x, p1.position.y);
                        ray1 = (p1V2 - _hitSave.point).normalized;
                        points2.Add(_hit.point);
                        return;
                    }

                    m1 = _hit.transform.GetComponent<Mirror>();
                    p1 = m1.reflectionPoint.transform;
                    repositionRefPoint(m1, _hit, p1);
                    m1.beingHit = true;
                    //m1.angleReceived = calcRecAngle(m1, points2[points2.Count - 1]);
                    m1.angleReceived = calcRecAngle(m1, ray1);
                    gM.hitMirros.Add(m1);
                    if (!gM.hitMirrosStable.Contains(m1)) gM.hitMirrosStable.Add(m1);

                    // Stop the Beam if it goes into a mirror that wants the energy
                    if (m1.needsChargeToRotate && m1.beingHit && m1.angleReceived == m1.angleToCharge && m1.startingMir)
                    {
                        points2.Add(_hit.point);
                        return;
                    }


                }
                else
                {
                    if (_hit.transform.CompareTag("CombiTile"))
                    {
                        CombiTile c = _hit.transform.GetComponent<CombiTile>();
                        gM.hitCombi.Add(c);
                        if (!gM.hitCombiStable.Contains(c)) gM.hitCombiStable.Add(c);
                    }
                    else if (_hit.transform.CompareTag("Pipe"))
                    {
                        Pipe p = _hit.transform.GetComponent<Pipe>();
                        gM.hitPipes.Add(p);
                        if (!gM.hitPipesStable.Contains(p)) gM.hitPipesStable.Add(p);
                    }

                    points2.Add(p1V2 + ray1 * distance);
                    return;
                }
            }
        }
        else
        {
            //points2.Add(firingPoint.position + transform.right * distance); ALLLLLLLLLLLAH
            points2.Add(_hit.point);
        }

    }

    float calcRecAngle(Mirror mir, Vector3 ray)
    {
        Vector3 point = mir.transform.position + ((ray * -1) * 2);
        //Debug.Log((ray, mir.transform.position, point));
        float Mangle;
        // Mir X > Point X
        if (mir.transform.position.x > point.x)
        {
            // Mir Y > Point Y
            if (mir.transform.position.y > point.y)
            {
                Mangle = 1;
            }
            // Mir Y < Point Y
            else
            {
                Mangle = 2;
            }
        }
        // Mir X < Point X
        else
        {
            // Mir Y > Point Y
            if (mir.transform.position.y >= point.y)
            {
                Mangle = 3; // X
            }
            // Mir Y < Point Y
            else
            {
                Mangle = 0; // X
            }
        }

        return Mangle;
    }
    float calcRecAngle2(Mirror mir, Vector3 point)
    {
        Debug.Log((mir.transform.position, point));
        float Mangle;
        // Mir X > Point X
        if (mir.transform.position.x > point.x)
        {
            // Mir Y > Point Y
            if (mir.transform.position.y > point.y)
            {
                Mangle = 1;
            }
            // Mir Y < Point Y
            else
            {
                Mangle = 2;
            }
        }
        // Mir X < Point X
        else
        {
            // Mir Y > Point Y
            if (mir.transform.position.y > point.y)
            {
                Mangle = 3; // X
            }
            // Mir Y < Point Y
            else
            {
                Mangle = 0; // X
            }
        }

        return Mangle;
    }

    void repositionRefPoint(Mirror mir, RaycastHit2D _hit, Transform refP)
    {
        // Get Middle Point of Cell that has been hit
        Vector3 middle = ground.GetCellCenterWorld(ground.WorldToCell(_hit.transform.position));
        Vector2 middle2 = new Vector2(middle.x, middle.y);
        middle2 = new Vector2(middle2.x, middle2.y - (ground.cellSize.y * 0.25f));

        // Get Middle Point of Cell that has been hit
        Vector3 middleRefP = ground.GetCellCenterWorld(mir.refStartCell);
        Vector2 middle2RefP = new Vector2(middleRefP.x, middleRefP.y);
        middle2RefP = new Vector2(middle2RefP.x, middle2RefP.y - (ground.cellSize.y * 0.25f));

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
        Debug.DrawLine(e, s, Color.black);

        // Get an imaginary line that the laser is impacting on the object 
        Vector2 movingLineM = (eM - sM).normalized;
        Debug.DrawLine(eM, sM, Color.green);

        // ??? could be a fault
        Vector2 h1 = _hit.point + movingLineM * (ground.cellSize.x * 0.5f);
        Debug.DrawLine(_hit.point, h1, Color.red);

        // Calculate elements needed for line intersection calculation
        List<float> l1 = getABCForLine(cM, c2M);
        List<float> l2 = getABCForLine(_hit.point, h1);

        Debug.DrawLine(cM, c2M, Color.magenta);
        Debug.DrawLine(_hit.point, h1, Color.blue);

        // Get point where imaginary lines are intersecting to calculate offset
        Vector2 distanceHelp = getIntersection(l1, l2);

        // Calculate distance between intersecting point and actual hit point 
        float distance2 = Vector2.Distance(distanceHelp, _hit.point);

        if (mir.startingMir)
        {
            if (mir.angle == 0 || mir.angle == 2)
            {
                // Use the distance to position the refPoint somewhere on the movingline
                refP.position = (e - movingLine * distance2);
            }
            if (mir.angle == 1 || mir.angle == 3)
            {
                refP.position = (e + movingLine * distance2);
            }
            


        }
        else
        {
            Vector2 upray = (_hit.point - middle2).normalized;
           
            float distanceUp = Vector2.Distance(middle2, _hit.point);

            s = middle2RefP;
            e = middle2RefP + (upray * distanceUp);

            Debug.DrawLine(middle2, _hit.point, Color.red);
            Debug.DrawLine(s, e, Color.yellow);

            Debug.DrawRay(middle2RefP, upray, Color.yellow);


            refP.position = e;
        }
        
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

    private void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        ground = GameObject.Find("Grid").GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        name1 = "";
        name2 = "";
        name3 = "";

        points2 = new List<Vector3>();
        gM.clearHitMirrors();
        Shoot3();
        DrawLine(points2);
        
    }
}
