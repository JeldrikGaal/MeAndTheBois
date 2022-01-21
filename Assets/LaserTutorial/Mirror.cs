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
    public bool  beingHit;
    private int counter;

    public bool needsChargeToRotate;
    public int angleToCharge;
    public float chargeTimer;
    private bool hitCalced;

    private void Awake()
    {
        
        // Save variables needed for ray placment
        midPoint = ground.GetCellCenterWorld(ground.WorldToCell(transform.position));
        midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));

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
    } 


    void Start()
    {
        timerSave = timer;
        sR.sprite = sprites[angle];
        Flip();
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
        if (counter == 0)
        {
            counter = 5;
            beingHit = false;
            angleReceived = -1;
        }



        // Keep current cell updated
        currentCell = ground.WorldToCell(this.transform.position);

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
        else
        {
            playerNear = false;
            playerNearInt = 0;
        }


        // Let the mirror turn if a player is nearby
        if (Input.GetKeyDown(KeyCode.Alpha8) && playerNear)
        {
            TurnLeft();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) && playerNear)
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

        
    }

    bool checkAdjacentCells(MovementController p)
    {
        if (p.currentCell == new Vector3Int(currentCell.x - 1, currentCell.y, currentCell.z)) return true;
        if (p.currentCell == new Vector3Int(currentCell.x + 1, currentCell.y, currentCell.z)) return true;
        if (p.currentCell == new Vector3Int(currentCell.x, currentCell.y - 1, currentCell.z)) return true;
        if (p.currentCell == new Vector3Int(currentCell.x, currentCell.y + 1, currentCell.z)) return true;
        return false;
    }

    void placeDirectionPoint()
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
        startingPos = new Vector2(reflectionPoint.position.x, reflectionPoint.position.y);
        refStartCell = ground.WorldToCell(startingPos);
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
