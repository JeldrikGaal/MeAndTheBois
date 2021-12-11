using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovementController : MonoBehaviour
{
    public GameObject spawnPoint;
    public Grid ground;
    public bool moving = false;
    public GameObject movingPoint;

    public Vector2 currentCell;
    public GameObject collisionsG;
    public CompositeCollider2D collisions;
    public List<Vector2> collisionCells;
    public List<Vector2> collisionCellsPos;

    public List<Vector3Int> collisionsInGrid;

    public List<Vector3>  tileWorldLocations;

    public Tilemap test;

    // Start is called before the first frame update
    void Start()
    {
        collisions = collisionsG.GetComponent<CompositeCollider2D>();

        Tilemap tilemap = collisionsG.GetComponent<Tilemap>();
        tilemap = test;

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        tileWorldLocations = new List<Vector3>();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tilemap.CellToWorld(localPlace);
            if (tilemap.HasTile(localPlace))
            {
                tileWorldLocations.Add(place);
                collisionsInGrid.Add(ground.WorldToCell(place));
            }
        }

        /*for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    collisionCells.Add(new Vector2(x, y));
                    //Vector3 temp = tilemap.CellToWorld(new Vector3Int(x, y));
                    Vector3 temp = tilemap.GetCellCenterWorld(new Vector3Int(x, y));
                    collisionCellsPos.Add(temp);
                    collisionsInGrid.Add(ground.WorldToCell(temp));
                }
                else
                {

                }


            }
        }*/


        this.transform.position =  ground.GetCellCenterWorld(ground.WorldToCell(spawnPoint.transform.position));
        this.movingPoint.transform.position = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(test.WorldToCell(movingPoint.transform.position));
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log(ground.WorldToCell(this.transform.position));

            Vector3 newPos = ground.GetCellCenterWorld(ground.WorldToCell(this.transform.position));
            //newPos = new Vector3(newPos.x + (ground.cellSize.x / 2), newPos.y + (ground.cellSize.y / 2), newPos.z);
            this.transform.position = newPos;
            Debug.Log(newPos);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            moving = !moving;
        }

        if (moving)
        {
            if (!collisions.bounds.Contains(movingPoint.transform.position))
            {

                this.transform.position = Vector3.MoveTowards(this.transform.position, movingPoint.transform.position, 0.01f);
            }

        }

        Vector3 newPosMP = this.transform.position;
        if (true)
        {
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

            bool moveallowed = !collisionsInGrid.Contains(ground.WorldToCell(newPosMP));
            if ((Vector3.Distance(this.transform.position, movingPoint.transform.position) < 0.01f || collisions.bounds.Contains(transform.position)) && moveallowed)
            {
                movingPoint.transform.position = newPosMP;
            }

        }
    }
}
