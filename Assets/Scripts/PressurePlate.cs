using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PressurePlate : DynamicObject
{
    /// <summary>
    /// Allows loading of sprites via unity editor
    /// </summary>
    [SerializeField]
    private List<Sprite> loadSprites;

    /// <summary>
    /// Holds all guard sprites, keyed by directions
    /// </summary>
    private Dictionary<int, Sprite> sprites;

    /// <summary>
    /// Object that will be changed by pressure plate status
    /// </summary>
    [SerializeField]
    public DynamicObject linkedObject;

    private Grid grid;
    //Holds a list of every potential trigger for the pressure plate in the scene
    private List<DynamicObject> potentialTriggers;
    //Holds if the pressure plate is pressed or not
    private bool isPressed;

    public override bool IsTraversable(DynamicObject mover)
    {
        if (mover is Party)
            return true;
        if (mover is Guard)
            return true;
        if (mover is Boulder)
            return true;
        return false;
    }

    public override bool CanMove(Vector3Int tilePosition)
    {
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        sprites = new Dictionary<int, Sprite>();
        for(int i=0;i<loadSprites.Count;i++)
        {
            sprites.Add(i, loadSprites[i]);
        }
        isPressed = false;
        grid = GameObject.FindGameObjectsWithTag("Grid")[0].GetComponent<Grid>();
        FindPotentialTriggers();
    }

    public override void PostAction() 
    {
        CheckIfPressed();
        SendSignal();
    }

    /// <summary>
    /// Sends the value of isPressed to the Run method of it's linked object
    /// </summary>
    public void SendSignal()
    {
        linkedObject.Run(isPressed);
    }

    /// <summary>
    /// Checks if the pressure plate is being pressed by a viable object 
    /// </summary>
    private void CheckIfPressed()
    {
        foreach(DynamicObject presser in potentialTriggers)
        {
            if(presser.TilePosition==grid.WorldToCell(this.transform.position))
            {
                isPressed = true;
                changeSprite();
                return;
            }
        }
        isPressed=false;
        changeSprite();
    }

    /// <summary>
    /// Fill potentialTriggers with ever boulder, guard, and party in the scene
    /// </summary>
    private void FindPotentialTriggers()
    {
        potentialTriggers = new List<DynamicObject>();
        GameObject[] foundObjects;
        string[] tags = { "Party", "Guard", "Boulder" };
        for (int i = 0; i < tags.Length; i++)
        {
            foundObjects = GameObject.FindGameObjectsWithTag(tags[i]);
            if (foundObjects.Length > 0)
            {
                foreach (GameObject g in foundObjects)
                {
                    potentialTriggers.Add(g.GetComponent<DynamicObject>());
                }
            }
        }
    }

    private void changeSprite()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (isPressed)
            spriteRenderer.sprite = sprites[1];
        else
            spriteRenderer.sprite = sprites[0];
    }
}
