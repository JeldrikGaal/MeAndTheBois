using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ReceivingTile : MonoBehaviour
{
    [Header("Variables to set")]
    [Tooltip("Speed of the Door and Plate Animation")]
    public float animSpeed = 2;
    [Tooltip("Needs to be checked if used as receiving tile for combi Robot and not checked if used for splitting up")]
    public bool single;

    public int elevation;

    [Header("Variables not to be touched. Only for Script")]
    public Transform left;
    public Transform plate;
    public Transform right;
    public GameManager gM;
    public Grid ground;
    public MovementController player;
    public Vector3 leftGoal = new Vector3(-0.24f, -0.385f, 0);
    public Vector3 rightGoal = new Vector3(0.216f, -0.125f, 0);
    public Vector3 plateGoal = new Vector3(-0.018f, -0.269f, 0);
    public Vector3 player3Goal = new Vector3();
    public Vector3Int currentCell;
    
    public bool openGate = false;
    public SortingGroup sG;
    public bool keepElv;

    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        ground = GameObject.Find("Grid").GetComponent<Grid>(); ;
        currentCell = ground.WorldToCell(this.transform.position);
        this.transform.position = ground.GetCellCenterWorld(currentCell);

        left = transform.Find("Sprite Mask/Shift/left");
        right = transform.Find("Sprite Mask/Shift/right");
        plate = transform.Find("Sprite Mask/Shift/plate");

        sG = transform.GetChild(0).GetComponent<SortingGroup>();
        if (!keepElv) elevation = gM.elevSL.IndexOf(sG.sortingLayerID);
    }

    // Update is called once per frame
    void Update()
    {
        if (openGate)
        {
            left.localPosition = Vector3.Lerp(left.localPosition, leftGoal, Time.deltaTime * animSpeed);
            right.localPosition = Vector3.Lerp(right.localPosition, rightGoal, Time.deltaTime * animSpeed);
            plate.localPosition = Vector3.Lerp(plate.localPosition, plateGoal, Time.deltaTime * animSpeed);

            player.transform.position = Vector3.Lerp(player.transform.position, player3Goal, Time.deltaTime * animSpeed);


            foreach (SpriteRenderer sr in player.getAllSR())
            {
                sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            if (player.GetComponent<SpriteRenderer>())
            {
                player.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            

            if (Vector3.Distance(left.localPosition, leftGoal) <= 0.01f && Vector3.Distance(right.localPosition, rightGoal) <= 0.01f && Vector3.Distance(plate.localPosition, plateGoal) <= 0.01f)
            {
                openGate = false;
                left.gameObject.SetActive(false);
                right.gameObject.SetActive(false);

                foreach (SpriteRenderer sr in player.getAllSR())
                {
                    sr.maskInteraction = SpriteMaskInteraction.None;
                }
                if (player.GetComponent<SpriteRenderer>())
                {
                    player.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
                }
                player.transform.parent = null;
                player.movingPoint.transform.position = player.transform.position;
                player.controllBool = true;
            }
        }
    }

    public void openGateF(MovementController p)
    {
        openGate = true;
        player = p;

        player.gameObject.SetActive(true);
        player.transform.position = new Vector3(plate.transform.position.x, plate.transform.position.y, plate.transform.position.z) ;
        
        foreach (SpriteRenderer sr in player.getAllSR())
        {
            sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        if (player.GetComponent<SpriteRenderer>())
        {
            player.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        player.transform.parent = this.transform.GetChild(1);
        player.controllBool = false;
        player.elevation = elevation;

        player3Goal = this.transform.position;
    }
}
