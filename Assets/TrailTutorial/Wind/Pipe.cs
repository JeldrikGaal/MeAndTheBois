using System;
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

    public int elevation;
    public LayerMask ignore;
    public RaycastHit2D hitH;

    public GameObject windCont;

    public Obstacle specialCaseOne;
    public bool specialCaseOneB;

    public bool specialCaseTwoB;
    public bool specialCaseTwoBHelp;
    public bool specialCaseTwoBHelp2;
    public Box specialCaseTwo;
    public Vector3Int specialCaseTwoVec;

    public bool specialCaseThreeB;
    public Mirror specialCaseThreeM;
    public int specialCaseInt;

    public bool specialCaseFourB;
    public Box specialCaseFourBo;
    public Pipe specialCasePipe;
    public Mirror specialCaseFourMirror;

    public bool specialCaseFiveB;

    public bool specialCaseSixB;
    public Pipe specialCaseSixP;
    public Mirror specialCaseSixM1;
    public Mirror specialCaseSixM2;
    public Box specialCaseSixBo;

    public bool specialCaseSevenB;


    // Start is called before the first frame update
    void Start()
    {
        ground = GameObject.Find("Grid").GetComponent<Grid>();
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();

        specialCaseTwoVec = new Vector3Int(0, 0, 0);
        Vector3 midPoint = new Vector3();

        string elevationHelp = this.GetComponent<SpriteRenderer>().sortingLayerName;
        var h = elevationHelp[elevationHelp.Length - 1];
        elevation = (int)Char.GetNumericValue(h) - 1;

        pList.Add(p1);
        pList.Add(p2);
        pList.Add(p3);
        pList.Add(p4);
        pList.Add(p5);

        windCont = this.transform.GetChild(7).gameObject;

        on = true;
        if (specialCaseTwoB) on = false;
        if (specialCaseFiveB) on = false;
        timer = randomInterval;

        foreach (ParticleSystem p in pList)
        {
            pPositionList.Add(p.transform.position);
        }

        switch (type)
        {
            case 0:
                midPoint = ground.GetCellCenterWorld(ground.WorldToCell(middle.transform.position));
                midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));
                //midPoint = pList[0].transform.position;
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



        ignore = getMaskToIgnore2();
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


    LayerMask getMaskToIgnore2()
    {
        LayerMask ret = new LayerMask();
        LayerMask l1 = new LayerMask();
        LayerMask l2 = new LayerMask();
        LayerMask l3 = new LayerMask();
        LayerMask l4 = new LayerMask();
        List<LayerMask> layerMaskList = new List<LayerMask>();
        layerMaskList.Add(l1);
        layerMaskList.Add(l2);
        layerMaskList.Add(l3);
        layerMaskList.Add(l4);
        List<int> layerIDsToIgnore = new List<int>();
        for (int i = 0; i < 5; i ++)
        {
            string layerName = "Col" + (i + 1).ToString();
            layerIDsToIgnore.Add(LayerMask.NameToLayer(layerName));
        }

        int j = 0;
        for (int i = 0; i < 5; i++)
        {
            if (!(i + 1 == elevation))
            {
                layerMaskList[j] = 1 << layerIDsToIgnore[i];
                j += 1;
            }
        }

        ret = ~(layerMaskList[0] | layerMaskList[1] | layerMaskList[2] | layerMaskList[3]);

        return ret;
        
    }

    LayerMask getMaskToIgnore()
    {
        string layerName = "Col" + (elevation).ToString();
        Debug.Log(layerName);
        return ~(1 << LayerMask.NameToLayer(layerName));
    }

    void determinWindArea()
    {
        Vector3 ray = new Vector3();
        RaycastHit2D _hit = new RaycastHit2D();
        Vector2 startPoint = new Vector2();
        switch (type)
        {
            case 0:
                ray = (directionPoint - directionPoint2).normalized;
                startPoint = directionPoint2 + ray * 0.2f;
                _hit = Physics2D.Raycast(startPoint, ray, Mathf.Infinity, ignore);
                break;
            case 1:
                ray = (directionPoint2 - directionPoint).normalized;
                startPoint = directionPoint2 + ray * -0.5f;
                _hit = Physics2D.Raycast(startPoint, ray, Mathf.Infinity, ignore);
                break;
            case 2:
                ray = (directionPoint - directionPoint2).normalized;
                startPoint = directionPoint2 + ray * 0.2f;
                _hit = Physics2D.Raycast(startPoint, ray, Mathf.Infinity, ignore);
                break;
            case 3:
                ray = (directionPoint2 - directionPoint).normalized;
                startPoint = directionPoint2 + ray * 0.2f;
                _hit = Physics2D.Raycast(startPoint, ray, Mathf.Infinity, ignore);
                break;
        }


        if (type == 0 || type == 2) 
        {
            Debug.DrawRay(startPoint, ray * 10, Color.green);
        }
        else
        {
            Debug.DrawRay(startPoint, ray * 10, Color.green);
        }
            
        if (_hit)
        {
            //Debug.Log(_hit.transform.name);
            hitH = _hit;
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
            hitH = new RaycastHit2D();
            //Debug.Log("ALARM");
        }
     }

     void toggleParticles(bool o)
     {
        foreach (ParticleSystem p in pList)
        {
            p.gameObject.SetActive(o);
        }
        windCont.SetActive(o);
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
        else
        {
            //hitH = new RaycastHit2D();
        }
        toggleParticles(on);
        
        if (specialCaseOneB)
        {
            if (specialCaseOne.isActiveAndEnabled)
            {
                on = false;
            }
            else
            {
                on = true;
            }
        }

        if (specialCaseTwoB)
        {
            if (specialCaseTwo.currentCell == specialCaseTwoVec)
            {
                on = false;
            }
            else
            {
                on = true;
            }
            if (specialCaseTwoBHelp == false)
            {
                specialCaseTwoBHelp = true;
            }
            if (specialCaseTwoBHelp && !specialCaseTwoBHelp2)
            {
                specialCaseTwoBHelp2 = true;
            }
            if (specialCaseTwoBHelp2 && specialCaseTwoVec == new Vector3Int(0, 0, 0))
            {
                specialCaseTwoVec = specialCaseTwo.currentCell;

            }
        }

        if (specialCaseThreeB)
        {
            if (specialCaseThreeM.angle == specialCaseInt)
            {
                on = true;
            }
            else
            {
                on = false;
            }
        }

        if (specialCaseFourB)
        {
            Vector3 ray = new Vector3();
            RaycastHit2D _hit = new RaycastHit2D();
            Vector2 startPoint = new Vector2();
            switch (type)
            {
                case 0:
                    ray = (directionPoint - directionPoint2).normalized;
                    startPoint = directionPoint2 + ray * 0.2f;
                    _hit = Physics2D.Raycast(startPoint, ray, Mathf.Infinity, ignore);
                    break;
                case 1:
                    ray = (directionPoint2 - directionPoint).normalized;
                    startPoint = directionPoint2 + ray * -0.5f;
                    _hit = Physics2D.Raycast(startPoint, ray, Mathf.Infinity, ignore);
                    break;
                case 2:
                    ray = (directionPoint - directionPoint2).normalized;
                    startPoint = directionPoint2 + ray * 0.2f;
                    _hit = Physics2D.Raycast(startPoint, ray, Mathf.Infinity, ignore);
                    break;
                case 3:
                    ray = (directionPoint2 - directionPoint).normalized;
                    startPoint = directionPoint2 + ray * 0.2f;
                    _hit = Physics2D.Raycast(startPoint, ray, Mathf.Infinity, ignore);
                    break;
            }
            hitH = _hit;

            if (hitH)
            {
                //Debug.Log(hitH.transform.gameObject == specialCaseFourBo.transform.GetChild(0).gameObject);
                if (hitH.transform.gameObject == specialCaseFourBo.transform.GetChild(0).gameObject)
                {
                    this.on = false;
                    specialCasePipe.on = true;
                    specialCaseFourMirror.timed = false;
                    //Debug.Log("true");
                }
                else
                {
                    this.on = true;
                    specialCasePipe.on = false;
                    //Debug.Log("false 1");
                    specialCaseFourMirror.timed = true;
                }
            }
            else
            {
                this.on = true;
                specialCasePipe.on = false;
                //Debug.Log("false 2");
                specialCaseFourMirror.timed = true;
            }


        }

        if (specialCaseSixB)
        {
            if (specialCaseSixM1.angle == 0 || specialCaseSixM1.angle == 1)
            {
                if (specialCaseSixP.hitH != new RaycastHit2D())
                {
                    if (specialCaseSixP.hitH.transform.gameObject == specialCaseSixBo.transform.GetChild(0).gameObject)
                    {
                        this.on = true;
                        specialCaseSixP.on = false;
                        specialCaseSixM2.timed = false;
                    }
                    else
                    {
                        this.on = false;
                        specialCaseSixP.on = true;
                        specialCaseSixM2.timed = true;
                    }
                }
                else
                {
                    this.on = false;
                    specialCaseSixP.on = true;
                    specialCaseSixM2.timed = true;
                }
            }
            else
            {
                this.on = false;
                specialCaseSixP.on = false;
                specialCaseSixM2.timed = false;
            }
        }

        if (specialCaseSevenB)
        {
            Vector3 ray = new Vector3();
            RaycastHit2D _hit = new RaycastHit2D();
            Vector2 startPoint = new Vector2();
            switch (type)
            {
                case 0:
                    ray = (directionPoint - directionPoint2).normalized;
                    startPoint = directionPoint2 + ray * 0.2f;
                    _hit = Physics2D.Raycast(startPoint, ray, Mathf.Infinity, ignore);
                    break;
                case 1:
                    ray = (directionPoint2 - directionPoint).normalized;
                    startPoint = directionPoint2 + ray * -0.5f;
                    _hit = Physics2D.Raycast(startPoint, ray, Mathf.Infinity, ignore);
                    break;
                case 2:
                    ray = (directionPoint - directionPoint2).normalized;
                    startPoint = directionPoint2 + ray * 0.2f;
                    _hit = Physics2D.Raycast(startPoint, ray, Mathf.Infinity, ignore);
                    break;
                case 3:
                    ray = (directionPoint2 - directionPoint).normalized;
                    startPoint = directionPoint2 + ray * 0.2f;
                    _hit = Physics2D.Raycast(startPoint, ray, Mathf.Infinity, ignore);
                    break;
            }
            hitH = _hit;
        }
        {

        }
    }
}
