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

    MovementController movC;
    void Start()
    {
        energyLevel = startingEnergy;
        movC = this.transform.GetComponent<MovementController>();
        vineMovS = vineMovementPoint.GetComponent<VineMovingPoint>();
        vineRange = 3;
    }

    

    void Update()
    {
        vineCell = movC.ground.WorldToCell(vineMovementPoint.transform.position);


        if (!vineExtended)
        {
            vineMovementPoint.transform.position = this.transform.position;
        }
        else
        {
            Debug.DrawLine(this.transform.position, this.vineMovementPoint.transform.position, Color.green);
        }

        if (Input.GetKeyDown(movC.playerControlls.ability1))
        {
            TryVineMove();
        }

        
    }

    void TryVineMove()
    {
        if (!vineMovS.currentBox)
        {
            showVine();
            //return;
        }
        Vector3 newPosMP = this.transform.position;
        Grid ground = movC.ground;
        if (!vineExtended)
        {
            vineExtended = true;
            
            switch (movC.directionFacing)
            {
                case 0:
                    newPosMP = new Vector3(this.transform.position.x + ((ground.cellSize.x / 2) * vineRange + (ground.cellGap.x / 2)), this.transform.position.y + (ground.cellSize.y / 2) * vineRange, this.transform.position.z);
                    break;
                case 1:
                    newPosMP = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2) * vineRange + (ground.cellGap.x / 2)), this.transform.position.y - (ground.cellSize.y / 2) * vineRange, this.transform.position.z);
                    break;
                case 2:
                    newPosMP = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2) * vineRange + (ground.cellGap.x / 2)), this.transform.position.y + (ground.cellSize.y / 2) * vineRange, this.transform.position.z);
                    break;
                case 3:
                    newPosMP = new Vector3(this.transform.position.x + ((ground.cellSize.x / 2) * vineRange + (ground.cellGap.x / 2)), this.transform.position.y - (ground.cellSize.y / 2) * vineRange, this.transform.position.z);
                    break;
                default:
                    break;
            }

            vineMovementPoint.transform.position = newPosMP;
        }

        else
        {
            if (vineMovS.currentBox != null)
            {

                if (this.transform.position.x > vineMovS.currentBox.transform.position.x)
                {
                    if (this.transform.position.y > vineMovS.currentBox.transform.position.y)
                    {
                        newPosMP = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2) + (ground.cellGap.x / 2)), this.transform.position.y - (ground.cellSize.y / 2), this.transform.position.z);
                    }
                    else
                    {
                        newPosMP = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2) + (ground.cellGap.x / 2)), this.transform.position.y + (ground.cellSize.y / 2), this.transform.position.z);
                    }
                }
                else
                {
                    if (this.transform.position.y > vineMovS.currentBox.transform.position.y)
                    {
                        newPosMP = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2) + (ground.cellGap.x / 2)), this.transform.position.y + (ground.cellSize.y / 2), this.transform.position.z);
                    }
                    else
                    {
                        newPosMP = new Vector3(this.transform.position.x + ((ground.cellSize.x / 2) + (ground.cellGap.x / 2)), this.transform.position.y + (ground.cellSize.y / 2), this.transform.position.z);
                    }
                }

                vineMovS.currentBox.GetComponent<Box>().destination = newPosMP;
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

    }
}
