using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{

    public int angle;

    public Transform reflectionPoint;
    public Vector2 startingPos;
    public Grid ground;
    public Vector3 midPoint;

    Transform laserBox;

    public Sprite zero;
    public Sprite one;
    public Sprite two;
    public Sprite three;

    public List<Sprite> sprites = new List<Sprite>();

    public Vector3Int currentCell;

    public GameManager gM;

    public bool timed;
    public float timer;
    private float timerSave;

    public bool portable;

    public bool playerNear;
    public int playerNearInt;

    public float yOffset;

    public Vector3Int refStartCell;
    public bool startingMir;

    public bool beingCarried;

    public SpriteRenderer sR;

    private void Awake()
    {
        reflectionPoint = this.transform.GetChild(0);

        Vector3 temp = ground.GetCellCenterWorld(ground.WorldToCell(reflectionPoint.position));
        reflectionPoint.transform.position = new Vector3(temp.x, temp.y - (ground.cellSize.y * 0.25f) + yOffset, temp.z);

        startingPos = new Vector2(reflectionPoint.position.x, reflectionPoint.position.y);

        laserBox = this.transform.parent;

        midPoint = ground.GetCellCenterWorld(ground.WorldToCell(transform.position));
        midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));

        sprites.Add(zero);
        sprites.Add(one);
        sprites.Add(two);
        sprites.Add(three);

        refStartCell = ground.WorldToCell(reflectionPoint.transform.position);

        sR = GetComponentInChildren<SpriteRenderer>();
    } 
    // Start is called before the first frame update
    void Start()
    {
        timerSave = timer;
        placeDirectionPoint();
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
