using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : DynamicObject
{
    /// <summary>
    /// Allows loading of sprites via unity editor
    /// </summary>
    [SerializeField]
    protected List<Sprite> loadSprites;

    /// <summary>
    /// Holds all guard sprites, keyed by directions
    /// </summary>
    protected Dictionary<int, Sprite> sprites;

    /// <summary>
    /// Tracks if the "door" is accessible to move on
    /// </summary>
    protected bool isOpen;

    public bool IsOpen => isOpen;

    protected bool willOpen;
    public bool WillOpen
    {
        get  { return willOpen;} 
        set 
        {
            if (!triggered)
            {
                willOpen = value;
                triggered = value;
            }
        } 
    }

    /// <summary>
    /// Holds if the gate has been triggered this turn already
    /// </summary>
    protected bool triggered;

    public override bool IsTraversable(DynamicObject mover)
    {
        if (willOpen||isOpen)
        {
            if (mover is Party)
                return true;
            if (mover is Guard)
                return true;
            if (mover is Boulder)
                return true;
        }
        return false;
    }

    public override bool CanMove(Vector3Int tilePosition)
    {
        return false;
    }

    public override void Run(bool canRun)
    {
        if (!triggered) { isOpen = canRun; }
        if(isOpen) { triggered = true; }
    }

    public override void PreAction()
    {
        triggered = false;
    }


    protected void Awake()
    {
        sprites = new Dictionary<int, Sprite>();
        for (int i = 0; i < loadSprites.Count; i++)
        {
            sprites.Add(i, loadSprites[i]);
        }
        isOpen = false;
        willOpen = false;
        triggered = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Switch between the door's open and closed sprites
    /// </summary>
    public void ChangeSprite(SpriteRenderer spriteRenderer)
    {
        if (isOpen)
            spriteRenderer.sprite = sprites[1];
        else
            spriteRenderer.sprite = sprites[0];
    }
}
