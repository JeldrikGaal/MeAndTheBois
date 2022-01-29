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

        if (beingcarried) boxSprite.sortingLayerID = gM.elevSL[elevation];
        newElv = gM.getHighestElevation(currentCell, this.gameObject);
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
        if (!beingcarried)
        {
            
            if (elevation > newElv)
            {
                this.boxSprite.sortingLayerID = gM.elevSL[newElv + 1 ];
                elevation = newElv;
            }
        }
    }
}
