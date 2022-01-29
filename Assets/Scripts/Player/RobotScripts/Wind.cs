using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        highestElv = gM.getHighestElevation(movC.currentCell);
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
        // Debug.Log((movC.currentCell, t));
        highestElv2 = gM.getHighestElevation(t);
        
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

        //robotSpriteR.sortingLayerID = gM.elevSL[elevation];

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
            // Putting Box back on the ground
            if (carryingBox )
            {
                // create effect of the box being actually lifted
                //boxS.boxSprite.transform.localPosition = new Vector3(0, -0.375f, 0); // REDO
                //boxS.boxSprite.sortingLayerName = "Interactable"; // REDO
                Vector3 temp = gM.ground.GetCellCenterWorld(gM.ground.WorldToCell(this.transform.position)); // REDO
                box.transform.position = new Vector3(temp.x, temp.y - gM.ground.cellSize.y * 0.75f, 0);
                boxS.boxSprite.transform.localPosition = new Vector3(0, 0, 0);
                boxS.beingcarried = false;
                box = null;
                boxS = null;
                carryingBox = false;
                offset = new Vector3();
            }

            // Picking Box Up
            else if (gM.isBoxOnCell(movC.currentCell, movC.ground) && !carryingBox)
            {
                box = gM.getBoxOnCell(movC.currentCell, movC.ground);
                boxS = box.GetComponent<Box>();
                if (this.elevation > boxS.elevation)
                {
                    carryingBox = true;
                    boxS.beingcarried = true;
                }
               
            }

            // Setting Mirror down
            else if (carryingMirror)
            {
                mir.transform.position = this.transform.position - offset;
                mirS.beingCarried = false;
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
                mirS.beingCarried = true;
                carryingMirror = true;
                offset = transform.position - mir.transform.position;
                offset = new Vector3(offset.x, offset.y);
            }

        }

        if (carryingBox && box)
        {
            //boxS.boxSprite.transform.localPosition = new Vector3(0, spriteOffSet + ( (movC.ground.cellSize.y * 0.5f) * (elevation - boxS.elevation - 1)) , 0);

            boxS.boxSprite.transform.localPosition = new Vector3(0, movC.ground.cellSize.y * elevation, 0);
            box.transform.position = this.transform.position + offset;
            boxS.elevation = this.elevation - 1;
           
        }

        else if (carryingMirror && mir)
        {
            mir.transform.position = this.transform.position - offset;

            mirS.sR.transform.localPosition = new Vector3(0, elevation * 0.2f, 0);
        }

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
