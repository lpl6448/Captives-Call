using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : DynamicObject
{
    /// <summary>
    /// Allows loading of sprites via unity editor
    /// </summary>
    [SerializeField]
    protected List<Sprite> sprites;

    /// <summary>
    /// Tracks if the "door" is accessible to move on
    /// </summary>
    [SerializeField]
    protected bool isOpen;

    public bool IsOpen => isOpen;

    protected bool willOpen;
    public bool WillOpen
    {
        get { return willOpen; }
        set
        {
            if (!triggered)
            {
                willOpen = value;
                triggered = value;
            }
        }
    }

    /// <summary>
    /// Holds if the gate has been triggered this turn already
    /// </summary>
    protected bool triggered;

    public override bool IsTraversable(DynamicObject mover)
    {
        if (willOpen || isOpen)
        {
            if (mover is Party)
                return true;
            if (mover is Guard)
                return true;
            if (mover is Boulder)
                return true;
        }
        return false;
    }

    public override bool CanMove(Vector3Int tilePosition)
    {
        return false;
    }

    public override void Run(bool canRun, DynamicObject trigger)
    {
        if (LevelController.Instance.StasisCount < 1)
        {
            if (!triggered) { ChangeState(canRun, trigger); }
            if (isOpen) { triggered = true; }
        }
    }

    protected virtual void ChangeState(bool open, DynamicObject trigger)
    {
        isOpen = open;
        StartAnimation(WaitToUpdateState(trigger));
    }

    public override void PreAction()
    {
        triggered = false;
    }


    protected void Awake()
    {
        isOpen = false;
        willOpen = false;
        triggered = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    protected IEnumerator WaitToUpdateState(DynamicObject trigger)
    {
        if (trigger != null)
            if (isOpen)
                yield return WaitForTrigger("activate");
            else
                yield return WaitForTrigger("deactivate");

        // If the trigger was a pressure plate, we should only change the sprite if this was actually the pressure plate that triggered opening/closing
        if (!(trigger is PressurePlate) || (trigger as PressurePlate).IsPressed == IsOpen)
            UpdateState();
        StopAnimation();
    }

    /// <summary>
    /// By default, switch between the door's open and closed sprites
    /// </summary>
    protected virtual void UpdateState()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (isOpen)
            spriteRenderer.sprite = sprites[1];
        else
            spriteRenderer.sprite = sprites[0];
    }
}
