using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombiRobot : MonoBehaviour
{

    public bool shooting;

    public List<string> tagsToErode = new List<string>();

    public MovementController mC;

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


    // Start is called before the first frame update
    void Start()
    {

        tagsToErode.Add("Obstacle");
        mC = this.GetComponent<MovementController>();
        bomb = transform.GetChild(0).gameObject;
        robotSprite = transform.GetChild(1).gameObject;
        particles = transform.GetChild(2).gameObject;

        maxShootDist = 5;
    }

    // Update is called once per frame
    void Update()
    {
        // Controll up and down movement / elevation of player
        robotSprite.transform.localPosition = new Vector3(0, mC.ground.cellSize.y * elevation, 0);
        if (Input.GetKeyDown(mC.playerControlls.up))
        {
            elevation += 1;
        }
        if (Input.GetKeyDown(mC.playerControlls.down))
        {
            if (elevation > 0)
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
    }

    public void shootSeedBomb()
    {
        if (shooting) return;

        Vector3 pos1 = mC.ground.GetCellCenterWorld(mC.currentCell);
        Vector3 pos2 = mC.Move(mC.directionFacing);
        Vector3 ray = (pos2 - pos1).normalized;
        RaycastHit2D _hit = Physics2D.Raycast(pos1, ray);

        GameObject g1 = null;
        if (!_hit.transform)
        {
            Vector3 destB = this.transform.position + (ray * maxShootDist);
            Debug.Log(Vector3.Distance(this.transform.position, destB));
            showBomb(this.transform.position, destB);
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

        switch (mC.directionFacing)
        {
            case 0:
                particles.transform.rotation = new Quaternion(0, 0, 0, 0);
                break;
            case 1:
                particles.transform.rotation = new Quaternion(0, 0, -120, 0);
                break;
            case 2:
                particles.transform.rotation = new Quaternion(0, 0, 180, 0);
                break;
            case 3:
                particles.transform.rotation = new Quaternion(0, 0, -50, 0);
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
