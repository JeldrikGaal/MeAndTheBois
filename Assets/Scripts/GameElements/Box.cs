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

    public SpriteRenderer boxSprite;

    public Vector3Int currentCell;

    // Start is called before the first frame update
    void Awake()
    {

        Vector3 temp = ground.GetCellCenterWorld(ground.WorldToCell(this.transform.position));
        //transform.position = new Vector3(temp.x, temp.y + (ground.cellSize.y * 0.25f), temp.z);
        transform.position = temp;
        // Properly calculate elevation !!
        elevation = 0;

        boxSprite = this.GetComponentInChildren<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        currentCell = ground.WorldToCell(transform.position);

        if (moving)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, destination, 0.01f);
            if (Vector3.Distance(this.transform.position,destination) <= 0.01f)
            {
                moving = false;
            }
        }
        
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
}
