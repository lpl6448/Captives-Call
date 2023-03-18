using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Main playable unit in the game, containing a list of PartyMembers and controlling
/// the logic for player actions and other game mechanics.
/// </summary>
public class Party : DynamicObject
{
    /// <summary>
    /// Singleton variable (if we want to use this convention) to allow easy access
    /// to the Party from any script in the scene
    /// </summary>
    public static Party Instance;

    /// <summary>
    /// List containing all PartyMembers that are in this party, in the turn order specified
    /// </summary>
    public List<PartyMember> partyMembers;

    /// <summary>
    /// Index of the PartyMember whose turn it is (from partyMembers)
    /// </summary>
    private int currentMemberIndex = 0;
    public int CurrentMemberIndex => currentMemberIndex;

    /// <summary>
    /// The PartyMember whose turn it is
    /// </summary>
    public PartyMember currentMember;

    public int nextMember;

    /// <summary>
    /// Whether the party has a powerUp or not
    /// </summary>
    public bool poweredUp;

    /// <summary>
    /// Whether this Party has been caught or not
    /// </summary>
    public bool dead;

    /// <summary>
    /// Tracks whether the party is holding a key or not
    /// </summary>
    public int keyCount;

    /// <summary>
    /// List containing the sprites for each character in the party
    /// </summary>
    [SerializeField]
    public List<Sprite> sprites;

    /// <summary>
    /// Dictionary allows sprites to be selected with PartyMember enum as a key
    /// </summary>
    public Dictionary<PartyMember, Sprite> charSprites;

    /// <summary>
    /// Reference to the current moveCoroutine (if there is one) so that it can be interrupted if the party is caught
    /// </summary>
    private Coroutine moveCoroutine;

    public override bool IsTraversable(DynamicObject mover)
    {
        if (mover is Party)
            return false; // Multiple Parties (if we implement them) cannot occupy the same space
        if (mover is Boulder)
            return false;
        return true;
    }

    public override bool CanMove(Vector3Int tilePosition)
    {
        // Party cannot move to a tile with a wall on it, unless it is brush
        TileBase tile = LevelController.Instance.wallMap.GetTile(tilePosition);
        if (tile != null && !LevelController.Instance.GetTileData(tile).isPartyAccessible)
            return false;

        // Party cannot move to a tile with another DynamicObject on it if the object is not traversable
        foreach (DynamicObject collidingObj in LevelController.Instance.GetDynamicObjectsOnTile(tilePosition))
            if (!collidingObj.IsTraversable(this))
                return false;

        return true;
    }

    /// <summary>
    /// After every turn, update the active PartyMember
    /// </summary>
    public override void PostAction()
    {
        //Check for key collision
        GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
        if(keys.Length>0)
        {
            foreach (GameObject key in keys)
            {
                if (TilePosition == key.GetComponent<DynamicObject>().TilePosition)
                {
                    keyCount++;
                    Destroy(key);
                }
            }
        }
        //Check for powerup collision
        GameObject[] powerUps = GameObject.FindGameObjectsWithTag("Power");
        if (powerUps.Length > 0)
        {
            foreach (GameObject pUp in powerUps)
            {
                if (TilePosition == pUp.GetComponent<DynamicObject>().TilePosition)
                {
                    poweredUp = true;
                    Destroy(pUp);
                }
            }
        }
        //Check for locked door collision
        List<LockedDoor> locks = LevelController.Instance.GetDynamicObjectsOnTile<LockedDoor>(TilePosition);
        if(locks.Count>0)
        {
            keyCount = locks[0].Unlock(keyCount);
        }

        //currentMemberIndex++;
        //if (currentMemberIndex >= partyMembers.Count)
        //    currentMemberIndex = 0;
        currentMember = partyMembers[currentMemberIndex];

        UpdateSprite();
    }

