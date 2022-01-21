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
    public int angle;
    public bool portable;
    public float yOffset;
    public bool timed;
    public float timer;
    private float timerSave;

    // Various Variables other scripts may need to access
    public Vector3 midPoint;
    public Transform reflectionPoint;
    public Vector2 startingPos;
    public bool startingMir;
    public bool beingCarried;
    public bool playerNear;
    public int playerNearInt;
    public Vector3Int refStartCell;
    public Vector3Int currentCell;
    public SpriteRenderer sR;

    private void Awake()
    {
        reflectionPoint = this.transform.GetChild(0);

        // Initial placment of reflection point
        placeDirectionPoint();
        Vector3 temp = ground.GetCellCenterWorld(ground.WorldToCell(reflectionPoint.position));
        reflectionPoint.transform.position = new Vector3(temp.x, temp.y - (ground.cellSize.y * 0.25f) + yOffset, temp.z);
        refStartCell = ground.WorldToCell(reflectionPoint.transform.position);

        // Save variables needed for ray placment
        startingPos = new Vector2(reflectionPoint.position.x, reflectionPoint.position.y);
        midPoint = ground.GetCellCenterWorld(ground.WorldToCell(transform.position));
        midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));

        // Initialize sprite List
        sprites.Add(zero);
        sprites.Add(one);
        sprites.Add(two);
        sprites.Add(three);
        
        // Initialize SpriteRenderer reference
        sR = GetComponentInChildren<SpriteRenderer>();
    } 


    void Start()
    {
        timerSave = timer;
        sR.sprite = sprites[angle];
    }

    // Update is called once per frame
    void Update()
    {
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
