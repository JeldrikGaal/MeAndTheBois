using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cellDebugger : MonoBehaviour
{

    public GameManager gM;
    public Grid ground;

    public Vector3Int currentCell;
    public Vector3 p;

    public int highest;

    public int layer;
    public Vector3Int checkVec;
    public bool checkElv;

    public bool checkCurrent;

    public float distance;
    public GameObject distanceGameObject;


    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        ground = GameObject.Find("Grid").GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        currentCell = ground.WorldToCell(this.transform.position);
        p = this.transform.position;
        highest = gM.getHighestElevation(currentCell);

        checkElv = gM.tilesList[layer].Contains(checkVec);
        checkCurrent = gM.tilesList[layer].Contains(currentCell);

        distance = Vector3.Distance(this.transform.position, distanceGameObject.transform.position);
    }
}
