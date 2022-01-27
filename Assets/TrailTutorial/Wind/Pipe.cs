using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{

    public float randomInterval;
    public float spawnCount;
    public float type;

    public ParticleSystem p1;
    public ParticleSystem p2;
    public ParticleSystem p3;
    public ParticleSystem p4;
    public ParticleSystem p5;
    public ParticleSystem p6;
    public ParticleSystem p7;
    public List<ParticleSystem> pList = new List<ParticleSystem>();
    public List<Vector3> pPositionList = new List<Vector3>();

    private float counterTime;
    public float length;
    public List<float> counter;
    public bool on;
    public float timer;
    public List<Vector3Int> windArea;
    public Vector3 directionPoint;
    public Vector3 directionPoint2;
    public Grid ground;
    public GameManager gM;
    private GameObject hitStore;
    public GameObject ssio;
    public GameObject ssio2;
    public GameObject middle;

    // Start is called before the first frame update
    void Start()
    {
        ground = GameObject.Find("Grid").GetComponent<Grid>();
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        Vector3 midPoint = new Vector3();
        switch (type)
        {
            case 0:
                midPoint = ground.GetCellCenterWorld(ground.WorldToCell(middle.transform.position));
                midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));
                directionPoint2 = new Vector3(midPoint.x - (ground.cellSize.x * 0.25f), midPoint.y + (ground.cellSize.y * 0.25f));
                directionPoint = midPoint;
                break;
            case 1:
                midPoint = ground.GetCellCenterWorld(ground.WorldToCell(middle.transform.position));
                midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));
                directionPoint2 = new Vector3(midPoint.x - (ground.cellSize.x * 0.25f), midPoint.y - (ground.cellSize.y * 0.25f));
                directionPoint = midPoint;
                break;
            case 2:
                midPoint = ground.GetCellCenterWorld(ground.WorldToCell(middle.transform.position));
                midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));
                directionPoint2 = new Vector3(midPoint.x + (ground.cellSize.x * 0.25f), midPoint.y - (ground.cellSize.y * 0.25f));
                directionPoint = midPoint;
                break;
            case 3:
                midPoint = ground.GetCellCenterWorld(ground.WorldToCell(middle.transform.position));
                midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));
                directionPoint2 = new Vector3(midPoint.x + (ground.cellSize.x * 0.25f), midPoint.y + (ground.cellSize.y * 0.25f));
                directionPoint = midPoint;
                break;
        }



        pList.Add(p1);
        pList.Add(p2);
        pList.Add(p3);
        pList.Add(p4);
        pList.Add(p5);
        pList.Add(p6);
        pList.Add(p7);

        on = true;
        timer = randomInterval;

        foreach(ParticleSystem p in pList)
        {
            pPositionList.Add(p.transform.position);
        }

    }

    bool isLeft(Vector2 a, Vector2 b, Vector2 c)
    {
        //Debug.DrawLine(a, b);
        return ((b.x - a.x) * (c.y- a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
    }

    void killParticles(ParticleSystem ps, Vector2 p1, Vector2 p2)
    {
        ParticleSystem.Particle[] particles;
        var main = ps.main;
        particles = new ParticleSystem.Particle[main.maxParticles];
        int count = ps.GetParticles(particles);
        for (int i = 0; i < count; i++)
        {
            Debug.DrawLine(p1, p2);
            
            if (! isLeft(p1, p2,  ps.transform.TransformPoint(particles[i].position)))
            {
                //Debug.Log(("KILL", particles[i]));
                particles[i].remainingLifetime  = -1.0f;
            }
        }
        ps.SetParticles(particles, count);
    }


    void determinWindArea()
    {
        Vector3 ray = new Vector3();
        RaycastHit2D _hit = new RaycastHit2D();
        switch (type)
        {
            case 0:
                ray = (directionPoint - directionPoint2).normalized;
                _hit = Physics2D.Raycast(directionPoint2, ray);
                break;
            case 1:
                ray = (directionPoint2 - directionPoint).normalized;
                _hit = Physics2D.Raycast(directionPoint2, ray);
                break;
            case 2:
                ray = (directionPoint - directionPoint2).normalized;
                _hit = Physics2D.Raycast(directionPoint2, ray);
                break;
            case 3:
                ray = (directionPoint2 - directionPoint).normalized;
                _hit = Physics2D.Raycast(directionPoint2, ray);
                break;
        }


        Debug.DrawRay(directionPoint2, ray * 10, Color.green) ;
        if (_hit)
        {
            foreach (ParticleSystem p in pList)
            {
                Vector2 ray2 = new Vector2(-ray.y, ray.x);
                Vector3 p1 = _hit.point - ray2 * 5;
                Vector3 p2 = _hit.point + ray2 * 5;
                killParticles(p, p1, p2);
            }

            if (!gM.windPowered.Contains(_hit.transform.gameObject))
            {
                gM.windPowered.Add(_hit.transform.gameObject);
                GameManager.windHit w = new GameManager.windHit();
                w.hitOjbect = _hit.transform.gameObject;
                w.direction = gM.calcRecAngle(_hit.transform.gameObject, _hit.point);
                gM.windHis.Add(w);
            }

            if (_hit.transform.gameObject)
            {
                if (_hit.transform.gameObject != hitStore)
                {
                    gM.windPowered.Remove(hitStore);
                    gM.removeByObject(hitStore);
                }
            }

            if (_hit.transform.gameObject)
            {
                hitStore = _hit.transform.gameObject;

                if (_hit.transform.CompareTag("CombiTile"))
                {
                    _hit.transform.gameObject.GetComponent<CombiTile>().HitByWind(_hit.point, ray);
                }
                
            }
        }
        else
        {
            Debug.Log("ALARM");
        }
     }


     // Update is called once per frame
     void Update()
     { 

        if (Input.GetKeyDown(KeyCode.U))
        {
            on = !on;
        }
        if (on)
        {
            determinWindArea();
        }
        

    }
}
