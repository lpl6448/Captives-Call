using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    //Variable to hold canvas transform space
    [SerializeField]
    private RectTransform canvasRect;
    [SerializeField]
    private Canvas canvas;

    //Variables for the UI text objects
    public TextMeshProUGUI currentCharacterText;
    public TextMeshProUGUI nextCharacterText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI stasisText;
    public TextMeshProUGUI hiddenText;
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI bAbLabelText;
    public TextMeshProUGUI pAbLabelText;
    public TextMeshProUGUI bAbText;
    public TextMeshProUGUI pAbText;
    public TextMeshProUGUI coinsText;

    //Variables for move prompt buttons
    public Button moveButton;
    public Button abilityButton;
    public Button leftButton;
    public Button rightButton;
    public Button switchButton;
    
    
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

    private bool moving;
    private bool usingAbility;

    private void Awake()
    {
        Instance = this;
    }

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

        //Darken character switch buttons if there is only one character
        Button[] rotateButtons = { leftButton, rightButton, switchButton };
        if(partySize<2)
        {
            foreach(Button button in rotateButtons)
            {
                button.interactable = false;
            }
        }
        else
        {
            foreach(Button button in rotateButtons)
            {
                button.interactable = true;
            }
        }

        //Update the current level using the nextLevel value from the levelcontroller
        levelText.text = $"Level: \n{levelController.CurrentLevel}";
        //Update moves taken with the property from the levelcontroller
        movesText.text = $"Moves: \n{levelController.MovesTaken}";

        //Update stasis remaining text if relevant
        if (levelController.StasisCount > 0 || levelController.DistortionCount>0)
            stasisText.alpha = 255;
        else
            stasisText.alpha = 0;
        if (levelController.StasisCount > 0)
            stasisText.text = $"Stasis Turns Left:\n{levelController.StasisCount}";
        if (levelController.DistortionCount > 0)
            stasisText.text = $"Distortion Turns Left:\n{levelController.DistortionCount}";

        //Update distortion remaining text if relevant
        if (levelController.HiddenCount > 0)
            hiddenText.alpha = 255;
        else
            hiddenText.alpha=0;
        if (levelController.HiddenCount > 0)
            hiddenText.text = $"Hidden Turns Left:\n{levelController.HiddenCount}";

        //Update key HUD element, darken if player has none, display number of how many the player has
        if(party.keyCount<1)
        {
            keySprite.color = new Color(keySprite.color.r, keySprite.color.g, keySprite.color.b, 0.2f);
            keyText.alpha = 0;
        }
        else
        {
            keySprite.color = new Color(keySprite.color.r, keySprite.color.g, keySprite.color.b, 1.0f);
            keyText.alpha = 255;
            keyText.text = $"{party.keyCount}";
        }

        //Update powerup ui element, darken if player is not powered up
        if (!party.poweredUp)
        {
            powerUpSprite.color = new Color(powerUpSprite.color.r, powerUpSprite.color.g, powerUpSprite.color.b, 0.2f);
        }
        else
        {
            powerUpSprite.color = new Color(powerUpSprite.color.r, powerUpSprite.color.g, powerUpSprite.color.b, 1.0f);
        }

        //Update abiltiy text boxes
        switch(party.currentMember)
        {
            case PartyMember.Warlock:
                bAbText.text = "Telekinetic Push";
                pAbText.text = "Warlock Crush";
                Color wlColor = new Color(0.93f, 0.29f, 0.19f, 1);
                bAbText.color = wlColor;
                pAbText.color = wlColor;
                break;
            case PartyMember.Wizard:
                bAbText.text = "Stasis";
                pAbText.text = "Temporal Distortion";
                Color wzColor = new Color(0.20f, 0.18f, 0.93f, 1);
                bAbText.color = wzColor;
                pAbText.color = wzColor;
                break;
            case PartyMember.Pickpocket:
                bAbText.text = "Sneak Attack";
                pAbText.text = "Out of Sight";
                Color pColor = new Color(0.26f,0.03f,0.43f,1);
                bAbText.color = pColor;
                pAbText.color = pColor;
                break;
            case PartyMember.Sailor:
                bAbText.text = "Shantyman";
                pAbText.text = "Grapple Dash";
                Color sColor = new Color(0.03f, 0.43f, 0.06f, 1);
                bAbText.color = sColor;
                pAbText.color = sColor;
                break;
        }

        //Update coins text
        coinsText.text = $"Coins Collected: {GameData.CoinCount}";
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

    /// <summary>
    /// Method activates the move or ability UI and triggers the level controller ability or move turn based on player decision
    /// </summary>
    /// <returns></returns>
    public IEnumerator ChooseAction(Vector3Int clickGrid, Vector3 clickPos)
    {
        //Moves the buttons to the tile that was clicked/mouse position
        RectTransform movePos = moveButton.GetComponent<RectTransform>();
        RectTransform abilityPos = abilityButton.GetComponent<RectTransform>();
        Vector2 anchoredPos;
        Camera mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, clickPos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPos);
        movePos.anchoredPosition = new Vector2(anchoredPos.x - 100, anchoredPos.y);
        abilityPos.anchoredPosition = new Vector2(anchoredPos.x+100, anchoredPos.y);

        while (!moving&&!usingAbility)
        {
            yield return null;
        }
        if (moving)
        {
            levelController.CallMoveGuards();
            levelController.MoveTurn(clickGrid);
            moving = false;
        }
        else if (usingAbility)
        {
            levelController.AbilityTurn(clickGrid);
            levelController.CallMoveGuards();
            usingAbility = false;
        }
        moveButton.transform.position = new Vector3(2000,0,0);
        abilityButton.transform.position = new Vector3(2000, 0, 0);
        levelController.AcceptingActionInput = false;
        yield break;
    }

    public void MoveSelected()
    {
        moving = true;
    }
    public void AbilitySelected()
    {
        usingAbility = true;
    }
}
