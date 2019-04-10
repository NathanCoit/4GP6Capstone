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
    private UIManager UIMan;

    public Ability ability;
    public Material mousedOverValid;
    public Material mousedOverInvalid;
    public Material air;
    public GameObject CubeAOEPrefab;
    public GameObject SphereAOEPrefab;
    private GameObject AOEShape;
    private bool buttonSwitch = false;

    private Vector3 target;
    private bool inPlace;
    private Vector3 SmoothDampV;
    public float targetTolerance;

    public bool valid;
    public List<Unit> targets;

    void Start ()
    {
        autoClick = false;

        //They're here
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
        SetupMan = GameObject.Find("SetupManager").GetComponent<SetupManager>();
        UIMan = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        //Is this a valid tile (a targetable unit on it)
        valid = false;
    }
	
	void Update ()
    {
        //Do the pretty animation
        if (!(System.Math.Abs(transform.position.y - target.y) < targetTolerance))
        {
            transform.position = Vector3.SmoothDamp(
                    transform.position, target, ref SmoothDampV, 0.1f * (Random.Range(0.9f, 1.1f)));

            gameObject.GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g,
                GetComponent<Renderer>().material.color.b, Mathf.Abs(target.y / transform.position.y) * mousedOverInvalid.color.a);
        }
        else
            inPlace = true;
    }

    //Set where we go with the animation
    public void setTarget(Vector3 target)
    {
        this.target = target;
    }


    public void OnMouseOver()
    {
        //If the animation is done
        if (inPlace)
        {
            //Init our lists of targets
            targets = new List<Unit>();

            //If it's single target we're just target the one tile
            if (ability.AbiltyType == Ability.ABILITYTYPE.SingleTarget)
            {
                //Check who we're targeting
                if (BoardMan.playerUnits.Contains(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()))
                {
                    targets = BoardMan.enemyUnits;
                }
                else
                {
                    targets = BoardMan.playerUnits;
                }

                //Check if we're a valid tile
                foreach (Unit u in targets)
                {
                    if (pos.x == u.getPos().x && pos.y == u.getPos().y)
                        valid = true;
                }

                //If we're valid, change material when moused over
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
            else if (ability.AbiltyType == Ability.ABILITYTYPE.MultiTarget)
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
                if (aMi.AbilityShape == Ability.MultiTargetShape.Line)
                {
                    if (BoardMan.abilityDirection == 0)
                    {
                        AOEShape.transform.eulerAngles = new Vector3(0, 0, 0);
                        AOEShape.transform.position = new Vector3(transform.position.x + aMi.Length / 2, transform.position.y, transform.position.z);

                    }
                    else if (BoardMan.abilityDirection == 1)
                    {
                        AOEShape.transform.eulerAngles = new Vector3(0, 270, 0);
                        AOEShape.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + aMi.Length / 2);

                    }
                    else if (BoardMan.abilityDirection == 2)
                    {
                        AOEShape.transform.eulerAngles = new Vector3(0, 180, 0);
                        AOEShape.transform.position = new Vector3(transform.position.x - aMi.Length / 2, transform.position.y, transform.position.z);

                    }
                    else if (BoardMan.abilityDirection == 3)
                    {
                        AOEShape.transform.eulerAngles = new Vector3(0, 90, 0);
                        AOEShape.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - aMi.Length / 2);

                    }
                }

                //Check what units we should be targeting.
                if (ability.AbiltyType != Ability.ABILITYTYPE.Buff)
                {
                    if (BoardMan.playerUnits.Contains(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()))
                    {
                        targets = AOEShape.GetComponent<AOE>().getTargets(true);
                    }
                    else
                    {
                        targets = AOEShape.GetComponent<AOE>().getTargets(false);
                    }
                }
                else
                {
                    if (BoardMan.playerUnits.Contains(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()))
                    {
                        targets = AOEShape.GetComponent<AOE>().getTargets(false);
                    }
                    else
                    {
                        targets = AOEShape.GetComponent<AOE>().getTargets(true);
                    }
                }



                if (targets != new List<Unit>())
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
            else if (ability.AbiltyType == Ability.ABILITYTYPE.Buff)
            {
                //If we're a buff, we target friendly units
                if (BoardMan.playerUnits.Contains(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()))
                {
                    targets = BoardMan.playerUnits;
                }
                else
                {
                    targets = BoardMan.enemyUnits;
                }

                foreach (Unit u in targets)
                {
                    if (pos.x == u.getPos().x && pos.y == u.getPos().y)
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
            else if (ability.AbiltyType == Ability.ABILITYTYPE.Debuff)
            {
                //Debuff targets enemy units
                if (BoardMan.playerUnits.Contains(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()))
                {
                    targets = BoardMan.enemyUnits;
                }
                else
                {
                    targets = BoardMan.playerUnits;
                }

                foreach (Unit u in targets)
                {
                    if (pos.x == u.getPos().x && pos.y == u.getPos().y)
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

            //Actually clicking (using the ability)
            if ((Input.GetMouseButtonDown(0) || autoClick) && !buttonSwitch)
            {
                buttonSwitch = true;
                
                //Subtract faith cost and then update the ui
                (MapMan.Selected.GetComponent<UnitObjectScript>().getUnit() as God).faith -= ability.FaithCost;
                UIMan.updateFaithLabels();

                //Just damage the one unit if single target
                if (ability.AbiltyType == Ability.ABILITYTYPE.SingleTarget)
                {
                    SingleTargetAbility aSi = (SingleTargetAbility)SingleTargetAbility.LoadAbilityFromName(ability.AbilityName);
                    Unit target = new Unit();
                    foreach (Unit u in targets)
                    {
                        if (pos.x == u.getPos().x && pos.y == u.getPos().y)
                            target = u;
                    }

                    target.dealDamage(aSi.AbilityDamage);

                    //Kill target if it died
                    if (target.WorshiperCount <= 0)
                    {
                        BoardMan.killUnit(target);
                    }
                }
                // Damage all the target within the AOE if it's multi
                else if (ability.AbiltyType == Ability.ABILITYTYPE.MultiTarget)
                {
                    MultiTargetAbility aMi = (MultiTargetAbility)MultiTargetAbility.LoadAbilityFromName(ability.AbilityName);
                    foreach (Unit u in targets)
                    {
                        u.dealDamage(aMi.AbilityDamage);
                        if (u.WorshiperCount <= 0)
                        {
                            BoardMan.killUnit(u);
                        }
                    }
                    GameObject[] AOEShapes = GameObject.FindGameObjectsWithTag("AOEShapes");
                    foreach (GameObject g in AOEShapes)
                    {
                        Destroy(g);
                    }

                }
                else if (ability.AbiltyType == Ability.ABILITYTYPE.Buff)
                {
                    Unit target = new Unit();
                    foreach (Unit u in targets)
                    {
                        if (pos.x == u.getPos().x && pos.y == u.getPos().y)
                            target = u;
                    }

                    //Add the new status effect when used
                    target.addNewStatusEffect(ability, true);
                }
                else if (ability.AbiltyType == Ability.ABILITYTYPE.Debuff)
                {
                    Unit target = new Unit();
                    foreach (Unit u in targets)
                    {
                        if (pos.x == u.getPos().x && pos.y == u.getPos().y)
                            target = u;
                    }

                    //Add the new status effect when used
                    target.addNewStatusEffect(ability, false);

                }

                //End turn once we used an ability
                MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().EndTurnButton();

                //Unslecting
                MapMan.Selected = null;

                //Clean up Tiles
                MapMan.ClearSelection();
            }


            if (Input.GetMouseButtonUp(0) && buttonSwitch)
            {
                buttonSwitch = false;
            }

            //Udating direction for the line AOE
            if (Input.GetMouseButtonDown(1))
                if (BoardMan.abilityDirection < 3)
                    BoardMan.abilityDirection++;
                else
                    BoardMan.abilityDirection = 0;

        }
    }
    

    //Unhighlight a tile we mouse out
    public void OnMouseExit()
    {
        if (inPlace)
        {
            GetComponent<MeshRenderer>().material = mousedOverInvalid;
            Destroy(AOEShape);
        }
    }

    //For spoofing clicks for testing (and AI)
    public void testClick()
    {
        autoClick = true;
    }
}
