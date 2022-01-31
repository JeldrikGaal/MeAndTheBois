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

    MovementController movC;
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        energyLevel = startingEnergy;
        movC = this.transform.GetComponent<MovementController>();
        vineMovS = vineMovementPoint.GetComponent<VineMovingPoint>();
        vineRange = 3;
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
            positionVinePoint(movC.ground);
            showVine();
        }

        if (Input.GetKeyDown(movC.playerControlls.ability1))
        {
            TryVineMove();
        }

        
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

        vineMovementPoint.transform.position = temp;
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
                        newPosMP = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2)), this.transform.position.y - (ground.cellSize.y / 2), this.transform.position.z);
                    }
                    else
                    {
                        newPosMP = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2)), this.transform.position.y + (ground.cellSize.y / 2), this.transform.position.z);
                    }
                }
                else
                {
                    if (this.transform.position.y > vineMovS.currentBox.transform.position.y)
                    {
                        newPosMP = new Vector3(this.transform.position.x + ((ground.cellSize.x / 2)), this.transform.position.y - (ground.cellSize.y / 2), this.transform.position.z);
                    }
                    else
                    {
                        newPosMP = new Vector3(this.transform.position.x + ((ground.cellSize.x / 2)), this.transform.position.y + (ground.cellSize.y / 2), this.transform.position.z);
                    }
                }

                Vector3 temp = ground.GetCellCenterWorld(ground.WorldToCell(newPosMP));
                vineMovS.currentBox.GetComponent<Box>().destination = new Vector3(temp.x, temp.y - (ground.cellSize.y * 0.72f), temp.z);
                //vineMovS.currentBox.GetComponent<Box>().destination = temp; 
                vineMovS.currentBox.GetComponent<Box>().moving = true;
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
