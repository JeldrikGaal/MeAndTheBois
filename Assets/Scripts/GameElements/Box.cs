using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public Vector3 destination;
    public bool moving;
    public int elevation;

    // Start is called before the first frame update
    void Start()
    {
        destination = this.transform.position;
        // Properly calculate elevation !!
        elevation = 0;
    }

    // Update is called once per frame
    void Update()
    {
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
