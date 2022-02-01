using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Wind : MonoBehaviour
{

    public SpriteRenderer shadow;
    public GameObject robotSprite;
    public SpriteRenderer robotSpriteR;
    public GameManager gM;
    public MovementController movC;
    public int elevation = 1;


    public float shadowStartSize;
    public float spriteOffSet;


    public bool carryingBox;
    private GameObject box;
    private Box boxS;

    public bool carryingMirror;
    private GameObject mir;
    private Mirror mirS;

    Vector3 offset;

    public int highestElv;
    public int highestElv2;

    public int maxElevation;

    public float boneStartYFor;
    public float boneStartYBac;
    public float currentBoneStartY;



    // Start is called before the first frame update
    void Start()
    {
        shadow = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        //robotSprite = this.transform.GetChild(0).gameObject;
        //robotSpriteR = robotSprite.GetComponent<SpriteRenderer>();
        movC = this.transform.GetComponent<MovementController>();

        //robotSprite.transform.localPosition = new Vector3(0, movC.ground.cellSize.y * elevation, 0);
        shadowStartSize = shadow.size.x - 0.05f;
        spriteOffSet = 0f;
        box = null;

        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        maxElevation = 7;
        
    }

    // Update is called once per frame
    void Update()
    {
        robotSprite = movC.currentFB;
        highestElv = gM.getHighestElevation(movC.currentCell, box);
        Vector3Int t = movC.currentCell;

        switch (movC.directionFacing)
        {
            case 0:
                t = new Vector3Int(t.x + highestElv, t.y, t.z);
                break;
            case 1:
                t = new Vector3Int(t.x, t.y + highestElv, t.z);
                break;
            case 2:
                t = new Vector3Int(t.x - highestElv, t.y, t.z);
                break;
            case 3:
                t = new Vector3Int(t.x, t.y - highestElv, t.z);
                break;
        }

        t = movC.currentCell;
        t = new Vector3Int(t.x + highestElv, t.y + highestElv, t.z);
        highestElv2 = gM.getHighestElevation(t, box);
        
        // Controll up and down movement / elevation of player
        robotSprite.transform.localPosition = new Vector3(0, movC.ground.cellSize.y * elevation, 0);
        if (Input.GetKeyDown(movC.playerControlls.up) && elevation < maxElevation)
        {
            elevation += 1;
        }
        if (Input.GetKeyDown(movC.playerControlls.down))
        {
            if (elevation > 0 && elevation > gM.getHighestElevation(movC.currentCell))
            {
                elevation -= 1;
            }
        }


        // Regulate shadow size according to elevation
        shadow.size = new Vector2(shadowStartSize - elevation * 0.02f, shadowStartSize - elevation * 0.02f); // REDO
        if (gM.getHighestElevation(movC.currentCell) != -1)
        {
            if (highestElv > 0 && highestElv2 >= highestElv)
            {
                shadow.transform.localPosition = new Vector3(0, highestElv2 * movC.ground.cellSize.y, 0);
            }
            else
            {
                shadow.transform.localPosition = new Vector3(0, highestElv * movC.ground.cellSize.y, 0);
            }
           
            
            //shadow.transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            shadow.transform.localPosition = new Vector3(0, 0, 0);
        }
       

        // Check if box is being carried or can be picked up
        if (Input.GetKeyDown(movC.playerControlls.ability1))
        {
            Vector3Int helpV = new Vector3Int(movC.currentCell.x - 1, movC.currentCell.y - 1, movC.currentCell.z);
            if (gM.isBoxOnCell(helpV, movC.ground))
            {
                box = gM.getBoxOnCell(helpV, movC.ground);
                boxS = box.GetComponent<Box>();
                if (boxS.saveTile == null) helpV = movC.currentCell;
            }
            else if (gM.isBoxOnCell(movC.currentCell, movC.ground))
            {
                helpV = movC.currentCell;
            }

            // Putting Box back on the ground
            if (carryingBox )
            {
                Vector3 temp = movC.ground.GetCellCenterWorld(movC.ground.WorldToCell(this.transform.position));
                box.transform.position = new Vector3(temp.x, temp.y - movC.ground.cellSize.y * 0.75f, 0);
                boxS.boxSprite.transform.localPosition = new Vector3(0, 0, 0);
                boxS.beingcarried = false;
                box = null;
                boxS = null;
                carryingBox = false;
                offset = new Vector3();
            }

            // Picking Box Up
            
            else if (gM.isBoxOnCell(helpV, movC.ground) && !carryingBox && !carryingMirror)
            {
                box = gM.getBoxOnCell(helpV, movC.ground);
                boxS = box.GetComponent<Box>();
                
                if (this.elevation > boxS.elevation)
                {
                    if (boxS.saveTile != null)
                    {
                       
                        gM.waterMap.SetTile(helpV, boxS.saveTile);
                        gM.boxPlacingMap.SetTile(helpV, null);
                        gM.tilesList[0].Remove(helpV);
                        boxS.saveTile = null;
                        boxS.boxSprite.enabled = true;
                    }
                    carryingBox = true;
                    boxS.beingcarried = true;
                }
               
            }

            // Setting Mirror down
            else if (carryingMirror)
            {
                Vector3 temp = movC.ground.GetCellCenterWorld(movC.ground.WorldToCell(this.transform.position));
                mir.transform.position = new Vector3(temp.x, temp.y - movC.ground.cellSize.y * 0.75f, 0);
                mirS.beingCarried = false;
                mirS.sR.transform.localPosition = new Vector3(0, 0, 0);
                mirS.laserG.SetActive(true);
                mirS.placeDirectionPoint();
                mir = null;
                mirS = null;
                carryingMirror = false;
                offset = new Vector3();
                

            }

            // Picking Mirror up
            else if (gM.isMirrorOnCell(movC.currentCell, movC.ground) && !carryingMirror && !carryingBox)
            {

                mir = gM.getMirOnCell(movC.currentCell, movC.ground);
                mirS = mir.GetComponent<Mirror>();
                mir = mir.transform.parent.gameObject;
                mirS.laserG.SetActive(false);
                if (this.elevation > mirS.elevation)
                {
                    mirS.beingCarried = true;
                    carryingMirror = true;
                }
            }

        }

        if (carryingBox && box)
        {
          
            box.transform.position = new Vector3(this.movC.currentFB.transform.position.x, this.movC.currentFB.transform.position.y -boxS.boxSprite.bounds.size.y * 0.75f, this.movC.currentFB.transform.position.z);
            boxS.boxSprite.transform.localPosition = new Vector3(0, 0, 0);
            boxS.boxSprite.transform.position = new Vector3(boxS.boxSprite.transform.position.x , boxS.boxSprite.transform.position.y + ((movC.currentFB.transform.GetChild(11).TransformVector(movC.currentMovingBone.transform.localPosition).y - currentBoneStartY)), boxS.boxSprite.transform.position.z);
            boxS.elevation = this.elevation - 1;
           
        }

        else if (carryingMirror && mir)
        {
            Debug.Log(mir.name);
            mir.transform.position = new Vector3(this.movC.currentFB.transform.position.x, this.movC.currentFB.transform.position.y - mirS.sR.sprite.bounds.size.y * 0.75f, this.movC.currentFB.transform.position.z);
            mirS.sR.transform.localPosition = new Vector3(0, 0, 0);
            mirS.sR.transform.position = new Vector3(mirS.sR.transform.position.x, mirS.sR.transform.position.y + ((movC.currentFB.transform.GetChild(11).TransformVector(movC.currentMovingBone.transform.localPosition).y - currentBoneStartY)), mirS.sR.transform.position.z);
            mirS.elevation = this.elevation - 1;
        }

        // Shadow sorting layer
        shadow.sortingOrder = 5;
        if (! (elevation - 1 < 0))
        {
            shadow.sortingLayerID = gM.elevSL[gM.getHighestElevation(movC.currentCell) + 1];
        }
        else
        {
            shadow.sortingLayerID = gM.elevSL[0];
        }
        
    }

    
   
}
