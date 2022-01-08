using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

    public List<Box> boxxes = new List<Box>();
    public EnergyManager EM;
    public MovementController p1;
    public MovementController p2;

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
    }

    public bool isBoxOnCell(Vector3Int cell, Grid ground)
    {
        foreach (Box b in boxxes)
        {
            if (ground.WorldToCell(b.transform.position) == cell)
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

    void Update()
    {
    }
}
