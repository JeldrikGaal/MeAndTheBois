using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;
using static GameManager.movementSet;

public class MovementController : MonoBehaviour
{
    // Various Objects that are needed as Drag and Drop in Inspector
    public GameObject spawnPoint;
    public GameObject movingPoint;
    public GameObject collisionsG;
    public Tilemap collisionTileMap;
    public Tilemap elevationTileMap;
    public Grid ground;
    public int playerIndex;
    public GameManager gM;

    // Needs to be accessible for other Scripts
    public int directionFacing;


    // private variables
    public GameManager.movementSet playerControlls;

    // Only Public for Debbung
    public Vector3Int currentCell;
    public List<Vector3>  tileWorldLocations;
    public List<List<Vector3Int>> collisionList;

    public List<Vector3Int> help1;
    public List<Vector3Int> help2;
    public List<Vector3Int> help3;

    public List<Vector3Int> collisionsInGrid;
    public List<Vector3Int> elevationsInGrid;
    public bool moving = true;

    private bool s = false;
    public Wind w;
    //public SpriteRenderer sR;
    public SortingGroup sR;
    public GameObject forward;
    public Animator forwardA;
    public GameObject backward;
    public Animator backwardA;


    public Animator currentAnimator;
    public GameObject currentFB;
    public GameObject currentMovingBone;
    public bool idling;

    public Vector3 startScale;

    public CombiRobot cR;

    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    public Sprite sprite4;

    public bool controllBool = true;

    public int elevation;

    public bool isMoving;

    public List<Sprite> sprites;

    public int blockage;
    public float blockDist;
    public bool moAll;
    public Vector3 newP;
    public Vector3 tra;
    private Solar so;
    public List<Vector3Int> p1Exceptions = new List<Vector3Int>();

    public GameObject soundManager;
    public List<List<AudioClip>> sounds;
    public AudioSource soundPlayer;


    void Start()
    {
        idling = true;
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        ground = GameObject.Find("Grid").GetComponent<Grid>();
        soundManager = GameObject.Find("SoundManager");

        sounds = soundManager.GetComponent<SoundManager>().sound;
        soundPlayer = this.GetComponent<AudioSource>();

        moving = true;
        sprites.Add(sprite4);
        sprites.Add(sprite3);
        sprites.Add(sprite2);
        sprites.Add(sprite1);



        forward = this.transform.GetChild(1).gameObject;
        forwardA = forward.transform.GetComponent<Animator>();
        backward = this.transform.GetChild(2).gameObject;
        backwardA = backward.transform.GetComponent<Animator>();
        sR = forward.transform.GetComponent<SortingGroup>();

        startScale = forward.transform.localScale;
        currentAnimator = forwardA;
        currentFB = forward;
        backward.SetActive(false);

        if (playerIndex == 1)
        {
            so = this.GetComponent<Solar>();
        }
        //sR.sprite = sprites[directionFacing];
        if (playerIndex == 2)
        {
            w = this.GetComponent<Wind>();
            w.boneStartYFor = forward.transform.GetChild(11).TransformVector(forward.transform.GetChild(11).GetChild(1).transform.localPosition).y;
            w.boneStartYBac = backward.transform.GetChild(11).TransformVector(backward.transform.GetChild(11).GetChild(1).transform.localPosition).y;
        }
        if (playerIndex == 3)
        {
            cR = this.GetComponent<CombiRobot>();
        }

        // Get all cell positions that have a collision
       
        collisionList = new List<List<Vector3Int>>();

        foreach (Transform child in collisionsG.transform)
        {
            tileWorldLocations = new List<Vector3>();
            collisionsInGrid = new List<Vector3Int>();
            collisionTileMap = child.GetComponent<Tilemap>();
            foreach (var pos in collisionTileMap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                Vector3 place = collisionTileMap.CellToWorld(localPlace);
                if (collisionTileMap.HasTile(localPlace))
                {
                    tileWorldLocations.Add(place);
                    collisionsInGrid.Add(ground.WorldToCell(place));
                }
            }
            collisionList.Add(collisionsInGrid);
        }

        /*help1 = collisionList[0];
        help2 = collisionList[1];
        help3 = collisionList[2];*/

        if (collisionTileMap && elevationTileMap)
        {
            
            // Get all cell positions that have elevation
            foreach (var pos in elevationTileMap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                Vector3 place = elevationTileMap.CellToWorld(localPlace);
                if (elevationTileMap.HasTile(localPlace))
                {
                    elevationsInGrid.Add(ground.WorldToCell(place));
                }
            }
        }
           

        // Move Player to SpawnPoint and Movingpoint to Player
        if (playerIndex != 3)
        {
            Vector3 tempPos = ground.GetCellCenterWorld(ground.WorldToCell(spawnPoint.transform.position));
            //this.transform.position = new Vector3(tempPos.x, tempPos.y - ground.cellSize.y - 0.25f, tempPos.z);
            //this.transform.position = tempPos;
        }

        if (playerIndex == 3)this.transform.position = spawnPoint.transform.position;
        this.movingPoint.transform.position = this.transform.position;

        playerControlls = gM.controlls[playerIndex - 1];

        if (playerIndex == 3 && !s)
        {
            //this.gameObject.SetActive(false);
            s = true;
        }



        // Corner of shame

        p1Exceptions.Add(new Vector3Int(24, -52, 0));
        p1Exceptions.Add(new Vector3Int(23, -52, 0));

    }

