using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : Unit
{
    private string godName;
    public bool isInBattle;
    private string[] Abilites;
    private SoundManager SoundMan;
    private BoardManager BoardMan;

    public int faith;

    public God(string name) : base()
    {
        godName = name;
        isInBattle = false;
        SoundMan = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        faith = 10;
    }

    public void setAbilities(string[] Abilites)
    {
        this.Abilites = Abilites;
    }

    public string[] getAbilites()
    {
        return Abilites;
    }

    public string getName()
    {
        return godName;
    }

    public void enterBattle()
    {
        isInBattle = true;
        Camera.main.GetComponent<CombatCam>().resetCamera();
        unitGameObject().GetComponent<UnitObjectScript>().drawEnterBattleTiles();
        UIMan.removeMenu();
        UIMan.godEnteringBattle = true;
    }

    public void forcedEnterBattle()
    {
        isInBattle = true;
        unitGameObject().transform.GetChild(0).gameObject.SetActive(true);
        SoundMan.playGodEnterBattle(this);

        List<Tile> travesableTiles = new List<Tile>();

        foreach (Tile t in MapMan.tiles)
        {
            if (t.isTraversable() && !BoardMan.findTeamTiles(BoardMan.playerUnits).Contains(t) && !BoardMan.findTeamTiles(BoardMan.enemyUnits).Contains(t))
            {
                travesableTiles.Add(t);
            }
        }

        System.Random r = new System.Random();
        Tile randomTile = travesableTiles[r.Next(0, travesableTiles.Count - 1)];

        MoveTo(new Vector2(randomTile.getX(), randomTile.getZ()), MapMan.tiles);
    }

    public override void Draw(Tile[,] tiles)
    {
        if(isInBattle)
            parentObject.transform.position = new Vector3(tiles[(int)pos.x, (int)pos.y].getX() + ((1 - parentObject.transform.lossyScale.x) / 2) + parentObject.transform.lossyScale.x / 2,
                tiles[(int)pos.x, (int)pos.y].getY() + parentObject.transform.lossyScale.y - 1.0f, 
                tiles[(int)pos.x, (int)pos.y].getZ() + ((1 - parentObject.transform.lossyScale.z) / 2) + parentObject.transform.lossyScale.x / 2);
    }

    public override void updateAttackStrength()
    {
        //There a god, you can't make them hit less hard
    }


}
