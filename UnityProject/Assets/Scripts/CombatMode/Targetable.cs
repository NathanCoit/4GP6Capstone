using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script associated with Godly abilities.
 * Changes tile materials or creates shapes for better feedback to see who/what is being targeted by a Godly ability.
 * Is attached to targetable tiles that are created on every tile when an ability is being used
 */ 
public class Targetable : MonoBehaviour {

    public Vector2 pos;
    private bool autoClick;
    private MapManager MapMan;
    private BoardManager BoardMan;
    private SetupManager SetupMan;

    public Ability ability;
    public Material mousedOverValid;
    public Material mousedOverInvalid;
    public Material air;
    public GameObject CubeAOEPrefab;
    public GameObject SphereAOEPrefab;
    private GameObject AOEShape;
    private bool buttonSwitch = false;
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
        bool valid = false;

        //If it's single target we're just target the one tile
        if (ability.AbiltyType == Ability.ABILITYTYPE.SingleTarget)
        {
            if (BoardMan.playerUnits.Contains(MapMan.Selected))
            {
                targets = BoardMan.enemyUnits;
            }
            else
            {
                targets = BoardMan.playerUnits;
            }

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
        }

        //If it's multi target, we gotta worry about shapes
        else if(ability.AbiltyType == Ability.ABILITYTYPE.MultiTarget)
        {
            MultiTargetAbility aMi = (MultiTargetAbility)MultiTargetAbility.LoadAbilityFromName(ability.AbilityName);
            if (AOEShape == null)
            {
                //Cone is actually a sphere (just didn't change the name elsewhere yet)
                if (aMi.AbilityShape == Ability.MultiTargetShape.Cone)
                {
                    AOEShape = Instantiate(SphereAOEPrefab);
                    AOEShape.transform.localScale *= aMi.Length;
                    AOEShape.gameObject.layer = 2;
                    AOEShape.transform.position = transform.position;
                }
                else if (aMi.AbilityShape == Ability.MultiTargetShape.Line)
                {
                    AOEShape = Instantiate(CubeAOEPrefab);
                    AOEShape.gameObject.layer = 2;
                    AOEShape.transform.localScale = new Vector3(AOEShape.transform.localScale.x * aMi.Length, AOEShape.transform.localScale.y, AOEShape.transform.localScale.z);
                    AOEShape.transform.position = new Vector3(transform.position.x + aMi.Length / 2, transform.position.y, transform.position.z);
                }
                else if (aMi.AbilityShape == Ability.MultiTargetShape.Square)
                {
                    AOEShape = Instantiate(CubeAOEPrefab);
                    AOEShape.transform.localScale *= aMi.Length;
                    AOEShape.gameObject.layer = 2;
                    AOEShape.transform.position = transform.position;
                }

            }

            //Get the line offset and facing the right way
            if(aMi.AbilityShape == Ability.MultiTargetShape.Line)
            {
                if (MapMan.Selected.GetComponent<Gods>().direction == 0)
                {
                    AOEShape.transform.eulerAngles = new Vector3(0, 0, 0);
                    AOEShape.transform.position = new Vector3(transform.position.x + aMi.Length / 2, transform.position.y, transform.position.z);

                }
                else if (MapMan.Selected.GetComponent<Gods>().direction == 1)
                {
                    AOEShape.transform.eulerAngles = new Vector3(0, 270, 0);
                    AOEShape.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + aMi.Length / 2);

                }
                else if (MapMan.Selected.GetComponent<Gods>().direction == 2)
                {
                    AOEShape.transform.eulerAngles = new Vector3(0, 180, 0);
                    AOEShape.transform.position = new Vector3(transform.position.x - aMi.Length / 2, transform.position.y, transform.position.z);

                }
                else if (MapMan.Selected.GetComponent<Gods>().direction == 3)
                {
                    AOEShape.transform.eulerAngles = new Vector3(0, 90, 0);
                    AOEShape.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - aMi.Length / 2);

                }
            }

            //Check what units we should be targeting.
            if (BoardMan.playerUnits.Contains(MapMan.Selected))
            {
                targets = AOEShape.GetComponent<AOE>().getTargets(true);
            }
            else
            {
                targets = AOEShape.GetComponent<AOE>().getTargets(false);
            }
            

            if (targets != new List<GameObject>())
                valid = true;
            else
                valid = false;

            if (valid)
            {
                GetComponent<MeshRenderer>().material = mousedOverValid;
                AOEShape.GetComponent<MeshRenderer>().material = mousedOverValid;
            }
            else
            {
                GetComponent<MeshRenderer>().material = mousedOverInvalid;
                AOEShape.GetComponent<MeshRenderer>().material = mousedOverInvalid;
            }
            
            
        }

        //Actually clicking (using the ability)
        if ((Input.GetMouseButtonDown(0) || autoClick) && !buttonSwitch)
        {
            buttonSwitch = true;
            //Just damage the one unit if single target
            if (ability.AbiltyType == Ability.ABILITYTYPE.SingleTarget)
            {
                SingleTargetAbility aSi = (SingleTargetAbility)SingleTargetAbility.LoadAbilityFromName(ability.AbilityName);
                GameObject target = new GameObject();
                foreach (GameObject g in targets)
                {
                    if (pos.x == g.GetComponent<Units>().getPos().x && pos.y == g.GetComponent<Units>().getPos().y)
                        target = g;
                }

                target.GetComponent<Units>().setWorshiperCount(target.GetComponent<Units>().getWorshiperCount() - aSi.AbilityDamage);

                if(target.GetComponent<Units>().WorshiperCount >= 0)
                {
                    if (BoardMan.enemyUnits.Contains(target))
                        BoardMan.enemyUnits.Remove(target);
                    else if (BoardMan.playerUnits.Contains(target))
                        BoardMan.playerUnits.Remove(target);
                    Destroy(target);
                    checkEnd();
                }
            }
            // Damage all the target within the AOE if it's multi
            else if (ability.AbiltyType == Ability.ABILITYTYPE.MultiTarget)
            {
                MultiTargetAbility aMi = (MultiTargetAbility)MultiTargetAbility.LoadAbilityFromName(ability.AbilityName);
                Debug.Log(targets.Count);
                foreach (GameObject g in targets)
                {
                    g.GetComponent<Units>().setWorshiperCount(g.GetComponent<Units>().getWorshiperCount() - aMi.AbilityDamage);
                    if (g.GetComponent<Units>().WorshiperCount <= 0)
                    {
                        if (BoardMan.enemyUnits.Contains(g))
                            BoardMan.enemyUnits.Remove(g);
                        else if (BoardMan.playerUnits.Contains(g))
                            BoardMan.playerUnits.Remove(g);
                        Destroy(g);
                        checkEnd();
                    }
                }
                GameObject[] AOEShapes = GameObject.FindGameObjectsWithTag("AOEShapes");
                foreach(GameObject g in AOEShapes)
                {
                    Destroy(g);
                }

            }
            // TODO
            else if (ability.AbiltyType == Ability.ABILITYTYPE.Buff)
            {
                Debug.Log("Using buff ability " + ability.AbilityName + " !!!");
            }
            // TODO
            else if (ability.AbiltyType == Ability.ABILITYTYPE.Debuff)
            {
                Debug.Log("Using debuff ability " + ability.AbilityName + " !!!");
            }

            //End turn once we used an ability
            MapMan.Selected.GetComponent<Units>().EndTurnButton();

            //Unslecting
            MapMan.Selected = null;

            //Clean up Tiles
            MapMan.ClearSelection();
            Debug.Log("Succeed");
        }

        if (Input.GetMouseButtonUp(0) && buttonSwitch)
        {
            buttonSwitch = false;
            Debug.Log("Fail");
        }

        //Udating direction for the line AOE
        if (Input.GetMouseButtonDown(1))
            if (MapMan.Selected.GetComponent<Gods>().direction < 3)
                MapMan.Selected.GetComponent<Gods>().direction++;
            else
                MapMan.Selected.GetComponent<Gods>().direction = 0;

    }

    //This is here in case we kill everything with an ability
    private void checkEnd()
    {
        if (BoardMan.playerUnits.Count == 1)
        {
            BoardMan.Defeat();
        }
        else if (BoardMan.enemyUnits.Count == 1)
        {
            BoardMan.Victory();
        }
    }

    //Unhighlight a tile we mouse out
    public void OnMouseExit()
    {
        GetComponent<MeshRenderer>().material = air;
        Destroy(AOEShape);
    }

    //For spoofing clicks for testing (and AI)
    public void testClick()
    {
        autoClick = true;
    }
}
