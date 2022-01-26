using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceivingTile : MonoBehaviour
{

    public Transform left;
    public Vector3 leftGoal = new Vector3(-0.24f, -0.385f, 0);
    public Transform right;
    public Vector3 rightGoal = new Vector3(0.216f, -0.125f, 0);
    public Transform plate;
    public Vector3 plateGoal = new Vector3(-0.018f, -0.269f, 0);
    public Vector3 player3Goal = new Vector3();
    public GameManager gM;

    public float animSpeed = 2;
    public bool openGate = false;
    public Vector3Int currentCell;

    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        currentCell = this.gM.p3.ground.WorldToCell(this.transform.position);
        this.transform.position = gM.p3.ground.GetCellCenterWorld(currentCell);

        left = transform.Find("Sprite Mask/Shift/left");
        right = transform.Find("Sprite Mask/Shift/right");
        plate = transform.Find("Sprite Mask/Shift/plate");
       

        player3Goal = (this.gM.p3.ground.GetCellCenterWorld(this.gM.p3.ground.WorldToCell(this.transform.position)));
        player3Goal = new Vector3(player3Goal.x, player3Goal.y , player3Goal.z); //+ gM.p3.ground.cellSize.y * 0.5f

        player3Goal = this.transform.position;
        Debug.Log(currentCell);
        Debug.Log(player3Goal);

    }

    // Update is called once per frame
    void Update()
    {
        if (openGate)
        {
            left.localPosition = Vector3.Lerp(left.localPosition, leftGoal, Time.deltaTime * animSpeed);
            right.localPosition = Vector3.Lerp(right.localPosition, rightGoal, Time.deltaTime * animSpeed);
            plate.localPosition = Vector3.Lerp(plate.localPosition, plateGoal, Time.deltaTime * animSpeed);
            //gM.p3.transform.localPosition = plate.localPosition;
            //gM.p3.transform.localPosition = Vector3.Lerp(gM.p3.transform.localPosition, player3Goal, Time.deltaTime * animSpeed);
            gM.p3.transform.position = Vector3.Lerp(gM.p3.transform.position, player3Goal, Time.deltaTime * animSpeed);

            //Debug.Log((Vector3.Distance(left.localPosition, leftGoal) <= 0.01f && Vector3.Distance(right.localPosition, rightGoal) <= 0.01f && Vector3.Distance(plate.localPosition, plateGoal) <= 0.01f));
            if (Vector3.Distance(left.localPosition, leftGoal) <= 0.01f && Vector3.Distance(right.localPosition, rightGoal) <= 0.01f && Vector3.Distance(plate.localPosition, plateGoal) <= 0.01f)
            {
                openGate = false;
                left.gameObject.SetActive(false);
                right.gameObject.SetActive(false);
                gM.p3.sR.maskInteraction = SpriteMaskInteraction.None;
                gM.p3.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
                gM.p3.transform.parent = null;
                gM.p3.movingPoint.transform.position = gM.p3.transform.position;
                gM.p3.controllBool = true;

            }
        }
    }

    public void openGateF()
    {
        //gM.p3.transform.localPosition = new Vector3(plate.transform.position.x, plate.transform.position.y + (plate.GetComponent<SpriteRenderer>().sprite.bounds.size.y * 0.5f), plate.transform.position.z) ; 
        gM.p3.transform.position = new Vector3(plate.transform.position.x, plate.transform.position.y, plate.transform.position.z) ; 
        gM.p3.gameObject.SetActive(true);
        openGate = true;
        gM.p3.sR.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        gM.p3.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        gM.p3.transform.parent = this.transform.GetChild(1);
        gM.p3.controllBool = false;
    }
}
