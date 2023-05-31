using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : DynamicObject
{
    /// <summary>
    /// Whether spikes are raised or not
    /// </summary>
    [SerializeField]
    private bool raised;

    /// <summary>
    /// Reference to the child's SpriteRenderer, containing the Spikes_Up sprite
    /// </summary>
    [SerializeField]
    private SpriteRenderer spikes;

    /// <summary>
    /// Stores whether the spikes were raised in the last turn
    /// </summary>
    private bool wasRaised;

    public override bool IsTraversable(DynamicObject mover)
    {

        if (mover is Party)
            return true;
        if (mover is Guard)
            return true;
        if (mover is Boulder)
            return false;
        return false;
    }

    public override bool CanMove(Vector3Int tilePosition)
    {
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        wasRaised = raised;
    }

    public override void PostAction()
    {
        Run(LevelController.Instance.StasisCount < 1, null);
        if (raised)
        {
            //Check if anything is on spikes that will be impaled
            List<Guard> guards = LevelController.Instance.GetDynamicObjectsOnTile<Guard>(TilePosition);
            List<Party> party = LevelController.Instance.GetDynamicObjectsOnTile<Party>(TilePosition);
            List<DynamicObject> killable = new List<DynamicObject>();
            foreach (DynamicObject stabbed in guards)
            {
                killable.Add(stabbed);
            }
            foreach (DynamicObject stabbed in party)
            {
                killable.Add(stabbed);
            }
            //Impale the object
            foreach (DynamicObject stabbed in killable)
                LevelController.Instance.DestroyDynamicObject(TilePosition, stabbed, this);

            if (!wasRaised)
                StartCoroutine(AnimateSpikesIn());
        }
        else if (wasRaised)
            StartCoroutine(AnimateSpikesOut());

        wasRaised = raised;
    }

    public override void Run(bool canRun, DynamicObject trigger)
    {
        if (canRun)
            raised = !raised;
    }

    private IEnumerator AnimateSpikesIn()
    {
        yield return new WaitForSeconds(AnimationUtility.StandardAnimationDuration / 2);

        float startTime = Time.time;
        float duration = 0.125f;
        Color color = spikes.color;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            color.a = 1 - (1 - t) * (1 - t);
            spikes.color = color;
            yield return null;
        }
        color.a = 1;
        spikes.color = color;
    }
    private IEnumerator AnimateSpikesOut()
    {
        float startTime = Time.time;
        float duration = 0.25f;
        Color color = spikes.color;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            color.a = 1 - t;
            spikes.color = color;
            yield return null;
        }
        color.a = 0;
        spikes.color = color;
    }
}
