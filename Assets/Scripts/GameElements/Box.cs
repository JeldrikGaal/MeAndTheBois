using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Box : MonoBehaviour
{
    public Vector3 destination;
    public bool moving;
    public int elevation;

    public Grid ground;
    public GameManager gM;

    public SpriteRenderer boxSprite;

    public Vector3Int currentCell;

    public float speed;
    public bool beingcarried;
    public int newElv;

    public Tile boxTile;
    public Tile saveTile = null;

    // Start is called before the first frame update
    void Awake()
    {
        ground = GameObject.Find("Grid").GetComponent<Grid>();
        Vector3 temp = ground.GetCellCenterWorld(ground.WorldToCell(this.transform.position));
        transform.position = new Vector3(temp.x, temp.y - ground.cellSize.y * 0.75f, 0);
        elevation = 0;
        speed = 3f;
        boxSprite = this.GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        currentCell = ground.WorldToCell(transform.position);

        if (moving)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, destination, speed * Time.deltaTime);
            if (Vector3.Distance(this.transform.position,destination) <= 0.01f)
            {
                moving = false;
            }
        }

        //isBelowSolid();
        //Debug.Log(gM.getHighestElevation(currentCell, this.gameObject));


        //if (beingcarried) boxSprite.sortingLayerID = gM.elevSL[elevation + 1];
        if (beingcarried) boxSprite.sortingLayerID = SortingLayer.NameToID("Flying");
        if (beingcarried) boxSprite.sortingOrder = 1;
        newElv = gM.getHighestElevation(currentCell, this.gameObject);
        //newElv = gM.getHighestElevation(currentCell);

        updateGravity();
    }

    bool isBelowSolid()
    {
        if (gM.collisionList.Count > elevation)
        {
            if (gM.collisionList[elevation].Contains(currentCell))
            {
                // Debug.Log(("1", true));
                return true;
            }
        }
        if (gM.getBoxSOnCell(currentCell, ground) != this.GetComponent<Box>())
        {
            //Debug.Log(("2", true));
            return true;
        }

        //Debug.Log(("3", false));
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("VineMovingPoint"))
        {
            collision.GetComponent<VineMovingPoint>().currentBox = this.gameObject;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("VineMovingPoint"))
        {
            collision.GetComponent<VineMovingPoint>().currentBox = null;
        }
    }

    public void updateGravity()
    {
        if (!beingcarried && !moving)
        {
            elevation = newElv;
            if (elevation != -1)
            {
                this.boxSprite.sortingLayerID = gM.elevSL[elevation + 1];
                //this.boxSprite.transform.localPosition = new Vector3(this.boxSprite.transform.localPosition.x, 0.5f * elevation, this.boxSprite.transform.localPosition.z); ALLAH fix correct display while on obstacles
            }
            else
            {
                this.boxSprite.sortingLayerID = SortingLayer.NameToID("Layer1");
                this.boxSprite.enabled = false;
                if (saveTile == null)
                {
                    saveTile = (Tile)gM.waterMap.GetTile(currentCell);
                    gM.tilesList[0].Add(currentCell);
                    Debug.Log((currentCell, "HIER IST NE BOX"));
                }
                //gM.waterMap.SetTile(currentCell, boxTile);
                gM.boxPlacingMap.SetTile(currentCell, boxTile);
                

                //this.boxSprite.transform.localPosition = new Vector3(this.boxSprite.transform.localPosition.x, 0f, this.boxSprite.transform.localPosition.z);
            }



           
        }
    }
}
