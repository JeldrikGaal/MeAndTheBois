using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{

    public float angle;

    public Transform reflectionPoint;
    public Vector2 startingPos;
    public Grid ground;

    private void Awake()
    {
        reflectionPoint = this.transform.GetChild(0);
        angle = 0;

        Vector3 temp = ground.GetCellCenterWorld(ground.WorldToCell(reflectionPoint.position));
        reflectionPoint.transform.position = new Vector3(temp.x, temp.y - (ground.cellSize.y * 0.25f), temp.z);

        startingPos = new Vector2(reflectionPoint.position.x, reflectionPoint.position.y);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
