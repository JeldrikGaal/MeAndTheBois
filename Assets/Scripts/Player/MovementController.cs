using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovementController : MonoBehaviour
{
    // Various Objects that are needed as Drag and Drop in Inspector
    public GameObject spawnPoint;
    public GameObject movingPoint;
    public GameObject collisionsG;
    public Tilemap collisionTileMap;
    public Grid ground;
   

    // Only Public for Debbung
    public Vector2 currentCell;
    public List<Vector3>  tileWorldLocations;
    public List<Vector3Int> collisionsInGrid;
    public bool moving = false;


    void Start()
    {
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

        // Move Player to SpawnPoint and Movingpoint to Player
        this.transform.position =  ground.GetCellCenterWorld(ground.WorldToCell(spawnPoint.transform.position));
        this.movingPoint.transform.position = this.transform.position;
    }


    void Update()
    {
        // Toggle Movement
        if (Input.GetKeyDown(KeyCode.M))
        {
            moving = !moving;
        }

        // Move player to moving point TODO: calculate delta time to make undepeding of fps
        if (moving)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, movingPoint.transform.position, 0.01f);
        }


        // Calculate new position for player to move to depeding on input
        Vector3 newPosMP = this.transform.position;
        if (Input.GetKeyDown(KeyCode.W))
        {
            newPosMP = new Vector3(this.transform.position.x + ((ground.cellSize.x / 2) + (ground.cellGap.x / 2)), this.transform.position.y + (ground.cellSize.y / 2), this.transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            newPosMP = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2) + (ground.cellGap.x / 2)), this.transform.position.y - (ground.cellSize.y / 2), this.transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            newPosMP = new Vector3(this.transform.position.x - ((ground.cellSize.x / 2) + (ground.cellGap.x / 2)), this.transform.position.y + (ground.cellSize.y / 2), this.transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            newPosMP = new Vector3(this.transform.position.x + ((ground.cellSize.x / 2) + (ground.cellGap.x / 2)), this.transform.position.y - (ground.cellSize.y / 2), this.transform.position.z);
        }

        // Check if new position is valid and if so move moving point there
        bool moveallowed = !collisionsInGrid.Contains(ground.WorldToCell(newPosMP));
        bool changeAllowed = collisionsInGrid.Contains(ground.WorldToCell(transform.position));
        // Only Allow change of position if player is already at moving point or moving point is in an invalid position
        if ((Vector3.Distance(this.transform.position, movingPoint.transform.position) < 0.01f || changeAllowed) && moveallowed)
        {
            movingPoint.transform.position = newPosMP;
        }

    }
}
