using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{

    public SpriteRenderer shadow;
    public GameObject robotSprite;
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

    // Start is called before the first frame update
    void Start()
    {
        shadow = this.transform.GetComponent<SpriteRenderer>();
        robotSprite = this.transform.GetChild(0).gameObject;
        movC = this.transform.GetComponent<MovementController>();

        robotSprite.transform.localPosition = new Vector3(0, movC.ground.cellSize.y * elevation, 0);
        shadowStartSize = shadow.size.x - 0.05f;
        spriteOffSet = 0f;
        box = null;

        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Controll up and down movement / elevation of player
        robotSprite.transform.localPosition = new Vector3(0, movC.ground.cellSize.y * elevation, 0);
        if (Input.GetKeyDown(movC.playerControlls.up))
        {
            elevation += 1;
        }
        if (Input.GetKeyDown(movC.playerControlls.down))
        {
            if (elevation > 0)
            {
                elevation -= 1;
            }
        }

        // Regulate shadow size according to elevation
        shadow.size = new Vector2(shadowStartSize - elevation * 0.02f, shadowStartSize - elevation * 0.02f);

        // Check if box is being carried or can be picked up
        if (Input.GetKeyDown(movC.playerControlls.ability1))
        {
            if (carryingBox )
            {
                // create effect of the box being actually lifted
                boxS.boxSprite.transform.localPosition = new Vector3(0, -0.375f, 0);
                boxS.boxSprite.sortingLayerName = "Interactable";
                
                box.transform.position = this.transform.position - offset;
                box = null;
                boxS = null;
                carryingBox = false;
                offset = new Vector3();
            }

            else if (gM.isBoxOnCell(movC.currentCell, movC.ground) && !carryingBox)
            {
                box = gM.getBoxOnCell(movC.currentCell, movC.ground);
                boxS = box.GetComponent<Box>();
                if (this.elevation > boxS.elevation)
                {
                    carryingBox = true;
                    offset = transform.position - box.transform.position;
                }
               
            }

            else if (carryingMirror)
            {
                mir.transform.position = this.transform.position - offset;
                mirS.beingCarried = false;
                mir = null;
                mirS = null;
                carryingMirror = false;
                offset = new Vector3();

            }

            else if (gM.isMirrorOnCell(movC.currentCell, movC.ground) && !carryingMirror && !carryingBox)
            {
                mir = gM.getMirOnCell(movC.currentCell, movC.ground);
                mirS = mir.GetComponent<Mirror>();
                mirS.beingCarried = true;
                carryingMirror = true;
                offset = transform.position - mir.transform.position;
            }

        }

        if (carryingBox && box)
        {
            /*boxS.moving = true;
            //Vector3 temp = movC.ground.GetCellCenterWorld(movC.currentCell);
            Vector3 temp = this.transform.position;
            boxS.destination = new Vector3(temp.x , temp.y + (movC.ground.cellSize.y * 0.25f), temp.z);
            boxS.destination = temp;
            boxS.boxSprite.sortingLayerName = "Flying"; */

            boxS.boxSprite.transform.localPosition = new Vector3(0, spriteOffSet + ( (movC.ground.cellSize.y * 0.5f) * (elevation - boxS.elevation - 1)) , 0);

            box.transform.position = this.transform.position + offset;
            boxS.elevation = this.elevation - 1;
           
        }

        else if (carryingMirror && mir)
        {
            mir.transform.position = this.transform.position - offset;

            mirS.sR.transform.localPosition = new Vector3(0, elevation * 0.2f, 0);
        }
    }

   
}
