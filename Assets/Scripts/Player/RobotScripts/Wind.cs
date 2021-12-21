using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{

    public SpriteRenderer shadow;
    public GameObject robotSprite;
    public GameManager gM;
    public MovementController movC;
    public int elevation = 1;

    public float shadowStartSize;

    // Start is called before the first frame update
    void Start()
    {
        shadow = this.transform.GetComponent<SpriteRenderer>();
        robotSprite = this.transform.GetChild(0).gameObject;
        movC = this.transform.GetComponent<MovementController>();

        robotSprite.transform.localPosition = new Vector3(0, movC.ground.cellSize.y * elevation, 0);
        shadowStartSize = shadow.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        robotSprite.transform.localPosition = new Vector3(0, movC.ground.cellSize.y * elevation, 0);
        if (Input.GetKeyDown(movC.playerControlls.up))
        {
            elevation += 1;
        }
        if (Input.GetKeyDown(movC.playerControlls.down))
        {
            if (elevation > 0)
            {
                elevation -= 1;
            }
        }

        shadow.size = new Vector2(shadowStartSize - elevation * 0.02f, shadowStartSize - elevation * 0.02f);
    }
}
