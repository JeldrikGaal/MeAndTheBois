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
    public bool moving = false;

    private Wind w;

    void Start()
    {
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


    void Update()
    {
        currentCell = ground.WorldToCell(transform.position);
        // Toggle Movement
        if (Input.GetKeyDown(KeyCode.M))
        {
            moving = !moving;
        }

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
            //newPosMP = new Vector3(this.transform.position.x + ((ground.cellSize.x / 2), this.transform.position.y + (ground.cellSize.y / 2), this.transform.position.z);
            newPosMP = ground.GetCellCenterWorld(new Vector3Int(currentCell.x + 1, currentCell.y, currentCell.z));
            directionFacing = 0;
        }
        if (Input.GetKeyDown(playerControlls.backward))
        {
            //newPosMP = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2), this.transform.position.y - (ground.cellSize.y / 2), this.transform.position.z);
            newPosMP = ground.GetCellCenterWorld(new Vector3Int(currentCell.x - 1, currentCell.y, currentCell.z));
            directionFacing = 1;
        }
        if (Input.GetKeyDown(playerControlls.left))
        {
            //newPosMP = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2), this.transform.position.y + (ground.cellSize.y / 2), this.transform.position.z);
            newPosMP = ground.GetCellCenterWorld(new Vector3Int(currentCell.x, currentCell.y + 1, currentCell.z));
            directionFacing = 2;
        }
        if (Input.GetKeyDown(playerControlls.right))
        {
            //newPosMP = new Vector3(this.transform.position.x + ((ground.cellSize.x / 2), this.transform.position.y - (ground.cellSize.y / 2), this.transform.position.z);
            newPosMP = ground.GetCellCenterWorld(new Vector3Int(currentCell.x, currentCell.y - 1, currentCell.z));
            directionFacing = 3;
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
                    if (gM.getBoxSOnCell(ground.WorldToCell(newPosMP), ground).elevation <= w.elevation)
                    {

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
