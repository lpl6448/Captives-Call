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
    private List<Sprite> characterSprites;

    int level = 0;
    int moves = 0;

    string currentCharacterName = "";


    void Start()
    {
        levelController = LevelController.Instance;
        party = Party.Instance;

        //level = levelController.

        characterSprites = party.sprites;
        
        currentPartyMember = party.currentMember;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPartyMember == PartyMember.Warlock) //sprite index 0
        {
            currentSprite.sprite = characterSprites[0];
            currentCharacterText.text = "Warlock";
            nextSprite.sprite = characterSprites[1];
            nextCharacterText.text = "Next up: Wizard";
        }
        if (currentPartyMember == PartyMember.Wizard) //sprite index 1
        {
            currentSprite.sprite = characterSprites[1];
            currentCharacterText.text = "Wizard";
            nextSprite.sprite = characterSprites[2];
            nextCharacterText.text = "Next up: PickPocket";
        }
        if (currentPartyMember == PartyMember.Pickpocket) //sprite index 2
        {
            currentSprite.sprite = characterSprites[2];
            currentCharacterText.text = "Pickpocket";
            nextSprite.sprite = characterSprites[3];
            nextCharacterText.text = "Next up: Sailor";
        }
        if (currentPartyMember == PartyMember.Sailor) //sprite index 3
        {
            currentSprite.sprite = characterSprites[3];
            currentCharacterText.text = "Sailor";
            nextSprite.sprite = characterSprites[0];
            nextCharacterText.text = "Next up: Warlock";
        }




    }
}
