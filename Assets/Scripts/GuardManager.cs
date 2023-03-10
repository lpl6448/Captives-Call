using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GuardManager : MonoBehaviour
{
    /// <summary>
    /// List to hold all guards that are in the scene
    /// </summary>
    public List<Guard> guardList;

    private Tilemap wallMap;



    // Start is called before the first frame update
    void Start()
    {
        //Find guards and add them to list
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag("Guard");
        foreach (GameObject g in foundObjects)
        {
            guardList.Add(g.GetComponent<DynamicObject>().GetComponent<Guard>());
        }
        //Find wall tilemap and assign to wallMap
        wallMap = GameObject.FindGameObjectsWithTag("Walls")[0].GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveGuards(Dictionary<TileBase, TileData> dataFromTiles, HighlightManager highlightManager)
    {
        foreach (Guard guard in guardList)
        {
            if (guard.canMove)
            {
                Vector3Int translate = toTranslate(guard);
                Vector3Int gridPosition = wallMap.WorldToCell(guard.transform.position + translate);
                bool accessible = guard.CanMove(gridPosition);
                if (!accessible)
                {
                    gridPosition -= translate * 2;
                    //Don't move if tile behind isn't accessible either
                    if (!guard.CanMove(gridPosition))
                        continue;
                    //Reverse guard.facing
                    switch (guard.facing)
                    {
                        case Directions.Up:
                            guard.ChangeDirections(Directions.Down);
                            break;
                        case Directions.Down:
                            guard.ChangeDirections(Directions.Up);
                            break;
                        case Directions.Left:
                            guard.ChangeDirections(Directions.Right);
                            break;
                        case Directions.Right:
                            guard.ChangeDirections(Directions.Left);
                            break;
                    }
                    highlightManager.HighlightGuardLOS(guardList);
                }
                LevelController.Instance.MoveDynamicObject(gridPosition, guard);
            }
        }
    }

    /// <summary>
    /// Check if the given party object is sharing a tile with any guard
    /// If so, return true
    /// </summary>
    /// <param name="party"></param>
    /// <returns></returns>
    public bool TouchingParty(Party party)
    {
        // More concise code that uses the new DynamicObject grid functionality to check if any Guards are on the Party's tile.
        // I just used this to test the grid system, but you can revert to the commented code below if it's more readable.
        return LevelController.Instance.GetDynamicObjectsOnTile<Guard>(party.TilePosition).Count > 0;
        //foreach (Guard guard in guardList)
        //{
        //    if (guard.transform.position == party.transform.position)
        //        return true;
        //}
        //return false;
    }

    public Vector3Int toTranslate(Guard guard)
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
