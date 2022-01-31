using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    // Need to be dragged into script
    public GameManager gM;
    public Grid ground;

    // Sprites (need to be public to easily change in prefab)
    public Sprite zero;
    public Sprite one;
    public Sprite two;
    public Sprite three;
    private List<Sprite> sprites = new List<Sprite>();

    // Setting in script
    public int    angle;
    public bool   portable;
    public float  yOffset;
    public bool   timed;
    public float  timer;
    private float timerSave;

    // Various Variables other scripts may need to access
    public Vector3Int refStartCell;
    public Vector3Int currentCell;
    public Vector3 midPoint;
    public Vector2 startingPos;
    public bool startingMir;
    public bool beingCarried;
    public bool playerNear;
    public int playerNearInt;
    public Transform reflectionPoint;
    public SpriteRenderer sR;

    // Store the angle if a ray is hitting this mirror
    public float angleReceived;
    public float angleReceivedStable;
    public float angleReceivedStableHelp;
    public bool  beingHit;
    private int counter;

    public bool needsChargeToRotate;
    public int angleToCharge;
    public float chargeTimer;
    private bool hitCalced;

    public EdgeCollider2D coll;
    public Vector2 collOffset;
    public int elevation;
    public int newElv;

    public GameObject parentG;
    public GameObject mirrorG;
    public SpriteRenderer mirrorSr;
    public GameObject laserG;
    public bool gravityAffected;
    public bool DebugTurning;

    public bool moving;
    public Vector3 destination;

    private void Awake()
    {

        ground = GameObject.Find("Grid").GetComponent<Grid>();
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        coll = this.GetComponent<EdgeCollider2D>();

        // Save variables needed for ray placment
        midPoint = ground.GetCellCenterWorld(ground.WorldToCell(transform.position));
        midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));
        this.transform.position = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.49f), midPoint.z);

        // Initial placment of reflection point
        reflectionPoint = this.transform.GetChild(0);
        Vector3 temp = ground.GetCellCenterWorld(ground.WorldToCell(reflectionPoint.position));
        reflectionPoint.transform.position = new Vector3(temp.x, temp.y - (ground.cellSize.y * 0.25f) + yOffset, temp.z);
        placeDirectionPoint();
        refStartCell = ground.WorldToCell(reflectionPoint.transform.position);
        startingPos = new Vector2(reflectionPoint.position.x, reflectionPoint.position.y);
        
        // Initialize sprite List
        sprites.Add(zero);
        sprites.Add(one);
        sprites.Add(two);
        sprites.Add(three);
        
        // Initialize SpriteRenderer reference
        sR = GetComponentInChildren<SpriteRenderer>(); 
       
        // Where does the ray come from variables
        counter = 5;

        if (startingMir)
        {
            parentG = this.transform.parent.gameObject;
            mirrorG = parentG.transform.GetChild(0).gameObject;
            laserG = parentG.transform.GetChild(1).gameObject;
            mirrorSr = mirrorG.GetComponent<SpriteRenderer>();
        }
        else
        {
            parentG = this.transform.parent.gameObject; ;
            laserG = null;
            mirrorG = this.gameObject;
            mirrorSr = this.transform.GetChild(1).GetComponent<SpriteRenderer>();
            
        }



    }



    void Start()
    {
        timerSave = timer;
        sR.sprite = sprites[angle];
        Flip();
        collOffset = coll.bounds.center - midPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (needsChargeToRotate)
        {
            if (beingHit)
            {
                if (angleReceived == angleToCharge)
                {
                    if (!hitCalced)
                    {
                        timed = true;
                        timer = chargeTimer;
                        timerSave = chargeTimer;
                        hitCalced = true;
                    }
                }
            }
            else
            {
                if (!gM.hitMirrosStable.Contains(this))
                {
                    hitCalced = false;
                    timed = false;
                }

            }
        }

        // Reset every 5 frames --> no other way found to reset those variables
        counter -= 1;
        if (counter == 5)
        {
            if (angleReceived != angleReceivedStableHelp)
            {
                angleReceivedStableHelp = angleReceived;
            }
        }
        if (counter == 0)
        {
            angleReceivedStableHelp = angleReceived;
            counter = 5;
            
            angleReceived = -1;
        }
        if (angleReceivedStable != -1)
        {
            angleReceivedStable = correctAngleRec(angleReceivedStableHelp);
            beingHit = true;
        }
        else
        {
            beingHit = false;
        }



        // Keep current cell updated
        if (startingMir)
        {
            currentCell = ground.WorldToCell(parentG.transform.position);
        }
        else
        {
            currentCell = ground.WorldToCell(parentG.transform.position);
        }


        // check if one of the players is adjacent
        if (checkAdjacentCells(gM.p1))
        {
            playerNear = true;
            playerNearInt = 1;
        }
        else if (checkAdjacentCells(gM.p2))
        {
            playerNear = true;
            playerNearInt = 2;
        }
        else if (checkAdjacentCells(gM.p3))
        {
            playerNear = true;
            playerNearInt = 3;
        }
        else
        {
            playerNear = false;
            playerNearInt = 0;
        }


        // Let the mirror turn if a player is nearby
        if (Input.GetKeyDown(KeyCode.Space) && playerNear && playerNearInt == 2)
        {
            TurnLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightShift) && playerNear && (playerNearInt == 1 || playerNearInt == 3 ))
        {
            TurnRight();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) && DebugTurning)
        {
            TurnLeft();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) && DebugTurning)
        {
            TurnRight();
        }

        // Handle logic of a timed mirror turning
        if (timed && !beingCarried)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                TurnLeft();
                timer = timerSave;
            }

        }

        // Handle logic of player carrying mirror
        if (beingCarried)
        {
            midPoint = ground.GetCellCenterWorld(ground.WorldToCell(transform.position));
            midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));
            placeDirectionPoint();
        }
        else
        {
            if (gravityAffected)
            {
                newElv = gM.getHighestElevation(currentCell, this.gameObject);
                if (newElv > elevation)
                {
                    int diff = newElv - elevation;
                    elevation = newElv;
                    mirrorG.transform.position = new Vector3(mirrorG.transform.position.x, mirrorG.transform.position.y + diff * ground.cellSize.y, mirrorG.transform.position.z);
                    if (startingMir)
                    {
                        laserG.transform.position = new Vector3(laserG.transform.position.x, laserG.transform.position.y + diff * ground.cellSize.y, laserG.transform.position.z);
                    }
                        

                }
                if (newElv < elevation)
                {
                    int diff = elevation - newElv;
                    elevation = newElv;
                    mirrorG.transform.position = new Vector3(mirrorG.transform.position.x, mirrorG.transform.position.y - diff * ground.cellSize.y, mirrorG.transform.position.z);
                    if (startingMir)
                    {
                        laserG.transform.position = new Vector3(laserG.transform.position.x, laserG.transform.position.y - diff * ground.cellSize.y, laserG.transform.position.z);
                    }

                }

                mirrorSr.sortingLayerID = gM.elevSL[elevation+1];
                mirrorSr.sortingOrder = 2;
                midPoint = ground.GetCellCenterWorld(ground.WorldToCell(transform.position));
                midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));
                Vector3 temp = ground.GetCellCenterWorld(ground.WorldToCell(reflectionPoint.position));
                reflectionPoint.transform.position = new Vector3(temp.x, temp.y - (ground.cellSize.y * 0.25f) + yOffset, temp.z);
                placeDirectionPoint();
                refStartCell = ground.WorldToCell(reflectionPoint.transform.position);
                startingPos = new Vector2(reflectionPoint.position.x, reflectionPoint.position.y);
            }
        }

        if (!startingMir && beingHit) 
        {
            placeDirectionPoint();
        }

        if (moving && portable)
        {
            this.transform.parent.position = Vector3.MoveTowards(this.transform.parent.position, destination, 3 * Time.deltaTime);
            midPoint = ground.GetCellCenterWorld(ground.WorldToCell(transform.position));
            midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));
            placeDirectionPoint();
            if (Vector3.Distance(this.transform.parent.position, destination) <= 0.01f)
            {
                moving = false;
            }
        }

        if (beingCarried) sR.sortingLayerID = SortingLayer.NameToID("Flying");
        if (beingCarried) sR.sortingOrder = 1;
    }

    bool checkAdjacentCells(MovementController p)
    {
        if (p.currentCell == new Vector3Int(currentCell.x - 1, currentCell.y, currentCell.z)) return true;
        if (p.currentCell == new Vector3Int(currentCell.x + 1, currentCell.y, currentCell.z)) return true;
        if (p.currentCell == new Vector3Int(currentCell.x, currentCell.y - 1, currentCell.z)) return true;
        if (p.currentCell == new Vector3Int(currentCell.x, currentCell.y + 1, currentCell.z)) return true;
        return false;
    }

    public void placeDirectionPoint()
    {
        if (startingMir)
        {
            switch (angle)
            {
                case 0:
                    reflectionPoint.position = new Vector3(midPoint.x + ground.cellSize.x * 0.5f, midPoint.y - ground.cellSize.y * 0.5f);
                    break;
                case 1:
                    reflectionPoint.position = new Vector3(midPoint.x + ground.cellSize.x * 0.5f, midPoint.y + ground.cellSize.y * 0.5f);
                    break;
                case 2:
                    reflectionPoint.position = new Vector3(midPoint.x - ground.cellSize.x * 0.5f, midPoint.y + ground.cellSize.y * 0.5f);
                    break;
                case 3:
                    reflectionPoint.position = new Vector3(midPoint.x - ground.cellSize.x * 0.5f, midPoint.y - ground.cellSize.y * 0.5f);
                    break;
            }
            Debug.DrawLine(midPoint, reflectionPoint.position, Color.black);
            startingPos = new Vector2(reflectionPoint.position.x, reflectionPoint.position.y);
            refStartCell = ground.WorldToCell(startingPos);
        }
        else
        {
           // Debug.Log("SSIO");

            float usedAngle = -1;
            switch (angle)
            {
                case 0:
                    switch(angleReceivedStable)
                    {
                        case 1:
                            usedAngle = 1;
                            break;
                        case 2:
                            usedAngle = 4;
                            break;
                        case 3:
                            usedAngle = -1;
                            break;
                        case 4:
                            usedAngle = -1;
                            break;
                    }       
                    break;
                case 1:
                    switch (angleReceivedStable)
                    {
                        case 1:
                            usedAngle = -1;
                            break;
                        case 2:
                            usedAngle = -1;
                            break;
                        case 3:
                            usedAngle = 3;
                            break;
                        case 4:
                            usedAngle = 2;
                            break;
                    }
                    break;
                case 2:
                    switch (angleReceivedStable)
                    {
                        case 1:
                            usedAngle = -1;
                            break;
                        case 2:
                            usedAngle = 2;
                            break;
                        case 3:
                            usedAngle = 1;
                            break;
                        case 4:
                            usedAngle = -1;
                            break;
                    }
                    break;
                case 3:
                    switch (angleReceivedStable)
                    {
                        case 1:
                            usedAngle = 3;
                            break;
                        case 2:
                            usedAngle = -1;
                            break;
                        case 3:
                            usedAngle = -1;
                            break;
                        case 4:
                            usedAngle = 4;
                            break;
                    }
                    break;

            }

            switch (usedAngle)
            {
                case 1:
                    reflectionPoint.position = new Vector3(midPoint.x + ground.cellSize.x * 0.5f, midPoint.y - ground.cellSize.y * 0.5f);
                    break;
                case 2:
                    reflectionPoint.position = new Vector3(midPoint.x + ground.cellSize.x * 0.5f, midPoint.y + ground.cellSize.y * 0.5f);
                    break;
                case 3:
                    reflectionPoint.position = new Vector3(midPoint.x - ground.cellSize.x * 0.5f, midPoint.y + ground.cellSize.y * 0.5f);
                    break;
                case 4:
                    reflectionPoint.position = new Vector3(midPoint.x - ground.cellSize.x * 0.5f, midPoint.y - ground.cellSize.y * 0.5f);
                    break;
                case -1:
                    reflectionPoint.position = this.transform.position;
                    break;
            }
            Debug.DrawLine(midPoint, reflectionPoint.position, Color.black);
            startingPos = new Vector2(reflectionPoint.position.x, reflectionPoint.position.y);
            refStartCell = ground.WorldToCell(startingPos);
        }
        
    }

    float correctAngleRec(float a)
    {
        switch (a)
        {
            case 0:
                return 3;
            case 1:
                return 1;
            case 2:
                return 4;
            case 3:
                return 2;
        }
        return 0;
    }

    float ShiftAngleLeft(float a)
    {
        if (a == 1)
        {
            a = 4;
        }
        else
        {
            a -= 1;
        }
        return a;
    }
    float ShiftAngleRight(float a)
    {
        if (a == 4)
        {
            a = 1;
        }
        else
        {
            a += 1;
        }
        return a;
    }

    // Flip the Mirrorsprite for the starting mirror type 
    void Flip()
    {
        if (!startingMir) return; 
        if (angle == 0 || angle == 2)
        {
            sR.flipX = true;
        }
        else
        {
            sR.flipX = false;
        }
    }

    // Turn refelection point one step to the left and update sprite
    void TurnLeft()
    {
        
        if (angle == 0)
        {
            angle = 3;
        }
        else
        {
            angle -= 1;
        }
        placeDirectionPoint();
        sR.sprite = sprites[angle];
        Flip();
        
    }

    // Turn refelection point one step to the right and update sprite
    void TurnRight()
    {
        if (angle == 3)
        {
            angle = 0;
        }
        else
        {
            angle += 1;
        }
        placeDirectionPoint();
        sR.sprite = sprites[angle];
        Flip();
    }
}
