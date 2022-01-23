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

    public List<GameObject> windPowered = new List<GameObject>();
    public List<windHit> windHis = new List<windHit>();

    public EnergyManager EM;
    public MovementController p1;
    public MovementController p2;

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
        modifying = true;
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
        modifying = false;
    }

    public void clearHitMirrors()
    {
        if (!modifying) hitMirros = new List<Mirror>();
    }

    private void LateUpdate()
    {
        UpdateStableMirList();
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
}
