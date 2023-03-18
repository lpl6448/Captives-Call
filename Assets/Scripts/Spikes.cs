using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : DynamicObject
{
    /// <summary>
    /// Whether spikes are raised or not
    /// </summary>
    [SerializeField]
    private bool raised;

    /// <summary>
    /// Allows loading of sprites via unity editor
    /// </summary>
    [SerializeField]
    protected List<Sprite> sprites;

    public override bool IsTraversable(DynamicObject mover)
    {

        if (mover is Party)
            return true;
        if (mover is Guard)
            return true;
        if (mover is Boulder)
            return false;
        return false;
    }

    public override bool CanMove(Vector3Int tilePosition)
    {
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void PostAction()
    {
        Run(LevelController.Instance.stasisCount<1);
        if (raised)
        {
            //Check if anything is on spikes that will be impaled
            List<Guard> guards = LevelController.Instance.GetDynamicObjectsOnTile<Guard>(TilePosition);
            List<Party> party = LevelController.Instance.GetDynamicObjectsOnTile<Party>(TilePosition);
            List<DynamicObject> killable = new List<DynamicObject>();
            foreach (DynamicObject stabbed in guards)
            {
                killable.Add(stabbed);
            }
            foreach (DynamicObject stabbed in party)
            {
                killable.Add(stabbed);
            }
            //Impale the object
            foreach (DynamicObject stabbed in killable)
                LevelController.Instance.DestroyDynamicObject(TilePosition, stabbed, this);
        }

        ChangeSprite(gameObject.GetComponent<SpriteRenderer>());
    }

    public override void Run(bool canRun)
    {
        if(canRun)
            raised = !raised;
    }

    /// <summary>
    /// Switch between the door's open and closed sprites
    /// </summary>
    public void ChangeSprite(SpriteRenderer spriteRenderer)
    {
        if (raised)
            spriteRenderer.sprite = sprites[1];
        else
            spriteRenderer.sprite = sprites[0];
    }
}
