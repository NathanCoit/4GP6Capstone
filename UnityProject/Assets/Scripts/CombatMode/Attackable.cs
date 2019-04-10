using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attackable : MonoBehaviour
{
    /*
     * Associated with the attack tile (the red tile that appears when trying to attack with a worshipper).
     * When a worshipper's Attack action is picked, this will be called to show the possible attackable targets that worshipper has.
     * Also involved with the damage infliction unto target victim.
     * Is attached to attackable tiles (the red ones)
     */

    public Vector2 pos;
    private MapManager MapMan;
    private BoardManager BoardMan;
    private SetupManager SetupMan;
    private UIManager UIMan;
    private SoundManager SoundMan;

    public Material hoverMaterial;
    public Material baseMaterial;

    private bool autoClick;

    void Start()
    {
        //*******EVERYBODYS'S HERE********//
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
        SetupMan = GameObject.Find("SetupManager").GetComponent<SetupManager>();
        UIMan = GameObject.Find("UIManager").GetComponent<UIManager>();
        SoundMan = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }


    void Update()
    {

    }

    private Tile[,] GetTiles()
    {
        return MapMan.tiles;
    }

    public void OnMouseOver()
    {
        //Wait till target has been chosen, inflict damage, check (and remove if necessary) if that unit is dead, recalculate that team's morale
        //end the attacker's turn, ans check to see if one side won the battle.
        if (Input.GetMouseButtonDown(0) || autoClick)
        {
            // Get position of clicked tile
            List<Unit> targets = new List<Unit>();
            if (BoardMan.playerUnits.Contains(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()))
            {
                targets = BoardMan.enemyUnits;
            }
            else
            {
                targets = BoardMan.playerUnits;
            }
            // Find unit that matches tiles position
            Tile[,] tiles = GetTiles();
            Unit attackedUnit = new Unit();

            foreach (Unit u in targets)
                if (u.getPos() == pos)
                    attackedUnit = u;

            //Decreases the "HP" of the attacked unit - decreases their worshipper count
            int damage = (int)MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().getAttackStrength();

            //Set remaining worshippers accordingly

            if(!attackedUnit.getBlindDebuff())
                attackedUnit.dealDamage(damage);
            else
            {
                //If we're blinded, we have a 50% chance of missing
                if(Random.Range(0,100) > 50)
                    attackedUnit.dealDamage(damage);
            }

            //Adjust rotation
            if(Mathf.Abs(attackedUnit.getPos().y - MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().getPos().y) 
                > Mathf.Abs(attackedUnit.getPos().x - MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().getPos().x))
            {
                if (attackedUnit.getPos().y - MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().getPos().y > 0)
                    MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().turnToFace(2);
                else
                    MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().turnToFace(0);

            }
            else
            {
                if (attackedUnit.getPos().x - MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().getPos().x > 0)
                    MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().turnToFace(3);
                else
                    MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().turnToFace(1);
            }
            
            SoundMan.playUnitAttack();

            Camera.main.GetComponent<CombatCam>().lookAt(attackedUnit.unitGameObject().transform.position);

            //Adjust that team's morale
            if (BoardMan.playerUnits.Contains(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit())) //check to see who initiated the attack
            {
                SetupMan.enemyMorale = ((BoardMan.GetRemainingWorshippers(false)) * 1.0f / (SetupMan.enemyWorshiperCount));
                //SetupMan.enemyWorshiperCount already takes into consideration the 0.8f (i.e. only 80% of that god's worshipper participates in war)
            }
            else
            {
                SetupMan.playerMorale = ((BoardMan.GetRemainingWorshippers(true)) * 1.0f / (SetupMan.playerWorshiperCount));
            }


            //Did the attacked unit's HP reach 0? If so, remove them from the board AND from the appropriate unit array
            //Done in unit.dealDamage now
            /*
            if (attackedUnit.getWorshiperCount() <= 0)
                BoardMan.killUnit(attackedUnit);
            */

            //End the turn of the unit who initiated the attack
            MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().EndTurnButton();

            //Unslecting
            MapMan.Selected = null;

            //Hide Menu
            UIMan.removeMenu();

            //Clean up Tiles
            MapMan.ClearSelection();

        }
    }

    private void OnMouseEnter()
    {
        SoundMan.playUiHover();
        gameObject.GetComponent<Renderer>().material = hoverMaterial;
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<Renderer>().material = baseMaterial;
    }

    //For spoofing clicks for testing
    public void TestClick()
    {
        autoClick = true;
    }
}
