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
    public int currentMemberIndex = 0;

    /// <summary>
    /// The PartyMember whose turn it is
    /// </summary>
    public PartyMember CurrentMember => partyMembers[currentMemberIndex];

    /// <summary>
    /// After every turn, update the active PartyMember
    /// </summary>
    public override void UpdateTick()
    {
        currentMemberIndex++;
        if (currentMemberIndex >= partyMembers.Count)
            currentMemberIndex = 0;
    }

    /// <summary>
    /// Performs the action associated with the current PartyMember
    /// </summary>
    /// <param name="target">Optional grid position passed in for the action</param>
    /// <returns>IEnumerator used for coroutines (for animating)</returns>
    public IEnumerator UseAbility(Vector3Int target)
    {
        yield break; // Remove once the actions are implemented
    }
}