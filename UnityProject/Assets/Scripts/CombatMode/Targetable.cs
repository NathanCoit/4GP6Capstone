using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour {

    public Vector2 pos;
    private bool autoClick;
    private MapManager MapMan;
    private BoardManager BoardMan;
    private SetupManager SetupMan;

    public Ability a;
    public Material mousedOverValid;
    public Material mousedOverInvalid;
    public Material air;
    // Use this for initialization
    void Start ()
    {
        autoClick = false;

        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();

        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        SetupMan = GameObject.Find("SetupManager").GetComponent<SetupManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private Tile[,] getTiles()
    {
        return MapMan.tiles;
    }

    public void OnMouseOver()
    {
        List<GameObject> targets = new List<GameObject>();

        if (BoardMan.playerUnits.Contains(MapMan.Selected))
        {
            targets = BoardMan.enemyUnits;
        }
        else
        {
            targets = BoardMan.playerUnits;
        }

        bool valid = false;

        foreach (GameObject g in targets)
        {
            if (pos.x == g.GetComponent<Units>().getPos().x && pos.y == g.GetComponent<Units>().getPos().y)
                valid = true;
        }

        if (valid)
        {
            GetComponent<MeshRenderer>().material = mousedOverValid;
        }
        else
        {
            GetComponent<MeshRenderer>().material = mousedOverInvalid;
        }


        //Actually clicking
        if (Input.GetMouseButtonDown(0) || autoClick)
        {
            if (a.Type == Ability.AbilityType.SingleTarget)
            {
                SingleTargetAbility aSi = (SingleTargetAbility)SingleTargetAbility.LoadAbilityFromName(a.AbilityName);
                GameObject target = new GameObject();
                foreach (GameObject g in targets)
                {
                    if (pos.x == g.GetComponent<Units>().getPos().x && pos.y == g.GetComponent<Units>().getPos().y)
                        target = g;
                }
                target.GetComponent<Units>().setWorshiperCount(target.GetComponent<Units>().getWorshiperCount() - aSi.AbilityDamage);
                Debug.Log("Using single target ability " + a.AbilityName + " !!!");
            }
            else if (a.Type == Ability.AbilityType.MultiTarget)
            {
                MultiTargetAbility aMi = (MultiTargetAbility)MultiTargetAbility.LoadAbilityFromName(a.AbilityName);
                List<GameObject> target = new List<GameObject>();
                foreach (GameObject g in targets)
                {
                    if (pos.x == g.GetComponent<Units>().getPos().x && pos.y == g.GetComponent<Units>().getPos().y)
                        target.Add(g);
                }
                foreach (GameObject g in target)
                {
                    g.GetComponent<Units>().setWorshiperCount(g.GetComponent<Units>().getWorshiperCount() - aMi.AbilityDamage);
                }
                Debug.Log("Using multi target ability " + a.AbilityName + " !!!");
            }
            else if (a.Type == Ability.AbilityType.Buff)
            {
                Debug.Log("Using buff ability " + a.AbilityName + " !!!");
            }
            else if (a.Type == Ability.AbilityType.Debuff)
            {
                Debug.Log("Using debuff ability " + a.AbilityName + " !!!");
            }

            //Hide Menu
            MapMan.Selected.GetComponent<Units>().EndTurnButton();

            //Unslecting
            MapMan.Selected = null;

            //Clean up Tiles
            MapMan.ClearSelection();

        }

        

    }

    public void OnMouseExit()
    {
        GetComponent<MeshRenderer>().material = air;
    }

    //For spoofing clicks for testing
    public void testClick()
    {
        autoClick = true;
    }
}
