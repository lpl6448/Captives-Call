using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Door
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

    // Start is called before the first frame update
    void Start()
    {
        sprites = new Dictionary<int, Sprite>();
        for (int i = 0; i < loadSprites.Count; i++)
        {
            sprites.Add(i, loadSprites[i]);
        }
        isOpen = false;
    }

    public override void PostAction()
    {
        if (!isOpen)
        {
            //Check if anything is on gate that will be crushed by gate
            List<Guard> guards = LevelController.Instance.GetDynamicObjectsOnTile<Guard>(TilePosition);
            List<Party> party = LevelController.Instance.GetDynamicObjectsOnTile<Party>(TilePosition);
            List<DynamicObject> crushable = new List<DynamicObject>();
            foreach (DynamicObject crunchy in guards)
            {
                crushable.Add(crunchy);
            }
            foreach (DynamicObject cruncy in party)
            {
                crushable.Add(cruncy);
            }
            //Crush the object
            foreach (DynamicObject crunchy in crushable)
                LevelController.Instance.DestroyDynamicObject(TilePosition, crunchy, this);

            //Check if a boulder is present to stop the gate from closing
            List<Boulder> boulders = LevelController.Instance.GetDynamicObjectsOnTile<Boulder>(TilePosition);
            foreach (Boulder boulder in boulders)
            {
                isOpen = true;
            }
        }

        changeSprite();
    }

    private void changeSprite()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (isOpen)
            spriteRenderer.sprite = sprites[1];
        else
            spriteRenderer.sprite = sprites[0];
    }
}
