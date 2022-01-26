using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombiTile : MonoBehaviour
{
    bool beingHitWind = false;
    bool beingHitLight = false;
    float directionHitW = -1;
    float directionHitL = -1;

    public bool _beingHitWind;
    public bool _beingHitLight;
    public float _directionHitW;
    public float _directionHitL;
    GameManager gM;
    Grid ground;
    SpriteRenderer sR;

    public int windNeeded = -1;
    public int sunNeeded = -1;

    public bool playerOn;
    public int playerInt;

    public Vector3Int currentCell;

    public CombiTile partnerTile;
    public bool main;

    public Animator anim;

    public float chargeTimer = 4f;
    public bool charging;

    public GameObject energyReceiver;
    public GameObject partenForEnergyReceiver;

    public Animation openAnim;
    public bool decendER = false;
    public float decendERTime = 2f;
    public float decendERDist = 2f;

    private Transform left;
    private Vector3 leftGoal = new Vector3(-0.24f, -0.385f, 0);
    private Transform right;
    private Vector3 rightGoal = new Vector3(0.216f, -0.125f, 0);
    private Transform plate;
    private Vector3 plateGoal = new Vector3(-0.018f, -0.269f, 0);
    private Vector3 energyRGoal = new Vector3(0.034f, -1.82f, 0);
    private Transform pT = null;
    private bool openGate = false;
    private bool movePlayersDown = false;

    private Vector3 playerGoal1 = new Vector3();
    private Vector3 playerGoal2 = new Vector3();
    private Vector3 plateGoal1 = new Vector3();
    private Vector3 plateGoal2 = new Vector3();

    public float animSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        left = transform.Find("Sprite Mask/Shift/left");
        right = transform.Find("Sprite Mask/Shift/right");
        plate = transform.Find("Sprite Mask/Shift/plate");

        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        sR = this.GetComponent<SpriteRenderer>();
        ground = GameObject.Find("Grid").GetComponent<Grid>();
        openAnim = this.GetComponent<Animation>();

        spawnFittingEnergy();

        anim = this.GetComponentInChildren<Animator>();

        anim.SetInteger("Direction", (windNeeded));
        anim.SetInteger("Type", (sunNeeded));
    }

    // Update is called once per frame
    void Update()
    {
        currentCell = ground.WorldToCell(transform.position);
        //Debug.Log((_beingHitWind, _beingHitLight, _directionHitL, _directionHitW));

        if (checkForPlayer(gM.p1))
        {
            playerOn = true;
            playerInt = 1;
        }
        else if (checkForPlayer(gM.p2))
        {
            playerOn = true;
            playerInt = 2;
        }
        else
        {
            playerOn = false;
            playerInt = 0;
        }

        //Debug.Log((checkEnergy(), this.name));
        //Debug.Log(checkForCombination());
        if (checkEnergy())
        {
            if (chargeTimer > 0) charging = true;
            anim.SetTrigger("Go");
        }
        if (charging)
        {
            chargeTimer -= Time.deltaTime;
        }

        if (chargeTimer <= 0 && charging)
        {
            charging = false;
            //energyReceiver.SetActive(false);
            
            decendER = true;
        }

        if (decendER)
        {
            //decendERTime -= Time.deltaTime;
            //energyReceiver.transform.position = new Vector3(energyReceiver.transform.position.x, energyReceiver.transform.position.y - ((Time.deltaTime / decendERTime ) * decendERDist), energyReceiver.transform.position.z);
            energyReceiver.transform.localPosition = Vector3.Lerp(energyReceiver.transform.localPosition, energyRGoal, Time.deltaTime * animSpeed);
            if (Vector3.Distance(energyReceiver.transform.localPosition, energyRGoal) <= 0.01f)
            {
                decendER = false;
                energyReceiver.SetActive(false);
                openGate = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            openAnim.Play();
        }

        if (openGate)
        {
            left.localPosition = Vector3.Lerp(left.localPosition, leftGoal, Time.deltaTime * animSpeed);
            right.localPosition = Vector3.Lerp(right.localPosition, rightGoal, Time.deltaTime * animSpeed);
            plate.localPosition = Vector3.Lerp(plate.localPosition, plateGoal, Time.deltaTime * animSpeed);

            if (Vector3.Distance(left.position, leftGoal) <= 0.01f && Vector3.Distance(right.position, rightGoal) <= 0.01f && Vector3.Distance(plate.position, plateGoal) <= 0.01f)
            {
                openGate = false;
                left.gameObject.SetActive(false);
                right.gameObject.SetActive(false);
            }
        }

        if (!movePlayersDown)
        {
            if (checkForCombination())
            {
                movePlayersDown = true;

                playerGoal1 = new Vector3(gM.p1.transform.position.x, gM.p1.transform.position.y - 2, gM.p1.transform.position.z);
                playerGoal2 = new Vector3(gM.p2.transform.position.x, gM.p2.transform.position.y - 2, gM.p2.transform.position.z);

                plateGoal1 = new Vector3(plate.localPosition.x, plate.localPosition.y - 2, plate.localPosition.z);
                plateGoal2 = new Vector3(partnerTile.plate.localPosition.x, partnerTile.plate.localPosition.y - 2, partnerTile.plate.localPosition.z);

                if (playerInt == 1)
                {
                    gM.p1.transform.parent = this.partenForEnergyReceiver.transform;
                    gM.p2.transform.parent = partnerTile.partenForEnergyReceiver.transform;
                }
                if (playerInt == 2)
                {
                    gM.p2.transform.parent = this.partenForEnergyReceiver.transform;
                    gM.p1.transform.parent = partnerTile.partenForEnergyReceiver.transform;
                }


                gM.p1.sR.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                gM.p2.sR.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }
       

        if (movePlayersDown && main)
        {
            gM.p2.transform.position = Vector3.MoveTowards(gM.p2.transform.position, playerGoal2, Time.deltaTime * animSpeed);
            gM.p1.transform.position = Vector3.MoveTowards(gM.p1.transform.position, playerGoal1, Time.deltaTime * animSpeed);
            plate.localPosition = Vector3.MoveTowards(plate.localPosition, plateGoal1, Time.deltaTime * animSpeed);
            partnerTile.plate.localPosition = Vector3.MoveTowards(partnerTile.plate.localPosition, plateGoal2, Time.deltaTime * animSpeed);




            if (Vector3.Distance(gM.p1.transform.position, playerGoal1) <= 0.01f && (Vector3.Distance(gM.p2.transform.position, playerGoal2) <= 0.01f))
            {
                movePlayersDown = false;
                gM.p1.gameObject.SetActive(false);
                gM.p2.gameObject.SetActive(false);

            }
        }
    }


    void spawnFittingEnergy()
    {
        string path = "_PREFABS/COMBI/SW " + (sunNeeded) +  " -" + (windNeeded) + "_0";
        Debug.Log(path);
        Object o = Resources.Load(path);

        GameObject g = (GameObject)Instantiate(o);
        g.transform.parent = partenForEnergyReceiver.transform;
        g.transform.localPosition = new Vector3(0.034f, -0.336f, 0);
        g.GetComponent<SpriteRenderer>().sortingOrder = 3;

        energyReceiver = g;
        
    }

    private void LateUpdate()
    {
        _beingHitWind = beingHitWind;
        _beingHitLight = beingHitLight;
        _directionHitL = directionHitL;
        _directionHitW = directionHitW;

        beingHitWind = false;
        beingHitLight = false;
        directionHitL = -1;
        directionHitW = -1;
    }

    public bool checkForCombination()
    {
        if (!main) return false;
        if (checkEnergy() && partnerTile.checkEnergy() && playerOn && partnerTile.playerOn && (playerInt != partnerTile.playerInt))
        {
            return true;
        }

        return false;

    }

    public bool checkEnergy()
    {
        if (windNeeded != -1) 
        {
            if (windNeeded == _directionHitW && _beingHitWind)
            {

            }
            else
            {
                return false;
            }
        }

        if (sunNeeded != -1)
        {
            if (sunNeeded == _directionHitL && _beingHitLight)
            {

            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public void HitByWind(Vector3 _hitPoint, Vector3 ray)
    {
        directionHitW = gM.calcRecAngleByRay(_hitPoint, ray) +1;
        beingHitWind = true;

    }

    public void HitBySolar(Vector3 _hitPoint, Vector3 ray)
    {
        directionHitL = gM.calcRecAngleByRay(_hitPoint, ray) +1;
        beingHitLight = true;
    }

    public bool checkForPlayer(MovementController p)
    {
        if (p.currentCell == currentCell)
        {
            return true;
        }
        return false;
    }

}