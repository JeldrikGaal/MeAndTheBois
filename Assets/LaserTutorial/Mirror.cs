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

    private void Awake()
    {
        reflectionPoint = this.transform.GetChild(0);

        Vector3 temp = ground.GetCellCenterWorld(ground.WorldToCell(reflectionPoint.position));
        reflectionPoint.transform.position = new Vector3(temp.x, temp.y - (ground.cellSize.y * 0.25f), temp.z);

        startingPos = new Vector2(reflectionPoint.position.x, reflectionPoint.position.y);

        laserBox = this.transform.parent;

        midPoint = ground.GetCellCenterWorld(ground.WorldToCell(transform.position));
        midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));

        sprites.Add(zero);
        sprites.Add(one);
        sprites.Add(two);
        sprites.Add(three);
    }
    // Start is called before the first frame update
    void Start()
    {
        timerSave = timer;
    }

    // Update is called once per frame
    void Update()
    {
        currentCell = ground.WorldToCell(this.transform.position);

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

        if (Input.GetKeyDown(KeyCode.Alpha8) && playerNear)
        {
            TurnLeft();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) && playerNear)
        {
            TurnRight();
        }

        if (timed)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                TurnLeft();
                timer = timerSave;
            }

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
        this.GetComponent<SpriteRenderer>().sprite = sprites[angle];
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
        this.GetComponent<SpriteRenderer>().sprite = sprites[angle];
    }
}
