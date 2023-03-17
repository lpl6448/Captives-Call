using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    //Variables for the UI text objects
    public TextMeshProUGUI currentCharacterText;
    public TextMeshProUGUI nextCharacterText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI movesText;

    
    
    //Variables for the UI sprite objects
    public Image currentSprite;
    public Image nextSprite;
    public Image rightCharacterSprite;
    public Image leftCharacterSprite;
    public Image normalAbilitySprite;
    public Image specialAbilitySprite;
    public Image keySprite;
    public Image powerUpSprite;

    private LevelController levelController;
    private Party party;

    private PartyMember currentPartyMember;
    [SerializeField]
    public List<Sprite> characterPortsList;
    private Dictionary<PartyMember, Sprite> characterPorts;

    private int nextIndexer;
    private int rightIndexer;
    private int leftIndexer;
    private int partySize;


    void Start()
    {
        levelController = LevelController.Instance;
        party = Party.Instance;

        //level = levelController.

        characterPorts = new Dictionary<PartyMember, Sprite>();
        characterPorts.Add(PartyMember.Warlock, characterPortsList[0]);
        characterPorts.Add(PartyMember.Wizard, characterPortsList[1]);
        characterPorts.Add(PartyMember.Pickpocket, characterPortsList[2]);
        characterPorts.Add(PartyMember.Sailor, characterPortsList[3]);
        
        currentPartyMember = party.currentMember;
        partySize = party.partyMembers.Count;

        nextIndexer = 1;
        rightIndexer = 2;
        leftIndexer = 3;
        CheckForCurrent(1);
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: THESE ARE TEMPORARY KEYBOARD CONTROLS FOR SWITCHING CHARACTERS
        //TODO: WE NEED TO PATCH HOW CLICKING IS HANDLED!!!!!!!!!!!!!!
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveSelectionLeft();

        if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveSelectionRight();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchCharacter();
        }

        //if the current party member has changed, reset the indexers
        if (currentPartyMember != party.currentMember)
            ResetIndexers();
        //Update the currentPartyMember to this frame's state
        currentPartyMember=party.currentMember;
        //Update HUD with the current character sprite
        currentSprite.sprite = characterPorts[currentPartyMember];
        currentCharacterText.text = currentPartyMember.ToString();
        //Fill in the other sprite portraits with available party members
        nextSprite.sprite = characterPorts[party.partyMembers[(party.CurrentMemberIndex+nextIndexer)%partySize]];
        rightCharacterSprite.sprite = characterPorts[party.partyMembers[(party.CurrentMemberIndex + rightIndexer) % partySize]];
        leftCharacterSprite.sprite = characterPorts[party.partyMembers[(party.CurrentMemberIndex + leftIndexer) % partySize]];
        nextCharacterText.text = "Party Members: " + party.partyMembers[(party.CurrentMemberIndex + nextIndexer) % partySize].ToString();

        //Update the current level using the nextLevel value from the levelcontroller
        levelText.text = $"Level: {int.Parse(levelController.CurrentLevel)}";
        //Update moves taken with the property from the levelcontroller
        movesText.text = $"Moves: {levelController.MovesTaken}";
    }

    /// <summary>
    /// Reset the indexers to their default values
    /// </summary>
    private void ResetIndexers()
    {
        nextIndexer = 1;
        rightIndexer = 2;
        leftIndexer = 3;
        CheckForCurrent(1);
    }

    /// <summary>
    /// Shift the character selection wheel to the right
    /// </summary>
    public void MoveSelectionRight()
    {
        nextIndexer++;
        rightIndexer++;
        leftIndexer++;
        CheckForCurrent(1);
    }

    /// <summary>
    /// Shift character selection wheel to the left
    /// Does this via addition because mod calculations cannot be done with negative numbers
    /// </summary>
    public void MoveSelectionLeft() 
    {
        nextIndexer += partySize - 1;
        rightIndexer += partySize - 1;
        leftIndexer += partySize - 1;
        CheckForCurrent(-1);
    }

    /// <summary>
    /// Trigger the switch of characters to character in next slot
    /// </summary>
    public void SwitchCharacter()
    {
        if(NextNotCurrent())
            levelController.TriggerCharacterSwitch((party.CurrentMemberIndex + nextIndexer) % partySize);
    }

    /// <summary>
    /// check that none of the other party members match the current, if so, +1 again
    /// </summary>
    private void CheckForCurrent(int direction)
    {
        if (!NextNotCurrent())
            nextIndexer += direction;
        if ((rightIndexer + party.CurrentMemberIndex) % partySize == party.CurrentMemberIndex)
            rightIndexer += direction;
        if ((leftIndexer+party.CurrentMemberIndex) % partySize == party.CurrentMemberIndex)
            leftIndexer += direction;
    }

    /// <summary>
    /// Helper method to check that the character in next is not the same as the current character
    /// Returns false if next is current, true if the opposite
    /// </summary>
    /// <returns></returns>
    private bool NextNotCurrent()
    {
        return !((nextIndexer + party.CurrentMemberIndex) % partySize == party.CurrentMemberIndex);
    }
}
