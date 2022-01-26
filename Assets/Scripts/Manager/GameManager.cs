using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

    public List<Box> boxxes = new List<Box>();
    public List<Mirror> mirrors = new List<Mirror>();

    public List<Mirror> hitMirros = new List<Mirror>();
    public List<Mirror> hitMirrosStable = new List<Mirror>();

    public List<Pipe> hitPipes = new List<Pipe>();
    public List<Pipe> hitPipesStable = new List<Pipe>();

    public List<CombiTile> hitCombi = new List<CombiTile>();
    public List<CombiTile> hitCombiStable = new List<CombiTile>();

    public List<GameObject> windPowered = new List<GameObject>();
    public List<windHit> windHis = new List<windHit>();

    public EnergyManager EM;
    public MovementController p1;
    public MovementController p2;
    public MovementController p3;

    private bool modifying;

    public class movementSet
    {
        public int playerId;
        public KeyCode forward;
        public KeyCode backward;
        public KeyCode left;
        public KeyCode right;
        public KeyCode ability1;
        public KeyCode up;
        public KeyCode down;
    }

    public class windHit
    {
        public GameObject hitOjbect;
        public float direction; 
    }

    public void removeByObject(GameObject o)
    {
        List<windHit> remove = new List<windHit>();
        foreach (windHit w in windHis)
        {
            if (w.hitOjbect == o) 
            {
                remove.Add(w);
            }
        }

        foreach (windHit w2 in remove)
        {
            windHis.Remove(w2);
        }
    }

    public List<movementSet> controlls = new List<movementSet>();

    void Awake()
    {
        DontDestroyOnLoad(this);

        movementSet p1 = new movementSet();
        p1.playerId = 1;
        p1.forward = KeyCode.W;
        p1.backward = KeyCode.S;
        p1.left = KeyCode.A;
        p1.right = KeyCode.D;
        p1.ability1 = KeyCode.F;
        p1.up = new KeyCode();
        p1.down = new KeyCode();
        controlls.Add(p1);

        movementSet p2 = new movementSet();
        p2.playerId = 2;
        p2.forward = KeyCode.UpArrow;
        p2.backward = KeyCode.DownArrow;
        p2.left = KeyCode.LeftArrow;
        p2.right = KeyCode.RightArrow;
        p2.ability1 = KeyCode.Return;
        p2.up = KeyCode.O;
        p2.down = KeyCode.L;
        controlls.Add(p2);

        movementSet p3 = new movementSet();
        p3.playerId = 3;
        p3.forward = KeyCode.W;
        p3.backward = KeyCode.S;
        p3.left = KeyCode.LeftArrow;
        p3.right = KeyCode.RightArrow;
        p3.up = KeyCode.R;
        p3.down = KeyCode.F;
        p3.ability1 = KeyCode.Return;
        controlls.Add(p3);

    }

    public void Start()
    {
        foreach (GameObject b in GameObject.FindGameObjectsWithTag("Box"))
        {
            boxxes.Add(b.GetComponent<Box>());
        }
        foreach (GameObject b in GameObject.FindGameObjectsWithTag("Mirror"))
        {
            mirrors.Add(b.GetComponent<Mirror>());
        }
    }

    public bool isBoxOnCell(Vector3Int cell, Grid ground)
    {
        //Debug.Log(cell);
        foreach (Box b in boxxes)
        {
            if (ground.WorldToCell(b.transform.position) == cell)
            {
                return true;
            }
        }
        return false;
    }

    public bool isMirrorOnCell(Vector3Int cell, Grid ground)
    {
        //Debug.Log(cell);
        foreach (Mirror m in mirrors)
        {
            if (ground.WorldToCell(m.transform.position) == cell)
            {
                return true;
            }
        }
        return false;
    }

    public GameObject getBoxOnCell(Vector3Int cell, Grid ground)
    {
        
        foreach (Box b in boxxes)
        {
            if (ground.WorldToCell(b.transform.position) == cell)
            {
                return b.gameObject;
            }
        }
        return null;
    }

    public GameObject getMirOnCell(Vector3Int cell, Grid ground)
    {

        foreach (Mirror m in mirrors)
        {
            if (ground.WorldToCell(m.transform.position) == cell)
            {
                return m.gameObject;
            }
        }
        return null;
    }

    public Box getBoxSOnCell(Vector3Int cell, Grid ground)
    {
        foreach (Box b in boxxes)
        {
            if (ground.WorldToCell(b.transform.position) == cell)
            {
                return b;
            }
        }
        return null;
    }

    public Mirror getMirSOnCell(Vector3Int cell, Grid ground)
    {
        foreach (Mirror m in mirrors)
        {
            if (ground.WorldToCell(m.transform.position) == cell)
            {
                return m;
            }
        }
        return null;
    }

    void Update()
    {

        //Debug.Log(windHis[0].direction);
        //Debug.Log(windHis[0].hitOjbect);
    }

    public void UpdateStableMirList()
    {
        List<Mirror> help1 = new List<Mirror>();
        foreach (Mirror m in hitMirrosStable)
        {
            if (!hitMirros.Contains(m))
            {
                help1.Add(m);
            }
        }

        foreach(Mirror m2 in help1)
        {
            hitMirrosStable.Remove(m2);
        }
    }

    public void UpdateStablePipeList()
    {
        List<Pipe> help1 = new List<Pipe>();
        foreach (Pipe p in hitPipesStable)
        {
            if (!hitPipes.Contains(p))
            {
                help1.Add(p);
            }
        }

        foreach (Pipe p2 in help1)
        {
            hitPipesStable.Remove(p2);
        }
    }

    public void UpdateStableCombiList()
    {
        List<CombiTile> help1 = new List<CombiTile>();
        foreach (CombiTile c in hitCombiStable)
        {
            if (!hitCombi.Contains(c))
            {
                help1.Add(c);
            }
        }

        foreach (CombiTile c2 in help1)
        {
            hitCombiStable.Remove(c2);
        }

    }

    public void clearHitMirrors()
    {
        hitMirros = new List<Mirror>();
    }
    public void clearHitPipes()
    {
        hitPipes = new List<Pipe>();
    }
    public void clearHitCombi()
    {
        hitCombi = new List<CombiTile>();
    }

    private void LateUpdate()
    {
        UpdateStableMirList();
        UpdateStableCombiList();
        UpdateStablePipeList();
    }

    public float calcRecAngle(GameObject hitObject, Vector3 point)
    {
        float Mangle;
        // Mir X > Point X
        if (hitObject.transform.position.x > point.x)
        {
            // Mir Y > Point Y
            if (hitObject.transform.position.y > point.y)
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
            if (hitObject.transform.position.y > point.y)
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

    public float calcRecAngle2(Vector3 midPoint, Vector3 point)
    {
        float Mangle;
        // Mir X > Point X
        if (midPoint.x > point.x)
        {
            // Mir Y > Point Y
            if (midPoint.y > point.y)
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
            if (midPoint.y > point.y)
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

    public float calcRecAngleByRay(Vector3 hit, Vector3 ray)
    {
        Vector3 helpPoint = hit + ((ray * -1) * 2);
        float Mangle;
        // Mir X > Point X
        if (helpPoint.x > hit.x)
        {
            // Mir Y > Point Y
            if (helpPoint.y > hit.y)
            {
                Mangle = 2;
            }
            // Mir Y < Point Y
            else
            {
                Mangle = 1;
            }
        }
        // Mir X < Point X
        else
        {
            // Mir Y > Point Y
            if (helpPoint.y > hit.y)
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
    

    
}
