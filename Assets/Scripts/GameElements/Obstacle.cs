using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public int elevation;
    public GameManager gM;

    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        Vector3 temp = gM.ground.GetCellCenterWorld(gM.ground.WorldToCell(this.transform.position));
        transform.position = new Vector3(temp.x, temp.y - gM.ground.cellSize.y * 0.75f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
