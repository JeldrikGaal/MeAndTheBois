using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solar : MonoBehaviour
{
    public float energyLevel;
    public float startingEnergy;

    // Vine Mechanic variables
    public GameObject vineMovementPoint;
    public Vector3Int vineCell;
    public VineMovingPoint vineMovS;
    public int vineRange;
    bool vineExtended;
    public int highestElv;
    public GameManager gM;

    public bool movingPointATM;
    public Vector3 mPGoal;
    public LineRenderer lR;
    public List<Vector3> points2 = new List<Vector3>();

    MovementController movC;
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        energyLevel = startingEnergy;
        movC = this.transform.GetComponent<MovementController>();
        vineMovS = vineMovementPoint.GetComponent<VineMovingPoint>();
        vineRange = 4;
        lR = this.GetComponent<LineRenderer>();
    }

    

    void Update()
    {
        highestElv = gM.getHighestElevation(movC.currentCell);
        vineCell = movC.ground.WorldToCell(vineMovementPoint.transform.position);


        if (!vineExtended)
        {
            vineMovementPoint.transform.position = this.transform.position;
        }
        else
        {
            if (!movingPointATM)
            {
                mPGoal = positionVinePoint(movC.ground);
                movingPointATM = true;
            }
            else
            {
                vineMovementPoint.transform.position = Vector3.MoveTowards(vineMovementPoint.transform.position, mPGoal, Time.deltaTime * 2);
                if (vineMovS.currentBox != null || Vector3.Distance(vineMovementPoint.transform.position, mPGoal) < 0.01f)
                {
                    movingPointATM = false;
                }
            }
                
            showVine();
        }

        if (Input.GetKeyDown(movC.playerControlls.ability1))
        {
            TryVineMove();
        }

        points2 = new List<Vector3>();
        points2.Add(transform.position);
        points2.Add(vineMovementPoint.transform.position);
        DrawLine(points2);

    }

    void DrawLine(List<Vector3> points)
    {
        if (points == null)
        {
            return;
        }
        lR.positionCount = points.Count;
        Vector3[] temp = points.ToArray();
        lR.SetPositions(temp);
    }

    Vector3 positionVinePoint(Grid ground)
    {
        Vector3 temp = new Vector3();
        switch (movC.directionFacing)
        {
            case 0:
                temp = new Vector3(this.transform.position.x + ((ground.cellSize.x / 2) * vineRange + (ground.cellGap.x / 2)), this.transform.position.y + (ground.cellSize.y / 2) * vineRange, this.transform.position.z);
                break;
            case 1:
                temp = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2) * vineRange + (ground.cellGap.x / 2)), this.transform.position.y + (ground.cellSize.y / 2) * vineRange, this.transform.position.z);
                break;
            case 2:
                temp = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2) * vineRange + (ground.cellGap.x / 2)), this.transform.position.y - (ground.cellSize.y / 2) * vineRange, this.transform.position.z);
                break;
            case 3:
                temp = new Vector3(this.transform.position.x + ((ground.cellSize.x / 2) * vineRange + (ground.cellGap.x / 2)), this.transform.position.y - (ground.cellSize.y / 2) * vineRange, this.transform.position.z);
                break;
            default:
                break;
        }

        //vineMovementPoint.transform.position = temp;
        return temp;
    }

    void TryVineMove()
    {
        Vector3 newPosMP = this.transform.position;
        Grid ground = movC.ground;
        if (!vineExtended)
        {
            vineExtended = true;

            newPosMP = positionVinePoint(ground);
        }

        else
        {
            if (vineMovS.currentBox != null)
            {

                if (this.transform.position.x > vineMovS.currentBox.transform.position.x)
                {
                    if (this.transform.position.y > vineMovS.currentBox.transform.position.y)
                    {
                        Debug.Log("1");
                        newPosMP = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2)), this.transform.position.y - (ground.cellSize.y / 2), this.transform.position.z);
                        Vector3 help = ground.CellToWorld(new Vector3Int(movC.currentCell.x, movC.currentCell.y - 1, movC.currentCell.z));
                        newPosMP = help;
                    }
                    else
                    {
                        Debug.Log("2");
                        newPosMP = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2)), this.transform.position.y + (ground.cellSize.y / 2), this.transform.position.z);
                        Vector3 help = ground.CellToWorld(new Vector3Int(movC.currentCell.x, movC.currentCell.y + 1, movC.currentCell.z));
                        newPosMP = help;
                    }
                }
                else
                {
                    if (this.transform.position.y > vineMovS.currentBox.transform.position.y)
                    {
                        Debug.Log("3");
                        newPosMP = new Vector3(this.transform.position.x + ((ground.cellSize.x / 2)), this.transform.position.y - (ground.cellSize.y / 2), this.transform.position.z);
                        Vector3 help = ground.CellToWorld(new Vector3Int(movC.currentCell.x - 1, movC.currentCell.y, movC.currentCell.z));
                        newPosMP = help;
                    }
                    else
                    {
                        Debug.Log("4");
                        newPosMP = new Vector3(this.transform.position.x + ((ground.cellSize.x / 2)), this.transform.position.y + (ground.cellSize.y / 2), this.transform.position.z);
                        Vector3 help = ground.CellToWorld(new Vector3Int(movC.currentCell.x + 1, movC.currentCell.y, movC.currentCell.z));
                        Debug.Log(help);
                        newPosMP = help;
                    }
                }

                Vector3 temp = ground.GetCellCenterWorld(ground.WorldToCell(newPosMP));
                temp = newPosMP;
                if (vineMovS.currentBox.GetComponent<Box>())
                {
                    vineMovS.currentBox.GetComponent<Box>().destination = new Vector3(temp.x, temp.y, temp.z);
                    vineMovS.currentBox.GetComponent<Box>().moving = true;
                }
                if (vineMovS.currentBox.transform.GetChild(0).GetComponent<Mirror>())
                {
                    vineMovS.currentBox.transform.GetChild(0).GetComponent<Mirror>().destination = new Vector3(temp.x, temp.y, temp.z);
                    vineMovS.currentBox.transform.GetChild(0).GetComponent<Mirror>().moving = true;
                }

                //vineMovS.currentBox.GetComponent<Box>().destination = temp; 

            }
            else
            {
                vineExtended = false;
            }
        }



    }

    void showVine()
    {
        if (movC.gM.isBoxOnCell(vineCell, movC.ground))
        {
            //Debug.DrawLine(this.transform.position, this.vineMovementPoint.transform.position, Color.green);
        }

        Debug.DrawLine(this.transform.position, this.vineMovementPoint.transform.position, Color.green);
    }
}
