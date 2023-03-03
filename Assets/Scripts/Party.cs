using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Main playable unit in the game, containing a list of PartyMembers and controlling
/// the logic for player actions and other game mechanics.
/// </summary>
public class Party : DynamicObject
{
    /// <summary>
    /// List containing all PartyMembers that are in this party, in the turn order specified
    /// </summary>
    public List<PartyMember> partyMembers;

    /// <summary>
    /// Index of the PartyMember whose turn it is (from partyMembers)
    /// </summary>
    private int currentMemberIndex = 0;

    /// <summary>
    /// The PartyMember whose turn it is
    /// </summary>
    public PartyMember currentMember;

    /// <summary>
    /// List containing the sprites for each character in the party
    /// </summary>
    [SerializeField]
    private List<Sprite> sprites;

    /// <summary>
    /// Dictionary allows sprites to be selected with PartyMember enum as a key
    /// </summary>
    private Dictionary<PartyMember, Sprite> charSprites;

    public override bool IsTraversable(DynamicObject mover)
    {
        if (mover is Party)
            return false; // Multiple Parties (if we implement them) cannot occupy the same space
        return true;
    }

    /// <summary>
    /// After every turn, update the active PartyMember
    /// </summary>
    public override void PostMovement()
    {
        currentMemberIndex++;
        if (currentMemberIndex >= partyMembers.Count)
            currentMemberIndex = 0;
        currentMember = partyMembers[currentMemberIndex];

        UpdateSprite();
    }

    /// <summary>
    /// Performs the action associated with the current PartyMember
    /// </summary>
    /// <param name="target">Optional grid position passed in for the action</param>
    /// <returns>IEnumerator used for coroutines (for animating--unused currently)</returns>
    public void UseAbility(Vector3Int target)
    {

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

    public void UpdateSprite()
    {
        //Change sprite to current member
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = charSprites[currentMember];
    }
}