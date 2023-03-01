using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField]
    private HighlightManager hm;
    [SerializeField]
    private PlayerManager pm;
    [SerializeField]
    Grid grid;

    bool spawnHighlights;

    // Start is called before the first frame update
    void Start()
    {
        spawnHighlights = false;
        pm.party.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawnHighlights)
            hm.HighlightTiles(pm.party);

        if (Input.GetMouseButtonDown(0)&&validClick())
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pm.MovePlayer(mousePosition);
            pm.party.UpdateTick();
            hm.HighlightTiles(pm.party);
        }
    }

    private bool validClick()
    {
        Vector3Int gridPosition = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3Int partyPos = grid.WorldToCell(pm.party.transform.position);
        int dX = gridPosition.x-partyPos.x;
        int dY = gridPosition.y-partyPos.y;
        return (Mathf.Abs(dX) + Mathf.Abs(dY)) == 1;
    }
}
