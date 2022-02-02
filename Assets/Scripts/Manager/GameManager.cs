using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

    public List<Box> boxxes = new List<Box>();
    public List<Mirror> mirrors = new List<Mirror>();
    public List<Obstacle> obstacles = new List<Obstacle>();

    public List<Mirror> hitMirros = new List<Mirror>();
    public List<Mirror> hitMirrosStable = new List<Mirror>();

    public List<Pipe> hitPipes = new List<Pipe>();
    public List<Pipe> hitPipesStable = new List<Pipe>();

    public List<CombiTile> hitCombi = new List<CombiTile>();
    public List<CombiTile> hitCombiStable = new List<CombiTile>();

    public List<GameObject> windPowered = new List<GameObject>();
    public List<windHit> windHis = new List<windHit>();

    public List<collectedBones> bones = new List<collectedBones>();

    public GameObject collG;
    public List<List<Vector3Int>> collisionList = new List<List<Vector3Int>>();



    public List<List<Vector3Int>> tilesList = new List<List<Vector3Int>>();

    public List<Vector3Int> h1;
    public List<Vector3Int> h2;
    public List<Vector3Int> h3;
    public List<Vector3Int> h4;
    public List<Vector3Int> h5;

    public EnergyManager EM;
    public MovementController p1;
    public MovementController p2;
    public MovementController p3;

    public Grid ground;

    public Camera mainCam;
    public CameraPositions camPos;

    private bool modifying;

    public List<int> elevSL = new List<int>();
    public Tilemap waterMap;
    public Tilemap boxPlacingMap;

    public int currentCamPos = 0;
    public bool camMoving;
    public float camSpeed;

    public List<bool> camChangeCond = new List<bool>();
    public List<bool> camChangeCondHelp = new List<bool>();

    public bool sceneShowCase;

    public class movementSet
    {
        public int playerId;
        public KeyCode forward;
        public KeyCode backward;
        public KeyCode left;
        public KeyCode right;
        public KeyCode ability1;
        public KeyCode ability2;
        public KeyCode up;
        public KeyCode down;
    }

    public class windHit
    {
        public GameObject hitOjbect;
        public float direction; 
    }

    public class collectedBones
    {
        public GameObject bone;
        public MovementController playerCollected;
        public int number;
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
        currentCamPos = 0;
        DontDestroyOnLoad(this);

        movementSet p1 = new movementSet();
        p1.playerId = 1;
        p1.forward = KeyCode.UpArrow;
        p1.backward = KeyCode.DownArrow;
        p1.left = KeyCode.LeftArrow;
        p1.right = KeyCode.RightArrow;
        p1.ability1 = KeyCode.Return;
        p1.ability2 = new KeyCode();
        p1.up = new KeyCode();
        p1.down = new KeyCode();
        controlls.Add(p1);

        movementSet p2 = new movementSet();
        p2.playerId = 2;
        p2.forward = KeyCode.W;
        p2.backward = KeyCode.S;
        p2.left = KeyCode.A;
        p2.right = KeyCode.D;
        p2.ability1 = KeyCode.E;
        p2.ability2 = new KeyCode();
        p2.up = KeyCode.R;
        p2.down = KeyCode.F;
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
        p3.ability2 = KeyCode.Backspace;
        controlls.Add(p3);

        ground = GameObject.Find("Grid").GetComponent<Grid>();

        waterMap = ground.transform.GetChild(1).GetComponent<Tilemap>();
        boxPlacingMap = ground.transform.GetChild(2).GetComponent<Tilemap>();

        camSpeed = 2;

        for (int i = 0; i < 10; i++)
        {
            camChangeCond.Add(false);
        }
        foreach (bool b in camChangeCond)
        {
            camChangeCondHelp.Add(false);
        }

    }

    public void updateCamChangeCond()
    {
        camChangeCond[0] = (this.p1.currentCell.x >= 31 && this.p2.currentCell.x >= 31);
        camChangeCond[1] = camChangeCond[0] && (this.p3.currentCell.x >= 49 );
        camChangeCond[2] = camChangeCond[1] && (this.p1.currentCell.x >= 59 && this.p2.currentCell.x >= 59);
        camChangeCond[3] = camChangeCond[2] && (this.p3.currentCell.x >= 76); 
        camChangeCond[4] = camChangeCond[3] && (this.p1.currentCell.x >= 92 && this.p2.currentCell.x >= 92);
        camChangeCond[5] = camChangeCond[4] && (this.p1.currentCell.x >= 98 && this.p2.currentCell.x >= 98);
        camChangeCond[6] = camChangeCond[5] && (this.p3.currentCell.x >= 117);
        camChangeCond[7] = camChangeCond[6] && (this.p1.currentCell.x >= 112 && this.p2.currentCell.x >= 112);
        camChangeCond[8] = camChangeCond[7] && (this.p3.currentCell.x >= 154);
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
        foreach (GameObject b in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            obstacles.Add(b.GetComponent<Obstacle>());
        }

        foreach (Transform child in collG.transform)
        {
            List<Vector3Int>  collisionsInGrid = new List<Vector3Int>();
            Tilemap collisionTileMap = child.GetComponent<Tilemap>();
            foreach (var pos in collisionTileMap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                Vector3 place = collisionTileMap.CellToWorld(localPlace);
                if (collisionTileMap.HasTile(localPlace))
                {
                    collisionsInGrid.Add(ground.WorldToCell(place));
                }
            }
            collisionList.Add(collisionsInGrid);
        }

        for (int i = 2; i < 7; i++)
        {
            List<Vector3Int> tilesInGrid = new List<Vector3Int>();
            Tilemap tileTileMap = ground.transform.GetChild(i).GetComponent<Tilemap>();

            foreach (var pos in tileTileMap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                Vector3 place = tileTileMap.CellToWorld(localPlace);
                if (tileTileMap.HasTile(localPlace))
                {
                    tilesInGrid.Add(ground.WorldToCell(place));
                }
            }
            tilesList.Add(tilesInGrid);
        }



        int k = 0;
        List<List<Vector3Int>> hh = new List<List<Vector3Int>>();
        foreach (List<Vector3Int> l in tilesList)
        {
            int o = 0;
            List<Vector3Int> l2 = new List<Vector3Int>();
            foreach (Vector3Int v in l)
            {
                l2.Add(new Vector3Int(v.x - k, v.y - k, v.z));
                o += 1;
            }
            hh.Add(l2);
            k += 1;
        }

        //tilesList = hh;

        h1 = tilesList[0];
        h2 = tilesList[1];
        h3 = tilesList[2];
        h4 = tilesList[3];
        h5 = tilesList[4];

        elevSL.Add(SortingLayer.NameToID("Elevation1"));
        elevSL.Add(SortingLayer.NameToID("Elevation 2"));
        elevSL.Add(SortingLayer.NameToID("Elevation 3"));
        elevSL.Add(SortingLayer.NameToID("Elevation 4"));
        elevSL.Add(SortingLayer.NameToID("Elevation 5"));

        camPos = GameObject.Find("CAMERASETTING").GetComponent<CameraPositions>();
        mainCam = Camera.main;
        mainCam.transform.position = camPos.cameraPositions[0];

    }

    public void setCameraPos(int pos)
    {
        mainCam.transform.position = camPos.cameraPositions[pos];
    }

    public bool isBoxOnCell(Vector3Int cell, Grid ground)
    {
        //Debug.Log(cell);
        foreach (Box b in boxxes)
        {
            Vector3Int check = ground.WorldToCell(b.transform.position);
            check = new Vector3Int(check.x - b.elevation, check.y - b.elevation, check.z);
            if (check == cell)
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

    public bool isObstacleOnCell(Vector3Int cell, Grid ground)
    {
        //Debug.Log(cell);
        foreach (Obstacle o in obstacles)
        {
            if (ground.WorldToCell(o.transform.position) == cell)
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
            Vector3Int check = ground.WorldToCell(b.transform.position);
            check = new Vector3Int(check.x - b.elevation, check.y - b.elevation, check.z);
            if (check == cell)
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
    
    public GameObject getObstacleOnCell(Vector3Int cell, Grid ground)
    {

        foreach (Obstacle o in obstacles)
        {
            if (ground.WorldToCell(o.transform.position) == cell)
            {
                return o.gameObject;
            }
        }
        return null;
    }

    public Box getBoxSOnCell(Vector3Int cell, Grid ground)
    {
        foreach (Box b in boxxes)
        {
            Vector3Int check = ground.WorldToCell(b.transform.position);
            check = new Vector3Int(check.x - b.elevation, check.y - b.elevation, check.z);
            if (check == cell)
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
    public Obstacle getObstacleSOnCell(Vector3Int cell, Grid ground)
    {
        foreach (Obstacle o in obstacles)
        {
            if (ground.WorldToCell(o.transform.position) == cell)
            {
                return o;
            }
        }
        return null;
    }

    void Update()
    {
        if (bones.Count > 0)
        {
            //Debug.Log(bones[0].bone.name);
        }
        
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            camStepUp();
        }  
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            camStepDown();
        }
        
        if (camMoving && !sceneShowCase)
        {
            if (Vector3.Distance(mainCam.transform.position, camPos.cameraPositions[currentCamPos]) <= 0.01f)
            {
                camMoving = false;
            }
            else
            {
                mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, camPos.cameraPositions[currentCamPos], Time.deltaTime * camSpeed);
            }
        }
        
        updateCamChangeCond();
        mainCam.transform.position = camPos.cameraPositions[currentCamPos];
        if (camChangeCond[currentCamPos] && !camChangeCondHelp[currentCamPos])
        {
            camChangeCondHelp[currentCamPos] = false;
            camStepUp();
        }

        if (sceneShowCase)
        {
            if (Vector3.Distance(mainCam.transform.position, camPos.cameraPositions[currentCamPos]) <= 0.1f)
            {
                camStepUp();
            }
            else
            {
                mainCam.transform.position = Vector3.MoveTowards(mainCam.transform.position, camPos.cameraPositions[currentCamPos], Time.deltaTime * 3f);
            }
        }
    }

    public void camStepUp()
    {
        currentCamPos += 1;
        camMoving = true;
    }

    public void camStepDown()
    {
        currentCamPos -= 1;
        camMoving = true;
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
    
    public int getHighestElevation(Vector3Int cell, GameObject ignore = null)
    {
        int elv = -1;

        int j = 0;
        foreach (List<Vector3Int> l in tilesList)
        {
            if (l.Contains(cell))
            {
                if (j > elv)
                {
                    elv = j;
                }
            }
            j += 1;
        }

        j = 0;
        foreach ( List<Vector3Int> l in collisionList)
        {
            if (l.Contains(cell))
            {
                if (j > elv)
                {
                    //elv = j;
                }
            }
            j += 1;
        }

        if (isBoxOnCell(cell, ground))
        {
            int e = getBoxSOnCell(cell, ground).elevation;
            if (e == 0) e = 1;
            if ( e > elv)
            {
                GameObject b = getBoxOnCell(cell, ground);
                if (!(b == ignore) && b.activeInHierarchy && !getBoxSOnCell(cell, ground).beingcarried)
                {
                    elv = e;
                    if (elv == 0) elv = 1;
                    if (getBoxSOnCell(cell, ground).saveTile != null) elv = 0; 
                }
                    

            }
        }
        if (getMirOnCell(cell, ground))
        {
            int e = getMirSOnCell(cell, ground).elevation;
            if (e == 0) e = 1;
            if (e > elv)
            {
                GameObject m = getMirOnCell(cell, ground);
                if (!(m == ignore) && m.activeInHierarchy)
                {
                    elv = e;
                    if (elv == 0) elv = 1;
                }
                    

            }
        }

        if (isObstacleOnCell(cell, ground))
        {
            int e = getObstacleSOnCell(cell, ground).elevation;
            if (e == 0) e = 1;
            if ( e > elv)
            {
                GameObject o = getObstacleOnCell(cell, ground);
                if (!(o == ignore) && o.activeInHierarchy)
                {
                    elv = e;
                    if (elv == 0) elv = 1;
                }
            }
        }

        return elv;
    }
 
    
    public int getHighestElevationUnder(Vector3Int cell, int under, GameObject ignore = null)
    {
        int elv = -1;

        int j = 0;
        foreach (List<Vector3Int> l in tilesList)
        {
            if (l.Contains(cell))
            {
                if (j > elv && j < under)
                {
                    elv = j;
                }
            }
            j += 1;
        }

        j = 0;
        foreach (List<Vector3Int> l in collisionList)
        {
            if (l.Contains(cell))
            {
                if (j > elv && j < under)
                {
                    elv = j;
                }
            }
            j += 1;
        }

        if (isBoxOnCell(cell, ground))
        {
            if (getBoxSOnCell(cell, ground).elevation > elv && getBoxSOnCell(cell, ground).elevation < under)
            {
                if (!getBoxOnCell(cell, ground) == ignore) elv = getBoxSOnCell(cell, ground).elevation;

            }
        }
        if (getMirOnCell(cell, ground))
        {
            if (getMirSOnCell(cell, ground).elevation > elv && getMirSOnCell(cell, ground).elevation < under)
            {
                if (!getMirOnCell(cell, ground) == ignore) elv = getMirSOnCell(cell, ground).elevation;

            }
        }

        if (getObstacleOnCell(cell, ground))
        {
            if (getObstacleSOnCell(cell, ground).elevation > elv && getObstacleSOnCell(cell, ground).elevation < under)
            {
                if (!getObstacleOnCell(cell, ground) == ignore) elv = getObstacleSOnCell(cell, ground).elevation;
            }
        }

        return elv;
    }
}
