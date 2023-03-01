using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GuardManager : MonoBehaviour
{
    /// <summary>
    /// List to hold all guards that are in the scene
    /// </summary>
    [SerializeField]
    public List<Guard> guardList;
    [SerializeField]
    private Tilemap wallMap;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveGuards(Dictionary<TileBase, TileData> dataFromTiles)
    {
        foreach (Guard guard in guardList)
        {
            if (guard.canMove)
            {
                Vector3Int translate = toTranslate(guard);
                Vector3Int gridPosition = wallMap.WorldToCell(guard.transform.position + translate);
                TileBase target = wallMap.GetTile(gridPosition);
                bool accessible = true;
                if (target != null)
                    accessible = dataFromTiles[target].isAccessible;
                if (accessible)
                {
                    guard.transform.Translate(translate);
                }
                else
                {
                    guard.transform.Translate(-1 * translate);
                    //Reverse guard.facing
                    switch (guard.facing)
                    {
                        case Directions.Up:
                            guard.facing = Directions.Down;
                            break;
                        case Directions.Down:
                            guard.facing = Directions.Up;
                            break;
                        case Directions.Left:
                            guard.facing = Directions.Right;
                            break;
                        case Directions.Right:
                            guard.facing = Directions.Left;
                            break;
                    }
                }
            }
        }
    }

    private Vector3Int toTranslate(Guard guard)
    {
        switch (guard.facing)
        {
            case Directions.Up:
                return new Vector3Int(0, 1, 0);
            case Directions.Down:
                return new Vector3Int(0, -1, 0);
            case Directions.Left:
                return new Vector3Int(-1, 0, 0);
            case Directions.Right:
                return new Vector3Int(1, 0, 0);
            default:
                return new Vector3Int();
        }
    }
}
