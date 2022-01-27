using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cellDebugger : MonoBehaviour
{

    public GameManager gM;
    public Grid ground;

    public Vector3 currentCell;
    public Vector3 p;

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
    }
}
