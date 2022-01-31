using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneCollect : MonoBehaviour
{

    public GameManager gM;
    public Grid ground;
    public Vector3 currentCell;
    public MovementController collectingPlayer;
    public int elevation;


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
        currentCell = new Vector3(currentCell.x - elevation, currentCell.y - elevation, 0);

        if (playerOnCell())
        {
            GameManager.collectedBones b = new GameManager.collectedBones();
            b.bone = this.gameObject;
            b.playerCollected = collectingPlayer;
            b.number = 0;
            gM.bones.Add(b);
            collectingPlayer = null;
            this.gameObject.SetActive(false);
        }
    }

    bool playerOnCell()
    {
        if (gM.p1.currentCell == currentCell && gM.p1.isActiveAndEnabled)
        {
            collectingPlayer = gM.p1;
            return true;
        }
        if (gM.p2.currentCell == currentCell && gM.p2.isActiveAndEnabled)
        {
            collectingPlayer = gM.p2;
            return true;
        }
        if (gM.p3.currentCell == currentCell && gM.p3.isActiveAndEnabled)
        {
            collectingPlayer = gM.p3;
            return true;
        }
        return false;
    }

}
