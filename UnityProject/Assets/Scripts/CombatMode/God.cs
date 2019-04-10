using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Goes on the god unit
 * Handles storing abilites and entering battle 
 * Inherits from unit
 * */


public class God : Unit
{
    //Name
    private string godName;

    //If we're in battle
    public bool isInBattle;
    
    //List of our abilites
    private string[] Abilites;

    //The men
    private SoundManager SoundMan;
    private BoardManager BoardMan;

    //Faith, for using abiltiles
    public int faith;

    //Adding on to the base contructor
    public God(string name) : base()
    {
        godName = name;
        isInBattle = false;
        SoundMan = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        //Starting faith
        faith = 10;
    }

    //Getters and setters for abilities
    public void setAbilities(string[] Abilites)
    {
        this.Abilites = Abilites;
    }

    public string[] getAbilites()
    {
        return Abilites;
    }

    //Get our name
    public string getName()
    {
        return godName;
    }

    //Enter the battle (voluntarily)
    public void enterBattle()
    {
        //Set flag
        isInBattle = true;

        //Move camera to center
        Camera.main.GetComponent<CombatCam>().resetCamera();

        //Draw tiles
        unitGameObject().GetComponent<UnitObjectScript>().drawEnterBattleTiles();
        UIMan.removeMenu();

        //Flag moveable tiles script (so we know not to subtract movement)
        UIMan.godEnteringBattle = true;
    }

    //Enter battle, when forced to
    public void forcedEnterBattle()
    {
        isInBattle = true;
        unitGameObject().transform.GetChild(0).gameObject.SetActive(true);
        SoundMan.playGodEnterBattle(this);

        List<Tile> travesableTiles = new List<Tile>();

        //Get all tiles we could move to
        foreach (Tile t in MapMan.tiles)
        {
            if (t.isTraversable() && !BoardMan.findTeamTiles(BoardMan.playerUnits).Contains(t) && !BoardMan.findTeamTiles(BoardMan.enemyUnits).Contains(t))
            {
                travesableTiles.Add(t);
            }
        }

        //Move us to a random one
        System.Random r = new System.Random();
        Tile randomTile = travesableTiles[r.Next(0, travesableTiles.Count - 1)];

        MoveTo(new Vector2(randomTile.getX(), randomTile.getZ()), MapMan.tiles);
    }

    //Overide draw (becuase gods are taller)
    public override void Draw(Tile[,] tiles)
    {
        if(isInBattle)
            parentObject.transform.position = new Vector3(tiles[(int)pos.x, (int)pos.y].getX() + ((1 - parentObject.transform.lossyScale.x) / 2) + parentObject.transform.lossyScale.x / 2,
                tiles[(int)pos.x, (int)pos.y].getY() + parentObject.transform.lossyScale.y - 1.0f, 
                tiles[(int)pos.x, (int)pos.y].getZ() + ((1 - parentObject.transform.lossyScale.z) / 2) + parentObject.transform.lossyScale.x / 2);
    }

    public override void updateAttackStrength()
    {
        //They're a god, you can't make them hit less hard
    }


}
