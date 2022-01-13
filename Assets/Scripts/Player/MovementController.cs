using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static GameManager.movementSet;

public class MovementController : MonoBehaviour
{
    // Various Objects that are needed as Drag and Drop in Inspector
    public GameObject spawnPoint;
    public GameObject movingPoint;
    public GameObject collisionsG;
    public Tilemap collisionTileMap;
    public Tilemap elevationTileMap;
    public Grid ground;
    public int playerIndex;
    public GameManager gM;

    // Needs to be accessible for other Scripts
    public int directionFacing;


    // private variables
    public GameManager.movementSet playerControlls;

    // Only Public for Debbung
    public Vector3Int currentCell;
    public List<Vector3>  tileWorldLocations;
    public List<Vector3Int> collisionsInGrid;
    public List<Vector3Int> elevationsInGrid;
    public bool moving = true;

    private Wind w;
    private SpriteRenderer sR;

    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    public Sprite sprite4;

    public List<Sprite> sprites;
    void Start()
    {
        moving = true;
        sprites.Add(sprite4);
        sprites.Add(sprite3);
        sprites.Add(sprite2);
        sprites.Add(sprite1);

        sR = this.transform.GetComponent<SpriteRenderer>();
        sR.sprite = sprites[directionFacing];
        if (playerIndex == 2)
        {
            w = this.GetComponent<Wind>();
        }
        // Get all cell positions that have a collision
        tileWorldLocations = new List<Vector3>();
        foreach (var pos in collisionTileMap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = collisionTileMap.CellToWorld(localPlace);
            if (collisionTileMap.HasTile(localPlace))
            {
                tileWorldLocations.Add(place);
                collisionsInGrid.Add(ground.WorldToCell(place));
            }
        }

        // Get all cell positions that have elevation
        foreach (var pos in elevationTileMap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = elevationTileMap.CellToWorld(localPlace);
            if (elevationTileMap.HasTile(localPlace))
            {
                elevationsInGrid.Add(ground.WorldToCell(place));
            }
        }

        // Move Player to SpawnPoint and Movingpoint to Player
        this.transform.position =  ground.GetCellCenterWorld(ground.WorldToCell(spawnPoint.transform.position));
        //this.transform.position = spawnPoint.transform.position;
        this.movingPoint.transform.position = this.transform.position;

        playerControlls = gM.controlls[playerIndex - 1];
        
    }

    // Get the position of the center of the cell one cell away from the player in the direction given to the function
    Vector3 Move(int dir)
    {
        switch (dir)
        {
            case 0:
                return ground.GetCellCenterWorld(new Vector3Int(currentCell.x + 1, currentCell.y, currentCell.z));
            case 1:
                return ground.GetCellCenterWorld(new Vector3Int(currentCell.x, currentCell.y + 1, currentCell.z));
            case 2:
                return ground.GetCellCenterWorld(new Vector3Int(currentCell.x - 1, currentCell.y, currentCell.z));
            case 3:
                return ground.GetCellCenterWorld(new Vector3Int(currentCell.x, currentCell.y - 1, currentCell.z));
        }
        return new Vector3();
    }

    int incDir(int t)
    {
        if (t == 3)
        {
            t = 0;
        }
        else
        {
            t += 1;
        }
        return t;
    }

    int decDir(int t)
    {
        if (t == 0)
        {
            t = 3;
        }
        else
        {
            t -= 1;
        }
        return t;
    }

    void Update()
    {
        currentCell = ground.WorldToCell(transform.position);

        // Move player to moving point TODO: calculate delta time to make undepeding of fps
        if (moving)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, movingPoint.transform.position, 1* Time.deltaTime);
        }
        else
        {

        }

        // Calculate new position for player to move to depeding on input
        Vector3 newPosMP = this.transform.position;
        if (Input.GetKeyDown(playerControlls.forward))
        {
            newPosMP = Move(directionFacing);
        }
        if (Input.GetKeyDown(playerControlls.backward))
        {
            newPosMP = Move(incDir(incDir(directionFacing)));
        }
        if (Input.GetKeyDown(playerControlls.left))
        {
            directionFacing = incDir(directionFacing);
            sR.sprite = sprites[directionFacing];
        }
        if (Input.GetKeyDown(playerControlls.right))
        {
            directionFacing = decDir(directionFacing);
            sR.sprite = sprites[directionFacing];
        }

        // Check if new position is valid and if so move moving point there
        bool moveallowed = !collisionsInGrid.Contains(ground.WorldToCell(newPosMP));
        //bool changeAllowed = collisionsInGrid.Contains(currentCell);
        bool changeAllowed = false;

        if (playerIndex == 2 && !moveallowed)
        {
            if (w.elevation >= 1)
            {
                moveallowed = true;
            }
            if (elevationsInGrid.Contains(ground.WorldToCell(newPosMP)))
            {
                if (!(w.elevation >= 2))
                {
                    moveallowed = false;
                }
            }
            
        }

        if (moveallowed)
        {
            if (gM.isBoxOnCell(ground.WorldToCell(newPosMP), ground))
            {
                if (playerIndex == 2)
                {
                    if (gM.getBoxSOnCell(ground.WorldToCell(newPosMP), ground).elevation < w.elevation)
                    {
                        if (w.carryingBox)
                        {
                            if (! (gM.getBoxSOnCell(ground.WorldToCell(newPosMP), ground).elevation < w.elevation - 1))
                            {
                                moveallowed = false;
                            }
                        }
                    }
                    else
                    {
                        moveallowed = false;
                    }

                }
                else
                {
                    moveallowed = false;
                }
                
            }

        }
        // Only Allow change of position if player is already at moving point or moving point is in an invalid position
        if ((Vector3.Distance(this.transform.position, movingPoint.transform.position) < 0.01f || changeAllowed) && moveallowed)
        {
            movingPoint.transform.position = newPosMP;
        }

    }
}