    // Get the position of the center of the cell one cell away from the player in the direction given to the function
    public Vector3 Move(int dir)
    {
        switch (dir)
        {
            case 0:
                return ground.GetCellCenterWorld(new Vector3Int(currentCell.x + 1, currentCell.y, currentCell.z));
            case 1:
                return ground.GetCellCenterWorld(new Vector3Int(currentCell.x, currentCell.y + 1, currentCell.z));
            case 2:
                return ground.GetCellCenterWorld(new Vector3Int(currentCell.x - 1, currentCell.y, currentCell.z));
            case 3:
                return ground.GetCellCenterWorld(new Vector3Int(currentCell.x, currentCell.y - 1, currentCell.z));
        }
        return new Vector3();
    }

    int incDir(int t)
    {
        if (t == 3)
        {
            t = 0;
        }
        else
        {
            t += 1;
        }
        return t;
    }

    int decDir(int t)
    {
        if (t == 0)
        {
            t = 3;
        }
        else
        {
            t -= 1;
        }
        return t;
    }


    //Seedbomb ability for combined robot



    void Update()
    {
        int hElv = 0;
        switch (playerIndex)
        {
            case 1:
                //elevation = elevation;
                hElv = so.highestElv;
                break;
            case 2:
                hElv = w.highestElv;
                elevation = w.elevation;
                break;
            case 3:
                hElv = cR.highestElv;
                elevation = cR.elevation;
                break;
        }

        if (playerIndex == 2)
        {
            Vector3Int t = ground.WorldToCell(transform.position);
            currentCell = new Vector3Int(t.x - w.highestElv, t.y - w.highestElv, t.z);
            currentCell = t;
        }
        else
        {
            Vector3Int t = ground.WorldToCell(transform.position);
            currentCell = t;

        }
        Vector3Int te = ground.WorldToCell(transform.position);
        //currentCell = new Vector3Int(te.x -elevation + 1, te.y - -elevation + 1, te.z);


        // Move player to moving point TODO: calculate delta time to make undepeding of fps
        if (moving && controllBool)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, movingPoint.transform.position, 1* Time.deltaTime);
        }

        if (isMoving && Vector3.Distance(this.transform.position, movingPoint.transform.position) > 0.01f)
        {
            soundPlayer.clip = (sounds[playerIndex - 1][0]);
            if (!soundPlayer.isPlaying) soundPlayer.Play();
            idling = false;
        }

        if (Vector3.Distance(this.transform.position, movingPoint.transform.position) < 0.01f)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;

        }

        if (!isMoving)
        {
            if (!idling)
            {
                this.currentAnimator.SetTrigger("Idle");
                idling = true;

                if (soundPlayer.clip == (sounds[playerIndex - 1][0])) soundPlayer.Stop();
            }
            
        }


        // Calculate new position for player to move to depeding on input
        Vector3 newPosMP = this.transform.position;
        if (Input.GetKeyDown(playerControlls.forward))
        {
            newPosMP = Move(directionFacing);
        }
        if (Input.GetKeyDown(playerControlls.backward))
        {
            newPosMP = Move(incDir(incDir(directionFacing)));
        }
        if (Input.GetKeyDown(playerControlls.left))
        {
            directionFacing = incDir(directionFacing);
        }
        if (Input.GetKeyDown(playerControlls.right))
        {
            directionFacing = decDir(directionFacing);
        }

        if (playerIndex == 3)
        {
            if (Input.GetKeyDown(playerControlls.ability1))
            {
                cR.shootSeedBomb();
            }
        }

        // Check if new position is valid and if so move moving point there
        //bool moveallowed = !collisionsInGrid.Contains(ground.WorldToCell(newPosMP));
        bool moveallowed;
        //Debug.Log((elevation + 1, collisionList.Count));
        /*
        if (elevation + 1 < collisionList.Count)
        {
            moveallowed = !collisionList[elevation + 1].Contains(ground.WorldToCell(newPosMP));
            //Debug.Log(ground.WorldToCell(newPosMP));
        }
        else
        {
            moveallowed = true;
        }*/

        Vector3Int checkAgainst = new Vector3Int();
        if (playerIndex == 2)
        {
            Vector3Int t = ground.WorldToCell(newPosMP);
            checkAgainst = new Vector3Int(t.x + w.highestElv, t.y + w.highestElv, t.z);
        }
        else if (playerIndex == 1)
        {
            checkAgainst = ground.WorldToCell(newPosMP);
            Vector3Int t = ground.WorldToCell(newPosMP);
            checkAgainst = new Vector3Int(t.x + so.highestElv, t.y + so.highestElv, t.z);
            //checkAgainst = new Vector3Int(t.x + elevation + 1, t.y + elevation + 1, t.z);
        }
        else if (playerIndex == 3)
        {
            Vector3Int t = ground.WorldToCell(newPosMP);
            checkAgainst = new Vector3Int(t.x + cR.highestElv, t.y + cR.highestElv, t.z);
        }

        if (Input.GetKeyDown(playerControlls.forward))
        {
            //Debug.Log(checkAgainst);
            //Debug.Log(gM.getHighestElevation(checkAgainst));
        }
        if (Input.GetKeyDown(playerControlls.backward))
        {
            //Debug.Log(checkAgainst);
            //Debug.Log(gM.getHighestElevation(checkAgainst));
        }

        Vector3Int hhh = ground.WorldToCell(newPosMP);
        hhh = new Vector3Int(hhh.x - 1, hhh.y - 1, hhh.z);
        //Debug.Log((this.name, gM.getHighestElevation(ground.WorldToCell(newPosMP))));
        if (gM.getHighestElevation(checkAgainst) < elevation || (gM.getHighestElevation(ground.WorldToCell(newPosMP)) == 0 && elevation == 0))
        {
            moveallowed = true;
            if (playerIndex == 1 && !gM.tilesList[0].Contains(hhh))
            {
                moveallowed = false;
                blockage = 0;
            }
        }
        else
        {
            moveallowed = false;
            blockage = 1;
        }
        //bool changeAllowed = collisionsInGrid.Contains(currentCell);
        bool changeAllowed = false;

        if ((playerIndex == 2) && !moveallowed)
        {
            if (w.elevation >= 1)
            {
                //moveallowed = true;
            }
            if (elevationsInGrid.Contains(ground.WorldToCell(newPosMP)))
            {
                if (!(w.elevation >= 2))
                {
                    //moveallowed = false;
                }
            }
        }

        if ((playerIndex == 3) && !moveallowed)
        {
            if (cR.elevation >= 1)
            {
                //moveallowed = true;
            }
            if (elevationsInGrid.Contains(ground.WorldToCell(newPosMP)))
            {
                if (!(cR.elevation >= 2))
                {
                    //moveallowed = false;
                }
            }
        }

        if (moveallowed)
        {
            if (gM.isBoxOnCell(ground.WorldToCell(newPosMP), ground) && gM.getBoxSOnCell(ground.WorldToCell(newPosMP), ground).saveTile == null)
            {
                if (playerIndex == 2)
                {
                    if (gM.getBoxSOnCell(ground.WorldToCell(newPosMP), ground).elevation < w.elevation)
                    {
                        if (w.carryingBox)
                        {
                            if (! (gM.getBoxSOnCell(ground.WorldToCell(newPosMP), ground).elevation < w.elevation - 1))
                            {
                                //Debug.Log("BOX BLOCKT");
                                moveallowed = false;
                                blockage = 2;
                            }
                        }
                    }
                    else
                    {
                        moveallowed = false;
                        blockage = 3;
                    }

                }
                else
                {
                    moveallowed = false;
                    blockage = 4;
                }
                
            }

        }
        // Only Allow change of position if player is already at moving point or moving point is in an invalid position
        blockDist = Vector3.Distance(this.transform.position, movingPoint.transform.position);
        moAll = moveallowed;
        tra = this.transform.position;
        newP = newPosMP;

        //Debug.Log((tra != newP));

        if (!moveallowed && playerIndex == 1)
        {
            if (p1Exceptions.Contains(hhh))
            {
                moveallowed = true;
            }
        }

        if ((Vector3.Distance(this.transform.position, movingPoint.transform.position) < 0.01f || changeAllowed) && moveallowed)
        {
            blockage = -1;
            movingPoint.transform.position = newPosMP;
            if (Input.GetKeyDown(playerControlls.forward))
            {
                if (directionFacing == 0 ||directionFacing == 1)
                {
                    backwardA.SetTrigger("Forward");
                }
                if (directionFacing == 2 ||directionFacing == 3)
                {
                    forwardA.SetTrigger("Forward");
                }
                
            }
            if (Input.GetKeyDown(playerControlls.backward))
            {
                if (directionFacing == 0 || directionFacing == 1)
                {
                    backwardA.SetTrigger("Backward");
                }
                if (directionFacing == 2 || directionFacing == 3)
                {
                    forwardA.SetTrigger("Backward");
                }
            }
        }

        setDirectionSprite(directionFacing);
    }

    public List<SpriteRenderer> getAllSR()
    {
        List<SpriteRenderer> srList = new List<SpriteRenderer>();

        foreach (SpriteRenderer sR in transform.GetComponentsInChildren<SpriteRenderer>(true))
        {
            srList.Add(sR);
        }
        return srList;
    }

    public void setDirectionSprite(int dir)
    {
        switch (dir)
        {
            case 0:
                backward.SetActive(true);
                currentAnimator = backwardA;
                currentFB = backward;
                forward.SetActive(false);
                backward.transform.localScale = new Vector3(-startScale.x, startScale.y, startScale.z);
                if (playerIndex == 2) w.currentBoneStartY = w.boneStartYBac;
                break;
            case 1:
                backward.SetActive(true);
                currentAnimator = backwardA;
                currentFB = backward;
                if (playerIndex == 2) w.currentBoneStartY = w.boneStartYBac;
                forward.SetActive(false);
                backward.transform.localScale = new Vector3(startScale.x, startScale.y, startScale.z);
                break;
            case 2:
                forward.SetActive(true);
                currentAnimator = forwardA;
                currentFB = forward;
                if (playerIndex == 2) w.currentBoneStartY = w.boneStartYFor;
                backward.SetActive(false);
                forward.transform.localScale = new Vector3(-startScale.x, startScale.y, startScale.z);
                break;
            case 3:
                forward.SetActive(true);
                currentAnimator = forwardA;
                currentFB = forward;
                if (playerIndex == 2) w.currentBoneStartY = w.boneStartYFor;
                backward.SetActive(false);
                forward.transform.localScale = new Vector3(startScale.x, startScale.y, startScale.z);
                break;
        }
        if (playerIndex == 2) currentMovingBone = currentFB.transform.GetChild(11).GetChild(1).gameObject;
    }
}

