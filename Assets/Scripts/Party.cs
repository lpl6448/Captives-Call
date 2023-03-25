using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

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
    /// SpriteRenderer to apply the transparency/alpha effect to when the party is hidden
    /// </summary>
    [SerializeField]
    private SpriteRenderer hiddenAlphaEffect;

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

    public int hidden;
    public int Hidden => hidden;

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

    private bool moving = false;

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

        tile = LevelController.Instance.floorMap.GetTile(tilePosition);
        if (tile != null && !LevelController.Instance.GetTileData(tile).isPartyAccessible)
            return false;

        // Party cannot move to a tile with another DynamicObject on it if the object is not traversable
        foreach (DynamicObject collidingObj in LevelController.Instance.GetDynamicObjectsOnTile(tilePosition))
            if (!collidingObj.IsTraversable(this))
                return false;

        return true;
    }

    //Before every turn, drop cloaking by 1
    public override void PreAction()
    {
        moving = false;
        if (hidden > 0)
            hidden--;
    }

    /// <summary>
    /// After every turn, update the active PartyMember
    /// </summary>
    public override void PostAction()
    {
        //Check for key collision
        GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
        if (keys.Length > 0)
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
        //Check for coin collision
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        if(coins.Length>0)
        {
            foreach(GameObject coin in coins)
            {
                DynamicObject dCoin = coin.GetComponent<DynamicObject>();
                if (TilePosition == dCoin.TilePosition)
                {
                    int.TryParse(LevelController.Instance.CurrentLevel, out int currentLevel);
                    GameData.CollectCoin(currentLevel);
                    LevelController.Instance.DestroyDynamicObject(dCoin.TilePosition, dCoin);
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
                    LevelController.Instance.DestroyDynamicObject(TilePosition, pUp.GetComponent<DynamicObject>(), this);
                }
            }
        }
        //Check for locked door collision
        List<LockedDoor> locks = LevelController.Instance.GetDynamicObjectsOnTile<LockedDoor>(TilePosition);
        if (locks.Count > 0)
        {
            if (!locks[0].IsOpen)
                keyCount = locks[0].Unlock(keyCount);
        }

        //currentMemberIndex++;
        //if (currentMemberIndex >= partyMembers.Count)
        //    currentMemberIndex = 0;
        currentMember = partyMembers[currentMemberIndex];

        UpdateSprite();

        if (!moving)
            EndMove();
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
                if (boulder.Count > 0)
                {
                    // Telekinetic Push
                    Vector3Int movementDir = target - TilePosition;
                    LevelController.Instance.MoveDynamicObject(target + movementDir, boulder[0], this);
                    audio.Boulder();
                    break;
                }
                if (bWall.Count > 0)
                {
                    bWall[0].Run(true, this);
                    bWall[0].AnimationTrigger("activate");
                    poweredUp = false;
                    break;
                }
                break;
            case PartyMember.Wizard:
                LevelController.Instance.StasisCount = 3;
                if (poweredUp)
                {
                    LevelController.Instance.DistortionCount = 3;
                    poweredUp = false;
                }
                break;
            case PartyMember.Pickpocket:
                List<Guard> guards = LevelController.Instance.GetDynamicObjectsOnTile<Guard>(target);
                if (guards.Count > 0)
                {
                    StartAnimation(DoSneakAttack(guards));
                    return;
                }
                if (poweredUp)
                {
                    hidden = 2;
                    LevelController.Instance.MoveDynamicObject(target, this);
                    poweredUp = false;
                }
                break;
            case PartyMember.Sailor:
                if (poweredUp && target != TilePosition)
                {
                    LevelController.Instance.MoveDynamicObject(target, this);
                    poweredUp = false;
                    return;
                }
                StartAnimation(DoShanty());
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
                        if (!bWall.IsOpen)
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
            case PartyMember.Pickpocket:
                if (Mathf.Abs(target.x - TilePosition.x) + Mathf.Abs(target.y - TilePosition.y) != 1)
                    return false;
                //Sneak Attack
                List<Guard> guards = LevelController.Instance.GetDynamicObjectsOnTile<Guard>(target);
                if (guards.Count > 0)
                    return true;
                //Sneak
                if (poweredUp)
                    return true;
                return false;
            case PartyMember.Sailor:
                //Grapple dash
                if (((Mathf.Abs(target.x - TilePosition.x) == 2 && target.y == TilePosition.y) ||
                    (Mathf.Abs(target.y - TilePosition.y) == 2 && target.x == TilePosition.x)) && poweredUp && CanMove(target))
                {
                    //Check if tile being skipped can be dashed through
                    if(target.y==TilePosition.y)
                    {
                        if(target.x>TilePosition.x)
                        {
                            Vector3Int inBetween = new Vector3Int(TilePosition.x + 1, TilePosition.y, TilePosition.z);
                            return CanDashThrough(inBetween);
                        }
                        else
                        {
                            Vector3Int inBetween = new Vector3Int(TilePosition.x - 1, TilePosition.y, TilePosition.z);
                            return CanDashThrough(inBetween);
                        }
                    }
                    else
                    {
                        if (target.y > TilePosition.y)
                        {
                            Vector3Int inBetween = new Vector3Int(TilePosition.x, TilePosition.y+1, TilePosition.z);
                            return CanDashThrough(inBetween);
                        }
                        else
                        {
                            Vector3Int inBetween = new Vector3Int(TilePosition.x, TilePosition.y-1, TilePosition.z);
                            return CanDashThrough(inBetween);
                        }
                    }
                }
                //Shanty
                if (target == TilePosition)
                    return true;
                return false;
        }

        // For any unimplemented PartyMembers, return false
        return false;
    }

    /// <summary>
    /// Helper method to determing if the party can dash through a certain tile
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool CanDashThrough(Vector3Int target)
    {
        // Party cannot dash through a wall
        TileBase tile = LevelController.Instance.wallMap.GetTile(target);
        if (tile != null && !LevelController.Instance.GetTileData(tile).isPartyAccessible)
            return false;

        tile = LevelController.Instance.floorMap.GetTile(target);
        if (tile != null && !LevelController.Instance.GetTileData(tile).isDashable)
            return false;

        // Party cannot move to a tile with another DynamicObject on it if the object is not traversable
        foreach (DynamicObject collidingObj in LevelController.Instance.GetDynamicObjectsOnTile(target))
            if (!collidingObj.IsTraversable(this))
                return false;
        
        return true;
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
        moving = true;
        Vector3 start = transform.position;
        Vector3 end = LevelController.Instance.CellToWorld(TilePosition) + new Vector3(0.5f, 0.5f, 0);
        moveCoroutine = StartAnimation(MoveAnimation(start, end, context is float ? (float)context : 1));
    }

    private void EndMove()
    {
        // Notify any pressure plates that the object has finished moving
        foreach (DynamicObject dobj in LevelController.Instance.GetDynamicObjectsOnTile<PressurePlate>(TilePosition))
            dobj.AnimationTrigger("press");
    }

    public override void DestroyObject(object context)
    {
        StartAnimation(DestroyAnimation(context as Guard));
        dead = true;
    }

    private IEnumerator MoveAnimation(Vector3 start, Vector3 end, float multiplier)
    {
        yield return AnimationUtility.StandardLerp(transform, start, end, AnimationUtility.StandardAnimationDuration * multiplier);

        // Notify any pressure plates that the object has finished moving
        foreach (DynamicObject dobj in LevelController.Instance.GetDynamicObjectsOnTile(TilePosition))
            dobj.AnimationTrigger("press");

        EndMove();
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

        StopAllAnimations();
        //Destroy(gameObject);
    }

    private IEnumerator DoShanty()
    {
        // Once we have shanty music in, we could let the music play for a few seconds before triggering the UI effect

        UIEffects.Instance.AnimateArrowRotate();
        yield return new WaitForSeconds(4 / 3f);
        GameObject[] listeners = GameObject.FindGameObjectsWithTag("Guard");
        foreach (GameObject guard in listeners)
        {
            guard.GetComponent<Guard>().HearShanty();
        }

        StopAnimation();
    }

    private IEnumerator DoSneakAttack(List<Guard> guards)
    {
        foreach (Guard guard in guards)
            LevelController.Instance.DestroyDynamicObject(guard.TilePosition, guard, "sneak-attack");

        Vector3 tileWorld = LevelController.Instance.CellToWorld(TilePosition) + new Vector3(0.5f, 0.5f, 0);
        Vector3 guardWorld = LevelController.Instance.CellToWorld(guards[0].TilePosition) + new Vector3(0.5f, 0.5f, 0);
        yield return AnimationUtility.StandardLerp(transform, tileWorld, Vector3.Lerp(tileWorld, guardWorld, 0.5f), 0.2f);

        foreach (Guard guard in guards)
            guard.AnimationTrigger("kill");

        yield return AnimationUtility.StandardLerp(transform, Vector3.Lerp(tileWorld, guardWorld, 0.5f), tileWorld, 0.5f);

        StopAnimation();
    }

    /// <summary>
    /// Every frame, checks if the party is hidden and updates the alpha accordingly
    /// </summary>
    private void Update()
    {
        Color color = hiddenAlphaEffect.color;
        if (hidden > 0)
        {
            float frequency = hidden == 2 ? 0.75f
                : hidden == 1 ? 1.5f
                : 0;
            float t = Mathf.Cos(Mathf.PI * 2 * frequency * Time.time) / 2 + 0.5f;
            float st = t * t;
            color.a = Mathf.Lerp(0.7f, 0.35f, st);
        }
        else
            color.a = 1;
        hiddenAlphaEffect.color = color;
    }
}