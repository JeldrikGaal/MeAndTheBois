using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    public Sprite up;
    public Sprite down;
    public SpriteRenderer sR;

    public GameManager gM;
    public Grid ground;
    public Vector3 currentCell;

    public bool pressed;
    public bool isPlayerOn;

    public Pipe p1;
    public Pipe p2;

    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        ground = GameObject.Find("Grid").GetComponent<Grid>();
        sR = this.transform.GetComponent<SpriteRenderer>();
        pressed = false;

        Vector3 midPoint = ground.GetCellCenterWorld(ground.WorldToCell(transform.position));
        midPoint = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.25f));
        this.transform.position = new Vector3(midPoint.x, midPoint.y - (ground.cellSize.y * 0.49f), midPoint.z);
    }

    // Update is called once per frame
    void Update()
    {
        currentCell = ground.WorldToCell(this.transform.position);
        if (pressed)
        {
            sR.sprite = down;
            p1.on = false;
            p2.on = true;
        }
        else
        {
            sR.sprite = up;
            p1.on = true;
            p2.on = false;
        }
        isPlayerOn = checkForPlayer(gM.p1) || checkForPlayer(gM.p2) || checkForPlayer(gM.p3);
        pressed = isPlayerOn;
    }

    public bool checkForPlayer(MovementController p)
    {
        if (p.transform.gameObject.activeInHierarchy == false)
        {
            return false;
        }

        if (p.currentCell == currentCell)
        {
            return true;
        }
        return false;
    }
}