    /// <summary>
    /// Performs the action associated with the current PartyMember
    /// </summary>
    /// <param name="target">Optional grid position passed in for the action</param>
    /// <returns>IEnumerator used for coroutines (for animating--unused currently)</returns>
    public void UseAbility(Vector3Int target, FxController audio)
    {
        switch (currentMember)
        {
            case PartyMember.Warlock:
                //Check target tile for which ability is being used
                List<Boulder> boulder = LevelController.Instance.GetDynamicObjectsOnTile<Boulder>(target);
                List<BreakableWall> bWall = LevelController.Instance.GetDynamicObjectsOnTile<BreakableWall>(target);
                if (boulder.Count>0)
                {
                    // Telekinetic Push
                    Vector3Int movementDir = target - TilePosition;
                    LevelController.Instance.MoveDynamicObject(target + movementDir, boulder[0], this);
                    audio.Boulder();
                    break;
                }
                if(bWall.Count>0) 
                {
                    bWall[0].Run(true);
                    bWall[0].ChangeSprite(bWall[0].GetComponent<SpriteRenderer>());
                    poweredUp = false;
                    break;
                }
                break;
            case PartyMember.Wizard:
                LevelController.Instance.stasisCount = 3;
                if (poweredUp)
                {
                    LevelController.Instance.distortionCount = 3;
                    poweredUp = false;
                }
                break;
        }
    }

    /// <summary>
    /// Checks whether the action associated with the current PartyMember can be done
    /// on the given target position (if applicable to the ability)
    /// </summary>
    /// <param name="target">Optional grid position passed in for the ability</param>
    /// <returns>Whether the current PartyMember can use their ability on the target</returns>
    public bool CanUseAbility(Vector3Int target)
    {
        switch (currentMember)
        {
            case PartyMember.Warlock:
                if (Mathf.Abs(target.x - TilePosition.x) + Mathf.Abs(target.y - TilePosition.y) != 1)
                    return false;

                // Telekinetic Push
                List<Boulder> boulders = LevelController.Instance.GetDynamicObjectsOnTile<Boulder>(target);
                if (boulders.Count > 0)
                {
                    Boulder boulder = boulders[0];
                    Vector3Int movementDir = target - TilePosition;
                    // Check if the boulder on the target space can move forward
                    if (boulder.CanMove(target + movementDir))
                        return true;
                    return false;
                }

                //Warlock Crush
                if (poweredUp)
                {
                    List<BreakableWall> breakables = LevelController.Instance.GetDynamicObjectsOnTile<BreakableWall>(target);
                    if (breakables.Count > 0)
                    {
                        BreakableWall bWall = breakables[0];
                        if(!bWall.IsOpen)
                        {
                            return true;
                        }
                        return false;
                    }
                }
                return false;
            case PartyMember.Wizard:
                if (target == TilePosition)
                    return true;
                return false;
        }

        // For any unimplemented PartyMembers, return false
        return false;
    }

    /// <summary>
    /// Set the currentMember field to the current member and load all character sprites
    /// </summary>
    public override void Initialize()
    {
        currentMember = partyMembers[currentMemberIndex];
        charSprites = new Dictionary<PartyMember, Sprite>();
        charSprites.Add(PartyMember.Warlock, sprites[0]);
        charSprites.Add(PartyMember.Wizard, sprites[1]);
        charSprites.Add(PartyMember.Pickpocket, sprites[2]);
        charSprites.Add(PartyMember.Sailor, sprites[3]);
        UpdateSprite();
    }
    private void Awake()
    {
        Instance = this;
    }

    public void ChangeCharacter(int index)
    {
        currentMemberIndex = index;
    }

    public void UpdateSprite()
    {
        //Change sprite to current member
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = charSprites[currentMember];
    }

    public override void Move(Vector3Int tilePosition, object context)
    {
        Vector3 start = transform.position;
        Vector3 end = LevelController.Instance.CellToWorld(TilePosition) + new Vector3(0.5f, 0.5f, 0);
        moveCoroutine = StartAnimation(MoveAnimation(start, end));
    }

    public override void DestroyObject(object context)
    {
        StartAnimation(DestroyAnimation(context as Guard));
    }

    private IEnumerator MoveAnimation(Vector3 start, Vector3 end)
    {
        yield return AnimationUtility.StandardLerp(transform, start, end, AnimationUtility.StandardAnimationDuration);
        StopAnimation();
    }

    private IEnumerator DestroyAnimation(Guard collide)
    {
        if (collide != null)
        {
            yield return new WaitForSeconds(AnimationUtility.StandardAnimationDuration / 2);
            collide.StopAllAnimations();
            StopCoroutine(moveCoroutine);
        }
        else
        {
            yield return new WaitForSeconds(AnimationUtility.StandardAnimationDuration);
        }

        dead = true;
        StopAllAnimations();
        //Destroy(gameObject);
    }
}