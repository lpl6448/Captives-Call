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

    /// <summary>
    /// Whether this door will be activated from a pressure plate next turn
    /// </summary>
    public bool WillActivate { get; set; }

    /// <summary>
    /// Whether this door is inverted (meaning that it is closed when activated and opened when deactivated)
    /// </summary>
    private bool inactive;
    public bool Inactive => inactive;

    /// <summary>
    /// Holds if the door has been activated/deactivated so far this turn
    /// </summary>
    protected bool triggered;

    public override bool IsTraversable(DynamicObject mover)
    {
        if (isOpen)
        {
            if (mover is Guard)
                return true;
        }
        if (WillActivate != inactive || isOpen)
        {
            if (mover is Party)
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

    public override bool BlocksLOS()
    {
        if (!isOpen)
            return true;
        return false;
    }

    public override void Run(bool canRun, DynamicObject trigger)
    {
        if (LevelController.Instance.StasisCount < 1)
        {
            bool prevOpen = isOpen;
            if (!triggered || canRun)
            {
                ChangeState(canRun, trigger);
                triggered = true;
            }
        }
    }

    protected virtual void ChangeState(bool open, DynamicObject trigger)
    {
        if (open)
        {
            isOpen = !inactive;
        }
        else
        {
            isOpen = inactive;
        }
    }

    public override void PreAction()
    {
        triggered = false;
        WillActivate = false;
    }
    public override void PostAction()
    {
        if (triggered)
            StartAnimation(WaitToUpdateState());
    }


    protected void Awake()
    {
        //isOpen = false;
        WillActivate = false;
        triggered = false;
        inactive = isOpen;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    protected IEnumerator WaitToUpdateState()
    {
        if (isOpen != inactive)
        {
            yield return WaitForTrigger("activate");
            UpdateState(!inactive);
        }
        else
        {
            yield return WaitForTrigger("deactivate");
            UpdateState(inactive);
        }

        // If the trigger was a pressure plate, we should only change the sprite if this was actually the pressure plate that triggered opening/closing
        //if (!(trigger is PressurePlate) || (trigger as PressurePlate).IsPressed == !inactive)
        //if(!triggered)
        StopAnimation();
    }

    /// <summary>
    /// By default, switch between the door's open and closed sprites
    /// </summary>
    protected virtual void UpdateState(bool opened)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (opened)
            spriteRenderer.sprite = sprites[1];
        else
            spriteRenderer.sprite = sprites[0];
    }
}
