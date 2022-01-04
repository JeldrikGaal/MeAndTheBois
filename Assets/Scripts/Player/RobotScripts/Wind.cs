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


    // Start is called before the first frame update
    void Start()
    {
        shadow = this.transform.GetComponent<SpriteRenderer>();
        robotSprite = this.transform.GetChild(0).gameObject;
        movC = this.transform.GetComponent<MovementController>();

        robotSprite.transform.localPosition = new Vector3(0, movC.ground.cellSize.y * elevation, 0);
        shadowStartSize = shadow.size.x - 0.05f;
        spriteOffSet = 0.1f;
        box = null;
    }

    // Update is called once per frame
    void Update()
    {
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

        shadow.size = new Vector2(shadowStartSize - elevation * 0.02f, shadowStartSize - elevation * 0.02f);

        // Check if box is being carried or can be picked up
        if (Input.GetKeyDown(movC.playerControlls.ability1))
        {
            if (carryingBox)
            {
                box = null;
                boxS.boxSprite.transform.localPosition = new Vector3(0, 0, 0);
                boxS.boxSprite.sortingLayerName = "Interactable";
                boxS = null;
                carryingBox = false;
                
            }
            else if (gM.isBoxOnCell(movC.currentCell, movC.ground) && !carryingBox )
            {
                box = gM.getBoxOnCell(movC.currentCell, movC.ground);
                boxS = box.GetComponent<Box>();
                carryingBox = true;
            }

        }

        if (carryingBox && box)
        {
            boxS.moving = true;
            Vector3 temp = movC.ground.GetCellCenterWorld(movC.currentCell);
            boxS.destination = new Vector3(temp.x , temp.y + (movC.ground.cellSize.y * 0.25f), temp.z);
            boxS.boxSprite.transform.localPosition = new Vector3(0, spriteOffSet + ( movC.ground.cellSize.y * (elevation - boxS.elevation - 1)) , 0);
            boxS.boxSprite.sortingLayerName = "Flying";
        }
    }

   
}
