using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombiRobot : MonoBehaviour
{

    public bool shooting;

    public List<string> tagsToErode = new List<string>();

    public MovementController mC;
    public GameManager gM;

    public GameObject bomb;
    private Vector3 bombDest;
    private float bombSpeed = 2;
    private GameObject hitObject;
    public int elevation;
    public float maxShootDist;

    private bool eroding;
    private bool onlyFlying;
    private float currentTime = 0;
    private float totalTime = 2f;

    private GameObject robotSprite;
    private GameObject particles;
    public SpriteRenderer shadow;
    public float shadowStartSize;

    public int maxElevation;
    public MovementController movC;

    public int highestElv;
    public int highestElv2;

    public bool carryingBox;
    private GameObject box;
    private Box boxS;



    // Start is called before the first frame update
    void Start()
    {
        shadow = this.transform.GetChild(4).GetComponent<SpriteRenderer>();
        shadowStartSize = shadow.size.x - 0.05f;
        movC = this.transform.GetComponent<MovementController>();
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        tagsToErode.Add("Obstacle");
        mC = this.GetComponent<MovementController>();
        bomb = transform.GetChild(0).gameObject;
        //robotSprite = transform.GetChild(1).gameObject;
        particles = transform.GetChild(3).gameObject;

        maxShootDist = 5;
        maxElevation = 7;
    }

    // Update is called once per frame
    void Update()
    {
        robotSprite = movC.currentFB;
        shadow.size = new Vector2(shadowStartSize - elevation * 0.02f, shadowStartSize - elevation * 0.02f); // REDO
        // Shadow sorting layer
        shadow.sortingOrder = 5;
        if (!(elevation - 1 < 0))
        {
            shadow.sortingLayerID = gM.elevSL[gM.getHighestElevation(movC.currentCell) + 1];
        }
        else
        {
            shadow.sortingLayerID = gM.elevSL[0];
        }

        highestElv = gM.getHighestElevation(movC.currentCell, box);
        Vector3Int t = movC.currentCell;

        t = movC.currentCell;
        int modifier = highestElv;
        if (modifier > 1) modifier -= 1;
        t = new Vector3Int(t.x + modifier, t.y + modifier, t.z);
        highestElv2 = gM.getHighestElevation(t, box);


        if (gM.getHighestElevation(movC.currentCell) != -1)
        {
            if (highestElv > 0 && highestElv2 >= highestElv)
            {
                shadow.transform.localPosition = new Vector3(0, highestElv2 * movC.ground.cellSize.y, 0);
            }
            else
            {
                shadow.transform.localPosition = new Vector3(0, highestElv * movC.ground.cellSize.y, 0);
            }
        }
        else
        {
            shadow.transform.localPosition = new Vector3(0, 0, 0);
        }

        // Controll up and down movement / elevation of player
        robotSprite.transform.localPosition = new Vector3(0, mC.ground.cellSize.y * elevation, 0);
        if (Input.GetKeyDown(movC.playerControlls.up) && elevation < maxElevation)
        {
            elevation += 1;
        }
        if (Input.GetKeyDown(movC.playerControlls.down))
        {
            if (elevation > 0 && elevation > gM.getHighestElevation(movC.currentCell))
            {
                elevation -= 1;
            }
        }

        if (shooting)
        {
            bomb.transform.position = Vector3.MoveTowards(bomb.transform.position, bombDest, Time.deltaTime * bombSpeed);
            particles.transform.position = bomb.transform.position;
            if (Vector3.Distance(bomb.transform.position, bombDest) <= 0.01f)
            {
                shooting = false;
                bomb.SetActive(false);
                particles.SetActive(false);
                eroding = true;
            }
        }

        if (eroding && !onlyFlying)
        {
            applyErosion(hitObject);
            
        }

        if (eroding && onlyFlying)
        {
            eroding = false;
            onlyFlying = false;
        }

        // Check if box is being carried or can be picked up
        if (Input.GetKeyDown(movC.playerControlls.ability2))
        {
            // Putting Box back on the ground
            if (carryingBox)
            {
                Vector3 temp = movC.ground.GetCellCenterWorld(movC.ground.WorldToCell(this.transform.position));
                box.transform.position = new Vector3(temp.x, temp.y - movC.ground.cellSize.y * 0.75f, 0);
                boxS.boxSprite.transform.localPosition = new Vector3(0, 0, 0);
                boxS.beingcarried = false;
                box = null;
                boxS = null;
                carryingBox = false;
            }

            // Picking Box Up
            else if (gM.isBoxOnCell(movC.currentCell, movC.ground) && !carryingBox)
            {
                box = gM.getBoxOnCell(movC.currentCell, movC.ground);
                boxS = box.GetComponent<Box>();

                if (this.elevation > boxS.elevation)
                {
                    if (boxS.saveTile != null)
                    {
                        gM.waterMap.SetTile(movC.currentCell, boxS.saveTile);
                        gM.boxPlacingMap.SetTile(movC.currentCell, null);
                        gM.tilesList[0].Remove(movC.currentCell);
                        boxS.saveTile = null;
                        boxS.boxSprite.enabled = true;
                    }
                    carryingBox = true;
                    boxS.beingcarried = true;
                }

            }
        }
    }

    public void shootSeedBomb()
    {
        if (shooting) return;

        Vector3 pos1 = mC.ground.GetCellCenterWorld(mC.currentCell);
        pos1 = movC.currentFB.transform.position;
        Vector3 pos2 = mC.Move(mC.directionFacing);
        pos2 = new Vector3(pos2.x, pos2.y + movC.currentFB.transform.localPosition.y, pos2.z);


        Vector3 ray = (pos2 - pos1).normalized;
        RaycastHit2D _hit = Physics2D.Raycast(pos1, ray);

        GameObject g1 = null;
        if (!_hit.transform)
        {
            Vector3 destB = movC.currentFB.transform.position + (ray * maxShootDist);
            //Debug.Log(Vector3.Distance(movC.currentFB.transform.position, destB));
            showBomb(movC.currentFB.transform.position, destB);
            onlyFlying = true;
            return;
        }
        foreach (string tag in tagsToErode)
        {
            if (_hit.transform.CompareTag(tag))
            {
                g1 = _hit.transform.gameObject;
                break;
            }
        }
        if (g1 == null)
        {
            return;
        }

        hitObject = g1;
        showBomb(this.transform.position, _hit.point);


        //Debug.DrawRay(pos1, ray * 10, Color.red);

    }

    public void showBomb(Vector3 start, Vector3 end)
    {
        shooting = true;
        bomb.SetActive(true);
        bomb.transform.position = start;
        particles.SetActive(true);
        bomb.GetComponent<SpriteRenderer>().sortingLayerName = "Flying";

        switch (mC.directionFacing)
        {
            case 0:
                //particles.transform.rotation = new Quaternion(0, 0, 0, 0);
                particles.transform.localEulerAngles = new Vector3(particles.transform.localEulerAngles.x, particles.transform.localEulerAngles.y, 0);
                break;
            case 1:
                //particles.transform.rotation = new Quaternion(0, 0, -120, 0);
                particles.transform.localEulerAngles = new Vector3(particles.transform.localEulerAngles.x, particles.transform.localEulerAngles.y, 110);
                break;
            case 2:
                //particles.transform.rotation = new Quaternion(0, 0, 180, 0);
                particles.transform.localEulerAngles = new Vector3(particles.transform.localEulerAngles.x, particles.transform.localEulerAngles.y, 180);
                break;
            case 3:
                //particles.transform.rotation = new Quaternion(0, 0, -50, 0);
                particles.transform.localEulerAngles = new Vector3(particles.transform.localEulerAngles.x, particles.transform.localEulerAngles.y, -50);
                break;
        }

        bombDest = end;
    }

    public void applyErosion(GameObject g)
    {
        SpriteRenderer g_sR = g.GetComponent<SpriteRenderer>();
        var someValueFrom0To1 = currentTime / totalTime;
        currentTime += Time.deltaTime;
        g_sR.material.SetFloat("_Fade", 0.7f - someValueFrom0To1);
        //Debug.Log(someValueFrom0To1);
        if (someValueFrom0To1 >= 1)
        {
            eroding = false;
            hitObject.SetActive(false);
            hitObject = null;
            currentTime = 0;
        }
    }
}
