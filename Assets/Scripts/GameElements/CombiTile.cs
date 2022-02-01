using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CombiTile : MonoBehaviour
{
    [Header("Variables to adjust")]  
    public int windNeeded = -1;
    public int sunNeeded = -1;
    [Tooltip("One of two combination tiles needs to be flagged as 'main' It also needs the partnerTile to be assigned to it. The partnerTile needs to be not main and doesnt need a reference to this tile ")]
    public bool main;
    public CombiTile partnerTile;

    [Tooltip("The Time the tile needs until its charged")]
    public float chargeTimer = 4f;

    [Tooltip("Combining needs to be checked for a tile to act as combination tile. Otherwise its a seperation tile")]
    public bool combining = true;

    [Tooltip("How fast the Door open and closes")]
    public float animSpeed = 2;

    [Tooltip("How long the tile waits until it moves the players down")]
    public float movementTimer = 0.5f;

    
    public ReceivingTile receivingTile;
    public ReceivingTile receivingTile2;

    public float decendERTime = 2f;

    public string _ = "=============================================";
    
    GameManager gM;
    Grid ground;
    SpriteRenderer sR;
    bool beingHitWind = false;
    bool beingHitLight = false;
    float directionHitW = -1;
    float directionHitL = -1;
    [Header("Variables not to be touched. Only for Script")]
    public bool _beingHitWind;
    public bool _beingHitLight;
    public float _directionHitW;
    public float _directionHitL;
    public bool playerOn;
    public int playerInt;
    public Vector3Int currentCell;
    
    public Animator anim;
    public bool charging;
    public GameObject energyReceiver;
    public GameObject partenForEnergyReceiver;
    public Animation openAnim;
    public bool decendER = false;
    public float decendERDist = 2f;
    private Transform left;
    private Vector3 leftGoal = new Vector3(-0.24f, -0.385f, 0);
    private Transform right;
    private Vector3 rightGoal = new Vector3(0.216f, -0.125f, 0);
    private Transform plate;
    private Vector3 plateGoal = new Vector3(-0.018f, -0.269f, 0);
    private Vector3 energyRGoal = new Vector3(0.034f, -1.82f, 0);
    private Transform spriteMask;
    private Transform pT = null;
    public bool openGate = false;
    public bool movePlayersDown = false;
    private Vector3 playerGoal1 = new Vector3();
    private Vector3 playerGoal2 = new Vector3();
    private Vector3 plateGoal1 = new Vector3();
    private Vector3 plateGoal2 = new Vector3();
    public bool receiving = false;
    public bool readyToCombine;

    public int elevation;

    public bool specialCaseOne;

    public float safeTimer;

    public bool cancelFirstCharge;

    // Start is called before the first frame update
    void Start()
    {
        safeTimer = chargeTimer;
        spriteMask = transform.Find("Sprite Mask");
        left = transform.Find("Sprite Mask/Shift/left");
        right = transform.Find("Sprite Mask/Shift/right");
        plate = transform.Find("Sprite Mask/Shift/plate");

        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        sR = this.GetComponent<SpriteRenderer>();
        ground = GameObject.Find("Grid").GetComponent<Grid>();
        openAnim = this.GetComponent<Animation>();

        spawnFittingEnergy();

        anim = this.GetComponentInChildren<Animator>();

        if (windNeeded != -1)
        {
            anim.SetInteger("Direction", (windNeeded));
            anim.SetInteger("Type", (sunNeeded));
            if (specialCaseOne) anim.SetInteger("Type", (sunNeeded + 1));
        }


        partenForEnergyReceiver.GetComponent<SortingGroup>().sortingLayerName = spriteMask.GetComponent<SortingGroup>().sortingLayerName;
        partenForEnergyReceiver.GetComponent<SortingGroup>().sortingOrder = spriteMask.GetComponent<SortingGroup>().sortingOrder + 1;
    }

    // Update is called once per frame
    void Update()
    {
        currentCell = ground.WorldToCell(transform.position);
        //Debug.Log((_beingHitWind, _beingHitLight, _directionHitL, _directionHitW));

        if (!receiving)
        {
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
            else if (checkForPlayer(gM.p3))
            {
                playerOn = true;
                playerInt = 3;
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
                if (windNeeded != -1 && cancelFirstCharge) anim.SetTrigger("Go");

            }
            else
            {
                if (!cancelFirstCharge)
                {
                    charging = false;
                    cancelFirstCharge = true;
                }
               
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

            if (openGate)
            {
                left.localPosition = Vector3.Lerp(left.localPosition, leftGoal, Time.deltaTime * animSpeed);
                right.localPosition = Vector3.Lerp(right.localPosition, rightGoal, Time.deltaTime * animSpeed);
                plate.localPosition = Vector3.Lerp(plate.localPosition, plateGoal, Time.deltaTime * animSpeed);

                //Debug.Log((Vector3.Distance(left.localPosition, leftGoal) <= 0.01f, Vector3.Distance(right.localPosition, rightGoal) <= 0.01f , Vector3.Distance(plate.localPosition, plateGoal) <= 0.01f));
                if (Vector3.Distance(left.localPosition, leftGoal) <= 0.01f && Vector3.Distance(right.localPosition, rightGoal) <= 0.01f && Vector3.Distance(plate.localPosition, plateGoal) <= 0.01f)
                {
                    openGate = false;
                    left.gameObject.SetActive(false);
                    right.gameObject.SetActive(false);
                    //plate.gameObject.SetActive(false);
                    readyToCombine = true;

                }
            }

            if (!movePlayersDown)
            {
                if ((checkForCombination() && readyToCombine) || (chargeTimer <= 0 && readyToCombine))
                {
                    if (combining)
                    {
                        movePlayersDown = true;

                        foreach (SpriteRenderer sr in gM.p1.getAllSR())
                        {
                            sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                        }
                        foreach (SpriteRenderer sr in gM.p2.getAllSR())
                        {
                            sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                        }

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


                    }
                    else
                    {
                        movePlayersDown = true;

                        playerGoal1 = new Vector3(gM.p3.transform.position.x, gM.p3.transform.position.y - 2, gM.p3.transform.position.z);

                        plateGoal1 = new Vector3(plate.localPosition.x, plate.localPosition.y - 2, plate.localPosition.z);

                        gM.p3.transform.parent = this.partenForEnergyReceiver.transform;
                        foreach (SpriteRenderer sr in gM.p3.getAllSR())
                        {
                            sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                        }
                    }

                }
            }


            if (movePlayersDown && main || movePlayersDown && !combining && !main)
            {
                if (movementTimer <= 0)
                {
                    if (combining)
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
                            receiving = true;
                            movementTimer = 0.5f;

                        }
                    }
                    else
                    {
                        gM.p3.transform.position = Vector3.MoveTowards(gM.p3.transform.position, playerGoal1, Time.deltaTime * animSpeed);
                        plate.localPosition = Vector3.MoveTowards(plate.localPosition, plateGoal1, Time.deltaTime * animSpeed);
                        if (Vector3.Distance(gM.p3.transform.position, playerGoal1) <= 0.01f)
                        {
                            movePlayersDown = false;
                            gM.p3.gameObject.SetActive(false);
                            receiving = true;
                            movementTimer = 0.5f;

                        }
                    }
                }
                else
                {
                    movementTimer -= Time.deltaTime;
                }
                    
            }

           
        }

        if (receiving)
        {
            if (combining)
            {
                receivingTile.openGateF(gM.p3);
                receiving = false;
            }
            else
            {
                receivingTile.openGateF(gM.p1);
                receivingTile2.openGateF(gM.p2);
                receiving = false;
            }
            
        }

        

    }


    void spawnFittingEnergy()
    {
        if (sunNeeded != -1 && windNeeded != -1)
        {
            string path = "_PREFABS/COMBI/SW " + (sunNeeded) + " -" + (windNeeded) + "_0";
            Object o = Resources.Load(path);

            GameObject g = (GameObject)Instantiate(o);
            g.transform.parent = partenForEnergyReceiver.transform;
            g.transform.localPosition = new Vector3(0.034f, -0.336f, 0);
            g.GetComponent<SpriteRenderer>().sortingOrder = 3;

            energyReceiver = g;
            return;
        }

        if (windNeeded == -1)
        {
            string path = "_PREFABS/COMBI/SW " + (sunNeeded);
            Object o = Resources.Load(path);

            GameObject g = (GameObject)Instantiate(o);
            g.transform.parent = partenForEnergyReceiver.transform;
            g.transform.localPosition = new Vector3(0.034f, -0.336f, 0);
            g.GetComponent<SpriteRenderer>().sortingOrder = 3;

            energyReceiver = g;
            return;
        }
            
        
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
        if (!main && combining) return false;
        if (combining)
        {
            if (checkEnergy() && partnerTile.checkEnergy() && playerOn && partnerTile.playerOn && (playerInt != partnerTile.playerInt))
            {
                return true;
            }
        }
        else
        {
            if (checkEnergy() && playerOn && playerInt == 3)
            {
                return true;
            }
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
            if (sunNeeded == _directionHitL && _beingHitLight || (specialCaseOne && (sunNeeded + 1) == _directionHitL && _beingHitLight))
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
        if (p.transform.gameObject.activeInHierarchy == false)
        {
            return false;
        }

        Vector3Int checkCell = new Vector3Int(currentCell.x - elevation, currentCell.y - elevation, currentCell.z);
        if (p.currentCell == checkCell)
        {
            return true;
        }
        return false;
    }

}
