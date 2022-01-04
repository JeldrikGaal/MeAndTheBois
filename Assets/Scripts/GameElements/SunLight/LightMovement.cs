using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMovement : MonoBehaviour
{

    public List<GameObject> wayPoints;
    public int currentPoint;

    public float speed;


    // Start is called before the first frame update
    void Start()
    {
        currentPoint = 0;
        speed = 1f;

        foreach( GameObject g in GameObject.FindGameObjectsWithTag("WP"))
        {
            wayPoints.Add(g);
        }
    }

    // Update is called once per frame
    void Update()
    {
        travelWayPoints();
    }
    
    public void travelWayPoints()
    {
        if (currentPoint >= wayPoints.Count)
        {
            currentPoint = 0;
        }
        if (Vector3.Distance( this.transform.position, wayPoints[currentPoint].transform.position) <= 0.05f )
        {
            currentPoint += 1;
            if (currentPoint >= wayPoints.Count)
            {
                return;
            }
        }
        else
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, wayPoints[currentPoint].transform.position, speed * Time.deltaTime);
        }
    }
}
