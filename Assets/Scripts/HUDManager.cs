using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    //Variables for the UI text objects
    public TextMeshPro currentCharacterText;
    public TextMeshPro nextCharacterText;
    public TextMeshPro levelText;
    public TextMeshPro movesText;

    
    
    //Variables for the UI sprite objects
    public Image currentSprite;
    public Image nextSprite;
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

    int level = 0;
    int moves = 0;

    string currentCharacterName = "";


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

        
    }

    // Update is called once per frame
    void Update()
    {
        //Update the currentPartyMember to this frame's state
        currentPartyMember=party.currentMember;
        //Update HUD with most recent info
        currentSprite.sprite = characterPorts[currentPartyMember];
        currentCharacterText.text = currentPartyMember.ToString();
        PartyMember nextMem;
        if(party.CurrentMemberIndex!=party.partyMembers.Count-1)
        {
            nextMem = party.partyMembers[party.CurrentMemberIndex + 1];
        }
        else
        {
            nextMem = party.partyMembers[0];
        }
        nextSprite.sprite = characterPorts[nextMem];
        nextCharacterText.text = "Next up: " + nextMem.ToString();
    }
}
